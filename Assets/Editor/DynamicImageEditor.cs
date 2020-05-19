using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(DynamicImage))]
public class DynamicImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        DynamicImage image = target as DynamicImage;
        image.cacheTexture = EditorGUILayout.TextField("DynamicAsset", image.cacheTexture);
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}