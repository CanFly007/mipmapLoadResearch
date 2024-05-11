using System.Diagnostics;
using System.IO;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExternalProcessRunner : MonoBehaviour
{
    // 定义文件和路径
    private string executableName = @"D:\202405\TextureCompressionV1\bin\YaTCompress";
    private string inputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/input.png";
    private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/bc7.dds";
    //private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/astc6.astc";

    public Material quadMat;

    void Start()
    {
        // 调用外部程序
        //RunExternalExecutable();
    }

    private void Update()
    {
        //从头到尾来一遍
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //从input.png 到 bc7.dds
            RunExternalExecutable();

            //从dds到unity的Texture2D对象
            var bytes = File.ReadAllBytes(outputFilePath);
            Texture2D texture = LoadTextureDXT(bytes, TextureFormat.BC7);

            //从Texture2D对象 到 二进制文件
            BuildToLDAndHDBundle(texture);

            //测试：从二进制文件加载出来，显示这张图
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", "_hd.bytes");
            if (placeholderTex == null)
            {
                placeholderTex = new Texture2D(8, 8);
                quadMat.mainTexture = placeholderTex;
            }

            NativeArray<byte> hdBytes = Texture2D.ReadTextureDataFromFile(path);
            placeholderTex.SetStreamedBinaryData(hdBytes);
        }


        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            //从dds到unity的Texture2D对象
            var bytes = File.ReadAllBytes(outputFilePath);
            Texture2D texture = LoadTextureDXT(bytes, TextureFormat.BC7);
            //quadMat.mainTexture = texture;

            //从Texture2D对象 到 二进制文件
            BuildToLDAndHDBundle(texture);
        }



        ShowBinaryToUnityTexture2D();
    }

    void RunExternalExecutable()
    {
        // 获取绝对路径
        string absoluteInputPath = System.IO.Path.GetFullPath(inputFilePath);
        string absoluteOutputPath = System.IO.Path.GetFullPath(outputFilePath);

        // 设置进程信息
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = executableName;  // 或者使用完整路径
        startInfo.Arguments = $"bc 7 \"{absoluteInputPath}\" \"{absoluteOutputPath}\"";
        //startInfo.Arguments = $"astc 6x6 \"{absoluteInputPath}\" \"{absoluteOutputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            using (Process process = Process.Start(startInfo))
            {
                // 等待进程结束
                process.WaitForExit();

                // 检查进程返回值
                if (process.ExitCode == 0)
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

    Texture2D LoadTextureDXT(byte[] ddsBytes, TextureFormat format)
    {
        byte ddsSizeCheck = ddsBytes[4];
        if (ddsSizeCheck != 124)
            throw new System.Exception("Invalid DDS DXTn texture. Unable to read");  // 确保是DDS格式

        int height = ddsBytes[12] | ddsBytes[13] << 8 | ddsBytes[14] << 16 | ddsBytes[15] << 24;
        int width = ddsBytes[16] | ddsBytes[17] << 8 | ddsBytes[18] << 16 | ddsBytes[19] << 24;

        int DDS_HEADER_SIZE = 128;
        int DDS_HEADER_DX10_SIZE = 20;
        byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE - DDS_HEADER_DX10_SIZE];//减去bc7的头148个字节，后面是纯bc7数据
        System.Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE + DDS_HEADER_DX10_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE - DDS_HEADER_DX10_SIZE);

        Texture2D texture = new Texture2D(width, height, format, false); 
        texture.LoadRawTextureData(dxtBytes);//只把数据填充到m_TexData.data中即可
        texture.Apply();

        return texture;
    }

    void BuildToLDAndHDBundle(Texture2D texture2D)
    {
        Debug.Log("这张贴图只有一个mipmap，只会生成一个hd的二进制文件");
        int splitMipLevel = 9;//随便填一个，反正只有一个hd文件

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


    private Texture2D placeholderTex;
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
            string path = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes", "_hd.bytes");
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