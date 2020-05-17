using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

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
        StartUpdate,
        AfterUpdate,
    }


    private EUpdateState curState = EUpdateState.None;
    private bool needUpdate = false;
    private List<string> needUpdateCatalogs;
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
        text_info.text = String.Concat(infoList.ToArray());

        curState = EUpdateState.None;
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
        curState = EUpdateState.CheckUpdate;
        
        //初始化Addressable
        var init = Addressables.InitializeAsync();
        yield return init;
        
        var start = DateTime.Now;
        //开始连接服务器检查更新
        checkHandle = Addressables.CheckForCatalogUpdates(false);
        //检查结束，验证结果
        curState = EUpdateState.AfterCheck;
        checkHandle.Completed += operationHandle =>
        {
            if (checkHandle.Status == AsyncOperationStatus.Succeeded)
            {
                List<string> catalogs = checkHandle.Result;
                if (catalogs != null && catalogs.Count > 0)
                {
                    needUpdate = true;
                    needUpdateCatalogs = catalogs;
                }
            }
        };   
        yield return checkHandle;
        Debug.Log($"CheckIfNeededUpdate({needUpdate}) use {(DateTime.Now - start).Milliseconds} ms");    
        Addressables.Release(checkHandle);

        if (needUpdate)
        {
            //检查到有资源需要更新
            text_progress.text = "有资源需要更新";
            StartDownLoad();
        }
        else
        {
            //没有资源需要更新，或者连接服务器失败
            Skip();
        }
 
    }

    public void StartDownLoad()
    {
        if (needUpdate)
        {
            StartCoroutine(download());
        }
    }
    
    public void Skip()
    {
        slider.fillAmount = 1;
        text_progress.text = $"下载完成";
    }

    IEnumerator download()
    {
        var start = DateTime.Now;
        //开始下载资源
        curState = EUpdateState.StartUpdate;
        updateHandle = Addressables.UpdateCatalogs(needUpdateCatalogs, false);
        updateHandle.Completed += handle =>
        {
            //下载完成
            curState = EUpdateState.AfterUpdate;
            Debug.Log($"下载完成-------------");
            Debug.Log($"UpdateFinish use {(DateTime.Now - start).Milliseconds} ms");
        }; 
        yield return updateHandle;
        Addressables.Release(updateHandle);
        Skip();
    }

	void Update () {
        switch (curState)
        {
            case EUpdateState.CheckUpdate:
            {
                checkUpdateTime += Time.deltaTime;
                if (checkUpdateTime > CHECKTIMEMAX)
                {
                    curState = EUpdateState.AfterCheck;
                    StopAllCoroutines();
                    Skip();
                }
                else if(checkHandle.IsValid())
                {
                    slider.fillAmount = checkHandle.PercentComplete;
                    text_progress.text = $"检测更新:{checkHandle.PercentComplete}";
                }
                break;
            }
            case EUpdateState.StartUpdate:
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
}
