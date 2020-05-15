using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
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
    static int MAX_DOWNLOAD_NUM = 5;
    static int UPDATE_SIZE_LIMIT = 5 * 1024 * 1024;
    static string APK_FILE_PATH = "/xluaframework_{0}_{1}.apk";
    
    int downloadSize = 0;
    int totalDownloadCount = 0;
    int finishedDownloadCount = 0;

    TextMeshProUGUI statusText;
    Slider slider;

    private long totalSize = 0;

    private float lasttime = 0f;
    
    private AsyncOperationHandle assetInitHandle;

    void Awake()
    {
        statusText = transform.Find("ContentRoot/LoadingDesc").GetComponent<TextMeshProUGUI>();
        slider = transform.Find("ContentRoot/SliderBar").GetComponent<Slider>();
        slider.maxValue = 100;
        slider.value = 0;
        slider.gameObject.SetActive(false);
    }

    void Start ()
    {
//        resVersionPath = AssetBundleUtility.GetPersistentDataPath(UtilityBuild.ResVersionFileName);
//        noticeVersionPath = AssetBundleUtility.GetPersistentDataPath(UtilityBuild.NoticeVersionFileName);
//        DateTime startDate = new DateTime(1970, 1, 1);
//        timeStamp = (DateTime.Now - startDate).TotalMilliseconds;
//        statusText.text = "正在检测资源更新...";
    }

    #region 主流程
    public void StartCheckUpdate()
    {
        StartCoroutine(CheckUpdateOrDownloadGame());
//#if UNITY_EDITOR || CLIENT_DEBUG
//        TestHotfix();
//#endif
    }

    IEnumerator CheckUpdateOrDownloadGame()
    {
        // 启动资源管理模块
        assetInitHandle = Addressables.InitializeAsync();
        yield return assetInitHandle;
        Debug.Log($"InitializeAsync {assetInitHandle.Status}   {assetInitHandle.IsDone}  {assetInitHandle.Result}");

        ResourceLocationMap map = assetInitHandle.Result as ResourceLocationMap;

        {
            lasttime = Time.realtimeSinceStartup;

//            foreach (var key in map.Keys)
//            {
//                Debug.Log($"location {key}: {map.Locations[key]}");
//                var aoSize = Addressables.GetDownloadSizeAsync(map.Locations[key]);
//                yield return aoSize;
//                if (aoSize.IsValid() && aoSize.IsDone)
//                {
//                    totalSize += aoSize.Result;
//                }
//            }
//            Debug.LogError($"下载内容大小{totalSize}");
            
            foreach (var key in map.Keys)
            {
                Debug.Log($"location {key}: {map.Locations[key]}");
                var ao = Addressables.DownloadDependenciesAsync(map.Locations[key]);
                yield return ao;
                IResourceLocation data = ao.Result as IResourceLocation;
                Debug.Log($"down data {key} : {ao.Result}");
            }
//            Debug.LogError($"下载内容大小{totalSize}");
        }

        var cnt = 0;
        while (cnt <1000)
        {
            cnt++;
            yield return new WaitForEndOfFrame();
        }
        

//        // 启动xlua热修复模块
//        XLuaManager.Instance.Startup();
//        yield return XLuaManager.Instance.LoadLuaRes();
//        XLuaManager.Instance.OnInit();
//        XLuaManager.Instance.StartHotfix();
//        XLuaManager.Instance.StartGame();
//        gameObject.SetActive(false);
//        Destroy(gameObject);
//        
//        
//        // 启动easytouch扩展管理
//        Sword.SceneRootManager.instance.Init();
//        Sword.EventManager.instance.Init();
//        Sword.TouchManager.instance.Init();
        
        yield break;
    }

    private interface ResourceLocators
    {
    }

    #endregion

    
    #region 资源更新
	void Update () {
        if (assetInitHandle.IsValid())
        {
            Debug.Log($"address init ing ----- {totalSize / 1024 / 1024}M   useTime({Time.realtimeSinceStartup - lasttime})");
            slider.normalizedValue = Mathf.Ceil(assetInitHandle.PercentComplete * 100);
        }
    }

    #endregion
}
