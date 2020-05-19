using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class KVResourceMgr : MonoSingleton<KVResourceMgr>
{
    Dictionary<string, KVResourceLoader> loaders = new Dictionary<string, KVResourceLoader>();

    public KVResourceLoader LoadAssetAsync(string path, Type assetType = null)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"LoadAssetAsync assetPath:{path}");
        var op = Addressables.LoadAssetAsync<object>(path);
#if UNITY_EDITOR
        op.Completed += handle =>
        {
            if (handle.IsValid() && handle.IsDone)
            {
                GameObject obj = handle.Result as GameObject;
                if (obj != null)
                {
//                    UtilityGame.ShaderRecover(obj);                    
                }
            }
        };
#endif
        KVResourceLoader loader = new KVResourceLoader(op, path);
        return loader;
    }

    public KVResourceLoader LoadSceneAsync(string path)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"LoadSceneAsync assetPath:{path}");
        var op = Addressables.LoadSceneAsync(path);
        KVResourceLoader loader = new KVResourceLoader(op, path);
//        loaders.Add(path, loader);
        return loader;
    }
    
    public KVResourceLoader UnloadSceneAsync(string path)
    {
//        path = "Assets/AssetsPackage/" + path;
        Debug.Log($"UnloadSceneAsync assetPath:{path}");
        KVResourceLoader loader;
        if (loaders.TryGetValue(path, out loader))
        {
            loader.Dispose();
            return loader;
        }

        return null;
    }
}
