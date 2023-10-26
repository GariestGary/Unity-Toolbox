#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public static class ResourcesUtils
    {
        public static T ResolveScriptable<T>(string path) where T: ScriptableObject
        {
            int lastDot = path.LastIndexOf('.');
            
            var tool = Resources.Load(path[..lastDot]);

#if UNITY_EDITOR
            if (tool == null)
            {
                var fullAssetPath = "Assets/Resources/" + path;
                int lastSlash = fullAssetPath.LastIndexOf('/');
                var dirPath = (lastSlash > -1) ? fullAssetPath[..lastSlash] : fullAssetPath;

                //Create folder if it doesn't exist
                if (!AssetDatabase.IsValidFolder(dirPath))
                {
                    string[] dirs = dirPath.Split("/");

                    for (int i = 1; i < dirs.Length; i++)
                    {
                        var parent = dirs[0];

                        for (int j = 1; j < i; j++)
                        {
                            parent += "/" + dirs[j];
                        }

                        AssetDatabase.CreateFolder(parent, dirs[i]);
                    }
                }
                
                tool = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(tool, fullAssetPath);
                AssetDatabase.SaveAssets();
            }
#endif

            return tool as T;
        }
    }
}