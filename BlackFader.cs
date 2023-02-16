using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class BlackFader : Fader
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public override void FadeInInstantly()
        {
            if (!useFade) return;

            base.FadeInInstantly();
        }

        public override void FadeOutInstantly()
        {
            if (!useFade) return;

            base.FadeOutInstantly();
            canvasGroup.alpha = 0;
            canvasGroup.SetInteractions(false);
        }

        public async override UniTask FadeInForCoroutine(float fadeInDuration)
        {
            if (!useFade)
            {
                return;
            }
            
            StopCoroutine(nameof(FadeOutForCoroutine));
            canvasGroup.SetInteractions(true);

            float alpha = 0;
            float stack = 0;

            while (alpha < 1)
            {
                stack += Time.deltaTime / fadeInDuration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                await Task.Yield();
            }
        }

        public async override UniTask FadeOutForCoroutine(float fadeOutDuration)
        {
            if (!useFade)
            {
                return;
            }

            StopCoroutine(nameof(FadeInForCoroutine));

            float alpha = 1;
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
