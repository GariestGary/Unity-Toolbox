using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Default implementation of Fader, simply fades in and out black screen
    /// </summary>
    public class BlackFader : Fader
    {
        [SerializeField] private CanvasGroup canvasGroup;

        protected async override UniTask FadeInForAsync(float fadeInDuration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            canvasGroup.SetInteractions(true);

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha < 1)
            {
                if(token.IsCancellationRequested)
                {
                    return;
                }

                stack += Time.deltaTime / fadeInDuration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
        }

        protected async override UniTask FadeOutForAsync(float fadeOutDuration, CancellationToken token)
        {
            if (canvasGroup == null) return;

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha > 0)
            {
                if(token.IsCancellationRequested)
                {
                    return;
                }

                stack += Time.deltaTime / fadeOutDuration;
                alpha = Mathf.Lerp(1, 0, stack);
                canvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
            
            canvasGroup.SetInteractions(false);
        }
    }
}
