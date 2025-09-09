using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class SceneHandler<TArgs> : SceneHandlerBase where TArgs : SceneArgs
    {
        protected TArgs Args;

        public sealed override async UniTask OnLoadCallbackAsync(SceneArgs args)
        {
            Args = args as TArgs;

            if (Args != null)
            {
                if (args is not TArgs)
                {
                    Debug.Log($"Current loaded {gameObject.scene.name} scene expected {typeof(TArgs)} args, but provided with {args.GetType()}");
                }
            }
            
            await SetupSceneAsync(Args);
        }

        public override async UniTask OnUnloadCallbackAsync()
        {
            await UnloadSceneAsync();
        }

        protected virtual UniTask UnloadSceneAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask SetupSceneAsync(TArgs args)
        {
            return UniTask.CompletedTask;
        }
    }

    public class SceneHandlerBase : MonoCached
    {
        public virtual async UniTask OnLoadCallbackAsync(SceneArgs args)
        {

        }

        public virtual async UniTask OnUnloadCallbackAsync()
        {
            
        }
    }
}