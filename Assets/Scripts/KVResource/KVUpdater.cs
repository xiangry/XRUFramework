using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = System.Object;

/// <summary>
/// added by wsh @ 2017.12.29
/// 功能：Assetbundle更新器
/// </summary>

public class KVUpdater : MonoBehaviour
{
    enum EUpdateState
    {
        None = 0,
        CheckUpdate,
        AfterCheck,
        StartUpdateCatalogs,
        StartUpdateData,
        AfterUpdate,
    }

    private EUpdateState curState = EUpdateState.None;
//    private bool needUpdate = false;
    private List<string> updateCatalogsList;
    private AsyncOperationHandle<List<string>> checkHandle;
    private AsyncOperationHandle<List<IResourceLocator>> updateHandle;
    private float checkUpdateTime = 0f;
    private float CHECKTIMEMAX = 5f;

    private Text text_info;
    private Text text_progress;
    private Image slider;
    
    List<string> infoList = new List<string>();

    void Awake()
    {
        text_info = transform.Find("text_info").GetComponent<Text>();
        text_progress = transform.Find("slider/progress").GetComponent<Text>();
        slider = transform.Find("slider/top").GetComponent<Image>();

        MarkNeedDownloadState(true);
    }

    void Start ()
    {
        infoList.Add($"dataPath:{Application.dataPath}\n");
        infoList.Add($"consoleLogPath:{Application.consoleLogPath}\n");
        infoList.Add($"persistentDataPath:{Application.persistentDataPath}\n");
        infoList.Add($"streamingAssetsPath:{Application.streamingAssetsPath}\n");
        infoList.Add($"temporaryCachePath:{Application.temporaryCachePath}\n");
        infoList.Add($"BuildPath:{Addressables.BuildPath}\n");
        infoList.Add($"PlayerBuildDataPath:{Addressables.PlayerBuildDataPath}\n");
        infoList.Add($"RuntimePath:{Addressables.RuntimePath}\n");
        string info = String.Concat(infoList.ToArray());
        text_info.text = info;
        
        Debug.Log($"Application:\n{info}");
        
        SetState(EUpdateState.None);
        StartCheckUpdate();
    }

    void StartCheckUpdate()
    {
        text_progress.text = "正在检测资源更新";
        slider.fillAmount = 0f;

        StartCoroutine(CheckUpdate());
    }

    IEnumerator CheckUpdate()
    {
        SetState(EUpdateState.CheckUpdate);

        var needUpdateCatalogs = false;
        var start = DateTime.Now;
        //开始连接服务器检查更新
        checkHandle = Addressables.CheckForCatalogUpdates(false);
        //检查结束，验证结果
        checkHandle.Completed += operationHandle =>
        {
            if (checkHandle.Status == AsyncOperationStatus.Succeeded)
            {
                List<string> catalogs = checkHandle.Result;
                if (catalogs != null && catalogs.Count > 0)
                {
                    needUpdateCatalogs = true;
                    updateCatalogsList = catalogs;
                }
            }
            SetState(EUpdateState.AfterCheck);
        };   
        yield return checkHandle;
        Debug.Log($"CheckIfNeededUpdate({needUpdateCatalogs}) use {(DateTime.Now - start).Milliseconds} ms");    
        Addressables.Release(checkHandle);
        
        if (needUpdateCatalogs)
        {
            yield return DownloadCatalogs();
        }
        
        yield return StartDownload();
    }

    IEnumerator StartDownload()
    {
        if (IsLastDownloadComplete())
        {
            //检查到有资源需要更新
            text_progress.text = "有资源需要更新";
            yield return DownloadUpdateData();
        }

        //没有资源需要更新，或者连接服务器失败
        DownComplete();
    }
    
    public void DownComplete(bool isSuccess = true)
    {
        slider.fillAmount = 1;
        text_progress.text = $"下载完成 state:{curState} isSuccess:{isSuccess}";
    }

    IEnumerator DownloadCatalogs()
    {
        var start = DateTime.Now;
        //开始下载资源
        SetState(EUpdateState.StartUpdateCatalogs);
        updateHandle = Addressables.UpdateCatalogs(updateCatalogsList, false);
        updateHandle.Completed += handle =>
        {
            MarkNeedDownloadState(true);
            //下载完成
            SetState(EUpdateState.AfterUpdate);
            Debug.Log($"下载完成Catalogs------------- use time:{(DateTime.Now - start).Milliseconds} ms");
        }; 
        yield return updateHandle;
    }
    
