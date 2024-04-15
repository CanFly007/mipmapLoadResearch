using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Custom/Build AssetBundle")]
    static void BuildAssetBundle()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";

        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        //win
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Debug.Log("Build ab finished");
        AssetDatabase.Refresh();
    }

    public static int splitMipLevel = 6;

    [MenuItem("Assets/Build Texture Binary", false, 0)]
    static void BuildBinary()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "TextureBytes");
        BuildBinary(folderPath);
    }

    private static void BuildBinary(string folderPath)
    {
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

        foreach (var selectedObj in Selection.objects)
        {
            if (selectedObj is Texture2D)
            {
                Texture2D tex2D = selectedObj as Texture2D;
                SerializationToBytes(tex2D, folderPath);
                Debug.Log("Build binary: " + tex2D.name + " finished.");
            }
            else
            {
                Debug.LogError("Selected object " + selectedObj.name + " is not a Texture2D.");
                continue;
            }
        }

        // Clean up
        EditorUtility.UnloadUnusedAssetsImmediate();
        AssetDatabase.Refresh();
    }

    public static void SerializationToBytes(Texture2D texture2D, string folderPath)
    {
        byte[] lowResBytes = texture2D.GetStreamedBinaryData(false, splitMipLevel);// For low quality texture
        byte[] highResBytes = texture2D.GetStreamedBinaryData(true, splitMipLevel); // For high quality texture

        string lowResFilePath = Path.Combine(folderPath, texture2D.name + "_ld.bytes");
        string highResFilePath = Path.Combine(folderPath, texture2D.name + "_hd.bytes");

        File.WriteAllBytes(lowResFilePath, lowResBytes);
        File.WriteAllBytes(highResFilePath, highResBytes);
    }

    [MenuItem("Assets/Build Texture Binary 100", false, 1)]
    static void BuildBinary100()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "TextureBytes100");
        BuildBinary(folderPath);
    }

    [MenuItem("Assets/ChangeTextureFormat", false, 2)]
    static void ChangeTextureFormat()
    {
        var selected = Selection.activeObject;
        if (selected == null || !(selected is Texture2D))
        {
            Debug.LogError("No texture selected.");
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(selected);
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer != null)
        {
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = "Standalone",
                format = TextureImporterFormat.DXT1,
                overridden = true
            });

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log("Texture format changed to DXT1.");
        }
        else
        {
            Debug.LogError("Texture importer could not be found.");
        }
    }
}
