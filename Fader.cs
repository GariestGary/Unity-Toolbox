using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    public class Fader : Singleton<Fader>
    {
        [SerializeField] private Image image;

        public IEnumerator FadeIn(float duration)
        {
            StopCoroutine("FadeOut");
            Color a = new Color(0, 0, 0, 0);

            float stack = 0;

            while(a.a < 1)
            {
                stack += Time.deltaTime / duration;
                a.a = Mathf.Lerp(0, 1, stack);
                image.color = a;
                yield return null;
            }
        }

        public IEnumerator FadeOut(float duration)
        {
            StopCoroutine("FadeIn");
            Color a = new Color(0, 0, 0, 1);

            float stack = 0;

            while(a.a > 0)
            {
                stack += Time.deltaTime / duration;
                a.a = Mathf.Lerp(1, 0, stack);
                image.color = a;
                yield return null;
            }
        }
    }
}
