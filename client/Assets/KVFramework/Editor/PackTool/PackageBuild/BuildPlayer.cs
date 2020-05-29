using System.Collections.Generic;
using System.IO;
using GameChannel;
using UnityEditor;

/// <summary>
/// added by wsh @ 2018.01.03
/// 功能： 打包相关配置和通用函数
/// 
/// 注意：
/// 1）如果为每个渠道分别打AB包，则将渠道名打入各个AB包<---为了解决iOS各个渠道包的提审问题
/// 
/// </summary>

public class BuildPlayer : UnityEditor.Editor
{
    public const string XCodeOutputPath = "vXCode";
    
    private static void SetPlayerSetting(BaseChannel channel)
    {
        if (channel != null)
        {
#if UNITY_5_6_OR_NEWER
            PlayerSettings.applicationIdentifier = channel.GetBundleID();
#else
            PlayerSettings.bundleIdentifier = channel.GetBundleID();
#endif
            PlayerSettings.productName = channel.GetProductName();
            PlayerSettings.companyName = channel.GetCompanyName();
        }
    }

    public static void BuildAndroid(string channelName, bool isTest)
    {
    }
    
    public static void BuildXCode(string channelName, bool isTest)
    {
       
    }
	
	static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e != null && e.enabled)
            {
                names.Add(e.path);
            }
        }
        return names.ToArray();
    }
}
