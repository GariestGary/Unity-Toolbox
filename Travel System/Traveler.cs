using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    public class Traveler : MonoBehaviour, IRunner
    {
        [SerializeField] private string levelsFolderPath;
        [SerializeField] private string uiLevelName;

        private LevelHandler currentLevelHandler;
        private string currentLevelName;
        private bool uiOpened;
        private Scene mainScene;

        private AsyncOperation unloadingLevel;
        private AsyncOperation loadingLevel;

        public string CurrentLevelName => currentLevelName;

        [Inject] private Updater updater;
        [Inject] private Messager messager;
        [Inject] private Resolver resolver;

        public void Run()
        {
            mainScene = SceneManager.GetActiveScene();
            messager.Subscribe(Message.LOAD_SCENE, x => LoadScene(x as string));
        }

        
        public bool LoadScene(string sceneName)
	    {
            StartCoroutine(LoadSceneCoroutine(sceneName));

            return true;
        }

        public IEnumerator LoadSceneCoroutine(string sceneName)
        {
            if (currentLevelHandler == null)
            {   //skip unloading level if current level is null
                unloadingLevel = null;
            }
            else
            {
                messager.Send(Message.SCENE_UNLOADING);
                updater.RemoveObjectsFromUpdate(SceneManager.GetSceneByName(CurrentLevelName).GetRootGameObjects());
                //unloading scene async operation set
                unloadingLevel = SceneManager.UnloadSceneAsync(currentLevelName);
            }

            //loading scene async operation set
            loadingLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            yield return StartCoroutine(WaitForLoadCoroutine(sceneName));
        }

        private IEnumerator WaitForLoadCoroutine(string loadingLevelName)
        {
            while(!IsScenesReady())
            {
                yield return null;
            }

            messager.Send(Message.SCENE_LOADED);

            yield return OpenScene(loadingLevelName);

            yield break;
        }

        private IEnumerator OpenScene(string openLevelName)
        {
            currentLevelHandler = LevelHandler.Instance;
            currentLevelName = openLevelName;

            //get all objects in scene and adds it to update
            GameObject[] rootObjs = SceneManager.GetSceneByName(currentLevelName).GetRootGameObjects();
            
            updater.InitializeObjects(rootObjs);

            currentLevelHandler.SetupLevel();

            //open UI if it is gameplay level
            if(currentLevelHandler.IsGameplayLevel)
            {
                yield return StartCoroutine(OpenUI());
                messager.Send(Message.GAMEPLAY_SCENE_OPENED);
            }
            else
            {
                yield return StartCoroutine(CloseUI());
                messager.Send(Message.SCENE_OPENED);
            }
        }

        private IEnumerator OpenUI()
        {
            if(string.IsNullOrEmpty(uiLevelName)) yield break;

            if(!uiOpened)
            {
                AsyncOperation loadingUi = SceneManager.LoadSceneAsync(uiLevelName, LoadSceneMode.Additive);

                while (!loadingUi.isDone)
                {
                    yield return null;
                }

                GameObject[] uiObjs = SceneManager.GetSceneByName(uiLevelName).GetRootGameObjects();

                updater.InitializeObjects(uiObjs);

                uiOpened = true;

                messager.Send(Message.UI_OPENED);
            }
        }

        private IEnumerator CloseUI()
        {
            if(string.IsNullOrEmpty(uiLevelName)) yield break;

            if(uiOpened)
            {
                Scene ui = SceneManager.GetSceneByName(uiLevelName);

                if(!ui.isLoaded)
                {
                    yield break;
                }
                
                updater.RemoveObjectsFromUpdate(ui.GetRootGameObjects());

                AsyncOperation unloadingUi = SceneManager.UnloadSceneAsync(uiLevelName);

                while (!unloadingUi.isDone)
                {
                    yield return null;
                }

                uiOpened = false;
                messager.Send(Message.UI_CLOSED);
            }
        }

        private bool IsScenesReady()
        {
            if(unloadingLevel == null)
            {
                return loadingLevel.isDone;
            }
            else
            {
                return loadingLevel.isDone && unloadingLevel.isDone;
            }
        }
    }
}
