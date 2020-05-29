using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameLaunch : MonoBehaviour
{
    private GameObject _uiRoot = null;
    private Canvas _rootLayer = null;
    private KVUpdater _updater = null;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _uiRoot = GameObject.Find("UIRoot");
        DontDestroyOnLoad(_uiRoot);

        _rootLayer = GameObject.Find("LaunchLayer").GetComponent<Canvas>();

        //初始化Addressable
        var init = Addressables.InitializeAsync();
        yield return init;

        yield return InitLaunchView();

//        DontDestroyOnLoad(_objPool);
        
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("scene_root"));
    }

    IEnumerator InitLaunchView()
    {
//        var loader = KVResourceMgr.Instance.LoadAssetAsync("prefabs/launch.prefab");
//        yield return loader;
//        GameObject obj = loader.asset as GameObject;
//        GameObject launch = Instantiate(obj, rootLayer.transform);
//        updater = launch.AddComponent<KVUpdater>();
//        loader.Dispose();
        yield break;
    }
}
