using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{
    public class SceneResolver: MonoBehaviour
    {
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void ResolveScene()
        {
            //CoroutineStarter.DontDestroy();
            //CoroutineStarter.Instance.StartCoroutine(ResolveSceneCoroutine());
        }

        private static IEnumerator ResolveSceneCoroutine()
        {
            Scene scene = SceneManager.GetActiveScene();

            switch(scene.buildIndex)
            {
                case 0:
                    Debug.Log("Main opened");
                    break;
                default:
                    SceneManager.LoadScene(0);
                    yield return SceneManager.UnloadSceneAsync(scene);
                    break;
            }

            yield return null;
        }
        #endif
    }
}
