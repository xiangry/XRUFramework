﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;
 
public class AddressablePackage
{
    [MenuItem("AddressableEditor/SetGroup => StaticContent")]
    public static void SetStaticContentGroup()
    {
        foreach (AddressableAssetGroup groupAsset in Selection.objects)
        {
            for (int i = 0; i < groupAsset.Schemas.Count; i++)
            {
                var schema = groupAsset.Schemas[i];
                Debug.Log($"                ===> {schema.name}");
                if (schema is ContentUpdateGroupSchema)
                {
                    var contentUpdateGroupSchema = schema as ContentUpdateGroupSchema;
                    contentUpdateGroupSchema.StaticContent = true;
                }
                else if (schema is BundledAssetGroupSchema)
                {
                    var bundledAssetGroupSchema = (schema as BundledAssetGroupSchema);
                    bundledAssetGroupSchema.BuildPath.SetVariableByName(groupAsset.Settings,AddressableAssetSettings.kLocalBuildPath);
                    bundledAssetGroupSchema.LoadPath.SetVariableByName(groupAsset.Settings,AddressableAssetSettings.kLocalLoadPath);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("AddressableEditor/SetGroup => Remote")]
    public static void SetRemoteContentGroup()
    {
        foreach (AddressableAssetGroup groupAsset in Selection.objects)
        {
            for (int i = 0; i < groupAsset.Schemas.Count; i++)
            {
                var schema = groupAsset.Schemas[i];
                Debug.Log($"                ===> {schema.name}");
                if (schema is ContentUpdateGroupSchema)
                {
                    var contentUpdateGroupSchema = schema as ContentUpdateGroupSchema;
                    contentUpdateGroupSchema.StaticContent = true;
                }
                else if (schema is BundledAssetGroupSchema)
                {
                    var bundledAssetGroupSchema = (schema as BundledAssetGroupSchema);
                    bundledAssetGroupSchema.BuildPath.SetVariableByName(groupAsset.Settings, AddressableAssetSettings.kRemoteBuildPath);
                    bundledAssetGroupSchema.LoadPath.SetVariableByName(groupAsset.Settings,AddressableAssetSettings.kRemoteLoadPath);
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("AddressableEditor/Build All Content")]
    public static void BuildContent()
    {
        AddressableAssetSettings.BuildPlayerContent();
    }

    [MenuItem("AddressableEditor/Prepare Update Content")]
    public static void CheckForUpdateContent()
    {
        //与上次打包做资源对比
        string buildPath = ContentUpdateScript.GetContentStateDataPath(false);
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        List<AddressableAssetEntry> entrys =
            ContentUpdateScript.GatherModifiedEntries(
                m_Settings, buildPath);
        if (entrys.Count == 0) return;
        StringBuilder sbuider = new StringBuilder();
        sbuider.AppendLine("Need Update Assets:");
        foreach (var _ in entrys)
        {
            sbuider.AppendLine(_.address);
        }
        Debug.Log(sbuider.ToString());
        
        //将被修改过的资源单独分组
        var groupName = string.Format("UpdateGroup_{0}",DateTime.Now.ToString("yyyyMMdd"));
        ContentUpdateScript.CreateContentUpdateGroup(m_Settings, entrys, groupName);
    }

    [MenuItem("AddressableEditor/BuildUpdate")]
    public static void BuildUpdate()
    {
        var path = ContentUpdateScript.GetContentStateDataPath(false);
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, path);
        Debug.Log("BuildFinish path = " + m_Settings.RemoteCatalogBuildPath.GetValue(m_Settings));
    }

    [MenuItem("AddressableEditor/Test")]
    public static void Test()
    {
        Debug.Log("BuildPath = " + Addressables.BuildPath);
        Debug.Log("PlayerBuildDataPath = " + Addressables.PlayerBuildDataPath);
        Debug.Log("RemoteCatalogBuildPath = " + AddressableAssetSettingsDefaultObject.Settings.RemoteCatalogBuildPath.GetValue(AddressableAssetSettingsDefaultObject.Settings));
    }
}
