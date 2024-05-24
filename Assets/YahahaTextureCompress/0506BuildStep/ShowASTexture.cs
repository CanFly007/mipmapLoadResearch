using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShowASTexture : MonoBehaviour
{
    public Material quadMat;
    public Object asObject;

    private Texture2D placeholderTex;

    private void OnEnable()
    {
        if (asObject == null)
            return;

#if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(asObject);
        string fullPath = System.IO.Path.Combine(Application.dataPath, "..", assetPath);
        fullPath = System.IO.Path.GetFullPath(fullPath);
#endif

        if (placeholderTex == null)
        {
            placeholderTex = new Texture2D(8, 8);
            quadMat.mainTexture = placeholderTex;
        }

        NativeArray<byte> hdBytes = Texture2D.ReadTextureDataFromFile(fullPath);
        if (hdBytes.Length > 0)
            placeholderTex.SetStreamedBinaryData(hdBytes);
    }
}
