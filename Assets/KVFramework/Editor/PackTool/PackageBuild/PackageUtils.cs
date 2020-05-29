using UnityEditor;
using System.IO;
using GameChannel;
using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Boo.Lang;

/// <summary>
/// added by wsh @ 2018.01.03
/// 功能： 打包相关配置和通用函数
/// </summary>

public enum LocalServerType
{
    CurrentMachine = 0,
    AnyMachine = 1,
}

public class PackageUtils
{
    public const string LocalServerPrefsKey = "AssetBundlesLocalServerType";
    public const string LocalServerIPPrefsKey = "AssetBundlesLocalServerIP";
    public const string AndroidBuildABForPerChannelPrefsKey = "AndroidBuildABForPerChannelPrefsKey";
    public const string IOSBuildABForPerChannelPrefsKey = "IOSBuildABForPerChannelPrefsKey";

    public static bool GetAndroidBuildABForPerChannelSetting()
    {
        if (!EditorPrefs.HasKey(AndroidBuildABForPerChannelPrefsKey))
        {
            SaveAndroidBuildABForPerChannelSetting(false);
            return false;
        }

        bool enable = EditorPrefs.GetBool(AndroidBuildABForPerChannelPrefsKey, false);
        return enable;
    }

    public static void SaveAndroidBuildABForPerChannelSetting(bool enable)
    {
        EditorPrefs.SetBool(AndroidBuildABForPerChannelPrefsKey, enable);
    }

    public static bool GetIOSBuildABForPerChannelSetting()
    {
        if (!EditorPrefs.HasKey(IOSBuildABForPerChannelPrefsKey))
        {
            SaveIOSBuildABForPerChannelSetting(false);
            return false;
        }

        bool enable = EditorPrefs.GetBool(IOSBuildABForPerChannelPrefsKey, false);
        return enable;
    }

    public static void SaveIOSBuildABForPerChannelSetting(bool enable)
    {
        EditorPrefs.SetBool(IOSBuildABForPerChannelPrefsKey, enable);
    }

    public static LocalServerType GetLocalServerType()
    {
        if (!EditorPrefs.HasKey(LocalServerPrefsKey))
        {
            SaveLocalServerType(LocalServerType.CurrentMachine);
            return LocalServerType.CurrentMachine;
        }

        int type = EditorPrefs.GetInt(LocalServerPrefsKey, (int)LocalServerType.CurrentMachine);
        return (LocalServerType)type;
    }

    public static void SaveLocalServerType(LocalServerType type)
    {
        EditorPrefs.SetInt(LocalServerPrefsKey, (int)type);
    }

    public static string GetLocalServerIP()
    {
        string ip = string.Empty;
        var type = GetLocalServerType();
        if (type == LocalServerType.CurrentMachine)
        {
            ip = GetCurrentMachineLocalIP();
        }
        else
        {
            ip = EditorPrefs.GetString(LocalServerIPPrefsKey, "127.0.0.1");
        }
        return ip;
    }

    public static void SaveLocalServerIP(string ip)
    {
        var type = GetLocalServerType();
        if (type == LocalServerType.CurrentMachine)
        {
            return;
        }
        EditorPrefs.SetString(LocalServerIPPrefsKey, ip);
    }

    public static string GetCurrentMachineLocalIP()
    {
        try
        {
            // 注意：这里获取所有内网地址后选择一个最小的，因为可能存在虚拟机网卡
            var ips = new System.Collections.Generic.List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }
            ips.Sort();
            if (ips.Count <= 0)
            {
                Logger.LogError("Get inter network ip failed!");
            }
            else
            {
                return ips[0];
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError("Get inter network ip failed with err : " + ex.Message);
            Logger.LogError("Go Tools/Package to specify any machine as local server!!!");
        }
        return string.Empty;
    }

    public static bool BuildAssetBundlesForPerChannel(BuildTarget buildTarget)
    {
        if (buildTarget == BuildTarget.Android && GetAndroidBuildABForPerChannelSetting() ||
            buildTarget == BuildTarget.iOS && GetIOSBuildABForPerChannelSetting())
        {
            return true;
        }
        return false;
    }

    public static string GetCurPlatformName()
    {
        return GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
    }
    
