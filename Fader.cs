using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    public abstract class Fader : Singleton<Fader>
    {
        [SerializeField] protected bool useFade;

        public virtual void FadeInInstantly()
        {
            if (!useFade)
            {
                return;
            }

            StopCoroutine(nameof(FadeOutForCoroutine));
            StopCoroutine(nameof(FadeInForCoroutine));
            //image.color = Color.black;
        }

        public virtual void FadeOutInstantly()
        {
            if (!useFade)
            {
                return;
            }

            StopCoroutine(nameof(FadeOutForCoroutine));
            StopCoroutine(nameof(FadeInForCoroutine));
        }

        public void FadeInFor(float duration)
        {
            if (!useFade)
            {
                return;
            }

            StartCoroutine(nameof(FadeInForCoroutine));
        }

        public void FadeOutFor(float duration)
        {
            if (!useFade)
            {
                return;
            }

            StartCoroutine(nameof(FadeOutForCoroutine));
        }

        public abstract Task FadeInForCoroutine(float duration);
        public abstract Task FadeOutForCoroutine(float duration);
    }
}
