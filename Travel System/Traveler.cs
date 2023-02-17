using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    public class Traveler : CachedSingleton<Traveler>, IRunner
    {
        [SerializeField] [Scene] private string uiScene;

        [Inject] private Updater _updater;
        [Inject] private Messager _messager;

        public string CurrentSceneName => _currentSceneName;

        private SceneArgs _currentOpeningSceneArgs;
        private AsyncOperation _unloadingScene;
        private AsyncOperation _loadingScene;
        private SceneHandlerBase _currentSceneHandler;
        private string _currentSceneName;
        private bool _uiOpened;
        private bool _loadingLevel;

        public void Run()
        {
            
        }

        public TArgs GetCurrentSceneArgs<TArgs>() where TArgs : SceneArgs
        {
            if (_currentOpeningSceneArgs == null) return null;

            if (!(_currentOpeningSceneArgs is TArgs))
            {
                Debug.LogError($"Current loading scene args is {_currentOpeningSceneArgs.GetType()}, but scene requires {typeof(TArgs)}");
                return null;
            }

            TArgs args = (TArgs)_currentOpeningSceneArgs;
            _currentOpeningSceneArgs = null;
            return args;
        }

        public async Task LoadScene(string sceneName, SceneArgs args = null, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
        {
            if (_loadingLevel) return;

            _currentOpeningSceneArgs = args;

            _loadingLevel = true;

            await Fader.Instance.FadeInForCoroutine(fadeInDuration);

            //skip unloading level if current level is null
            if (string.IsNullOrEmpty(_currentSceneName))
            {
                _unloadingScene = null;
            }
            else
            {
                if (_currentSceneHandler != null)
                {
                    _currentSceneHandler.OnSceneUnload();
                }
                
                await WaitForSceneUnloadCoroutine(_currentSceneName);
            }

            _loadingScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            //loadingScene.allowSceneActivation = false;

            while (!_loadingScene.isDone)
            {
                await Task.Yield();
            }

            _loadingScene.allowSceneActivation = true;

            await Task.Yield();

            await OpenLoadedScene(sceneName, fadeOutDuration);
        }

        private async Task OpenLoadedScene(string sceneName, float fadeOutDuration)
        {
            _currentSceneName = sceneName;

            GameObject[] rootObjs = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();

            SearchSceneBindings(rootObjs);

            _messager.Send(new SceneLoadedMessage(sceneName));
            
            _currentSceneHandler = null;

            foreach (var obj in rootObjs)
            {
                _currentSceneHandler = obj.GetComponent<SceneHandlerBase>();

                if (_currentSceneHandler != null)
                {
                    break;
                }
            }

            if (_currentSceneHandler == null)
            {
                Debug.Log("There is no SceneHandler in scene, skipping scene setup");
            }
            else
            {
                _updater.InitializeMono(_currentSceneHandler);
                _currentSceneHandler.OnLoadCallback();
            }

            _updater.InitializeObjects(rootObjs);
            _updater.OnSceneObjectsAdded(rootObjs);
            await Fader.Instance.FadeOutForCoroutine(fadeOutDuration);
            _messager.Send(new SceneOpenedMessage(sceneName));
            _messager.Send(new GameplaySceneOpenedMessage(sceneName));

            _loadingLevel = false;
        }

        private void SearchSceneBindings(GameObject[] objs)
        {
            List<SceneBinding> bindings = new List<SceneBinding>();

            foreach (var obj in objs)
            {
                Resolver.Instance.SearchObjectBindings(obj);
            }

            //messager.Send(new SceneBindingMessage() { instances = bindings });
        }

        private async Task WaitForSceneUnloadCoroutine(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;

            _messager.Send(new SceneUnloadingMessage(sceneName));
            _currentSceneHandler?.OnSceneUnload();

            GameObject[] rootObjs = SceneManager.GetSceneByName(CurrentSceneName).GetRootGameObjects();

            _updater.OnSceneObjectsRemoved(rootObjs);
            _updater.RemoveObjectsFromUpdate(rootObjs);

            string unloadingSceneName = _currentSceneName;

            _unloadingScene = SceneManager.UnloadSceneAsync(unloadingSceneName);

            while (!_unloadingScene.isDone)
            {
                await Task.Yield();
            }

            _messager.Send(new SceneUnloadedMessage(unloadingSceneName));
        }

        #region UI_Handle
        public async Task OpenUI()
        {
            if(string.IsNullOrEmpty(uiScene) || _uiOpened) return;

            AsyncOperation loadingUi = SceneManager.LoadSceneAsync(uiScene, LoadSceneMode.Additive);

            while (!loadingUi.isDone)
            {
                await Task.Yield();
            }

            GameObject[] uiObjs = SceneManager.GetSceneByName(uiScene).GetRootGameObjects();

            _updater.InitializeObjects(uiObjs);

            _uiOpened = true;
        }

        public async Task CloseUI()
        {
            if(string.IsNullOrEmpty(uiScene)) return;

            if(_uiOpened)
            {
                Scene ui = SceneManager.GetSceneByName(uiScene);

                if(!ui.isLoaded)
                {
                    return;
                }
                
                _updater.RemoveObjectsFromUpdate(ui.GetRootGameObjects());

                AsyncOperation unloadingUi = SceneManager.UnloadSceneAsync(uiScene);

                while (!unloadingUi.isDone)
                {
                    await Task.Yield();
                }

                _uiOpened = false;

                //TODO: messager.Send(Message.UI_CLOSED);
            }
        }
        #endregion
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
