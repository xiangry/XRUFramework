using System;
using UnityEngine;

namespace Test.ResManager
{
    public class TestResManager : MonoBehaviour
    {
        private Transform _objPool = null;

        private GUIStyle _btnGuiStyle;
        private GUIStyle _textFieldGuiStyle;

        private AudioSource _audioSource;
        

        private string inputRes = "sound/prefabs/systemsot_bgm";
        
        private string[] _prefabs = new[]
        {
            "prefabs/scene/level1",
            "prefabs/scene/level2",
            "prefabs/scene/level3",
            "prefabs/scene/level4",
            "prefabs/scene/level5",
        };

        private void Start()
        {
            _objPool = new GameObject("__OBJECT_POOL__").transform;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(new GameObject("AudioManager"));
            InitAudioSource();
        }
        
        private void OnGUI()
        {
            if (_btnGuiStyle == null)
            {
                _btnGuiStyle = new GUIStyle(GUI.skin.button);
                _btnGuiStyle.name = "KVDebugButton";
                _btnGuiStyle.fontSize = 20;
            }
            if (_textFieldGuiStyle == null)
            {
                _textFieldGuiStyle = new GUIStyle(GUI.skin.textField);
                _textFieldGuiStyle.name = "KVDebugTextField";
                _textFieldGuiStyle.fontSize = 20;
            }
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 5; i++)
            {
                var label = $"Level{i + 1}";
                if (GUILayout.Button(label, _btnGuiStyle))
                {
                    SwitchScene(i + 1);
                }
            }
            GUILayout.EndHorizontal();
        
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _prefabs.Length; i++)
            {
                var label = $"{_prefabs[i]}";
                if (GUILayout.Button(label, _btnGuiStyle))
                {
                    LoadPrefab(i);
                }
            }
            GUILayout.EndHorizontal();
        
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _prefabs.Length; i++)
            {
                var label = $"Destroy:{_prefabs[i]}";
                if (GUILayout.Button(label, _btnGuiStyle))
                {
                    KVResourceMgr.Instance.DestroyInstantiateObject(_prefabs[i]);
                }
            }
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            inputRes = GUILayout.TextField(inputRes, _textFieldGuiStyle);
            if (GUILayout.Button("加载", _btnGuiStyle))
            {
                LoadPrefab(inputRes);
            }
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("测试音效", _btnGuiStyle))
            {
                TestSound("sound/_clip/01.mp3");
            }
            GUILayout.EndHorizontal();
        }
        
        void SwitchScene(int level)
        {
            var path = $"scenes/Level{level}";
            KVResourceMgr.Instance.LoadSceneAsync(path, arg0 =>
            {
                _objPool = new GameObject("__OBJECT_POOL__").transform;
            });
        }
    
        void LoadPrefab(int index)
        {
            var path = $"{_prefabs[index]}";
            LoadPrefab(path);
        }
        
        void LoadPrefab(string path)
        {
            KVResourceMgr.Instance.InstanceObjectAsync(path, _objPool.transform, obj =>
            {
                GameObject gameObject = obj as GameObject;
                if (gameObject)
                {
                    gameObject.transform.position = Vector3.zero;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                }
            });
        }

        void InitAudioSource()
        {
            GameObject audioManager = GameObject.Find("AudioManager");
            KVResourceMgr.Instance.InstanceObjectAsync("sound/normal_sound", audioManager.transform, obj =>
            {
                GameObject gameObject = obj as GameObject;
                if (gameObject)
                {
                    gameObject.transform.position = Vector3.zero;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                }

                _audioSource = gameObject.GetComponent<AudioSource>();
                DontDestroyOnLoad(gameObject);
            });
        }
        
        void TestSound(string path)
        {
            if (!_audioSource)
            {
                Debug.Log("audio source not loaded");
                return;
            }
            KVResourceMgr.Instance.LoadAssetAsync(path,obj1 =>
            {
                AudioClip clip = obj1 as AudioClip;
                if (clip)
                {
                    _audioSource.clip = clip;
                    _audioSource.Play();
                }
            });
        }
    }
}