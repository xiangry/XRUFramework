using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.AddressableAssets.Settings;

public partial class PackageToolEditor : EditorWindow
{
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
}
