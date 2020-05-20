using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;


[CustomEditor(typeof(DynamicImage))]
public class DynamicImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        DynamicImage image = target as DynamicImage;
        image.cacheTexture = EditorGUILayout.TextField("DynamicAsset", image.cacheTexture);
        image.imagePath = EditorGUILayout.TextField("ImagePath", image.imagePath);
        Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AssetsPackage/" + image.imagePath);
        texture2D = EditorGUILayout.ObjectField("纹理", texture2D, typeof(Texture)) as Texture2D;
        image.imagePath = AssetDatabase.GetAssetPath(texture2D).Replace("Assets/AssetsPackage/", "");
        image.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect); 
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}