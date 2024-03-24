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

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                texture.ForceSetMipLevel(6);
                //Debug.Log(texture.name);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                texture.ForceSetMipLevel(0);
                //Debug.Log(texture.name);
            }
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
