using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    public class Fader : Singleton<Fader>
    {
        [SerializeField] private bool useFade;
        [SerializeField] private float fadeDuration;

        public static event Action FadedInEvent;
        public static event Action FadedOutEvent;

        [SerializeField] private Image image;

        private Color _transparent => new Color(0, 0, 0, 0);

        public void FadeInInstantly()
        {
            image.color = Color.black;
        }

        public void FadeOutInstantly()
        {
            image.color = _transparent;
        }

        public void FadeIn()
        {
            StartCoroutine(nameof(FadeInCoroutine));
        }

        public void FadeOut()
        {
            StartCoroutine(nameof(FadeOutCoroutine));
        }

        public IEnumerator FadeInCoroutine()
        {
            if (!useFade)
            {
                FadedInEvent?.Invoke();
                yield break;
            }

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

            FadedInEvent?.Invoke();
        }

        public IEnumerator FadeOutCoroutine()
        {
            if (!useFade)
            {
                FadedOutEvent?.Invoke();
                yield break;
            }

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

            FadedOutEvent?.Invoke();
        }
    }
}
