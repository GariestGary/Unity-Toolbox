using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    public class BlackScreenFader: Fader
    {
        [SerializeField] private Image image;

        public override IEnumerator FadeInCoroutine()
        {
            StopCoroutine(nameof(FadeOutCoroutine));

            image.raycastTarget = true;

            Color a = new Color(0, 0, 0, 0);

            float stack = 0;

            while (a.a < 1)
            {
                stack += Time.deltaTime / fadeDuration;
                a.a = Mathf.Lerp(0, 1, stack);
                image.color = a;
                yield return null;
            }
        }

        public override IEnumerator FadeOutCoroutine()
        {
            StopCoroutine(nameof(FadeInCoroutine));

            Color a = new Color(0, 0, 0, 1);

            float stack = 0;

            while (a.a > 0)
            {
                stack += Time.deltaTime / fadeDuration;
                a.a = Mathf.Lerp(1, 0, stack);
                image.color = a;
                yield return null;
            }

            image.raycastTarget = false;
        }
    }
}
