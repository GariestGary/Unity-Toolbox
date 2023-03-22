using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox
{
    public class Traveler : CachedSingleton<Traveler>, IRunner
    {
        private static AsyncOperation _currentUnloadingSceneOperation;
        private static AsyncOperation _currentLoadingSceneOperation;
        private static List<OpenedScene> _openedScenes = new List<OpenedScene>();
        private static MethodInfo _onLoadMethod;
        private static MethodInfo _onUnloadMethod;

        public void Run()
        {
            _openedScenes = new List<OpenedScene>();
            _onLoadMethod = typeof(SceneHandlerBase).GetMethod("OnLoadCallback", BindingFlags.NonPublic | BindingFlags.Instance);
            _onUnloadMethod = typeof(SceneHandlerBase).GetMethod("OnSceneUnload", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Returns SceneHandler with specified type, if it exists among loaded scenes
        /// </summary>
        /// <typeparam name="T">Type of SceneHandler which located in necessary scene hierarchy</typeparam>
        /// <returns>Instance of an requested SceneHandler, or null if it doesn't exist</returns>
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

        /// <summary>
        /// Loads scene with given name, args and fade time. It's recommend to use it with async/await, to prevent errors while loading and unloading scenes at the same time.
        /// </summary>
        /// <param name="sceneName">scene name other than empty string</param>
        /// <param name="args">custom scene arguments, null by default</param>
        /// <param name="fadeIn">fade in duration before scene starts loading, 0 by default</param>
        /// <param name="fadeOut">fade out duration after scene is loaded, 0 by default</param>
        public static async Task LoadScene(string sceneName, SceneArgs args = null, float fadeIn = 0, float fadeOut = 0)
        {
            if(!DoesSceneExist(sceneName))
            {
                Debug.LogWarning($"Scene with name {sceneName} you want to load doesn't exist");
                return;
            }

            await QueueSceneLoad(sceneName);

            await Fader.In(fadeIn);

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
                Debug.Log($"There is no SceneHandler in scene '{sceneName}', skipping scene setup");
            }
            else
            {
                Updater.Instance.InitializeMono(handler);
                _onLoadMethod.Invoke(handler, new object[] { args });
            }

            _openedScenes.Add(newOpenedScene);

            Updater.Instance.InitializeObjects(sceneObjects);

            await Fader.Out(fadeOut);

            _currentLoadingSceneOperation = null;

            Messager.Instance.Send(new SceneOpenedMessage(sceneName));
        }

        /// <summary>
        /// Unloads scene if it loaded now. It's recommend to use it with async/await, to prevent errors while loading and unloading scenes at the same time.
        /// </summary>
        /// <param name="sceneName">scene name other than empty string</param>
        /// <param name="fadeIn">fade in duration before scene starts unloading, 0 by default</param>
        /// <param name="fadeOut">fade out duration after scene is unloaded, 0 by default</param>
        public static async Task UnloadScene(string sceneName, float fadeIn = 0, float fadeOut = 0)
        {
            OpenedScene sceneToUnload = _openedScenes.FirstOrDefault(x => x.SceneDefinition.name == sceneName);

            if (sceneToUnload == null)
            {
                Debug.LogWarning($"Scene with name {sceneName} you want to unload doesn't exist");
                return;
            }

            await QueueSceneLoad(sceneName);

            await Fader.In(fadeIn);

            _onUnloadMethod.Invoke(sceneToUnload.Handler, null);

            _currentUnloadingSceneOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!_currentUnloadingSceneOperation.isDone)
            {
                await Task.Yield();
            }

            await Fader.Out(fadeOut);

            _currentUnloadingSceneOperation = null;
        }

        private static async Task QueueSceneLoad(string sceneName)
        {
            if(!CanLoadSceneNow(sceneName))
            {
                bool unloaded = _currentUnloadingSceneOperation == null || _currentUnloadingSceneOperation.isDone;
                bool loaded = _currentLoadingSceneOperation == null || _currentLoadingSceneOperation.isDone;

                while (!unloaded || !loaded)
                {
                    await Task.Yield();
                }
            }
        }

        private static bool CanLoadSceneNow(string sceneName)
        {
            return _currentUnloadingSceneOperation == null && _currentLoadingSceneOperation == null && !string.IsNullOrEmpty(sceneName);
        }

        private static bool DoesSceneExist(string name)
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

        private class OpenedScene
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
