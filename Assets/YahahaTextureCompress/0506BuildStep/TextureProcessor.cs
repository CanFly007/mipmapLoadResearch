using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public class TextureProcessor : MonoBehaviour
{

    public Material quadMat;
    private Texture2D placeholderTex;

    [StructLayout(LayoutKind.Sequential)]
    public struct RgbaSurface
    {
        public IntPtr ptr;
        public int width;
        public int height;
        public int stride;
    }


    private string imagePath = @"E:\202404\MyCompress01\build\ori.png";

    public static int splitMipLevel = 6;

    void Start()
    {
        //BuildTextureBytes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BuildTextureBytes();
        }

        //展示Show，测试二进制文件可以正确变成unity texture2d
        ShowBinaryToUnityTexture2D();
    }

    public byte[] GenerateMipmap(byte[] input, int inputWidth, int inputHeight, int channels, out int outputWidth, out int outputHeight)
    {
        // 计算输出大小
        outputWidth = inputWidth / 2;
        outputHeight = inputHeight / 2;
        if (outputWidth == 0) outputWidth = 1;
        if (outputHeight == 0) outputHeight = 1;

        // 分配输出图像数组
        byte[] output = new byte[outputWidth * outputHeight * channels];

        // 调用缩放函数
        StbImageLoader.MyStbirResizeUint8(input, inputWidth, inputHeight, 0, output, outputWidth, outputHeight, 0, channels);

        return output;
    }

    void BuildTextureBytes()
    {
        //Texture2D texture = LoadAndConvertImage(imagePath);
        //if (texture != null)
        //{
        //    byte[] rgba32Data = ConvertToRGBA32(texture);
        //    // 在这里，rgba32Data包含了纯RGBA32格式的像素数据


        //    int width = texture.width;
        //    int height = texture.height;



        //    // 初始化 RGBA32 数据和 Mipmap 数组
        //    byte[][] mipmaps = new byte[32][]; // 假设最大 Mipmap 级别为 32
        //    mipmaps[0] = rgba32Data;
        //    int num_levels = 1;
        //    int currentWidth = width;
        //    int currentHeight = height;

        //    while (currentWidth > 1 || currentHeight > 1)
        //    {
        //        int newWidth = Math.Max(1, currentWidth / 2);
        //        int newHeight = Math.Max(1, currentHeight / 2);

        //        mipmaps[num_levels] = GenerateMipmap(mipmaps[num_levels - 1], currentWidth, currentHeight, 4, out newWidth, out newHeight);
        //        currentWidth = newWidth;
        //        currentHeight = newHeight;
        //        num_levels++;

        //        if (newWidth == 1 && newHeight == 1) 
        //            break;
        //    }

        //    Texture2D compressedTexture = new Texture2D(width, height, TextureFormat.DXT5, true);
        //    compressedTexture.name = "dxt5";

        //    for (int i = 0; i < num_levels; i++)
        //    {
        //        currentWidth = Math.Max(1, width >> i);  // 右移操作用于计算 Mipmap 级别的宽度
        //        currentHeight = Math.Max(1, height >> i);  // 右移操作用于计算 Mipmap 级别的高度

        //        IntPtr unmanagedInput = Marshal.AllocHGlobal(mipmaps[i].Length);
        //        Marshal.Copy(mipmaps[i], 0, unmanagedInput, mipmaps[i].Length);

        //        int blockSize = 16; // BC3 压缩到 16 字节每 4x4 块
        //        int num_blocks_wide = (currentWidth + 3) / 4;
        //        int num_blocks_high = (currentHeight + 3) / 4;
        //        int blockCount = num_blocks_wide * num_blocks_high;
        //        IntPtr unmanagedOutput = Marshal.AllocHGlobal(blockCount * blockSize);

        //        RgbaSurface surfaceInput = new RgbaSurface
        //        {
        //            ptr = unmanagedInput,
        //            width = currentWidth,
        //            height = currentHeight,
        //            stride = currentWidth * 4 // 4 bytes per pixel
        //        };

        //        IntPtr unmanagedSurfaceInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RgbaSurface)));
        //        Marshal.StructureToPtr(surfaceInput, unmanagedSurfaceInput, false);

        //        TextureCompression.CompressBlocksBC3(unmanagedSurfaceInput, unmanagedOutput);

        //        byte[] compressedData = new byte[blockCount * blockSize];
        //        Marshal.Copy(unmanagedOutput, compressedData, 0, compressedData.Length);

        //        //compressedTexture.LoadRawTextureData(compressedData);
        //        compressedTexture.LoadRawTextureData(unmanagedOutput, blockCount * blockSize, i);

        //        Marshal.FreeHGlobal(unmanagedInput);
        //        Marshal.FreeHGlobal(unmanagedOutput);
        //        Marshal.FreeHGlobal(unmanagedSurfaceInput);
        //    }


        //    //// 分配非托管内存
        //    //IntPtr unmanagedInput = Marshal.AllocHGlobal(rgba32Data.Length);
        //    //Marshal.Copy(rgba32Data, 0, unmanagedInput, rgba32Data.Length);

        //    //// 输出缓冲区也需要分配，BC1每个块压缩到1byte，每块涵盖4x4像素
        //    //int blockSize = 16; // BC3压缩到16字节每4x4块
        //    //int num_blocks_wide = (width + 3) / 4;
        //    //int num_blocks_high = (height + 3) / 4;
        //    //int blockCount = num_blocks_wide * num_blocks_high;
        //    //IntPtr unmanagedOutput = Marshal.AllocHGlobal(blockCount * blockSize);

        //    //// 创建rgba_surface实例并填充数据
        //    //RgbaSurface surfaceInput = new RgbaSurface
        //    //{
        //    //    ptr = unmanagedInput,
        //    //    width = width,
        //    //    height = height,
        //    //    stride = width * 4 // 4 bytes per pixel
        //    //};

        //    //// 为RgbaSurface结构体分配非托管内存，第二句：将结构体数据复制到分配的内存中
        //    //IntPtr unmanagedSurfaceInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RgbaSurface)));            
        //    //Marshal.StructureToPtr(surfaceInput, unmanagedSurfaceInput, false);

        //    //TextureCompression.CompressBlocksBC3(unmanagedSurfaceInput, unmanagedOutput);


        //    //var compressedTexture = new Texture2D(width, height, TextureFormat.DXT5, false);
        //    //compressedTexture.name = "dxt5";
        //    //// 创建一个托管数组来存储压缩数据
        //    //byte[] compressedData = new byte[blockCount * blockSize];

        //    //// 从非托管内存复制到托管数组
        //    //Marshal.Copy(unmanagedOutput, compressedData, 0, compressedData.Length);

        //    //// 将压缩数据加载到纹理
        //    //compressedTexture.LoadRawTextureData(compressedData); //这个就是将compressedData拷贝给m_TexData.data()
        //    //compressedTexture.Apply();

        //    ////测试这张图是否成功
        //    //quadMat.mainTexture = compressedTexture;


        //    //Marshal.FreeHGlobal(unmanagedSurfaceInput);
        //    //Marshal.FreeHGlobal(unmanagedInput);
        //    //Marshal.FreeHGlobal(unmanagedOutput);

        //    //Build to _ld and _hd files
        //    if (compressedTexture.mipmapCount == 1)
        //    {
        //        Debug.Log("这张贴图只有一个mipmap，只会生成一个hd的二进制文件");

        //        byte[] highResBytes = compressedTexture.GetStreamedBinaryData(true, splitMipLevel);
        //        string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }
        //        else
        //        {
        //            DirectoryInfo di = new DirectoryInfo(folderPath);
        //            foreach (FileInfo file in di.GetFiles())
        //            {
        //                file.Delete();
        //            }
        //        }
        //        string highResFilePath = Path.Combine(folderPath, compressedTexture.name + "_hd.bytes");

        //        File.WriteAllBytes(highResFilePath, highResBytes);
        //    }
        //    else if (compressedTexture.mipmapCount < splitMipLevel + 2)
        //    {
        //        Debug.Log("要分离的splitMipLevel不是介于这张纹理的中间，请减少splitMipLevel后再分离，此次只会生成一个hd的二进制文件");

        //        byte[] highResBytes = compressedTexture.GetStreamedBinaryData(true, splitMipLevel);
        //        string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }
        //        else
        //        {
        //            DirectoryInfo di = new DirectoryInfo(folderPath);
        //            foreach (FileInfo file in di.GetFiles())
        //            {
        //                file.Delete();
        //            }
        //        }
        //        string highResFilePath = Path.Combine(folderPath, compressedTexture.name + "_hd.bytes");

        //        File.WriteAllBytes(highResFilePath, highResBytes);
        //    }
        //    else
        //    {
        //        byte[] lowResBytes = compressedTexture.GetStreamedBinaryData(false, splitMipLevel);
        //        byte[] highResBytes = compressedTexture.GetStreamedBinaryData(true, splitMipLevel);

        //        string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }
        //        else
        //        {
        //            DirectoryInfo di = new DirectoryInfo(folderPath);
        //            foreach (FileInfo file in di.GetFiles())
        //            {
        //                file.Delete();
        //            }
        //        }
        //        string lowResFilePath = Path.Combine(folderPath, compressedTexture.name + "_ld.bytes");
        //        string highResFilePath = Path.Combine(folderPath, compressedTexture.name + "_hd.bytes");

        //        File.WriteAllBytes(lowResFilePath, lowResBytes);
        //        File.WriteAllBytes(highResFilePath, highResBytes);
        //    }
        //}
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

    void ShowBinaryToUnityTexture2D()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", "dxt5_ld.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                quadMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> ldBytes = Texture2D.ReadTextureDataFromFile(path);
            placeholderTex.SetStreamedBinaryData(ldBytes);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", "dxt5_hd.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                quadMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> hdBytes = Texture2D.ReadTextureDataFromFile(path);
            placeholderTex.SetStreamedBinaryData(hdBytes);
        }
    }
}