using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using static UnityEngine.GraphicsBuffer;

namespace VolumeBox.Toolbox
{
    public class Traveler : MonoBehaviour, IRunner
    {
        [SerializeField] private string levelsFolderPath;
        [SerializeField] private string uiLevelName;

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
            messager.Subscribe<LoadSceneMessage<LevelHandler<LevelArgs>, LevelArgs>>(x => LoadScene(x.name));
        }

        public void LoadScene(string name)
        {
            if(true)//if (currentLevelHandler == null)
            {   //skip unloading level if current level is null
                unloadingLevel = null;
            }
            else
            {
                //TODO: messager.Send(Message.SCENE_UNLOADING, currentLevelName);
                updater.RemoveObjectsFromUpdate(SceneManager.GetSceneByName(CurrentLevelName).GetRootGameObjects());
                //unloading scene async operation set
                //yield return StartCoroutine(WaitForSceneUnloadCoroutine());
            }

            //loading scene async operation set
            //loadingLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            //yield return StartCoroutine(WaitForLoadCoroutine(sceneName));
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

                //TODO: messager.Send(Message.UI_OPENED);
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
                //TODO: messager.Send(Message.UI_CLOSED);
            }
        }
    }

    public class LoadSceneMessage<THandler, TArgs> 
        where THandler : LevelHandler<TArgs>
        where TArgs : LevelArgs, new()
    {
        public string name;
    }
}
