using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    public class Traveler : CachedSingleton<Traveler>, IRunner
    {
        private static AsyncOperation _currentUnloadingSceneOperation;
        private static AsyncOperation _currentLoadingSceneOperation;
        private static List<OpenedScene> _openedScenes = new List<OpenedScene>();
        private static Queue<QueuedScene> _scenesToOpen = new Queue<QueuedScene>();
        private static Queue<QueuedScene> _scenesToClose = new Queue<QueuedScene>();
        private static MethodInfo _onLoadMethod;

        public static List<OpenedScene> OpenedScenes => _openedScenes;

        public void Run()
        {
            _onLoadMethod = typeof(SceneHandlerBase).GetMethod("OnLoadCallback", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static T TryGetSceneHandler<T>() where T: SceneHandlerBase
        {
            OpenedScene scene = null;

            for(int i = 0; i < _openedScenes.Count; i++)
            {
                if (_openedScenes[i].Handler != null && _openedScenes[i].Handler.GetType() == typeof(T))
                {
                    scene = _openedScenes[i];
                    break;
                }
            }

            if(scene == null)
            {
                return null;
            }
            else
            {
                return scene.Handler as T;
            }
        }

        public static async Task LoadScene(string sceneName, SceneArgs args = null, float fadeIn = 0, float fadeOut = 0)
        {
            if(!DoesSceneExist(sceneName))
            {
                Debug.LogWarning($"Scene with name {sceneName} you want to load doesn't exist");
                return;
            }

            if(_currentLoadingSceneOperation != null && !string.IsNullOrEmpty(sceneName))
            {
                _scenesToOpen.Enqueue(new QueuedScene
                {
                    SceneName = sceneName,
                    Args = args,
                    FadeIn = fadeIn,
                    FadeOut = fadeOut
                });
                return;
            }

            await Fader.Instance.FadeInFor(fadeIn);

            _currentLoadingSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!_currentLoadingSceneOperation.isDone)
            {
                await Task.Yield();
            }


            Scene sceneDefinition = SceneManager.GetSceneByName(sceneName);

            SceneHandlerBase handler = null;

            GameObject[] sceneObjects = sceneDefinition.GetRootGameObjects();

            //Search for DI bindings
            foreach (var obj in sceneObjects)
            {
                Resolver.Instance.SearchObjectBindings(obj);
            }

            //Search for scene handler
            foreach (var obj in sceneObjects)
            {
                handler = obj.GetComponent<SceneHandlerBase>();

                if (handler != null)
                {
                    break;
                }
            }

            OpenedScene newOpenedScene = new OpenedScene(sceneDefinition, handler, args);

            if (handler == null)
            {
                Debug.Log("There is no SceneHandler in scene, skipping scene setup");
            }
            else
            {
                Updater.Instance.InitializeMono(handler);
                _onLoadMethod.Invoke(handler, new object[] { args });
            }

            _openedScenes.Add(newOpenedScene);

            Updater.Instance.InitializeObjects(sceneObjects);

            await Fader.Instance.FadeOutFor(fadeOut);

            _currentLoadingSceneOperation = null;

            Messager.Instance.Send(new SceneOpenedMessage(sceneName));

            if(_scenesToOpen.Count > 0)
            {
                var sceneToOpen = _scenesToOpen.Dequeue();

                LoadScene(sceneToOpen.SceneName, sceneToOpen.Args, sceneToOpen.FadeIn, sceneToOpen.FadeOut);
            }
        }

        public static async Task UnloadScene(string sceneName, float fadeIn = 0, float fadeOut = 0)
        {
            if (!_openedScenes.Any(x => x.SceneDefinition.name == sceneName))
            {
                Debug.LogWarning($"Scene with name {sceneName} you want to unload doesn't exist");
                return;
            }

            if (_currentUnloadingSceneOperation != null && !string.IsNullOrEmpty(sceneName))
            {
                _scenesToClose.Enqueue(new QueuedScene
                {
                    SceneName = sceneName,
                    FadeIn = fadeIn,
                    FadeOut = fadeOut
                });
                return;
            }

            await Fader.Instance.FadeInFor(fadeIn);

            _currentUnloadingSceneOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!_currentUnloadingSceneOperation.isDone)
            {
                await Task.Yield();
            }

            await Fader.Instance.FadeOutFor(fadeOut);

            _currentUnloadingSceneOperation = null;

            if (_scenesToOpen.Count > 0)
            {
                var sceneToClose = _scenesToClose.Dequeue();

                UnloadScene(sceneToClose.SceneName, sceneToClose.FadeIn, sceneToClose.FadeOut);
            }
        }

        public static bool DoesSceneExist(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/");
                var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0)
                    return true;
            }

            return false;
        }

        private struct QueuedScene
        {
            public string SceneName;
            public SceneArgs Args;
            public float FadeIn;
            public float FadeOut;
        }
    }

    public class OpenedScene
    {
        public Scene SceneDefinition { get; }
        public SceneArgs Args { get; } = null;
        public SceneHandlerBase Handler { get; } = null;

        public OpenedScene(Scene sceneDefinition, SceneHandlerBase sceneHandler, SceneArgs args)
        {
            SceneDefinition = sceneDefinition;
            Args = args;
            Handler = sceneHandler;
        }
    }

 #region Traveler's class messages
    public class UnloadScene: Message { }

    [Serializable]
    public abstract class SceneMessage: Message
    {
        public string SceneName => _sceneName;

        private string _sceneName;

        public SceneMessage(string sceneName)
        {
            _sceneName = sceneName;
        }

        public SceneMessage()
        {
            _sceneName = string.Empty;
        }
    }

    [Serializable]
    public class SceneUnloadingMessage : SceneMessage
    {
        public SceneUnloadingMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneUnloadingMessage()
        {

        }
    }

    [Serializable]
    public class SceneUnloadedMessage : SceneMessage
    {
        public SceneUnloadedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneUnloadedMessage()
        {
            
        }
    }

    [Serializable]
    public class SceneLoadingMessage : SceneMessage
    {
        public SceneLoadingMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneLoadingMessage()
        {

        }
    }

    [Serializable]
    public class SceneLoadedMessage : SceneMessage
    {
        public SceneLoadedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneLoadedMessage()
        {

        }
    }

    [Serializable]
    public class SceneOpenedMessage : SceneMessage
    {
        public SceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneOpenedMessage()
        {

        }
    }

    [Serializable]
    public class GameplaySceneOpenedMessage : SceneMessage
    {
        public GameplaySceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public GameplaySceneOpenedMessage()
        {

        }
    }

    public class UIOpenedMessage: Message { }

    public class UIClosedMessage: Message { }

    #endregion
}
