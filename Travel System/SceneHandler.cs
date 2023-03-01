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
        protected TArgs Args;

        sealed protected override void OnLoadCallback(SceneArgs args)
        {
            Args = args as TArgs;

            if (Args == null)
            {
                Debug.LogWarning("Current scene args is null");
            }

            SetupScene(Args);
        }

        protected abstract void SetupScene(TArgs args);
    }

    public class SceneHandlerBase : MonoCached
    {
        protected virtual void OnLoadCallback(SceneArgs args)
        {

        }

        protected virtual void OnSceneUnload()
        {

        }
    }
}