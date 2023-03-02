using System;
using System.Collections;
using System.Collections.Generic;
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

        public async Task FadeInFor(float duration)
        {
            if (!useFade || duration <= 0 || _fading)
            {
                return;
            }

            _fading = true;
            await FadeInForTask(duration);
            _fading = false;
        }

        public async Task FadeOutFor(float duration)
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
