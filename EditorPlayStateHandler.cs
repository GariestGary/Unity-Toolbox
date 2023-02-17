#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    [InitializeOnLoad]
    public static class EditorPlayStateHandler
    {
        private const string DevelopmentScenePath = "Assets/Plugins/Unity Toolbox/Scenes/MAIN.unity";
        private const string ProductionScenePath = "Packages/com.volumebox.toolbox/Scenes/MAIN.unity";
        private const string PackageName = "com.volumebox.toolbox";
        private const string MainSceneName = "MAIN";
        
        private static List<string> _scenesOpenedAtStart;
        
        public static bool EditorReady { get; private set; }

        static EditorPlayStateHandler()
        {
            EditorApplication.playModeStateChanged += OnStateChanged;
            IsMainSceneCorrectInBuild();
        }

        private static async void OnStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorReady = false;
                
                if (_scenesOpenedAtStart != null && _scenesOpenedAtStart.Count > 0)
                {
                    for (int i = 0; i < _scenesOpenedAtStart.Count; i++)
                    {
                        EditorSceneManager.LoadScene(_scenesOpenedAtStart[i], LoadSceneMode.Additive);
                    }
                }

                return;
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EditorReady = false;
                
                await HandleOpenedScenes();

                EditorReady = true;
            }
        }
        
        private static async Task HandleOpenedScenes()
        {
            _scenesOpenedAtStart = new List<string>();
            Debug.Log("initialized opened scenes handler");
            int scenesCount = SceneManager.sceneCount;
            bool mainLoaded = false;

            AsyncOperation[] unloads = new AsyncOperation[scenesCount];
            
            for (int i = 0; i < scenesCount; i++)
            {
                string sceneName = SceneManager.GetSceneAt(i).name;

                if (sceneName == MainSceneName && !mainLoaded)
                {
                    mainLoaded = true;
                }
                else if(SceneManager.sceneCount > 1)
                {
                    _scenesOpenedAtStart.Add(sceneName);
                    
                    unloads[i] = SceneManager.UnloadSceneAsync(sceneName);
                }
            }

            while (!unloads.All(x =>
                   {
                       if (x == null) return true;
                       return x.isDone;
                   }))
            {
                await Task.Yield();
            }

            AsyncOperation loading = null;
            
            if (!mainLoaded)
            {
                loading = SceneManager.LoadSceneAsync(0);
            }

            while (loading != null && !loading.isDone)
            {
                await Task.Yield();
            }
        }

        private static bool IsMainSceneCorrectInBuild()
        {
            if (EditorBuildSettings.scenes[0].path != DevelopmentScenePath && EditorBuildSettings.scenes[0].path != ProductionScenePath)
            {
                Debug.LogWarning("MAIN scene is not in build setting or it's index not 0. You can fix this from Toolbox/Init MAIN scene");
                return false;
            }

            return true;
        }

        private static async Task<string> GetCurrentMainScenePath()
        {
            var pack = Client.List();
            while (!pack.IsCompleted) await Task.Yield();

            string path = String.Empty;

            if (pack.Result.Any(x => x.name == PackageName))
            {
                return ProductionScenePath;
            }

            return DevelopmentScenePath;
        }

        [MenuItem("Toolbox/Init MAIN Scene")]
        public static async void InitializeMain()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            
            for (int i = 0; i < scenes.Count; i++)
            {
                string sceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
                
                if (sceneName == MainSceneName)
                {
                    scenes.RemoveAt(i);
                }
            }

            var path = await GetCurrentMainScenePath();
            
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));

            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("MAIN scene initialized");
        }

        [MenuItem("Toolbox/Open MAIN Scene")]
        public static async void OpenMainScene()
        {
            if(!IsMainSceneCorrectInBuild()) return;
            
            int scenesCount = EditorSceneManager.sceneCount;

            for (int i = 0; i < scenesCount; i++)
            {
                string sceneName = EditorSceneManager.GetSceneAt(i).name;

                if (sceneName == MainSceneName)
                {
                    Debug.Log("MAIN scene already opened");
                    return;
                }
            }

            Debug.Log("Opened MAIN scene");
            EditorSceneManager.OpenScene(await GetCurrentMainScenePath(), OpenSceneMode.Additive);
        }
    }
}
#endif