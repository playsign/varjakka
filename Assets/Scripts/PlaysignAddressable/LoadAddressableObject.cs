using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadAddressableObject : MonoBehaviour
{
    /* when AddressableAssetsManager is used for external catalog loading */
    private void Awake()
    {
        AddressableAssetsManager.Completed += LoadObject;
    }

    //manually normally set catalog url in inspector
    //void Start()
    //{
    //    LoadObject();
    //}
    
    void LoadObject()
    {
        Debug.Log("[LoadAddressableObject] - LoadObject");
        Addressables.InstantiateAsync("varjakka2"); //, randSpot, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
