using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VolumeBox.Toolbox
{
    public abstract class Fader : Singleton<Fader>
    {
        [SerializeField] protected bool useFade;

        protected bool _fading;
        protected CancellationTokenSource _cancellationTokenSource;

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

        public static async UniTask In(float duration)
        {
            await Instance.FadeInFor(duration);
        }

        public static async UniTask Out(float duration)
        {
            await Instance.FadeOutFor(duration);
        }

        private async UniTask FadeInFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new();

            _fading = true;
            await FadeInForAsync(duration, _cancellationTokenSource.Token);
            _fading = false;
        }

        private async UniTask FadeOutFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new();

            _fading = true;
            await FadeOutForAsync(duration, _cancellationTokenSource.Token);
            _fading = false;
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnStateChanged;
        }
#endif
        
#pragma warning disable
        protected async virtual UniTask FadeInForAsync(float duration, CancellationToken token) { }
        protected async virtual UniTask FadeOutForAsync(float duration, CancellationToken token) { }
#pragma warning restore
    }
}
