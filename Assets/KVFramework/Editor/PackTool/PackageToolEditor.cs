using System;
using GameChannel;
using UnityEditor;
using UnityEngine;

public partial class PackageToolEditor : EditorWindow
{
    private static Vector2 scrollPos;
    static private string bundleVersion = "1.0.000";
    private static BuildTarget buildTarget;
    private static string channelName;
    private static string resVersion;
    private static ChannelType _channelType;

    private static PackageToolEditor _packageToolEditor;
    
    // manifest：提供依赖关系查找以及hash值比对
    [MenuItem("Tools/PackageTool", false, 0)]
    static void Init() {
        _packageToolEditor = EditorWindow.GetWindow(typeof(PackageToolEditor)) as PackageToolEditor;
        _packageToolEditor.Show();
    }

    void Populate()
    {
        buildTarget = EditorUserBuildSettings.activeBuildTarget;
        _channelType = PackageUtils.GetCurSelectedChannel();

        bundleVersion = PlayerSettings.bundleVersion;
    }

    
    void OnSelectionChange()
    {
        Debug.Log($"OnSelectionChange ----------------- ");
    }
    
    void OnEnable()
    {
        Populate();
        Debug.Log($"OnEnable ----------------- ");
    }
    
    void OnDisable()
    {
        Debug.Log($"OnDisable ----------------- ");
    }
    
    void OnDestroy()
    {
        Debug.Log($"OnDestroy ----------------- ");
    }
    
    void OnFocus()
    {
        Populate();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        
        DrawAddressableToolsGUI();
        
        EditorGUILayout.Space();
        DrawAnimToolsGUI();
        
        EditorGUILayout.Space();
        DrawPackageToolsGUI();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    
    
    public static void LogUseTime(string tag, DateTime startTime)
    {
        EditorApplication.delayCall += () =>
        {
            Debug.Log($"{tag}完成，用时({(DateTime.Now - startTime).Milliseconds})ms");
        };
    }
}
