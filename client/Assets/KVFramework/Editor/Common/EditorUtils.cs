using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public class EditorUtils
{
    public static void ExplorerFolder(string folder)
    {
        folder = string.Format("\"{0}\"", folder);
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                System.Diagnostics.Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                break;
            case RuntimePlatform.OSXEditor:
                System.Diagnostics.Process.Start("open", folder);
                break;
            default:
                Debug.LogError(string.Format("Not support open folder on '{0}' platform.", Application.platform.ToString()));
                break;
        }
    }
    
    /// <summary>
    /// 获取指定Assets目录下所有filter的Asset
    /// </summary>
    /// <param name="folder">assets 文件夹 Assets/a/b </param>GetObjectByAssetFolder
    /// <param name="filter">t:Shader</param>
    /// <returns></returns>
    public static List<string> GetAssetPaths(string folder, string filter)
    {
        List<string> listAssetPaths = new List<string>();

        string[] folders = AssetDatabase.GetSubFolders(folder);
        string[] guids = AssetDatabase.FindAssets(filter, folders);

        for (int i = 0; i < guids.Length; i++)
        {
            listAssetPaths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
        return listAssetPaths;
    }

    public static List<T> GetObjectByAssetPaths<T>(List<string> assetPaths) where T : Object
    {
        if (assetPaths == null || assetPaths.Count <= 0) return null;
        List<T> objs = new List<T>();
        for (int i = 0; i < assetPaths.Count; i++)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<T>(assetPaths[i]);
            objs.Add(obj as T);
        }
        return objs;
    }

    public static List<T> GetObjectByAssetFolder<T>(string folder, string filter) where T : Object
    {
        return GetObjectByAssetPaths<T>(GetAssetPaths(folder, filter));
    }

    public static void ExitOrCreateFolder(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
}
