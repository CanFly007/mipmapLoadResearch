using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExternalProcessRunner : MonoBehaviour
{
    // 定义文件和路径
    private string executableName = @"E:\202405\YahahaTextureCompressionV1\bin\YaTCompress";  //公司电脑
    //private string executableName = @"D:\202405\TextureCompressionV1\bin\YaTCompress"; //家中电脑


    public Material quadMat;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //step1: input.png -> x.dds
            string inputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/input.png";
            string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/bc3.dds";
            LaunchTextureCompression(inputFilePath, outputFilePath, CompressionType.BC3, new List<string>() { "-mipmap" });
            //astc
            //string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/a4_mip.astc";
            //LaunchTextureCompression(inputFilePath, outputFilePath, CompressionType.ASTC_4x4, new List<string>() { "-mipmap" });

            //step2: x.dds -> texture2D in memory
            var ddsBytes = File.ReadAllBytes(outputFilePath);
            Texture2D tex2D = DDSByteToTexture2D(ddsBytes);
            //Texture2D tex2D = ASTCByteToTexture2D(ddsBytes);

            //step3: texture2D in memory -> _ld.bytes and _hd.bytes
            BuildToLDAndHDBundle(tex2D);
        }

        //测试：从二进制文件加载出来，显示这张图
        ShowBinaryToUnityTexture2D("_ld.bytes", "_hd.bytes");
    }

    public enum CompressionType
    {
        BC1,
        BC3,
        BC7,
        ASTC_4x4,
        ASTC_5x5,
        ASTC_6x6
    }
    private static string GetCompressionArguments(CompressionType compressionType, string inputPath, string outputPath, List<string> additionalArgs)
    {
        string args = compressionType switch
        {
            CompressionType.BC1 => $"bc 1 \"{inputPath}\" \"{outputPath}\"",
            CompressionType.BC3 => $"bc 3 \"{inputPath}\" \"{outputPath}\"",
            CompressionType.BC7 => $"bc 7 \"{inputPath}\" \"{outputPath}\"",
            CompressionType.ASTC_4x4 => $"astc 4x4 \"{inputPath}\" \"{outputPath}\"",
            CompressionType.ASTC_5x5 => $"astc 5x5 \"{inputPath}\" \"{outputPath}\"",
            CompressionType.ASTC_6x6 => $"astc 6x6 \"{inputPath}\" \"{outputPath}\"",
            _ => throw new System.ArgumentOutOfRangeException(nameof(compressionType), "Unsupported compression type.")
        };

        if (additionalArgs != null && additionalArgs.Count > 0)
        {
            args += " " + string.Join(" ", additionalArgs);
        }

        return args;
    }

    void LaunchTextureCompression(string inputFilePath, string outputFilePath, CompressionType compressionType, List<string> additionalArgs = null)
    {
        // 获取绝对路径
        string absoluteInputPath = System.IO.Path.GetFullPath(inputFilePath);
        string absoluteOutputPath = System.IO.Path.GetFullPath(outputFilePath);

        // 设置进程信息
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = executableName;
        startInfo.Arguments = GetCompressionArguments(compressionType, absoluteInputPath, absoluteOutputPath, additionalArgs);
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();

                if (process.ExitCode == 0) // 检查进程返回值
                {
                    Debug.Log("Process completed successfully.");
                    Debug.Log("Generated file: " + absoluteOutputPath);
                }
                else
                {
                    Debug.LogError("Process failed: " + process.StandardError.ReadToEnd());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception occurred: " + e.Message);
        }
    }

    Texture2D DDSByteToTexture2D(byte[] ddsBytes)
    {
        byte ddsSizeCheck = ddsBytes[4];
        if (ddsSizeCheck != 124)
            throw new System.Exception("Invalid DDS DXTn texture. Unable to read");  // 确保是DDS格式

        int DDS_HEADER_SIZE = 128;
        int DDS_HEADER_DX10_SIZE = 20;

        int height = ddsBytes[12] | ddsBytes[13] << 8 | ddsBytes[14] << 16 | ddsBytes[15] << 24;
        int width = ddsBytes[16] | ddsBytes[17] << 8 | ddsBytes[18] << 16 | ddsBytes[19] << 24;
        int mipMapCount = ddsBytes[28] | ddsBytes[29] << 8 | ddsBytes[30] << 16 | ddsBytes[31] << 24;
        bool hasMipMaps = mipMapCount > 1;

        int pfFourCC = ddsBytes[84] | ddsBytes[85] << 8 | ddsBytes[86] << 16 | ddsBytes[87] << 24;
        bool hasDX10Header = (pfFourCC == ('D' | ('X' << 8) | ('1' << 16) | ('0' << 24)));

        TextureFormat format = TextureFormat.RGBA32;
        int headerSize = DDS_HEADER_SIZE;
        if (hasDX10Header)
        {
            headerSize += DDS_HEADER_DX10_SIZE;
            format = TextureFormat.BC7;
        }
        else
        {
            switch (pfFourCC)
            {
                case 0x31545844:
                    format = TextureFormat.DXT1;
                    break;
                case 0x35545844:
                    format = TextureFormat.DXT5;
                    break;
            }
        }

        byte[] dxtBytes = new byte[ddsBytes.Length - headerSize];//减去bc7的头148个字节，后面是纯bc7数据
        System.Buffer.BlockCopy(ddsBytes, headerSize, dxtBytes, 0, ddsBytes.Length - headerSize);

        Texture2D texture = new Texture2D(width, height, format, hasMipMaps); 
        texture.LoadRawTextureData(dxtBytes);//只把数据填充到m_TexData.data中即可
        texture.Apply();

        return texture;
    }

    private const int ASTC_HEADER_SIZE = 16;
    public Texture2D ASTCByteToTexture2D(byte[] astcBytes)
    {
        if (astcBytes[0] != 0x13 || astcBytes[1] != 0xAB || astcBytes[2] != 0xA1 || astcBytes[3] != 0x5C)
            throw new System.Exception("Invalid ASTC texture. Unable to read"); // 确保是ASTC格式

        int blockWidth = astcBytes[4];
        int blockHeight = astcBytes[5];

        // 读取纹理尺寸
        int width = astcBytes[9] | (astcBytes[8] << 8) | (astcBytes[7] << 16);
        int height = astcBytes[12] | (astcBytes[11] << 8) | (astcBytes[10] << 16);

        // 获取对应的TextureFormat
        TextureFormat format = GetASTCTextureFormat(blockWidth, blockHeight);

        // 计算无Mipmap的理论最小文件大小
        int blockSize = blockWidth * blockHeight * 16 / 8; // 每个块编码为128位
        int numBlocksWide = (width + blockWidth - 1) / blockWidth;
        int numBlocksHigh = (height + blockHeight - 1) / blockHeight;
        int minSize = numBlocksWide * numBlocksHigh * 16 + ASTC_HEADER_SIZE;

        // 确定是否有Mipmap
        bool hasMipMaps = astcBytes.Length > minSize;

        // 创建Texture2D对象
        Texture2D texture = new Texture2D(width, height, format, hasMipMaps);

        // 拷贝除头部外的数据
        byte[] astcData = new byte[astcBytes.Length - ASTC_HEADER_SIZE];
        System.Buffer.BlockCopy(astcBytes, ASTC_HEADER_SIZE, astcData, 0, astcBytes.Length - ASTC_HEADER_SIZE);

        // 加载纹理数据
        texture.LoadRawTextureData(astcData);
        texture.Apply();

        return texture;
    }
    private TextureFormat GetASTCTextureFormat(int blockWidth, int blockHeight)
    {
        if (blockWidth == 4 && blockHeight == 4) return TextureFormat.ASTC_4x4;
        if (blockWidth == 5 && blockHeight == 5) return TextureFormat.ASTC_5x5;
        if (blockWidth == 6 && blockHeight == 6) return TextureFormat.ASTC_6x6;
        throw new System.Exception($"Unsupported ASTC block size: {blockWidth}x{blockHeight}");
    }


    public static int splitMipLevel = 6;
    void BuildToLDAndHDBundle(Texture2D texture2D)
    {
        //Build to _ld and _hd files
        if (texture2D.mipmapCount == 1)
        {
            Debug.Log("这张贴图只有一个mipmap，只会生成一个hd的二进制文件");

            byte[] highResBytes = texture2D.GetStreamedBinaryData(true, splitMipLevel);
            string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            string highResFilePath = Path.Combine(folderPath, texture2D.name + "_hd.bytes");

            File.WriteAllBytes(highResFilePath, highResBytes);
        }
        else if (texture2D.mipmapCount < splitMipLevel + 2)
        {
            Debug.Log("要分离的splitMipLevel不是介于这张纹理的中间，请减少splitMipLevel后再分离，此次只会生成一个hd的二进制文件");

            byte[] highResBytes = texture2D.GetStreamedBinaryData(true, splitMipLevel);
            string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            string highResFilePath = Path.Combine(folderPath, texture2D.name + "_hd.bytes");

            File.WriteAllBytes(highResFilePath, highResBytes);
        }
        else
        {
            byte[] lowResBytes = texture2D.GetStreamedBinaryData(false, splitMipLevel);
            byte[] highResBytes = texture2D.GetStreamedBinaryData(true, splitMipLevel);

            string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            string lowResFilePath = Path.Combine(folderPath, texture2D.name + "_ld.bytes");
            string highResFilePath = Path.Combine(folderPath, texture2D.name + "_hd.bytes");

            File.WriteAllBytes(lowResFilePath, lowResBytes);
            File.WriteAllBytes(highResFilePath, highResBytes);
        }
    }


    private Texture2D placeholderTex;
    void ShowBinaryToUnityTexture2D(string ldPath, string hdPath)
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", ldPath);
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
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", hdPath);
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