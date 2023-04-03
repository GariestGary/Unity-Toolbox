using System.Collections;
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

        protected async override Task FadeInForTask(float fadeInDuration)
        {
            if (canvasGroup == null) return;

            StopCoroutine(nameof(FadeOutForTask));
            canvasGroup.SetInteractions(true);

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha < 1)
            {
                stack += Time.deltaTime / fadeInDuration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                await Task.Yield();
            }
        }

        protected async override Task FadeOutForTask(float fadeOutDuration)
        {
            if (canvasGroup == null) return;

            StopCoroutine(nameof(FadeInForTask));

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha > 0)
            {
                stack += Time.deltaTime / fadeOutDuration;
                alpha = Mathf.Lerp(1, 0, stack);
                canvasGroup.alpha = alpha;
                await Task.Yield();
            }
            
            canvasGroup.SetInteractions(false);
        }
    }
}
