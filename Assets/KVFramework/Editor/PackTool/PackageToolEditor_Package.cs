using System;
using System.IO;
using KVGame;
using UnityEditor;
using UnityEngine;

public partial class PackageToolEditor : EditorWindow
{
    
    #region 打包相关操作
    void DrawPackageToolsGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("----------------------------------------------   游戏打包 工具   ----------------------------------------------");

        EditorGUILayout.LabelField(bundleVersion);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("同步Lua脚本"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += SyncLuaScript;
            LogUseTime("同步Lua脚本", start);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("一键打包"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += BuildWindows;
            LogUseTime("一键打包", start);
        }
        
        if (GUILayout.Button("打开打包目录", GUILayout.Width(200)))
        {
            var folder = KVPackageHelper.GetChannelOutputPath(buildTarget, channelName);
            EditorUtils.ExplorerFolder(folder);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("------------------------------------------------------------------------------------------------------");
        EditorGUILayout.EndVertical();
    }

    public static void IncreaseAppSubVersion()
    {
        // 每一次构建安装包，子版本号自增，注意：前两个字段这里不做托管，自行到PlayerSetting中设置
        string[] vers = bundleVersion.Split('.');
        if (vers.Length > 0)
        {
            int subVer = 0;
            int.TryParse(vers[vers.Length - 1], out subVer);
            vers[vers.Length - 1] = string.Format("{0:D3}", subVer + 1);
        }
        bundleVersion = string.Join(".", vers);
    }

    public static void BuildAndroidPlayerForCurrentChannel()
    {
        IncreaseAppSubVersion();

        var start = DateTime.Now;
        BuildAndroid(channelName);
        var detalTime = (DateTime.Now - start).TotalSeconds;
        var msg = $"Build player for : \n\nplatform : {buildTarget} \nchannel : {channelName} \n\ndone! use {detalTime}s";
//        EditorUtility.DisplayDialog("Success", msg,"Confirm");
        Debug.Log(msg);
    }

    public static void BuildAndroid(string channelName, bool isTest = false)
    {
        BuildTarget buildTarget = BuildTarget.Android;
        string savePath = KVPackageHelper.GetChannelOutputPath(buildTarget, channelName);
        string appName = "sot_dev.apk";
        savePath = Path.Combine(savePath, appName);
        UtilityGame.SafeDeleteDir(savePath);
        UtilityGame.SafeDeleteFile(savePath);
        BuildPipeline.BuildPlayer(
            KVPackageHelper.GetBuildScenes(), 
            savePath, 
            buildTarget, 
            BuildOptions.None | BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.BuildScriptsOnly);
        Debug.Log(string.Format("Build android player for : {0} done! output ：{1}", channelName, savePath));
    }

    public static void BuildWindows()
    {
        string savePath = KVPackageHelper.GetChannelOutputPath(buildTarget, channelName);
        UtilityGame.SafeDeleteFile(savePath);
        string appPath = $"{savePath}/XURGame.exe";
        BuildPipeline.BuildPlayer(
            KVPackageHelper.GetBuildScenes(), 
            appPath, 
            buildTarget, 
            BuildOptions.None);
        Debug.Log(string.Format("Build windows player for : {0} done! output ：{1}", channelName, savePath));
    }
    public static void SyncLuaScript()
    {
//        AssetBundleMenuItems.ToolsCopyLuaScriptsToAssetbundles();
        AssetDatabase.Refresh();
    }
    #endregion
}
