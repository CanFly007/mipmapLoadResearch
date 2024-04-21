using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        //copy Tools/astcenc-sse2.exe to _Data/Tools/astcenc-sse2.exe
        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        string outputDir = Path.GetDirectoryName(report.summary.outputPath);
        if (activeBuildTarget == BuildTarget.StandaloneOSX)
        {
            outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(report.summary.outputPath) + ".app");
        }

        string exeName = activeBuildTarget == BuildTarget.StandaloneOSX ? "astcenc-sse2-arm64" : "astcenc-sse2.exe";

        string sourcePath = Path.Combine(Application.dataPath, "Tools", exeName);
        string destinationToolsDir = Path.Combine(outputDir, Application.productName + "_Data", "Tools");
        string targetPath = Path.Combine(destinationToolsDir, exeName);

        if (activeBuildTarget == BuildTarget.StandaloneOSX)
        {
            string destinationEncoderDir = Path.Combine(report.summary.outputPath, "Contents", "Tools");
            targetPath = Path.Combine(destinationEncoderDir, exeName);
        }

        if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

        if (File.Exists(sourcePath))
            File.Copy(sourcePath, targetPath, true);
        else
            Debug.LogError("Tool not found: " + sourcePath);



        //copy Tools/crunch.exe to _Data/Tools/crunch.exe
        //BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        //string outputDir = Path.GetDirectoryName(report.summary.outputPath);
        //if (activeBuildTarget == BuildTarget.StandaloneOSX)
        //{
        //    outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(report.summary.outputPath) + ".app");
        //}

        exeName = "crunch.exe";

        sourcePath = Path.Combine(Application.dataPath, "Tools", exeName);
        destinationToolsDir = Path.Combine(outputDir, Application.productName + "_Data", "Tools");
        targetPath = Path.Combine(destinationToolsDir, exeName);

        //if (activeBuildTarget == BuildTarget.StandaloneOSX)
        //{
        //    string destinationEncoderDir = Path.Combine(report.summary.outputPath, "Contents", "Tools");
        //    targetPath = Path.Combine(destinationEncoderDir, exeName);
        //}

        if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

        if (File.Exists(sourcePath))
            File.Copy(sourcePath, targetPath, true);
        else
            Debug.LogError("Tool not found: " + sourcePath);



        // 复制图片
        string imageName = "RGBA32.png";
        string sourcePathImage = Path.Combine(Application.dataPath, "Images", imageName);
        string destinationImagesDir = Path.Combine(outputDir, Application.productName + "_Data", "Images");
        string targetPathImage = Path.Combine(destinationImagesDir, imageName);
        if (!Directory.Exists(Path.GetDirectoryName(targetPathImage)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetPathImage));
        }
        if (File.Exists(sourcePathImage))
        {
            File.Copy(sourcePathImage, targetPathImage, true);
        }
        else
        {
            Debug.LogError("Image not found: " + sourcePathImage);
        }
    }
}