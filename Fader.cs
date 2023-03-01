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

        protected bool _fading;

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
