using System.IO;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace KVGame
{
    public class KVPackageHelper
    {
        public static string GetAddressableBuildPath()
        {
            return UnityEngine.AddressableAssets.Addressables.BuildPath;
        }

        public static string GetChannelOutputPath(BuildTarget target, string channelName)
        {
            string outputPath = Path.Combine(Application.dataPath, $"../Build/{target.ToString()}");
            UtilityGame.CheckDirAndCreateWhenNeeded(outputPath);
            return outputPath;
        }

        public static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e != null && e.enabled)
                {
                    names.Add(e.path);
                }
            }
            return names.ToArray();
        }
       
    }
}