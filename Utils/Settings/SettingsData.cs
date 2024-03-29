using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class SettingsData : ScriptableObject
    {
        [SerializeField]
        public float TimeScale = 1;

        [SerializeField]
        public int TargetFrameRate = 120;

        [SerializeField, Scene]
        public string InitialSceneName;

        [SerializeField]
        public SceneArgs InitialSceneArgs;

        [SerializeField]
        public bool ManualFadeOut;

        [SerializeField]
        public float FadeOutDuration = 0.5f;

        [HideInInspector] public bool AutoResolveScenesAtPlay = true;
        public const string poolerResourcesDataPath = "Toolbox/PoolerData.asset";
        public const string audioPlayerResourcesDataPath = "Toolbox/AudioPlayerData.asset";
        public const string saverResourcesDataPath = "Toolbox/SaverData.asset";

        private void OnValidate()
        {
            if (Updater.HasInstance)
            {
                Updater.TimeScale = TimeScale;
            }
        }
    }
}
