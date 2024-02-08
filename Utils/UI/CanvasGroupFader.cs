using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace VolumeBox.Toolbox.Utils.UI
{
    public class CanvasGroupFader: MonoCached
    {
        [SerializeField] private bool customCanvas;
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(customCanvas))]
#else
        [NaughtyAttributes.ShowIf(nameof(customCanvas))]
#endif
        private CanvasGroup canvasGroup;
        [SerializeField, NaughtyAttributes.MinValue(0)] private float fadeInDuration = 0.2f;
        [SerializeField, NaughtyAttributes.MinValue(0)] private float fadeOutDuration = 0.2f;
        [SerializeField] private bool controlInteractions = true;

        private CancellationTokenSource _fadeInTokenSource;
        private CancellationTokenSource _fadeOutTokenSource;

        public float FadeInDuration => fadeInDuration;
        public float FadeOutDuration => fadeOutDuration;

        protected override void Rise()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        [NaughtyAttributes.Button("Fade In")]
        public void FadeIn()
        {
            FadeIn(fadeInDuration);
        }

        [NaughtyAttributes.Button("Fade Out")]
        public void FadeOut()
        {
            FadeOut(FadeOutDuration);
        }

        public void FadeOut(float duration)
        {
            _fadeOutTokenSource ??= new CancellationTokenSource();

#pragma warning disable
            FadeOutForAsync(duration, _fadeOutTokenSource.Token);
#pragma warning enable
        }

        public void FadeIn(float duration)
        {
            _fadeInTokenSource ??= new CancellationTokenSource();

#pragma warning disable
            FadeInForAsync(duration, _fadeInTokenSource.Token);
#pragma warning enable
        }

        public async UniTask FadeOutAsync()
        {
            _fadeOutTokenSource ??= new CancellationTokenSource();
            
            await FadeOutForAsync(fadeOutDuration, _fadeOutTokenSource.Token);
        }

        public async UniTask FadeInAsync()
        {
            _fadeInTokenSource ??= new CancellationTokenSource();
            
            await FadeInForAsync(fadeInDuration, _fadeInTokenSource.Token);
        }

        public async UniTask FadeOutAsync(float duration)
        {
            _fadeOutTokenSource ??= new CancellationTokenSource();
            
            await FadeOutForAsync(duration, _fadeOutTokenSource.Token);
        }

        public async UniTask FadeInAsync(float duration)
        {
            _fadeInTokenSource ??= new CancellationTokenSource();
            
            await FadeInForAsync(duration, _fadeInTokenSource.Token);
        }

        protected async UniTask FadeOutForAsync(float fadeOutDuration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            _fadeInTokenSource?.Cancel();
            _fadeInTokenSource?.Dispose();
            _fadeInTokenSource = new();

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha > 0)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                stack += Time.deltaTime / fadeOutDuration;
                alpha = Mathf.Lerp(1, 0, stack);
                canvasGroup.alpha = alpha;
                await UniTask.Yield();
            }

            if (controlInteractions)
            {
                canvasGroup.SetInteractions(false);
            }
        }

        protected async UniTask FadeInForAsync(float duration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            _fadeOutTokenSource?.Cancel();
            _fadeOutTokenSource?.Dispose();
            _fadeOutTokenSource = new();

            if (controlInteractions)
            {
                canvasGroup.SetInteractions(true);
            }

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha < 1)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                stack += Time.deltaTime / duration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
        }

        protected override void Destroyed()
        {
            _fadeInTokenSource?.Cancel();
            _fadeOutTokenSource?.Cancel();
        }
    }
}