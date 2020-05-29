using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(KVResourceMgr))]
    public class KVResourceMgrEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("只能在运行时查看");
                return;
            }
            
            var kvResourceMgr = target as KVResourceMgr;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("实例化的对象");
            var _instantiateObjs = kvResourceMgr._instantiateObjs;
            foreach (var key in _instantiateObjs.Keys)
            {
                EditorGUILayout.LabelField($"{key} count:{_instantiateObjs[key].Count}");
                foreach (var obj in _instantiateObjs[key])
                {
                    EditorGUILayout.ObjectField(obj, typeof(GameObject));
                }
            }
            EditorGUILayout.EndVertical();
            
        }
    }
}