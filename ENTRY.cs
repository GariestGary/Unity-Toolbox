#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VolumeBox.Toolbox.UIInformer;
using Cysharp.Threading.Tasks;

namespace VolumeBox.Toolbox
{   
    public class ENTRY : MonoBehaviour
    {
        private SettingsData settings => StaticData.Settings;

        private AudioPlayer audioPlayer;
        private Resolver resolver;
        private Messenger messenger;
        private Traveler traveler;
        private Updater updater;
        private Pooler pooler;
        private Saver saver;
        private Info info; 

        private void Awake()
        {
#pragma warning disable
            Init();
#pragma warning enable
        }

        private async UniTask Init()
        {
#if UNITY_EDITOR
            while (!EditorPlayStateHandler.EditorReady)
            {
                await UniTask.Yield();
            }
#endif

            Application.targetFrameRate = settings.TargetFrameRate;

            Resolver.Instance.RunInternal();

            Resolver.AddInstance(Resolver.Instance);

            Resolver.AddInstance(AudioPlayer.Instance);
            Resolver.AddInstance(Messenger.Instance);
            Resolver.AddInstance(Traveler.Instance);
            Resolver.AddInstance(Updater.Instance);
            Resolver.AddInstance(Pooler.Instance);
            Resolver.AddInstance(Saver.Instance);
            Resolver.AddInstance(Info.Instance);

            Resolver.InjectInstances();

            AudioPlayer.Instance.RunInternal();
            Messenger.Instance.RunInternal();
            Traveler.Instance.RunInternal();
            Updater.Instance.RunInternal();
            Pooler.Instance.RunInternal();
            Saver.Instance.RunInternal();
            Info.Instance.RunInternal();

            Updater.InitializeObjects(SceneManager.GetActiveScene().GetRootGameObjects());

            await Fader.In(0);

            if (!string.IsNullOrEmpty(settings.InitialSceneName) || settings.InitialSceneName != "MAIN")
            {
                await Traveler.LoadScene(settings.InitialSceneName, settings.InitialSceneArgs);

                if (!settings.ManualFadeOut)
                {
                    await Fader.Out(settings.FadeOutDuration);
                }
            }
        }

        private void OnDisable()
        {
            AudioPlayer.Instance.ClearInternal();
            Messenger.Instance.ClearInternal();
            Traveler.Instance.ClearInternal();
            Updater.Instance.ClearInternal();
            Pooler.Instance.ClearInternal();
            Saver.Instance.ClearInternal();
            Info.Instance.ClearInternal();
        }
    }
}
