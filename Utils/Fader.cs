using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

namespace VolumeBox.Toolbox
{
    public abstract class Fader : Singleton<Fader>
    {
        [SerializeField] protected bool useFade;

        protected bool _fading;

        [RuntimeInitializeOnLoadMethod]
        private void PlayStateChanged()
        {
            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private async void OnStateChanged(PlayModeStateChange state)
        {
            FadeOutFor(0);
        }

        public static async Task In(float duration)
        {
            await Instance.FadeInFor(duration);
        }

        public static async Task Out(float duration)
        {
            await Instance.FadeOutFor(duration);
        }

        private async Task FadeInFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _fading = true;
            await FadeInForTask(duration);
            _fading = false;
        }

        private async Task FadeOutFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _fading = true;
            await FadeOutForTask(duration);
            _fading = false;
        }

        protected async virtual Task FadeInForTask(float duration) { }
        protected async virtual Task FadeOutForTask(float duration) { }
    }
}
