using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class SceneHandler<TArgs> : SceneHandlerBase where TArgs : SceneArgs
    {
        protected TArgs Args;

        protected sealed override void OnLoadCallback(SceneArgs args)
        {
            Args = args as TArgs;

            if (Args != null)
            {
                if (args is not TArgs)
                {
                    Debug.Log($"Current loaded {gameObject.scene.name} scene expected {typeof(TArgs)} args, but provided with {args.GetType()}");
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