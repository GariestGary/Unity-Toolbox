using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class SettingsData : ScriptableObject
    {
        [SerializeField]
        public float TimeScale = 1;

        [SerializeField]
        public int TargetFrameRate = 120;

        [SerializeField]
        public string InitialSceneName;

        [SerializeField]
        public SceneArgs InitialSceneArgs;

        [SerializeField]
        public bool UseMessageCaching = true;

        [HideInInspector] public bool AutoResolveScenesAtPlay = true;
        public const string poolerResourcesDataPath = "Toolbox/PoolerData.asset";
        public const string audioPlayerResourcesDataPath = "Toolbox/AudioPlayerData.asset";

        private void OnValidate()
        {
            if (Toolbox.HasInstance)
            {
                Toolbox.Updater.TimeScale = TimeScale;
            }
        }
    }
}