    IEnumerator DownloadUpdateData()
    {
        SetState(EUpdateState.StartUpdateData);
        List<IResourceLocation> locations = new List<IResourceLocation>();
        foreach (var locator in Addressables.ResourceLocators)
        {
//            Debug.Log($"      locater {locator.LocatorId}");
            foreach (var key in locator.Keys)
            {
                IList<IResourceLocation> locationList;
                if (locator.Locate(key, typeof(object), out locationList))
                {
                    if (locationList.Count > 0)
                    {
                        foreach (var location in locationList)
                        {
//                            Debug.Log($"                                      =========> {location.PrimaryKey}");
                            locations.Add(location);
                        }
                    }
                }
            }
        }

        var start = DateTime.Now;
        var downSize = 0l;
        var sizeAo = Addressables.GetDownloadSizeAsync(locations);
        sizeAo.Completed += handle =>
        {
            downSize = sizeAo.Result;
            Debug.Log($"检查下载资源完成------------- use time:{(DateTime.Now - start).Milliseconds} ms");
            Debug.Log($"  GetDownloadSizeAsync 下载内容大小：{sizeAo.Result}  ");
        };
        yield return sizeAo;
        
        start = DateTime.Now;
        if (downSize > 0)
        {
            MarkNeedDownloadState(true);
            var downAo = Addressables.DownloadDependenciesAsync(locations);
            downAo.Completed += handle =>
            {
                Debug.Log($"下载DownLoadData------------- use time:{(DateTime.Now - start).Milliseconds} ms");
//                var list = handle.Result as List<IAssetBundleResource>;
//                foreach (var item in list)
//                {
//                    Debug.Log($"------------------ {item.GetAssetBundle().name}");
//                }
            };
            yield return  downAo;
        }
        
        {
            MarkNeedDownloadState(false);
        }
        SetState(EUpdateState.AfterUpdate);
        yield break;
    }


	void Update () {
        switch (curState)
        {
            case EUpdateState.CheckUpdate:
            {
                checkUpdateTime += Time.deltaTime;
                if (checkUpdateTime > CHECKTIMEMAX)
                {
                    SetState(EUpdateState.AfterCheck);
                    StopAllCoroutines();
                    DownComplete(false);
                }
                else if(checkHandle.IsValid())
                {
                    slider.fillAmount = checkHandle.PercentComplete;
                    text_progress.text = $"检测更新:{checkHandle.PercentComplete}";
                }
                break;
            }
            case EUpdateState.StartUpdateData:
            {
                if (updateHandle.IsValid())
                {
                    text_progress.text = $"下载中:{updateHandle.PercentComplete}";
                    slider.fillAmount = updateHandle.PercentComplete;
                }
                break;
            }
            default:
                break;
        }
    }
    
    IEnumerator ShowEffect()
    {
        {
            var ao = Addressables.InstantiateAsync("prefabs/cube.prefab");
            ao.Completed += handle =>
            {
                GameObject obj = ao.Result;
                obj.transform.position = Vector3.zero;
                obj.transform.localScale = Vector3.one;
            };
            yield return ao;
        }

        Addressables.ReleaseInstance(gameObject);
//        Addressables.ClearResourceLocators();
        Addressables.ClearDependencyCacheAsync("prefabs/launch.prefab");
        {
            var rootLayer = GameObject.Find("LaunchLayer").GetComponent<Canvas>();
            var loader = KVResourceMgr.Instance.LoadAssetAsync("prefabs/launch.prefab");
            yield return loader;
//            ao.Completed += handle =>
//            {
            GameObject obj = loader.asset as GameObject;
            obj = Instantiate(obj, rootLayer.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
//            };
//            yield return ao;
        }        DestroyImmediate(gameObject);
    }

    

    void SetState(EUpdateState state)
    {
        Debug.Log($"KVUpdate ====>SetState({state})");
        curState = state;
    }

    void MarkNeedDownloadState(bool needDownload)
    {
        FileStream fileStream = File.Open(GetDownLockFile(), FileMode.Create);
        StreamWriter writer = new StreamWriter(fileStream);
        writer.Write(needDownload);
        writer.Close();
        fileStream.Close();
    }
    
    bool IsLastDownloadComplete()
    {
        FileStream fileStream = File.Open(GetDownLockFile(), FileMode.Open);
        StreamReader reader = new StreamReader(fileStream);
        var result = reader.ReadLine();
        var isDownComplete = ("True".Equals(result));
        reader.Close();
        fileStream.Close();
        return isDownComplete;
    }

    string GetDownLockFile()
    {
        var writePath = Application.persistentDataPath;
        return Path.Combine(writePath, "down.lock");
    }
}
