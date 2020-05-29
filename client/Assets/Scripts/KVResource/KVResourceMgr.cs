using System;
using System.Collections.Generic;
using KVResource;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class KVResourceMgr : MonoSingleton<KVResourceMgr>
{
    private readonly Dictionary<string, KVResourceLoader> _loadingAsset = new Dictionary<string, KVResourceLoader>();
    private readonly Dictionary<string, KVResourceLoader> _loadedAsset = new Dictionary<string, KVResourceLoader>();
    
    public readonly Dictionary<string, List<Object>> _instantiateObjs = new Dictionary<string, List<Object>>();

    public KVResourceLoader LoadAssetAsync(string path, UnityAction<object> callback = null)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"LoadAssetAsync assetPath:{path}");
        KVResourceLoader loader;
        if (_loadedAsset.TryGetValue(path, out loader))
        {
            loader = _loadedAsset[path];
            callback?.Invoke(loader.asset);
            return loader;
        }
        
        if (_loadingAsset.TryGetValue(path, out loader))
        {
            loader.OnAssetLoaded += callback;
            return loader;
        }
        
        loader = new KVResourceLoader(path);
        loader.OnAssetLoaded += callback;
        loader.OnAssetLoaded += obj =>
        {
            _loadedAsset.Add(path, loader);
            _loadingAsset.Remove(path);
        };
        _loadingAsset.Add(path, loader);
        return loader;
    }
    
    
    public KVResourceLoader InstanceObjectAsync(string path, Transform parent, 
        UnityAction<object> callback = null)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"LoadAssetAsync assetPath:{path}");
        KVResourceLoader loader;
        if (_loadedAsset.TryGetValue(path, out loader))
        {
            callback?.Invoke(InstantiateObject(path, loader, parent));
            return loader;
        }
        
        if (_loadingAsset.TryGetValue(path, out loader))
        {
            callback?.Invoke(InstantiateObject(path, loader, parent));
            return loader;
        }
        
        loader = new KVResourceLoader(path);
        loader.OnAssetLoaded += obj =>
        {
            callback?.Invoke(InstantiateObject(path, loader, parent));
            _loadedAsset.Add(path, loader);
            _loadingAsset.Remove(path);
        };
        _loadingAsset.Add(path, loader);
        return loader;
    }

    public GameObject InstantiateObject(string path, KVResourceLoader loader, Transform parent)
    {
        if (loader.CheckValid())
        {
            var obj = loader.asset as GameObject;
            if (obj)
            {
                GameObject gameObject = Instantiate(obj, parent);
                KVResourceTracker kvResourceTracker = gameObject.AddComponent<KVResourceTracker>();
                kvResourceTracker.key = path;
                kvResourceTracker.OnDestroyed += OnDestroyTracker;
                

                List<Object> objList;
                if (!_instantiateObjs.TryGetValue(path, out objList))
                {
                    objList = new List<Object>();
                    _instantiateObjs.Add(path, objList);
                }
                objList.Add(gameObject.gameObject);
                return gameObject;
            }
        }
        
        Debug.LogError($"loader({loader.assetPath}) can not Instantiate");
        return null;
    }

    void OnDestroyTracker(KVResourceTracker tracker)
    {
        List<Object> list;
        if (_instantiateObjs.TryGetValue(tracker.key, out list))
        {
            list.Remove(tracker.gameObject);
        }
        else
        {
            Debug.LogError($"OnDestroyTracker({tracker.key}) not found objects");
        }
    }

    public void DestroyInstantiateObject(string key)
    {
        List<Object> list;
        if (_instantiateObjs.TryGetValue(key, out list))
        {
            if (list.Count > 0)
            {
                GameObject.DestroyImmediate(list[0]);
            }
        }
    }

    public KVResourceLoader LoadSceneAsync(string path, UnityAction<object> callback = null)
    {
        Debug.Log($"LoadSceneAsync assetPath:{path}");
        KVResourceLoader loader;
        loader = new KVResourceLoader(path, typeof(Scene));
        loader.OnAssetLoaded += arg0 =>
        {
            callback?.Invoke(null);
//            loader.Dispose();
        };
        return loader;
    }
    
    public KVResourceLoader Unload(string path)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"UnloadSceneAsync assetPath:{path}");
        KVResourceLoader loader;
        if (_loadedAsset.TryGetValue(path, out loader))
        {
            loader.Dispose();
            return loader;
        }
        
        if (_loadingAsset.TryGetValue(path, out loader))
        {
            loader.Dispose();
            return loader;
        }
        return null;
    }

    public void DestroyAllRes()
    {
        foreach (var loader in _loadedAsset.Values)
        {
            loader.Dispose();
        }
        _loadedAsset.Clear();
        
        
        foreach (var loader in _loadingAsset.Values)
        {
            loader.Dispose();
        }
        _loadingAsset.Clear();
        
        GC.Collect();
    }
}
