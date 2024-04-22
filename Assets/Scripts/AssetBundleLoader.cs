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
        //Step2 绕过AB，使用二进制代替
        if(Input.GetKeyDown(KeyCode.L))
        {
            string lowDefFileName = baseFileName;
            string lowDefFilePath = Path.Combine(folderPath, lowDefFileName + ".bytes");

            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            byte[] ldBytes = File.ReadAllBytes(lowDefFilePath);
            placeholderTex.SetStreamedBinaryData(ldBytes, true); 
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            string highDefFileName = baseFileName;
            if (baseFileName.EndsWith("_ld"))
                highDefFileName = baseFileName.Substring(0, baseFileName.Length - 3) + "_hd";
            string highDefFilePath = Path.Combine(folderPath, highDefFileName + ".bytes");

            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            byte[] hdBytes = File.ReadAllBytes(highDefFilePath);
            placeholderTex.SetStreamedBinaryData(hdBytes, false); 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (placeholderTex)
            {
                DestroyImmediate(placeholderTex);
                placeholderTex = null;
            }
            insteadABMat.mainTexture = null;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (placeholderTex)
            {
                string highDefFileName = baseFileName;
                if (baseFileName.EndsWith("_ld"))
                    highDefFileName = baseFileName.Substring(0, baseFileName.Length - 3) + "_hd";
                string highDefFilePath = Path.Combine(folderPath, highDefFileName + ".bytes");
                byte[] hdBytes = File.ReadAllBytes(highDefFilePath);

                --m_loadMipmapLevel;
                placeholderTex.ForceSetMipLevel2(m_loadMipmapLevel, hdBytes);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (placeholderTex)
            {
                string highDefFileName = baseFileName;
                if (baseFileName.EndsWith("_ld"))
                    highDefFileName = baseFileName.Substring(0, baseFileName.Length - 3) + "_hd";
                string highDefFilePath = Path.Combine(folderPath, highDefFileName + ".bytes");
                byte[] hdBytes = File.ReadAllBytes(highDefFilePath);

                ++m_loadMipmapLevel;
                placeholderTex.ForceSetMipLevel2(m_loadMipmapLevel, hdBytes);
            }
        }







        //调试AB
        if (Input.GetKeyDown(KeyCode.T))
        {
            AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
            AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));

            Object[] objects = prefabBundle.LoadAllAssets();
            teaportGo = Instantiate(objects[0] as GameObject);

            prefabBundle.Unload(false);
            texBundle.Unload(false);
        }




        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
            string path1 = Path.Combine(path0, "0_ld.bytes");

            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> ldBytes = Texture2D.ReadTextureDataFromFile(path1);
            placeholderTex.SetStreamedBinaryData(ldBytes); //用这个api
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
            string path1 = Path.Combine(path0, "0_hd.bytes");

            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> hdBytes = Texture2D.ReadTextureDataFromFile(path1);
            placeholderTex.SetStreamedBinaryData(hdBytes);
        }


        if (Input.GetKeyDown(KeyCode.Alpha9)) //参照组，和之前File.ReadAllBytes读出来数据相同
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
            string path1 = Path.Combine(path0, "0_ld.bytes");
            byte[] hdBytes = File.ReadAllBytes(path1);
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }
            m_loadMipmapLevel = 4;
            placeholderTex.ForceSetMipLevel2(m_loadMipmapLevel, hdBytes);
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
            string path1 = Path.Combine(path0, "0_hd.bytes");
            var nativeArray = Texture2D.ReadTextureDataFromFile(path1);
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }
            --m_loadMipmapLevel;
            placeholderTex.ForceSetMipLevel3(m_loadMipmapLevel, nativeArray);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
            string path1 = Path.Combine(path0, "0_hd.bytes");
            var nativeArray = Texture2D.ReadTextureDataFromFile(path1);
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }
            ++m_loadMipmapLevel;
            placeholderTex.ForceSetMipLevel3(m_loadMipmapLevel, nativeArray);
        }






        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 创建图像的宽度和高度
            uint width = 256;
            uint height = 256;

            // 创建NativeArray，并填充纯色数据
            NativeArray<Color32> imageData = new NativeArray<Color32>((int)(width * height), Allocator.TempJob);
            Color32 fillColor = new Color32(255, 0, 0, 255); // 红色

            for (int i = 0; i < imageData.Length; i++)
            {
                imageData[i] = fillColor;
            }

            // 指定图像格式
            UnityEngine.Experimental.Rendering.GraphicsFormat format = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;

            // 调用EncodeNativeArrayToPNG来编码图像数据为PNG
            NativeArray<byte> pngData = ImageConversion.EncodeNativeArrayToPNG(imageData, format, width, height);

            // 保存PNG数据到文件
            //SavePNG(pngData, "output.png");

            // 释放分配的内存资源
            imageData.Dispose();
            pngData.Dispose();
        }



        //Old Code with AssetBundle
        ////Step1 贴图和prefab分开打AB包，纹理是一个单独的AB包
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
        //    AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));

            //    Object[] objects = prefabBundle.LoadAllAssets();
            //    teaportGo = Instantiate(objects[0] as GameObject);

            //    prefabBundle.Unload(false);
            //    texBundle.Unload(false);
            //}
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
            //    AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));

            //    Object[] objects = prefabBundle.LoadAllAssets(new AssetLoadParameters(false, 6));
            //    teaportGo = Instantiate(objects[0] as GameObject);

            //    prefabBundle.Unload(false);
            //    texBundle.Unload(false);
            //}

            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    if (teaportGo != null)
            //    {
            //        --m_loadMipmapLevel;
            //        string path = Path.Combine(Application.streamingAssetsPath, "alienTex");
            //        StartCoroutine(ForceSetMipLevel(m_loadMipmapLevel, path));
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    if (teaportGo != null)
            //    {
            //        ++m_loadMipmapLevel;
            //        string path = Path.Combine(Application.streamingAssetsPath, "alienTex");
            //        StartCoroutine(ForceSetMipLevel(m_loadMipmapLevel, path));
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    if (teaportGo)
            //    {                
            //        DestroyImmediate(teaportGo);
            //        teaportGo = null;
            //    }
            //    Resources.UnloadUnusedAssets();
            //}
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
