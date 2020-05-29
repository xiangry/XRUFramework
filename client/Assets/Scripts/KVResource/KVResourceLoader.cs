using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// added by wsh @ 2017.12.22
/// 功能：异步操作抽象基类，继承自IEnumerator接口，支持迭代，主要是为了让异步操作能够适用于协程操作
/// 注意：提供对协程操作的支持，但是异步操作的进行不依赖于协程，可以在Update等函数中查看进度值
/// </summary>

public class KVResourceLoader : IEnumerator, IDisposable
{
    public AsyncOperationHandle loadHandle;
    public string assetPath;
    private Type loadType;
    public event UnityAction<object> OnAssetLoaded;
    
    public KVResourceLoader(string path = null, Type type = null)
    {
        loadType = type;
        assetPath = path;
        if (loadType == typeof(Scene))
        {
            loadHandle = Addressables.LoadSceneAsync(path);
        }
        else
        {
            loadHandle = Addressables.LoadAssetAsync<object>(path);
        }
        
        loadHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"加载{assetPath}成功: {loadHandle.Result}");
                OnAssetLoaded?.Invoke(loadHandle.Result);
            }
            else
            {
                Debug.LogError($"加载{assetPath}失败: {handle.OperationException}");
            }
        };
#if UNITY_EDITOR
        loadHandle.Completed += handle =>
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
    }

    public bool MoveNext()
    {
        return !IsDone();
    }

    public void Reset()
    {
    }

    public object Current { get; }

    public bool isDone
    {
        get
        {
            return IsDone();
        }
    }

    public float progress
    {
        get
        {
            return Progress();
        }
    }


    public bool IsDone()
    {
        return loadHandle.IsDone;
    }

    public float Progress()
    {
        return loadHandle.PercentComplete;
    }

    public void Dispose()
    {
        if (loadType == typeof(Scene))
        {
            Addressables.UnloadSceneAsync(loadHandle);
        }
        else
        {
            Addressables.Release(loadHandle);
        }

    }

    public bool CheckValid()
    {
        if (loadHandle.IsValid())
        {
            return true;
        }
        else
        {
            Debug.LogError($"asset({assetPath}) handle not valid");
            return false;
        }
    }

    public object asset
    {
        get
        {
            if (loadHandle.IsValid())
            {
//              Debug.Log($" loadHandle : {loadHandle.Status} {loadHandle.Result}");
                return loadHandle.Result;
            }
            else
            {
                Debug.LogError($"asset({assetPath}) handle not valid");
                return null;
            }
        }
    }
}
