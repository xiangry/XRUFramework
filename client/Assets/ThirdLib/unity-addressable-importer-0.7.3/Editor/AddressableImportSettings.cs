﻿using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityAddressableImporter.Helper;


[CreateAssetMenu(fileName = "AddressableImportSettings", menuName = "Addressable Assets/Import Settings", order = 50)]
public class AddressableImportSettings : ScriptableObject
{
    public const string kDefaultConfigObjectName = "addressableimportsettings";
    public const string kDefaultPath = "Assets/AddressableAssetsData/AddressableImportSettings.asset";

    [Tooltip("Creates a group if the specified group doesn't exist.")]
    public bool allowGroupCreation = false;

    [Tooltip("Rules for managing imported assets.")]
    public List<AddressableImportRule> rules;

    [ButtonMethod]
    private void Save()
    {
        AssetDatabase.SaveAssets();
    }

    [ButtonMethod]
    private void Documentation()
    {
        Application.OpenURL("https://github.com/favoyang/unity-addressable-importer/blob/master/Documentation~/AddressableImporter.md");
    }

    [ButtonMethod]
    private void CleanEmptyGroup()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            return;
        }
        var dirty = false;
        var emptyGroups = settings.groups.Where(x => x.entries.Count == 0 && !x.IsDefaultGroup()).ToArray();
        for (var i = 0; i < emptyGroups.Length; i++)
        {
            dirty = true;
            settings.RemoveGroup(emptyGroups[i]);
        }
        if (dirty)
        {
            AssetDatabase.SaveAssets();
        }
    }

    public static AddressableImportSettings Instance
    {
        get
        {
            AddressableImportSettings so;
            // Try to locate settings via EditorBuildSettings.
            if (EditorBuildSettings.TryGetConfigObject(kDefaultConfigObjectName, out so))
                return so;
            // Try to locate settings via path.
            so = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(kDefaultPath);
            if (so != null)
                EditorBuildSettings.AddConfigObject(kDefaultConfigObjectName, so, true);
            return so;
        }
    }
}