using UnityEditor;
using UnityEngine;

namespace KVGame
{
    [CreateAssetMenu(fileName = "KVGameSetting", menuName = "KVGame/CreateSetting", order = 50)] 
    public class KVGameSetting : ScriptableObject
    {
        public string CompanyName = "Default";
        public string Channel = "Default";
        public string ClientVersion = "1.0.000";
        public string BundleVersion = "1.0.000";
        public string BuildVersion = "0";
    }
}