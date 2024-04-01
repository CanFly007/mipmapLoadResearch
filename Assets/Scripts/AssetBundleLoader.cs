using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ASBundleWrapper
{
    public Object Asset { get; set; }
    public Object[] AllAsset { get; set; }

}

public class AssetBundleLoader : MonoBehaviour
{
    private static int m_loadMipmapLevel = 0;

    private GameObject teaportGo;

    //public Texture2D otherTex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                //m_loadMipmapLevel = Mathf.Clamp(--m_loadMipmapLevel, 0, texture.mipmapCount - 1);
                --m_loadMipmapLevel;
                texture.ForceSetMipLevel(m_loadMipmapLevel, Path.Combine(Application.streamingAssetsPath, "teaport"));
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                //m_loadMipmapLevel = Mathf.Clamp(++m_loadMipmapLevel, 0, texture.mipmapCount - 1);
                ++m_loadMipmapLevel;
                texture.ForceSetMipLevel(m_loadMipmapLevel, Path.Combine(Application.streamingAssetsPath, "teaport"));
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            m_loadMipmapLevel = 0;
            DeCompressBundleAsync().Forget();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_loadMipmapLevel = 10;
            DeCompressBundleAsync().Forget();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (teaportGo)
            {                
                DestroyImmediate(teaportGo);
                teaportGo = null;
            }
            Resources.UnloadUnusedAssets();
        }
    }

    async UniTaskVoid DeCompressBundleAsync()
    {
        string assetBundlePath = Path.Combine(Application.streamingAssetsPath, "teaport");
        ASBundleWrapper aSBundleWrapper = new ASBundleWrapper();
        await DeCompressBundle(aSBundleWrapper, 0, assetBundlePath);
        teaportGo = Instantiate(aSBundleWrapper.Asset) as GameObject;
    }

    //模仿ASBundleWrapper Load assetbundle
    static async UniTask DeCompressBundle(ASBundleWrapper instance, int downResult, string filePath)
    {
        AssetBundleCreateRequest assetBundleCreateRequest = null;
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"not found {filePath},but downLoad file result is success.Uri:");
                return;
            }

            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
            await assetBundleCreateRequest;

            AssetBundleRequest bundleRequest = assetBundleCreateRequest.assetBundle.LoadAllAssetsAsync(new AssetLoadParameters(false, m_loadMipmapLevel));
            await bundleRequest;

            instance.Asset = bundleRequest.asset;
            instance.AllAsset = bundleRequest.allAssets;
        }
        catch
        { 
        }
        assetBundleCreateRequest.assetBundle.Unload(false);
    }
}
