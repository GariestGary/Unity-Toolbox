#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace VolumeBox.Toolbox
{   
    public class ENTRY : MonoBehaviour
    {
        private SettingsData settings => StaticData.Settings;

        private AudioPlayer audioPlayer;
        private Messenger messenger;
        private Traveler traveler;
        private Updater updater;
        private Pooler pooler;
        private Database saver;

        private void Awake()
        {
#pragma warning disable
            Init();
#pragma warning enable
        }

        private async UniTask Init()
        {
#if UNITY_EDITOR
            while (!EditorLoadUtils.EditorReady)
            {
                await UniTask.Yield();
            }
#endif
            AudioPlayer.Instance.RunInternal();
            Messenger.Instance.RunInternal();
            Traveler.Instance.RunInternal();
            Updater.Instance.RunInternal();
            Pooler.Instance.RunInternal();
            Database.Instance.RunInternal();

            Updater.InitializeObjects(SceneManager.GetActiveScene().GetRootGameObjects());

            await Fader.In(0);

            if (!string.IsNullOrEmpty(settings.InitialSceneName) && settings.InitialSceneName != "MAIN")
            {
                await Traveler.LoadScene(settings.InitialSceneName, settings.InitialSceneArgs);

                if (!settings.ManualFadeOut)
                {
                    await Fader.Out(settings.FadeOutDuration);
                }
            }

            Application.targetFrameRate = settings.TargetFrameRate;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void PlayStateChanged()
        {
            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAll();
            }
        }
#endif

        public static void UpdateTargetFramerate(int value)
        {
            Application.targetFrameRate = value;
        }

        private static void ClearAll()
        {
            AudioPlayer.Instance.ClearInternal();
            Messenger.Instance.ClearInternal();
            Traveler.Instance.ClearInternal();
            Updater.Instance.ClearInternal();
            Pooler.Instance.ClearInternal();
            Database.Instance.ClearInternal();
        }

        private void OnApplicationQuit()
        {
            ClearAll();
        }
    }
}
