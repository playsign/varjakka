using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssetsManager : MonoBehaviour
{
    public static Action Completed;

    void Start()
    {
        AsyncOperationHandle catalogLoad = Addressables.LoadContentCatalogAsync("https://storage.googleapis.com/sites.playsign.net/warjakka-assets2/StandaloneWindows64/catalog_1.json");
        catalogLoad.Completed += CatalogLoad_Completed;
    }

    private void CatalogLoad_Completed(AsyncOperationHandle obj)
    {
        Completed();
    }
 }
