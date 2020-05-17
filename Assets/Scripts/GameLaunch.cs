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


        yield return InitLaunchView();

    }


    IEnumerator InitLaunchView()
    {
        List<object> list = new List<object>();
        list.Add("prefabs/launch.prefab");
            
        // 检测下载
        long size = 0;
        var downloadSizeHandle = Addressables.GetDownloadSizeAsync(list);
        downloadSizeHandle.Completed += handle =>
        {
            size = handle.Result;
            Debug.Log($"download launch size : {size}");
        }; 
        yield return downloadSizeHandle;
        Addressables.Release(downloadSizeHandle);

        if (size > 0)
        {
            var downloadHandle= Addressables.DownloadDependenciesAsync(list, Addressables.MergeMode.None, false);
            downloadHandle.Completed += handle =>
            {
                Debug.Log("download launch success --");
            };
            yield return downloadHandle;
            Addressables.Release(downloadHandle);
        }
            
        var loader = KVResourceMgr.Instance.LoadAssetAsync("prefabs/launch.prefab");
        yield return loader;
        GameObject obj = loader.asset as GameObject;
        GameObject launch = Instantiate(obj, rootLayer.transform);
        updater = launch.AddComponent<KVUpdater>();
    }
    
    
//
//        yield return OpenView();
//        {
//            List<object> list = new List<object>();
//            list.Add("prefabs/yellow.prefab");
////            list.Add("prefabs/launch.prefab");
//            var downloadsize = Addressables.GetDownloadSizeAsync(list);
//            downloadsize.Completed += handle =>
//            {
//                Debug.Log("start yellow download:" + downloadsize.Result);
//            }; 
//            yield return downloadsize;
////            var download = Addressables.DownloadDependenciesAsync(list,Addressables.MergeMode.None);
////            download.Completed += handle =>
////            {
////                Debug.Log($"     <download>:{handle.Result} ---------");
////            }; 
////            yield return download;
////            Debug.Log("download finish");
//        }
//
////        {
////            var checkHandle = Addressables.CheckForCatalogUpdates(false);
////            List<string> catalogs = null;
////            checkHandle.Completed += handle =>
////            {
////                if (checkHandle.Status == AsyncOperationStatus.Succeeded)
////                {
////                    catalogs = checkHandle.Result;
////                    Debug.Log($"check Result: catalogs:{catalogs}");
////                    if (catalogs != null)
////                    {
////                        Debug.Log($"check Result: catalogs count:{catalogs.Count}");
////                        foreach (var cat in catalogs)
////                        {
////                            Debug.Log($"        catalog --》{cat}");
////                        }
////                    }
////                }
////            };
////            yield return checkHandle;
////            Addressables.Release(checkHandle);
////            
////            
////            List<object> updateList = new List<object>();
////            
////            if (catalogs != null && catalogs.Count > 0)
////            {
////                Debug.Log($"check Result start {catalogs}");
////                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
////                updateHandle.Completed += handle =>
////                {
////                    foreach (var loc in handle.Result)
////                    {
////                        Debug.Log($"  ----->>> loc: {loc.LocatorId}");
////                        foreach (var r in loc.Keys)
////                        {
////                            Debug.Log($"          >>> loc: {r}");
////                            IList<IResourceLocation> tempList;
////                            if (loc.Locate(r, typeof(object), out tempList))
////                            {
////                                foreach (var t in tempList)
////                                {
////                                    updateList.Add(t);
////                                }
////                            }
////                        }
////                    }
////                };
////                yield return updateHandle;
////                foreach (var u in updateList)
////                {
////                    var l = u as IResourceLocation;
////                    Debug.Log($"  ------:{l.PrimaryKey}  {l.ResourceType}  \t\t {l.Data}");
////                }
////                Debug.Log("download finish");
////                Addressables.Release(updateHandle);
////                
////                
////                
////                List<object> list = new List<object>();
////                var downloadsize = Addressables.GetDownloadSizeAsync(updateList);
////                downloadsize.Completed += handle =>
////                {
////                    Debug.Log($"check download size:" + downloadsize.Result);
////                }; 
//////                yield return downloadsize;
//////                var download = Addressables.DownloadDependenciesAsync(updateList,Addressables.MergeMode.None);
//////                yield return download;
//////                Debug.Log("download finish");
////            }
////
////
////            
//////            if (catalogs != null && catalogs.Count > 0)
//////            {
//////                Debug.Log("check Result start");
//////                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
//////                yield return updateHandle;
//////                Debug.Log("download finish");
//////                Addressables.Release(updateHandle);
//////            }
////        }
//        
////
////
////        yield return ShowCube();
//    }
        
}
