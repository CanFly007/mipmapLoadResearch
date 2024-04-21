using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TextureCompress : MonoBehaviour 
{
    private string toolPath;
    private string pngPath;

    private string crunchToolPath;

    void Start()
    {
        string appData = Path.Combine(Application.dataPath, "Tools");
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                toolPath = Path.Combine(appData, "astcenc-sse2.exe");
                break;
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                toolPath = Path.Combine(appData, "astcenc-sse2-arm64");
                break;
        }
        if (!File.Exists(toolPath))
            throw new FileNotFoundException($"Cannot find astc encoder at {Path.GetFullPath(toolPath)}.");

        //crunch.exe path
        crunchToolPath = Path.Combine(appData, "crunch.exe");
        if (!File.Exists(crunchToolPath))
            throw new FileNotFoundException($"Cannot find dxt encoder at {Path.GetFullPath(crunchToolPath)}.");

        //图片
        pngPath = Path.Combine(Application.dataPath, "Images", "SampleTexture.png");
        if (!File.Exists(pngPath))
        {
            throw new FileNotFoundException($"Cannot find image at {Path.GetFullPath(pngPath)}.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AstcCompressAsync(pngPath).Forget();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            DXTCompressAsync(pngPath).Forget();
        }
    }

    async UniTaskVoid AstcCompressAsync(string pngPath)
    {
        await AstcCompress(pngPath);
    }

    async UniTask AstcCompress(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File is not exists: " + path);
            return;
        }

        int extIndex = path.LastIndexOf('.');
        string outputPath = (extIndex != -1) ? path.Substring(0, extIndex) + ".astc" : path + ".astc";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = toolPath,
            Arguments = $" -cl \"{path}\" \"{outputPath}\" 6x6 -medium -yflip",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            await UniTask.WaitUntil(() => process.HasExited); 

            if (process.ExitCode == 0)
            {
                Debug.Log("Texture compressed successfully.");
            }
            else
            {
                Debug.LogError("Texture compression failed.");
            }
        }
    }

    async UniTaskVoid DXTCompressAsync(string pngPath)
    {
        await CrunchCompress(pngPath);
    }

    async UniTask CrunchCompress(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File is not exists: " + path);
            return;
        }

        string outputDirectory = Path.Combine(Application.dataPath, "Images");
        string outputPath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(path) + ".dds");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = crunchToolPath,
            Arguments = $"-file \"{path}\" -fileformat dds -dxt1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = outputDirectory
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await UniTask.WaitUntil(() => process.HasExited);

            if (process.ExitCode == 0)
            {
                Debug.Log("Texture compressed successfully.");
                Debug.Log("Output File: " + outputPath);
            }
            else
            {
                Debug.LogError("Texture compression failed.");
                Debug.LogError("Error: " + error);
            }
        }
    }
}
