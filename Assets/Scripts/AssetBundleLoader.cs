using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ASBundleWrapper
{
    public Object Asset { get; set; }
    public Object[] AllAsset { get; set; }

}

public class AssetBundleLoader : MonoBehaviour
{
    private static int m_loadMipmapLevel = 0;

    private GameObject teaportGo;

    //public Texture2D testInsteadABTex;
    public Texture2D anotherNullTex;
    public Material insteadABMat;
    public string baseFileName = "Amazing Speed_Floor_D_ld";
    private string folderPath;

    private void Awake()
    {
        folderPath = Path.Combine(Application.dataPath, "TextureBytes");
    }

    private void OnDestroy()
    {
        insteadABMat.mainTexture = null;
    }

    void Update()
    {
        //Step2 绕过AB，使用二进制代替
        if(Input.GetKeyDown(KeyCode.W))
        {
            string lowDefFileName = baseFileName;
            string lowDefFilePath = Path.Combine(folderPath, lowDefFileName + ".bytes");

            byte[] ldBytes = File.ReadAllBytes(lowDefFilePath);
            anotherNullTex = new Texture2D(512, 512);
            anotherNullTex.SetStreamedBinaryData(ldBytes, true); //first?
            insteadABMat.mainTexture = anotherNullTex;
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            string highDefFileName = baseFileName;
            if (baseFileName.EndsWith("_ld"))
                highDefFileName = baseFileName.Substring(0, baseFileName.Length - 3) + "_hd";
            string highDefFilePath = Path.Combine(folderPath, highDefFileName + ".bytes");

            byte[] hdBytes = File.ReadAllBytes(highDefFilePath);
            anotherNullTex.SetStreamedBinaryData(hdBytes, false); //append?
        }



        //Step1 贴图和prefab分开打AB包，纹理是一个单独的AB包
        if (Input.GetKeyDown(KeyCode.H))
        {
            AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
            AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));
            
            Object[] objects = prefabBundle.LoadAllAssets();
            teaportGo = Instantiate(objects[0] as GameObject);

            prefabBundle.Unload(false);
            texBundle.Unload(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
            AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));

            Object[] objects = prefabBundle.LoadAllAssets(new AssetLoadParameters(false, 6));
            teaportGo = Instantiate(objects[0] as GameObject);

            prefabBundle.Unload(false);
            texBundle.Unload(false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (teaportGo != null)
            {
                --m_loadMipmapLevel;
                string path = Path.Combine(Application.streamingAssetsPath, "alienTex");
                StartCoroutine(ForceSetMipLevel(m_loadMipmapLevel, path));
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (teaportGo != null)
            {
                ++m_loadMipmapLevel;
                string path = Path.Combine(Application.streamingAssetsPath, "alienTex");
                StartCoroutine(ForceSetMipLevel(m_loadMipmapLevel, path));
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (teaportGo)
            {                
                DestroyImmediate(teaportGo);
                teaportGo = null;
            }
            Resources.UnloadUnusedAssets();
        }
    }

    IEnumerator ForceSetMipLevel(int mipmapLevel,string abPath)
    {
        Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;

        AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(abPath);
        yield return assetBundleCreateRequest;

        texture.ForceSetMipLevel(m_loadMipmapLevel, "");
        assetBundleCreateRequest.assetBundle.Unload(true);
    }
}