    public static string GetPlatformName(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            default:
                Logger.LogError("Error buildTarget!!!");
                return null;
        }
    }

    public static ChannelType GetCurSelectedChannel()
    {
        ChannelType channelType = ChannelType.Test;
        string channelName = EditorPrefs.GetString("ChannelName");
        if (Enum.IsDefined(typeof(ChannelType), channelName))
        {
            channelType = (ChannelType)Enum.Parse(typeof(ChannelType), channelName);
        }
        else
        {
            EditorPrefs.SetString("ChannelName", ChannelType.Test.ToString());
        }
        return channelType;
    }

    public static void SaveCurSelectedChannel(ChannelType channelType)
    {
        EditorPrefs.SetString("ChannelName", channelType.ToString());
    }

    public static string GetPlatformChannelFolderName(BuildTarget target, string channelName)
    {
        if (BuildAssetBundlesForPerChannel(target))
        {
            // 不同渠道的AB输出到不同的文件夹
            return channelName;
        }
        else
        {
            // 否则写入通用的平台文件夹
            return GetPlatformName(target);
        }
    }

    public static string GetChannelRelativePath(BuildTarget target, string channelName)
    {
        string outputPath = Path.Combine(GetPlatformName(target), GetPlatformChannelFolderName(target, channelName));
        return outputPath;
    }

    public static string GetAssetBundleRelativePath(BuildTarget target, string channelName)
    {
        string outputPath = GetChannelRelativePath(target, channelName);
        outputPath = Path.Combine(outputPath, UtilityBuild.ManifestBundleName);
        return outputPath;
    }

    public static string GetCurPlatformChannelRelativePath()
    {
        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var channelName = GetCurSelectedChannel().ToString();
        return GetChannelRelativePath(buildTarget, channelName);
    }
    
    public static AssetBundleManifest GetManifestFormLocal(string manifestPath)
    {
        FileInfo fileInfo = new FileInfo(manifestPath);
        if (!fileInfo.Exists)
        {
            Debug.LogError("You need to build assetbundles first to get assetbundle dependencis info!");
            return null;
        }
        byte[] bytes = UtilityGame.SafeReadAllBytes(fileInfo.FullName);
        if (bytes == null)
        {
            return null;
        }
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(bytes);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        assetBundle.Unload(false);
        return manifest;
    }

    public static void CopyLuaScriptsToStreamingAssets()
    {
        {
            string path = "LuaScripts";
            string destination = Path.Combine(Application.dataPath, "AssetsPackage/LuaScripts");
            string source = Path.Combine(Application.dataPath, "../" + path);
            string packLuaPath = "Assets/AssetsPackage/LuaScripts/";
            string debugLuaPath = "Assets/../LuaScripts/";
//          AssetDatabase.DeleteAsset(UtilityGame.FullPathToAssetPath(destination));
            try
            {
                string[] filePathsOld = Directory.GetFiles(packLuaPath, "*", SearchOption.AllDirectories);
                string[] filePathsNew = Directory.GetFiles(debugLuaPath, "*", SearchOption.AllDirectories);
                
                Debug.Log($"filePathsOld {filePathsOld.Length}");
                Debug.Log($"filePathsNew {filePathsNew.Length}");
                
                List<string> filesOld = new List<string>();
                foreach (var f in filePathsOld)
                {
                    if (f.Contains(".bytes") && !f.Contains(".meta"))
                    {
                        var p = f.Replace(".bytes", "").Replace(packLuaPath, "");
                        filesOld.Add(p);
                    }
                }

                List<string> filesNew = new List<string>();
                foreach (var f in filePathsNew)
                {
                    if (f.Contains(".lua"))
                    {
                        var p = f.Replace(".lua", "").Replace(debugLuaPath, "");
                        filesNew.Add(p);
                    }
                }
                
                foreach (var fileOld in filesOld)
                {
                    if (!filesNew.Contains(fileOld))
                    {
                        UtilityGame.SafeDeleteFile($"{packLuaPath}/{fileOld}.bytes");
                    }
                }
                
                foreach (var filePath in filesNew)
                {
                    var from = $"{debugLuaPath}/{filePath}.lua";
                    var to = $"{packLuaPath}/{filePath}.bytes";
                    UtilityGame.SafeDeleteFile(to);
                    UtilityGame.SafeCopyFile(from, to);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Something wrong, you need manual delete LuaScripts , err : " + ex);
                return;
            }
            Debug.Log("Copy LuaScripts assetbundles to streaming assets done!");
        }
        AssetDatabase.Refresh();
    }
    
    public static void CheckAndAddSymbolIfNeeded(BuildTarget buildTarget, string targetSymbol)
    {
        if (buildTarget != BuildTarget.Android && buildTarget != BuildTarget.iOS)
        {
            Debug.LogError("Only support Android and IOS !");
            return;
        }

        var buildTargetGroup = buildTarget == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS;
        var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        if (!symbols.Contains("HOTFIX_ENABLE"))
        {
            symbols = string.Format("{0};{1};", symbols, "HOTFIX_ENABLE");
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
    }
    
    public static void CopyAndroidSDKResources(string channelName)
    {
        string targetPath = Path.Combine(Application.dataPath, "Plugins");
        targetPath = Path.Combine(targetPath, "Android");
        UtilityGame.SafeClearDir(targetPath);
        
        string channelPath = Path.Combine(Environment.CurrentDirectory, "Channel");
        string resPath = Path.Combine(channelPath, "UnityCallAndroid_" + channelName);
        if (!Directory.Exists(resPath))
        {
            resPath = Path.Combine(channelPath, "UnityCallAndroid");
        }

        EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0f);
        PackageUtils.CopyJavaFolder(resPath + "/assets", targetPath + "/assets");
        EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0.3f);
        PackageUtils.CopyJavaFolder(resPath + "/libs", targetPath + "/libs");
        EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0.6f);
        PackageUtils.CopyJavaFolder(resPath + "/res", targetPath + "/res");
        if (File.Exists(resPath + "/bin/UnityCallAndroid.jar"))
        {
            File.Copy(resPath + "/bin/UnityCallAndroid.jar", targetPath + "/libs/UnityCallAndroid.jar", true);
        }
        if (File.Exists(resPath + "/AndroidManifest.xml"))
        {
            File.Copy(resPath + "/AndroidManifest.xml", targetPath + "/AndroidManifest.xml", true);
        }

        EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 1f);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    public static void CopyJavaFolder(string source, string destination)
    {
        if (!Directory.Exists(source))
        {
            return;
        }
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
            AssetDatabase.Refresh();
        }

        string[] sourceDirs = Directory.GetDirectories(source);
        for (int i = 0; i < sourceDirs.Length; i++)
        {
            CopyJavaFolder(sourceDirs[i] + "/", destination + "/" + Path.GetFileName(sourceDirs[i]));
        }

        string[] sourceFiles = Directory.GetFiles(source);
        for (int j = 0; j < sourceFiles.Length; j++)
        {
            if (sourceFiles[j].Contains("classes.jar"))
            {
                continue;
            }
            File.Copy(sourceFiles[j], destination + "/" + Path.GetFileName(sourceFiles[j]), true);
        }
    }
}
