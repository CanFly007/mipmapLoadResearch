using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader100 : MonoBehaviour
{
    private static int m_loadMipmapLevel = 0;

    private GameObject teaportGo;

    public Texture2D placeholderTex;
    public Material insteadABMat;
    public string baseFileName = "Amazing Speed_Floor_D_ld";
    private string folderPath;



    private int needCount = 100;
    private int lastTexCount;
    public GameObject quadPrefab;
    public float separation = 1.5f;
    private List<GameObject> quads = new List<GameObject>();
    private List<Texture2D> textures = new List<Texture2D>();
    public Transform parent;

    private int MAX_BYTES_PER_FRAME = 16 * 1024 * 1024;

    private void Start()
    {
        MAX_BYTES_PER_FRAME = QualitySettings.texture2DReadBufferSize * 1024 * 1024;
        folderPath = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
        UpdateQuads();
    }

    void UpdateQuads()
    {
        for (int i = quads.Count - 1; i >= needCount; i--)
        {
            if (quads[i]) 
                Destroy(quads[i]);
            quads.RemoveAt(i);
            if (textures[i]) 
                Destroy(textures[i]);
            textures.RemoveAt(i);
        }

        for (int i = 0; i < needCount; i++)
        {
            if (i >= quads.Count)
            {
                GameObject newQuad = Instantiate(quadPrefab, new Vector3(i % 10 * separation, i / 10 * -separation, 0), Quaternion.identity);
                if (i == 0)
                {
                    newQuad.transform.localScale = new Vector3(10, 10, 1);
                    newQuad.transform.position = new Vector3(-6.5f, -7.0f, 0);
                }
                newQuad.transform.parent = parent;
                quads.Add(newQuad);

                Texture2D placeholderTex = new Texture2D(8, 8);
                placeholderTex.name = i.ToString();
                textures.Add(placeholderTex);
                newQuad.GetComponent<Renderer>().material.mainTexture = placeholderTex;
            }
            else
            {
                quads[i].transform.position = new Vector3(i % 10 * separation, i / 10 * -separation, 0);
            }
        }
    }

    NativeArray<byte> LoadCustomBytes(Texture2D texture, bool isHd, int? mipmapLevel)
    {
        string path = isHd ? Path.Combine(folderPath, texture.name + "_hd.bytes") : Path.Combine(folderPath, texture.name + "_ld.bytes");

        NativeArray<byte> bytes;
        if (mipmapLevel.HasValue == false)
            bytes = Texture2D.ReadTextureDataFromFile(path);
        else
            bytes = Texture2D.ReadTextureDataFromFile2(path, mipmapLevel.Value, texture);
        return bytes;
    }

    IEnumerator LoadAndUploadTextures(bool isHd, int? mipmapLevel)
    {
        int currentFrameBytes = 0;
        List<(Texture2D, NativeArray<byte>)> pendingUploads = new List<(Texture2D, NativeArray<byte>)>();
        
        int i = 0;
        while (i < textures.Count)
        {
            Texture2D tex = textures[i];

            NativeArray<byte> bytes;
            if (mipmapLevel.HasValue == false)
                bytes = LoadCustomBytes(tex, isHd, null);
            else
               bytes = LoadCustomBytes(tex, isHd, mipmapLevel.Value);

            if (bytes.IsCreated) //m_Buffer != null
            {
                pendingUploads.Add((tex, bytes));
                currentFrameBytes += bytes.Length;
            }
            if (bytes.IsCreated == false || currentFrameBytes >= MAX_BYTES_PER_FRAME)
            {
                foreach (var (texture, uploadBytes) in pendingUploads)
                {
                    if(mipmapLevel.HasValue == false)
                        texture.SetStreamedBinaryData(uploadBytes);
                    else
                        texture.ForceSetMipLevel4(mipmapLevel.Value, uploadBytes);
                }
                pendingUploads.Clear();
                currentFrameBytes = 0;

                yield return null;

                if (bytes.IsCreated == false)
                    continue; // 如果当前纹理加载失败，重试当前纹理
            }
            ++i;
        }
        //处理剩余数据
        foreach (var (texture, uploadBytes) in pendingUploads)
        {
            if (mipmapLevel.HasValue == false)
                texture.SetStreamedBinaryData(uploadBytes);
            else
                texture.ForceSetMipLevel4(mipmapLevel.Value, uploadBytes);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(LoadAndUploadTextures(false, null));
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(LoadAndUploadTextures(true, null));
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_loadMipmapLevel = Mathf.Clamp(--m_loadMipmapLevel, 0, textures[0].mipmapCount);
            StartCoroutine(LoadAndUploadTextures(true, m_loadMipmapLevel));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_loadMipmapLevel = Mathf.Clamp(++m_loadMipmapLevel, 0, textures[0].mipmapCount);
            StartCoroutine(LoadAndUploadTextures(true, m_loadMipmapLevel));
        }
    }
}
