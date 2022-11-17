using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace VolumeBox.Toolbox
{
    public class Traveler : CachedSingleton<Traveler>, IRunner
    {
        [SerializeField] private string scenesFolderPath;
        [SerializeField] private string uiSceneName;

        [Inject] private Updater updater;
        [Inject] private Messager messager;

        public string CurrentSceneName => currentSceneName;

        private SceneArgs currentOpeningSceneArgs;
        private AsyncOperation unloadingScene;
        private AsyncOperation loadingScene;
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
            }

            TArgs args = (TArgs)currentOpeningSceneArgs;
            currentOpeningSceneArgs = null;
            return args;
        }

        public void LoadScene(string name, SceneArgs args = null)
        {
            if (loadingLevel)
            {
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
            if (string.IsNullOrEmpty(currentSceneName))
            {
                unloadingScene = null;
            }
            else
            {
                yield return StartCoroutine(WaitForSceneUnloadCoroutine());
            }

            loadingScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            while (!loadingScene.isDone)
            {
                yield return null;
            }

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


            if(sceneHandler != null)
            {
                updater.InitializeMono(sceneHandler);
            }

            updater.InitializeObjects(rootObjs);
            currentOpeningSceneArgs = null;
            messager.Send(new SceneOpenedMessage(name));
            messager.Send(new GameplaySceneOpenedMessage(name));

            yield return StartCoroutine(Fader.Instance.FadeOutCoroutine());

            loadingLevel = false;
        }

        private void SearchSceneBindings(GameObject[] objs)
        {
            List<SceneBinding> bindings = new List<SceneBinding>();

            foreach (var obj in objs)
            {
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

        private IEnumerator WaitForSceneUnloadCoroutine()
        {
            messager.Send(new SceneUnloadingMessage(currentSceneName));
            updater.RemoveObjectsFromUpdate(SceneManager.GetSceneByName(CurrentSceneName).GetRootGameObjects());

            string unloadingSceneName = currentSceneName;

            unloadingScene = SceneManager.UnloadSceneAsync(unloadingSceneName);

            while (!unloadingScene.isDone)
            {
                yield return null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            messager.Send(new SceneUnloadedMessage(unloadingSceneName));
        }

        #region UI_Handle
        private IEnumerator OpenUI()
        {
            if(string.IsNullOrEmpty(uiSceneName)) yield break;

            if(!uiOpened)
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
            if(string.IsNullOrEmpty(uiSceneName)) yield break;

            if(uiOpened)
            {
                Scene ui = SceneManager.GetSceneByName(uiSceneName);

                if(!ui.isLoaded)
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
        #endregion
    }

 #region Traveler's class messages
    public class SceneMessage
    {
        public string SceneName => _sceneName;

        private string _sceneName;

        public SceneMessage(string sceneName)
        {
            _sceneName = sceneName;
        }
    }

    public class SceneUnloadingMessage : SceneMessage
    {
        public SceneUnloadingMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class SceneUnloadedMessage : SceneMessage
    {
        public SceneUnloadedMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class SceneLoadingMessage : SceneMessage
    {
        public SceneLoadingMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class SceneLoadedMessage : SceneMessage
    {
        public SceneLoadedMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class SceneOpenedMessage : SceneMessage
    {
        public SceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class GameplaySceneOpenedMessage : SceneMessage
    {
        public GameplaySceneOpenedMessage(string sceneName) : base(sceneName)
        {
        }
    }

    public class UIOpenedMessage { }

    public class UIClosedMessage { }

    #endregion
}
