using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VolumeBox.Toolbox
{
    public abstract class Fader : Singleton<Fader>
    {
        [SerializeField] protected bool useFade;

        protected bool _fading;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void PlayStateChanged()
        {
            

            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            if (!HasInstance) return;
#pragma warning disable
            Instance.FadeOutFor(0);
#pragma warning restore
        }
#endif

        public static async Task In(float duration)
        {
            await Instance.FadeInFor(duration);
        }

        public static async Task Out(float duration)
        {
            await Instance.FadeOutFor(duration);
        }

        private async Task FadeInFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _fading = true;
            await FadeInForTask(duration);
            _fading = false;
        }

        private async Task FadeOutFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _fading = true;
            await FadeOutForTask(duration);
            _fading = false;
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnStateChanged;
        }
#endif
        
#pragma warning disable
        protected async virtual Task FadeInForTask(float duration) { }
        protected async virtual Task FadeOutForTask(float duration) { }
#pragma warning restore
    }
}
