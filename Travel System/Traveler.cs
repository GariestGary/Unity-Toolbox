using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
    public class Traveler : MonoBehaviour, IRunner
    {
        [SerializeField] private string scenesFolderPath;
        [SerializeField] private string uiSceneName;

        private SceneHandler currentSceneHandler;
        private SceneArgs currentOpeningSceneArgs;
        private string currentSceneName;
        private bool uiOpened;

        private AsyncOperation unloadingScene;
        private AsyncOperation loadingScene;

        public string CurrentSceneName => currentSceneName;
        public SceneHandler CurrentSceneHandler => currentSceneHandler;

        [Inject] private Updater updater;
        [Inject] private Messager messager;

        public void Run()
        {
            messager.Subscribe<LoadSceneMessage>(x => 
            {
                currentOpeningSceneArgs = x.Args;
                LoadScene(x.SceneName);
            });
        }
        
        public bool LoadScene(string sceneName)
	    {
            StartCoroutine(LoadSceneCoroutine(sceneName));

            return true;
        }

        public IEnumerator LoadSceneCoroutine(string sceneName)
        {
            //skip unloading level if current level is null
            if (currentSceneHandler == null)
            { 
                unloadingScene = null;
            }
            else
            {
                yield return StartCoroutine(WaitForSceneUnloadCoroutine());
            }

            yield return StartCoroutine(WaitForLoadCoroutine(sceneName));
        }

        private IEnumerator WaitForLoadCoroutine(string loadingSceneName)
        {
            loadingScene = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

            while(!loadingScene.isDone)
            {
                yield return null;
            }

            messager.Send(new SceneLoadedMessage(loadingSceneName));

            yield return OpenScene(loadingSceneName);
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
            
            messager.Send(new SceneUnloadedMessage(unloadingSceneName));
        }

        private IEnumerator OpenScene(string openingSceneName)
        {
            GameObject[] rootObjs = SceneManager.GetSceneByName(openingSceneName).GetRootGameObjects();

            SceneHandler openedSceneHandler = null;

            rootObjs.FirstOrDefault(x => x.TryGetComponent(out openedSceneHandler));

            if(openedSceneHandler == null)
            {
                Debug.LogWarning("There is no SceneHandler in opened scene, creating a new one");
                GameObject sceneHandlerGameObject = new GameObject("Scene Handler", typeof(SceneHandler));
                SceneManager.MoveGameObjectToScene(sceneHandlerGameObject, SceneManager.GetSceneByName(openingSceneName));
                openedSceneHandler = sceneHandlerGameObject.GetComponent<SceneHandler>();
                currentSceneHandler = openedSceneHandler;
            }    
            
            if(currentSceneHandler.IsGameplayScene)
            {
                yield return StartCoroutine(OpenUI());
            }
            else
            {
                yield return StartCoroutine(CloseUI());
            }

            updater.InitializeObject(openedSceneHandler.gameObject);
            updater.InitializeObjects(rootObjs);
            currentSceneHandler.SetupLevel(currentOpeningSceneArgs);
            currentOpeningSceneArgs = null;
            messager.Send(new SceneOpenedMessage(openingSceneName));
            messager.Send(new GameplaySceneOpenedMessage(openingSceneName));
        }

        #region UI
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

                messager.Send(new UIOpenedMessage());
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
                messager.Send(new UIClosedMessage());
            }
        }
        #endregion
    }
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

public class LoadSceneMessage : SceneMessage
{
    public SceneArgs Args => _args;

    private SceneArgs _args;

    public LoadSceneMessage(string sceneName, SceneArgs args) : base(sceneName)
    {
        _args = args;
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
