using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using NaughtyAttributes;

namespace VolumeBox.Toolbox
{
    public class Traveler : CachedSingleton<Traveler>, IRunner
    {
        [SerializeField] private bool useUiScene;
        [ShowIf(nameof(useUiScene))]
        [SerializeField] [Scene] private string uiScene;

        [Inject] private Updater updater;
        [Inject] private Messager messager;

        public string CurrentSceneName => currentSceneName;

        private SceneArgs currentOpeningSceneArgs;
        private AsyncOperation unloadingScene;
        private AsyncOperation loadingScene;
        private SceneHandlerBase currentSceneHandler;
        private string currentSceneName;
        private bool uiOpened;
        private bool loadingLevel;

        public void Run()
        {
            
        }

        public TArgs GetCurrentSceneArgs<TArgs>() where TArgs : SceneArgs
        {
            if (currentOpeningSceneArgs == null) return null;

            if (!(currentOpeningSceneArgs is TArgs))
            {
                Debug.LogError($"Current loading scene args is {currentOpeningSceneArgs.GetType()}, but scene requires {typeof(TArgs)}");
                return null;
            }

            TArgs args = (TArgs)currentOpeningSceneArgs;
            currentOpeningSceneArgs = null;
            return args;
        }

        public async Task LoadScene(string name, SceneArgs args = null, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
        {
            if (loadingLevel) return;

            currentOpeningSceneArgs = args;

            await LoadSceneCoroutine(name, fadeInDuration, fadeOutDuration);
        }

        private async Task LoadSceneCoroutine(string name, float fadeInDuration, float fadeOutDuration)
        {
            loadingLevel = true;

            //if(currentOpeningSceneArgs != null)
            //{
            //    Fader.Instance.SetFillImage(currentOpeningSceneArgs.backgroundSprite);
            //}
            //else
            //{
            //    Fader.Instance.SetFillImage(null);
            //}

            await Fader.Instance.FadeInForCoroutine(fadeInDuration);

            if (useUiScene)
            {
                await OpenUI();
            }

            //skip unloading level if current level is null
            if (string.IsNullOrEmpty(currentSceneName))
            {
                unloadingScene = null;
            }
            else
            {
                if (currentSceneHandler != null)
                {
                    currentSceneHandler.OnSceneUnload();
                }
                
                await WaitForSceneUnloadCoroutine(currentSceneName);
            }

            loadingScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            //loadingScene.allowSceneActivation = false;

            while (!loadingScene.isDone)
            {
                await Task.Yield();
            }

            loadingScene.allowSceneActivation = true;

            await Task.Yield();

            await OpenLoadedScene(name, fadeOutDuration);
        }

        public async Task OpenLoadedScene(string name, float fadeOutDuration)
        {
            currentSceneName = name;

            GameObject[] rootObjs = SceneManager.GetSceneByName(name).GetRootGameObjects();

            SearchSceneBindings(rootObjs);

            messager.Send(new SceneLoadedMessage(name));
            
            currentSceneHandler = null;

            foreach (var obj in rootObjs)
            {
                currentSceneHandler = obj.GetComponent<SceneHandlerBase>();

                if (currentSceneHandler != null)
                {
                    break;
                }
            }

            if (currentSceneHandler == null)
            {
                Debug.Log("There is no SceneHandler in scene, skipping scene setup");
            }
            else
            {
                updater.InitializeMono(currentSceneHandler);
                currentSceneHandler.OnLoadCallback();
            }

            updater.InitializeObjects(rootObjs);
            updater.OnSceneObjectsAdded(rootObjs);
            await Fader.Instance.FadeOutForCoroutine(fadeOutDuration);
            messager.Send(new SceneOpenedMessage(name));
            messager.Send(new GameplaySceneOpenedMessage(name));

            loadingLevel = false;
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

        public void AllowSceneOpen()
        {
            if (loadingScene == null) return;

            loadingScene.allowSceneActivation = true;
        }

        private async Task WaitForSceneUnloadCoroutine(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            messager.Send(new SceneUnloadingMessage(name));
            currentSceneHandler?.OnSceneUnload();

            GameObject[] rootObjs = SceneManager.GetSceneByName(CurrentSceneName).GetRootGameObjects();

            updater.OnSceneObjectsRemoved(rootObjs);
            updater.RemoveObjectsFromUpdate(rootObjs);

            string unloadingSceneName = currentSceneName;

            unloadingScene = SceneManager.UnloadSceneAsync(unloadingSceneName);

            while (!unloadingScene.isDone)
            {
                await Task.Yield();
            }

            messager.Send(new SceneUnloadedMessage(unloadingSceneName));
        }

        #region UI_Handle
        private async Task OpenUI()
        {
            if(string.IsNullOrEmpty(uiScene) || uiOpened) return;

            AsyncOperation loadingUi = SceneManager.LoadSceneAsync(uiScene, LoadSceneMode.Additive);

            while (!loadingUi.isDone)
            {
                await Task.Yield();
            }

            GameObject[] uiObjs = SceneManager.GetSceneByName(uiScene).GetRootGameObjects();

            updater.InitializeObjects(uiObjs);

            uiOpened = true;
        }

        private async Task CloseUI()
        {
            if(string.IsNullOrEmpty(uiScene)) return;

            if(uiOpened)
            {
                Scene ui = SceneManager.GetSceneByName(uiScene);

                if(!ui.isLoaded)
                {
                    return;
                }
                
                updater.RemoveObjectsFromUpdate(ui.GetRootGameObjects());

                AsyncOperation unloadingUi = SceneManager.UnloadSceneAsync(uiScene);

                while (!unloadingUi.isDone)
                {
                    await Task.Yield();
                }

                uiOpened = false;

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

        public SceneUnloadingMessage() : base()
        {

        }
    }

    [Serializable]
    public class SceneUnloadedMessage : SceneMessage
    {
        public SceneUnloadedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneUnloadedMessage() : base()
        {
            
        }
    }

    [Serializable]
    public class SceneLoadingMessage : SceneMessage
    {
        public SceneLoadingMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneLoadingMessage() : base()
        {

        }
    }

    [Serializable]
    public class SceneLoadedMessage : SceneMessage
    {
        public SceneLoadedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneLoadedMessage() : base()
        {

        }
    }

    [Serializable]
    public class SceneOpenedMessage : SceneMessage
    {
        public SceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneOpenedMessage() : base()
        {

        }
    }

    [Serializable]
    public class GameplaySceneOpenedMessage : SceneMessage
    {
        public GameplaySceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public GameplaySceneOpenedMessage() : base()
        {

        }
    }

    public class UIOpenedMessage: Message { }

    public class UIClosedMessage: Message { }

    #endregion
}
