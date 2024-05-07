using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class TextureProcessor : MonoBehaviour
{

    public Material mat;

    [StructLayout(LayoutKind.Sequential)]
    public struct RgbaSurface
    {
        public IntPtr ptr;
        public int width;
        public int height;
        public int stride;
    }


    private string imagePath = @"E:\202404\MyCompress01\build\ori.png";

    void Start()
    {
        Texture2D texture = LoadAndConvertImage(imagePath);
        if (texture != null)
        {
            byte[] rgba32Data = ConvertToRGBA32(texture);
            // 在这里，rgba32Data包含了纯RGBA32格式的像素数据


            int width = texture.width;
            int height = texture.height;

            // 分配非托管内存
            IntPtr unmanagedInput = Marshal.AllocHGlobal(rgba32Data.Length);
            Marshal.Copy(rgba32Data, 0, unmanagedInput, rgba32Data.Length);

            // 输出缓冲区也需要分配，BC1每个块压缩到1byte，每块涵盖4x4像素
            int blockSize = 16; // BC3压缩到16字节每4x4块
            int num_blocks_wide = (width + 3) / 4;
            int num_blocks_high = (height + 3) / 4;
            int blockCount = num_blocks_wide * num_blocks_high;
            IntPtr unmanagedOutput = Marshal.AllocHGlobal(blockCount * blockSize);

            // 创建rgba_surface实例并填充数据
            RgbaSurface surfaceInput = new RgbaSurface
            {
                ptr = unmanagedInput,
                width = width,
                height = height,
                stride = width * 4 // 4 bytes per pixel
            };

            // 为RgbaSurface结构体分配非托管内存
            IntPtr unmanagedSurfaceInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RgbaSurface)));
            // 将结构体数据复制到分配的内存中
            Marshal.StructureToPtr(surfaceInput, unmanagedSurfaceInput, false);

            TextureCompression.CompressBlocksBC3(unmanagedSurfaceInput, unmanagedOutput);


            var compressedTexture = new Texture2D(width, height, TextureFormat.DXT5, false);
            // 创建一个托管数组来存储压缩数据
            byte[] compressedData = new byte[blockCount * blockSize];

            // 从非托管内存复制到托管数组
            Marshal.Copy(unmanagedOutput, compressedData, 0, compressedData.Length);

            // 将压缩数据加载到纹理
            compressedTexture.LoadRawTextureData(compressedData);
            compressedTexture.Apply();
            
            mat.mainTexture = compressedTexture;



            Marshal.FreeHGlobal(unmanagedSurfaceInput);
            Marshal.FreeHGlobal(unmanagedInput);
            Marshal.FreeHGlobal(unmanagedOutput);
        }
    }

    // 加载并转换图像为Texture2D
    private Texture2D LoadAndConvertImage(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Image file not found: " + path);
            return null;
        }

        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (texture.LoadImage(fileData)) // 这会自动将图像转换为纹理
        {
            Debug.Log("Image loaded and converted successfully");
            return texture;
        }
        else
        {
            Debug.LogError("Failed to load image.");
            return null;
        }
    }

    // 从Texture2D中提取RGBA32数据
    private byte[] ConvertToRGBA32(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        byte[] result = new byte[pixels.Length * 4];
        for (int i = 0; i < pixels.Length; i++)
        {
            int index = i * 4;
            result[index] = pixels[i].r;
            result[index + 1] = pixels[i].g;
            result[index + 2] = pixels[i].b;
            result[index + 3] = pixels[i].a;
        }
        return result;
    }

}