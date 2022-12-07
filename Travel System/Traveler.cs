using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    public class Traveler: CachedSingleton<Traveler>, IRunner
    {
        [SerializeField] private string scenesFolderPath;
        [SerializeField] [Scene] private string uiSceneName;
        [SerializeField] private bool manualSceneUnload;
        [SerializeField] private bool manualSceneOpening;

        [Inject] private Updater updater;
        [Inject] private Messager messager;

        public string CurrentSceneName => currentSceneName;

        private SceneArgs currentOpeningSceneArgs;
        private AsyncOperation unloadingScene;
        private AsyncOperation loadingScene;
        private string currentSceneName;
        private string sceneToManualUnload;
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

        public void LoadScene(string name, SceneArgs args = null)
        {
            if (loadingLevel) return;

            if (!string.IsNullOrEmpty(sceneToManualUnload))
            {
                Debug.LogError("Firstly unload previous scene");
                return;
            }

            currentOpeningSceneArgs = args;

            StartCoroutine(LoadSceneCoroutine(name));
        }

        private IEnumerator LoadSceneCoroutine(string name)
        {
            loadingLevel = true;

            yield return StartCoroutine(Fader.Instance.FadeInCoroutine());

            //skip unloading level if current level is null
            if (string.IsNullOrEmpty(currentSceneName) || manualSceneUnload)
            {
                sceneToManualUnload = currentSceneName;
                unloadingScene = null;
            }
            else
            {
                yield return StartCoroutine(WaitForSceneUnloadCoroutine(currentSceneName));
            }

            loadingScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            //loadingScene.allowSceneActivation = false;

            while (!loadingScene.isDone)
            {
                yield return null;
            }

            if (!manualSceneOpening)
            {
                loadingScene.allowSceneActivation = true;

                OpenLoadedScene(name);
            }
        }

        public void OpenLoadedScene(string name)
        {
            currentSceneName = name;

            messager.Send(new SceneLoadedMessage(name));

            GameObject[] rootObjs = SceneManager.GetSceneByName(name).GetRootGameObjects();

            SearchSceneBindings(rootObjs);

            SceneHandlerBase sceneHandler = null;

            foreach (var obj in rootObjs)
            {
                sceneHandler = obj.GetComponent<SceneHandlerBase>();

                if (sceneHandler != null)
                {
                    break;
                }
            }

            if (sceneHandler == null)
            {
                Debug.Log("There is no SceneHandler in scene, skipping scene setup");
            }
            else
            {
                updater.InitializeMono(sceneHandler);
                sceneHandler.OnLoadCallback();
            }

            updater.InitializeMono(sceneHandler);
            updater.InitializeObjects(rootObjs);
            Fader.Instance.FadeOut();
            messager.Send(new SceneOpenedMessage(name));
            messager.Send(new GameplaySceneOpenedMessage(name));

            loadingLevel = false;
        }

        private void SearchSceneBindings(GameObject[] objs)
        {
            List<SceneBinding> bindings = new List<SceneBinding>();

            foreach (var obj in objs)
            {
                var canvas = obj.GetComponent<Canvas>();

                if (canvas != null)
                {
                }

                foreach (var cb in obj.GetComponentsInChildren<ComponentBinding>())
                {
                    if (cb.Context != null)
                    {
                        bindings.Add(new SceneBinding() { instance = cb.Context, id = cb.Id });
                    }
                }
            }

            messager.Send(new SceneBindingMessage() { instances = bindings });
        }

        public void AllowSceneOpen()
        {
            if (loadingScene == null) return;

            loadingScene.allowSceneActivation = true;
        }

        public void UnloadPreviousScene()
        {
            StartCoroutine(WaitForSceneUnloadCoroutine(sceneToManualUnload));
        }

        private IEnumerator WaitForSceneUnloadCoroutine(string name)
        {
            if (string.IsNullOrEmpty(name)) yield break;

            messager.Send(new SceneUnloadingMessage(name));
            updater.RemoveObjectsFromUpdate(SceneManager.GetSceneByName(CurrentSceneName).GetRootGameObjects());

            string unloadingSceneName = currentSceneName;

            unloadingScene = SceneManager.UnloadSceneAsync(unloadingSceneName);

            while (!unloadingScene.isDone)
            {
                yield return null;
            }

            messager.Send(new SceneUnloadedMessage(unloadingSceneName));

            sceneToManualUnload = string.Empty;
        }

        #region UI_Handle

        private IEnumerator OpenUI()
        {
            if (string.IsNullOrEmpty(uiSceneName)) yield break;

            if (!uiOpened)
            {
                AsyncOperation loadingUi = SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);

                while (!loadingUi.isDone)
                {
                    yield return null;
                }

                GameObject[] uiObjs = SceneManager.GetSceneByName(uiSceneName).GetRootGameObjects();

                updater.InitializeObjects(uiObjs);

                uiOpened = true;

                //TODO: messager.Send(Message.UI_OPENED);
            }
        }

        private IEnumerator CloseUI()
        {
            if (string.IsNullOrEmpty(uiSceneName)) yield break;

            if (uiOpened)
            {
                Scene ui = SceneManager.GetSceneByName(uiSceneName);

                if (!ui.isLoaded)
                {
                    yield break;
                }

                updater.RemoveObjectsFromUpdate(ui.GetRootGameObjects());

                AsyncOperation unloadingUi = SceneManager.UnloadSceneAsync(uiSceneName);

                while (!unloadingUi.isDone)
                {
                    yield return null;
                }

                uiOpened = false;

                //TODO: messager.Send(Message.UI_CLOSED);
            }
        }

        #endregion UI_Handle
    }

    #region Traveler's class messages

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

        }
    }

    [Serializable]
    public class SceneUnloadingMessage: SceneMessage
    {
        public SceneUnloadingMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneUnloadingMessage(): base() { }
    }

    [Serializable]
    public class SceneUnloadedMessage: SceneMessage
    {
        public SceneUnloadedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneUnloadedMessage(): base() { }
    }

    [Serializable]
    public class SceneLoadingMessage: SceneMessage
    {
        public SceneLoadingMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneLoadingMessage(): base() { }
    }

    [Serializable]
    public class SceneLoadedMessage: SceneMessage
    {
        public SceneLoadedMessage(string sceneName) : base(sceneName)
        {
        }
        
        public SceneLoadedMessage(): base() { }
    }

    [Serializable]
    public class SceneOpenedMessage: SceneMessage
    {
        public SceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public SceneOpenedMessage(): base() { }
    }

    [Serializable]
    public class GameplaySceneOpenedMessage: SceneMessage
    {
        public GameplaySceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }

        public GameplaySceneOpenedMessage(): base() { }
    }

    public class UIOpenedMessage
    { }

    public class UIClosedMessage
    { }

    #endregion Traveler's class messages
}