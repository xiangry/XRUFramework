using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameLaunch : MonoBehaviour
{
    private GameObject UIRoot = null;
    private Canvas rootLayer = null;
    private KVUpdater updater = null;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UIRoot = GameObject.Find("UIRoot");
        DontDestroyOnLoad(UIRoot);

        rootLayer = GameObject.Find("LaunchLayer").GetComponent<Canvas>();

        //初始化Addressable
        var init = Addressables.InitializeAsync();
        yield return init;

        yield return InitLaunchView();

    }

    IEnumerator InitLaunchView()
    {
        var loader = KVResourceMgr.Instance.LoadAssetAsync("prefabs/launch.prefab");
        yield return loader;
        GameObject obj = loader.asset as GameObject;
        GameObject launch = Instantiate(obj, rootLayer.transform);
        updater = launch.AddComponent<KVUpdater>();
        loader.Dispose();
    }
}
