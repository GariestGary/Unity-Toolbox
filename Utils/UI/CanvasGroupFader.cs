using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Threading;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Utils.UI
{
    public class CanvasGroupFader: MonoCached
    {
        [SerializeField] private bool customCanvas;
        [SerializeField, ShowIf(nameof(customCanvas))] private CanvasGroup canvasGroup;
        [SerializeField, MinValue(0)] private float fadeInDuration = 0.2f;
        [SerializeField, MinValue(0)] private float fadeOutDuration = 0.2f;
        [SerializeField] private bool controlInteractions = true;

        private CancellationTokenSource _tokenSource;

        public float FadeInDuration => fadeInDuration;
        public float FadeOutDuration => fadeOutDuration;

        protected override void Rise()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        [Button("Fade In")]
        public void FadeIn()
        {
            FadeIn(fadeInDuration);
        }

        [Button("Fade Out")]
        public void FadeOut()
        {
            FadeOut(FadeOutDuration);
        }

        public void FadeOut(float duration)
        {
            _tokenSource ??= new CancellationTokenSource();

#pragma warning disable
            FadeOutForAsync(duration, _tokenSource.Token);
#pragma warning enable
        }

        public void FadeIn(float duration)
        {
            _tokenSource ??= new CancellationTokenSource();

#pragma warning disable
            FadeInForAsync(duration, _tokenSource.Token);
#pragma warning enable
        }

        public async UniTask FadeOutAsync()
        {
            _tokenSource ??= new CancellationTokenSource();
            
            await FadeOutForAsync(fadeOutDuration, _tokenSource.Token);
        }

        public async UniTask FadeInAsync()
        {
            _tokenSource ??= new CancellationTokenSource();
            
            await FadeInForAsync(fadeInDuration, _tokenSource.Token);
        }

        public async UniTask FadeOutAsync(float duration)
        {
            _tokenSource ??= new CancellationTokenSource();
            
            await FadeOutForAsync(duration, _tokenSource.Token);
        }

        public async UniTask FadeInAsync(float duration)
        {
            _tokenSource ??= new CancellationTokenSource();
            
            await FadeInForAsync(duration, _tokenSource.Token);
        }

        protected async UniTask FadeOutForAsync(float fadeOutDuration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new();

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

        protected async UniTask FadeInForAsync(float fadeInDuration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new();

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

                stack += Time.deltaTime / fadeInDuration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
        }

        protected override void Destroyed()
        {
            _tokenSource?.Cancel();
        }
    }
}