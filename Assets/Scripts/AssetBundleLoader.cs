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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_loadMipmapLevel = 0;
            DeCompressBundleAsync().Forget();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_loadMipmapLevel = 3;
            DeCompressBundleAsync().Forget();
        }
    }

    async UniTaskVoid DeCompressBundleAsync()
    {
        string assetBundlePath = Path.Combine(Application.streamingAssetsPath, "teaport");
        ASBundleWrapper aSBundleWrapper = new ASBundleWrapper();
        await DeCompressBundle(aSBundleWrapper, 0, assetBundlePath);
        Instantiate(aSBundleWrapper.Asset);
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

            AssetBundleRequest bundleRequest = assetBundleCreateRequest.assetBundle.LoadAllAssetsAsync(m_loadMipmapLevel);
            await bundleRequest;

            instance.Asset = bundleRequest.asset;
            instance.AllAsset = bundleRequest.allAssets;
        }
        catch
        { 
        }
    }
}
