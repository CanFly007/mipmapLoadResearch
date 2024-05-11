using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExternalProcessRunner : MonoBehaviour
{
    // �����ļ���·��
    private string executableName = @"E:\202405\YahahaTextureCompressionV1\bin\YaTCompress";
    private string inputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/input.png";
    private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/bc7.dds";
    //private string outputFilePath = "Assets/YahahaTextureCompress/0506BuildStep/astc6.astc";

    void Start()
    {
        // �����ⲿ����
        RunExternalExecutable();
    }

    void RunExternalExecutable()
    {
        // ��ȡ����·��
        string absoluteInputPath = System.IO.Path.GetFullPath(inputFilePath);
        string absoluteOutputPath = System.IO.Path.GetFullPath(outputFilePath);

        // ���ý�����Ϣ
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = executableName;  // ����ʹ������·��
        startInfo.Arguments = $"bc 7 \"{absoluteInputPath}\" \"{absoluteOutputPath}\"";
        //startInfo.Arguments = $"astc 6x6 \"{absoluteInputPath}\" \"{absoluteOutputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            using (Process process = Process.Start(startInfo))
            {
                // �ȴ����̽���
                process.WaitForExit();

                // �����̷���ֵ
                if (process.ExitCode == 0)
                {
                    Debug.Log("Process completed successfully.");
                    Debug.Log("Generated file: " + absoluteOutputPath);





                    Debug.Log("������ͼֻ��һ��mipmap��ֻ������һ��hd�Ķ������ļ�");
                    
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