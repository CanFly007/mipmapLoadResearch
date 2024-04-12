using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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



    public int needCount = 100;
    private int lastTexCount;
    public GameObject quadPrefab;
    public float separation = 1.5f;
    private List<GameObject> quads = new List<GameObject>();
    private List<Texture2D> textures = new List<Texture2D>();
    public Transform parent;

    private void Start()
    {
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

    byte[] LoadCustomBytes(Texture2D texture, bool isHd)
    {
        string path = isHd ? Path.Combine(folderPath, texture.name + "_hd.bytes") : Path.Combine(folderPath, texture.name + "_ld.bytes");
        byte[] bytes = File.ReadAllBytes(path);
        return bytes;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < textures.Count; ++i)
            {
                var tex2D = textures[i];
                byte[] bytes = LoadCustomBytes(tex2D, false);
                tex2D.SetStreamedBinaryData(bytes, true);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < textures.Count; ++i)
            {
                var tex2D = textures[i];
                byte[] bytes = LoadCustomBytes(tex2D, true);
                tex2D.SetStreamedBinaryData(bytes, true);
            }
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_loadMipmapLevel = Mathf.Clamp(--m_loadMipmapLevel, 0, textures[0].mipmapCount);
            for (int i = 0; i < textures.Count; ++i)
            {
                var tex2D = textures[i];
                if (tex2D)
                {
                    byte[] bytes = LoadCustomBytes(tex2D, true);
                    tex2D.ForceSetMipLevel2(m_loadMipmapLevel, bytes);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_loadMipmapLevel = Mathf.Clamp(++m_loadMipmapLevel, 0, textures[0].mipmapCount);
            for (int i = 0; i < textures.Count; ++i)
            {
                var tex2D = textures[i];
                if (tex2D)
                {
                    byte[] bytes = LoadCustomBytes(tex2D, true);
                    tex2D.ForceSetMipLevel2(m_loadMipmapLevel, bytes);
                }
            }
        }
    }
}
