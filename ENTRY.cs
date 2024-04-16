#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace VolumeBox.Toolbox
{
    public class ENTRY : MonoBehaviour
    {
        private SettingsData m_Settings => StaticData.Settings;


        private void Awake()
        {
            Init().Forget();
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

            if(StaticData.Settings.AutoResolveScenesAtPlay)
            {
                await Fader.In(0);

                if (!string.IsNullOrEmpty(m_Settings.InitialSceneName) && m_Settings.InitialSceneName != "MAIN")
                {
                    await Traveler.LoadScene(m_Settings.InitialSceneName, m_Settings.InitialSceneArgs);

                    if (!m_Settings.ManualFadeOut)
                    {
                        await Fader.Out(m_Settings.FadeOutDuration);
                    }
                }
            }

            Application.targetFrameRate = m_Settings.TargetFrameRate;
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
