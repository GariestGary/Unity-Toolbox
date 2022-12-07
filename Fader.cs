using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    public abstract class Fader: Singleton<Fader>
    {
        [SerializeField] protected float fadeDuration;

        protected Color transparent => new Color(0, 0, 0, 0);

        public void FadeIn()
        {
            StartCoroutine(nameof(FadeInCoroutine));
        }

        public void FadeOut()
        {
            StartCoroutine(nameof(FadeOutCoroutine));
        }

        public abstract IEnumerator FadeInCoroutine();
        

        public abstract IEnumerator FadeOutCoroutine();
    }
}