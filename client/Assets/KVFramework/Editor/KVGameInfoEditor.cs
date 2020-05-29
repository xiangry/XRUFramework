using UnityEditor;

namespace KVGame
{
    [CustomEditor(typeof(KVGameSetting))]
    public class KVGameSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("游戏设置信息");
            base.OnInspectorGUI();
        }
    }
}
