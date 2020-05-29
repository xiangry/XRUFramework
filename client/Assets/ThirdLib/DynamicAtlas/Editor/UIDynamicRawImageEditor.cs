﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(UIDynamicRawImage))]
public class UIDynamicRawImageEditor : RawImageEditor
{
    private Texture lastTexture = null;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIDynamicRawImage myScript = (UIDynamicRawImage)target;
        GUILayout.Space(5);
        EditorGUILayout.LabelField("--------------------------------------------------------------------------------------------------------------------");
        GUILayout.Space(5);
        myScript.mGroup = (DynamicAtlasGroup)EditorGUILayout.EnumPopup("Group", myScript.mGroup);
        EditorGUILayout.LabelField("Texture Path", myScript.currentPath);

        GUILayout.Space(5);


        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Show DynamicAtlas"))
            {
                DynamicAtlasWindow.ShowWindow(myScript.mGroup);
            }
        }
        else
        {
            if ((lastTexture != myScript.texture) || (myScript.texture != null && myScript.currentPath == null))
            {
                lastTexture = myScript.texture;
                string path = AssetDatabase.GetAssetPath(myScript.texture);
                myScript.currentPath = path;
            }

            if (myScript.texture == null)
            {
                lastTexture = null;
                myScript.currentPath = null;
            }
            else
            {
                if (String.IsNullOrEmpty(myScript.currentPath))
                {
                    string path = AssetDatabase.GetAssetPath(myScript.texture);
//                    Debug.Log($"myScript.currentPath:{myScript.currentPath}   path:{path}");
                    myScript.currentPath = path;
                }
            }
        }
        EditorGUILayout.LabelField("--------------------------------------------------------------------------------------------------------------------");

        EditorGUILayout.LabelField("--------------------------------------------------------------------------------------------------------------------");
    }
}
