using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
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

    public Texture2D placeholderTex;
    public Material insteadABMat;
    public string baseFileName = "Amazing Speed_Floor_D_ld";
    private string folderPath;

    private void Awake()
    {
        folderPath = Path.Combine(Application.streamingAssetsPath, "TextureBytes");
    }

    private void OnDestroy()
    {
        insteadABMat.mainTexture = null;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes", "Amazing Speed_Floor_D_hd.bytes");
            //if (placeholderTex == null)
            //{
            //    placeholderTex = new Texture2D(8, 8);
            //    insteadABMat.mainTexture = placeholderTex;
            //}

            NativeArray<byte> bytes = Texture2D.ReadTextureDataFromFile(path, 5, placeholderTex);//placeholderTex必须之前加载过，有descriptor
            placeholderTex.ForceSetMipLevel(5, bytes);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes", "Amazing Speed_Floor_D_ld.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> ldBytes = Texture2D.ReadTextureDataFromFile(path);
            placeholderTex.SetStreamedBinaryData(ldBytes);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes", "Amazing Speed_Floor_D_hd.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> hdBytes = Texture2D.ReadTextureDataFromFile(path);
            placeholderTex.SetStreamedBinaryData(hdBytes);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes", "Amazing Speed_Floor_D_hd.bytes");
            --m_loadMipmapLevel;
            //必需已经创建过了placeholderTex
            var nativeArray = Texture2D.ReadTextureDataFromFile(path, m_loadMipmapLevel,placeholderTex);
            placeholderTex.ForceSetMipLevel(m_loadMipmapLevel, nativeArray);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes", "Amazing Speed_Floor_D_hd.bytes");
            ++m_loadMipmapLevel;
            //必需已经创建过了placeholderTex
            var nativeArray = Texture2D.ReadTextureDataFromFile(path, m_loadMipmapLevel, placeholderTex);
            placeholderTex.ForceSetMipLevel(m_loadMipmapLevel, nativeArray);
        }
    }
}
