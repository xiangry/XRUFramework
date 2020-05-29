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

    private Transform _objPool = null;

    private string[] _prefabs = new[]
    {
        "prefabs/scene/level1",
        "prefabs/scene/level2",
        "prefabs/scene/level3",
        "prefabs/scene/level4",
        "prefabs/scene/level5",
    };

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

        _objPool = new GameObject("__OBJECT_POOL__").transform;
//        DontDestroyOnLoad(_objPool);
        
        DontDestroyOnLoad(gameObject);
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

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        for (int i = 0; i < 5; i++)
        {
            var label = $"Level{i + 1}";
            if (GUILayout.Button(label, GUILayout.Width(80), GUILayout.Height(50)))
            {
                SwitchScene(i + 1);
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < _prefabs.Length; i++)
        {
            var label = $"{_prefabs[i]}";
            if (GUILayout.Button(label, GUILayout.Width(80), GUILayout.Height(50)))
            {
                LoadPrefab(i);
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < _prefabs.Length; i++)
        {
            var label = $"Destroy:{_prefabs[i]}";
            if (GUILayout.Button(label, GUILayout.Width(150), GUILayout.Height(50)))
            {
                KVResourceMgr.Instance.DestroyInstantiateObject(_prefabs[i]);
            }
        }
        GUILayout.EndHorizontal();
    }

    void SwitchScene(int level)
    {
        var path = $"scenes/Level{level}";
        KVResourceMgr.Instance.LoadSceneAsync(path, arg0 =>
        {
            _objPool = new GameObject("__OBJECT_POOL__").transform;
        });
    }
    
    void LoadPrefab(int index)
    {
        var path = $"{_prefabs[index]}";
        KVResourceMgr.Instance.InstanceObjectAsync(path, _objPool.transform, obj =>
        {
            GameObject gameObject = obj as GameObject;
            if (gameObject)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }
        });
    }
}
