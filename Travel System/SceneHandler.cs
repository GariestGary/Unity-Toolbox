using UnityEngine;

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
                Debug.Log("Current loaded scene args is null");
            }
            else
            {
                if (args is not TArgs)
                {
                    Debug.Log($"Current loaded scene expected {typeof(TArgs)} args, but provided with {args.GetType()}");
                }
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