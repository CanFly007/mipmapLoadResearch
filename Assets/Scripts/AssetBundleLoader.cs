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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (placeholderTex)
            {
                DestroyImmediate(placeholderTex);
                placeholderTex = null;
            }
            insteadABMat.mainTexture = null;
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes100", "0_ld.bytes");
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
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes100", "0_hd.bytes");
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
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes100", "0_hd.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            var nativeArray = Texture2D.ReadTextureDataFromFile(path);
            --m_loadMipmapLevel;
            placeholderTex.ForceSetMipLevel3(m_loadMipmapLevel, nativeArray);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "TextureBytes100", "0_hd.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            var nativeArray = Texture2D.ReadTextureDataFromFile(path);
            ++m_loadMipmapLevel;
            placeholderTex.ForceSetMipLevel3(m_loadMipmapLevel, nativeArray);
        }




        if (Input.GetKeyDown(KeyCode.Alpha9)) //参照组，和File.ReadAllBytes读出来数据相同
        {
            string path0 = Path.Combine(Application.streamingAssetsPath, "TextureBytes100", "0_ld.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                insteadABMat.mainTexture = placeholderTex;
            }

            byte[] hdBytes = File.ReadAllBytes(path0);
            m_loadMipmapLevel = 4;
            placeholderTex.ForceSetMipLevel2(m_loadMipmapLevel, hdBytes);
        }


        //测试unity api EncodeNativeArrayToPNG是什么
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
    }
}
