using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class SceneHandler<TArgs>: SceneHandlerBase where TArgs : SceneArgs
    {
        [SerializeField] private bool _skipSetup;
        [SerializeField] private bool _isGameplayLevel;

        protected TArgs args;

        public bool IsGameplayScene => _isGameplayLevel;

        public override sealed void OnLoadCallback()
        {
            args = Traveler.Instance.GetCurrentSceneArgs<TArgs>();

            if (args == null)
            {
                Debug.LogWarning("Current scene args is null");
            }

            SetupScene(args);
        }

        public abstract void SetupScene(TArgs args);
    }

    public class SceneHandlerBase: MonoCached
    {
        public virtual void OnLoadCallback()
        {
        }
    }
}