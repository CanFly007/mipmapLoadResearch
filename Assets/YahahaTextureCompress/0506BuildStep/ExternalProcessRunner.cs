using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExternalProcessRunner : MonoBehaviour
{
    // 定义文件和路径
    private string executableName = @"E:\202405\YahahaTextureCompressionV1\bin\YaTCompress";
    private string inputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/input.png";
    private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/bc7.dds";
    //private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/astc6.astc";

    void Start()
    {
        // 调用外部程序
        RunExternalExecutable();
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





                    Debug.Log("这张贴图只有一个mipmap，只会生成一个hd的二进制文件");
                    
                    //Texture2D compressedTexture = new Texture2D(2, 2, TextureFormat.BC7, false);
                    //byte[] highResBytes = compressedTexture.GetStreamedBinaryData(true, splitMipLevel);
                    //string folderPath = Path.Combine(Application.streamingAssetsPath, "BuildTextureBytes");
                    //if (!Directory.Exists(folderPath))
                    //{
                    //    Directory.CreateDirectory(folderPath);
                    //}
                    //else
                    //{
                    //    DirectoryInfo di = new DirectoryInfo(folderPath);
                    //    foreach (FileInfo file in di.GetFiles())
                    //    {
                    //        file.Delete();
                    //    }
                    //}
                    //string highResFilePath = Path.Combine(folderPath, compressedTexture.name + "_hd.bytes");

                    //File.WriteAllBytes(highResFilePath, highResBytes);


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
}