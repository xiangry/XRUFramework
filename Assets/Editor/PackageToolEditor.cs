using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;

/// <summary>
/// added by wsh @ 2018.01.03
/// 说明：打包工具
/// TODO：
/// 1、安卓打包可以不用区分渠道，没有IOS那样的机器审核难以通过的问题
/// </summary>

public class PackageToolEditor : EditorWindow
{
    static private BuildTarget buildTarget = BuildTarget.Android;
    static private string resVersion = "1.0.000";
    static private string bundleVersion = "1.0.000";
    static private string localServerIP = "127.0.0.1";
    static private bool androidBuildABForPerChannel;
    static private bool iosBuildABForPerChannel;
    static private bool buildABSForPerChannel;

    static private Vector2 scrollPos;
    
    // manifest：提供依赖关系查找以及hash值比对
    [MenuItem("Tools/PackageTool", false, 0)]
    static void Init() {
        EditorWindow.GetWindow(typeof(PackageToolEditor));
    }
    void OnEnable()
    {
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        
        DrawAddressableToolsGUI();
        
        EditorGUILayout.Space();
        
        DrawAnimToolsGUI();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    #region 项目打包工具

    void DrawAddressableToolsGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("----------------------------------------------   Addressables 工具   ----------------------------------------------");
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("全部编译"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += AddressablePackage.BuildContent;
            EditorApplication.delayCall += () => { Debug.Log($"Addressable全部编译完成，用时({(DateTime.Now - start).Milliseconds})ms"); };
        }
        
        if (GUILayout.Button("准备更新"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += AddressablePackage.CheckForUpdateContent;
            EditorApplication.delayCall += () => { Debug.Log($"准备更新完成，用时({(DateTime.Now - start).Milliseconds})ms"); };
        }
        
        if (GUILayout.Button("一键更新"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += AddressablePackage.CheckForUpdateContent;
            EditorApplication.delayCall += AddressablePackage.BuildUpdate;
            EditorApplication.delayCall += () => { Debug.Log($"一键准备更新完成，用时({(DateTime.Now - start).Milliseconds})ms"); };
        }
        
        if (GUILayout.Button("AddressableImport"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += () =>
            {
                AddressableImporter.FolderImporter.ReimportFolders(new []{"Assets"});
            };
            EditorApplication.delayCall += () => { Debug.Log($"AddressableImport，用时({(DateTime.Now - start).Milliseconds})ms"); };
        }
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ReplaceAddressablement"))
        {
            var start = DateTime.Now;
            EditorApplication.delayCall += RefreshAddressableName;
            EditorApplication.delayCall += () => { Debug.Log($"AddressableImport，用时({(DateTime.Now - start).Milliseconds})ms"); };
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("------------------------------------------------------------------------------------------------------");
        EditorGUILayout.EndVertical();
    }
    void RefreshAddressableName()
    {
        var list = AssetDatabase.FindAssets("t:ScriptableObject", new []{"Assets/AddressableAssetsData/AssetGroups"});
        foreach (var guid in list)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var group = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(path);
            if (group)
            {
                foreach (var entrty in group.entries)
                {
                    var assetPath = entrty.AssetPath.Replace("Assets/AssetsPackage/", "");
                    entrty.SetAddress(assetPath.Replace("Assets/AssetsPackage/", ""));
                }
            }
        }
    }
    
    #endregion

    #region 技能动画工具
    void DrawAnimToolsGUI()
    {
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.EndVertical();
    }
    #endregion
    
}
