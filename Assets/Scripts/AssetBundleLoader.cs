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
    AssetBundle assetBundle;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(LoadAssetBundle());
        }
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    assetBundle.Unload(false);
        //}
        //

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeCompressBundleAsync().Forget();
        }
    }

    IEnumerator LoadAssetBundle()
    {
        string assetBundlePath = Path.Combine(Application.streamingAssetsPath, "teaport");

        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load AssetBundle: " + uwr.error);
            yield break;
        }

        assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);

        Object asset = assetBundle.LoadAsset("TeapotPrefab");
        Instantiate(asset);

        assetBundle.Unload(false);
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

            AssetBundleRequest bundleRequest = assetBundleCreateRequest.assetBundle.LoadAllAssetsAsync();
            await bundleRequest;

            instance.Asset = bundleRequest.asset;
            instance.AllAsset = bundleRequest.allAssets;
        }
        catch
        { 
        }
    }
}
