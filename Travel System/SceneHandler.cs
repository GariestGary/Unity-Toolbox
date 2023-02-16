using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;

namespace VolumeBox.Toolbox
{
    public abstract class SceneHandler<TArgs> : SceneHandlerBase where TArgs : SceneArgs
    {
        [SerializeField] private bool _skipSetup;
        [SerializeField] private bool _isGameplayLevel;

        public bool IsGameplayScene => _isGameplayLevel;

        protected TArgs Args;

        sealed public override void OnLoadCallback()
        {
            Args = Traveler.Instance.GetCurrentSceneArgs<TArgs>();

            if (Args == null)
            {
                Debug.LogWarning("Current scene args is null");
            }

            SetupScene(Args);
        }

        public abstract void SetupScene(TArgs args);
    }

    public class SceneHandlerBase : MonoCached
    {
        public virtual void OnLoadCallback()
        {

        }

        public virtual void OnSceneUnload()
        {

        }
    }
}