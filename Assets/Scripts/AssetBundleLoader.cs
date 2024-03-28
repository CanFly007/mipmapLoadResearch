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

    private static bool m_UnloadAB = true;

    //public Texture2D otherTex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
            AssetBundle prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "teaport"));
            Object[] objects = prefabBundle.LoadAllAssets();
            //for (int i = 0; i < objects.Length; i++)
            {
                teaportGo = Instantiate(objects[0] as GameObject);
            }
            
            prefabBundle.Unload(false);
            //texBundle.Unload(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;

                //AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));
                //Texture2D otherTex = assetBundle.LoadAsset<Texture2D>(texture.name, 5);

                //texture.ForceSetMipLevel(5, null, Path.Combine(Application.streamingAssetsPath, "alienTex"));
                texture.ForceSetMipLevel(5, null, Path.Combine(Application.streamingAssetsPath, "teaport"));

                //assetBundle.Unload(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AssetBundle texBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alienTex"));            

            AssetLoadParameters assetLoadParameters = new AssetLoadParameters { KeepMeshVertexData = false, MinimumMipLevelToLoad = 99 };
            texBundle.LoadAsset("aaa", typeof(Texture2D), assetLoadParameters);
        }


        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            m_loadMipmapLevel = 0;
            m_UnloadAB = true;
            DeCompressBundleAsync().Forget();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            m_loadMipmapLevel = 3;
            m_UnloadAB = true;
            DeCompressBundleAsync().Forget();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_loadMipmapLevel = 0;
            m_UnloadAB = false;
            DeCompressBundleAsync().Forget();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    if (teaportGo != null)
        //    {
        //        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "othertexab"));
        //        var otherTex = assetBundle.LoadAsset<Texture2D>("Amazing Speed_Floor_D", 5);

        //        Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
        //        texture.ForceSetMipLevel(5, otherTex);
                
        //        assetBundle.Unload(false);
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    if (teaportGo != null)
        //    {
        //        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "othertexab"));
        //        var tex2D = assetBundle.LoadAsset<Texture2D>("Amazing Speed_Floor_D");
        //        Debug.Log("other tex: " + tex2D.GetInstanceID());


        //        Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
        //        Debug.Log(texture.GetInstanceID());


        //        texture.ForceSetMipLevel(0, tex2D);

        //        Debug.Log("------");
        //        Debug.Log(texture.GetInstanceID());
        //        assetBundle.Unload(false);
        //    }
        //}

        //GetPixel failed, because is not readable
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (teaportGo != null)
            {
                Texture2D texture = teaportGo.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                Color c = texture.GetPixel(0, 0);
                Debug.Log(c);
            }
        }

        //test LoadAB direct not async
        if (Input.GetKeyDown(KeyCode.L))
        {
            string assetBundlePath = Path.Combine(Application.streamingAssetsPath, "teaport");
            AssetBundle ab = AssetBundle.LoadFromFile(assetBundlePath);
            Object[] objects = ab.LoadAllAssets();
            for (int i = 0; i < objects.Length; i++)
            {
                teaportGo = Instantiate(objects[i] as GameObject);
            }
        }
    }


    //async UniTaskVoid LoadTeaportAsync()
    //{ 
        
    //}


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
        if(m_UnloadAB)
            assetBundleCreateRequest.assetBundle.Unload(false);
    }
}
