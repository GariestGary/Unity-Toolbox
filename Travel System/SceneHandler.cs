using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class SceneHandler<TArgs> : SceneHandlerBase where TArgs : SceneArgs
    {
        protected TArgs Args;
        private bool _SceneSettedUp = false;

        public sealed override void OnLoadCallback(SceneArgs args)
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

        public sealed override async UniTask OnLoadCallbackAsync()
        {
            await SetupSceneAsync(Args);
        }

        protected abstract UniTask SetupSceneAsync(TArgs args);

        protected abstract void SetupScene(TArgs args);
    }

    public class SceneHandlerBase : MonoCached
    {
        public virtual void OnLoadCallback(SceneArgs args)
        {

        }

        public virtual void OnUnloadCallback()
        {

        }

        public virtual async UniTask OnLoadCallbackAsync()
        {

        }
    }
}