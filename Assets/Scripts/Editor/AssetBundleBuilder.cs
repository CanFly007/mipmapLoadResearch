using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder : MonoBehaviour
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
        //android
        //BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
        Debug.Log("Build ab finished");
        //UnwrapParam
    }

    [MenuItem("Custom/Build Binary")]
    static void BuildBinary()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";

        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        if (Selection.activeObject is Texture2D == false)
        {
            Debug.LogError("Not tex2D");
            return;
        }

        Texture2D tex2D = Selection.activeObject as Texture2D;
        Debug.Log(tex2D.name);
        string filePath = Path.Combine(Application.streamingAssetsPath, "mybytes");
        SerizationToBytes(tex2D, filePath);
        Debug.Log("Build binary finished");
    }

    //runtime or build ab
    public static void SerizationToBytes(Texture2D texture2D, string filePath)
    {
        byte[] myBytes = texture2D.GetStreamedBinaryData(false); //save to myBytes
        File.WriteAllBytes(filePath, myBytes);
    }
}
