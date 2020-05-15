using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// added by wsh @ 2017.12.22
/// 功能：异步操作抽象基类，继承自IEnumerator接口，支持迭代，主要是为了让异步操作能够适用于协程操作
/// 注意：提供对协程操作的支持，但是异步操作的进行不依赖于协程，可以在Update等函数中查看进度值
/// </summary>

public class KVResourceLoader : IEnumerator, IDisposable
{
    private AsyncOperationHandle asyncOperationHandle;
    private string assetPath;
    
    
    public KVResourceLoader(AsyncOperationHandle ao, string path = null)
    {
        asyncOperationHandle = ao;
        assetPath = path;
        
        ao.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"加载{assetPath}失败: {handle.OperationException}");
            }
        };
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
        return asyncOperationHandle.IsDone;
    }

    public float Progress()
    {
        return asyncOperationHandle.PercentComplete;
    }

    public void Dispose()
    {
//        Addressables.Release(asyncOperationHandle);
//        asyncOperationHandle = Addressables.UnloadSceneAsync(asyncOperationHandle);
    }

    public object asset
    {
        get
        {
//            Debug.LogError($" asyncOperationHandle : {asyncOperationHandle.Status} {asyncOperationHandle.Result}");
            return asyncOperationHandle.Result;
        }
    }
}
