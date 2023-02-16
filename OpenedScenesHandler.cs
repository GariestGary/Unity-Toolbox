#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    [InitializeOnLoad]
    public static class OpenedScenesHandler
    {
        private static List<string> _scenesOpenedAtStart;

        static OpenedScenesHandler()
        {
            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                if (_scenesOpenedAtStart != null && _scenesOpenedAtStart.Count > 0)
                {
                    for (int i = 0; i < _scenesOpenedAtStart.Count; i++)
                    {
                        EditorSceneManager.LoadScene(_scenesOpenedAtStart[i]);
                    }
                }

                return;
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                HandleOpenedScenes();
            }
        }
        
        private static void HandleOpenedScenes()
        {
            _scenesOpenedAtStart = new List<string>();
            Debug.Log("initialized opened scenes handler");
            int scenesCount = SceneManager.sceneCount;
            
            for (int i = 0; i < scenesCount; i++)
            {
                string sceneName = SceneManager.GetSceneAt(i).name;

                if (sceneName != "MAIN")
                {
                    _scenesOpenedAtStart.Add(sceneName);
                    
                    AsyncOperation unload = SceneManager.UnloadSceneAsync(sceneName);
                }
            }
        }
    }
}
#endif