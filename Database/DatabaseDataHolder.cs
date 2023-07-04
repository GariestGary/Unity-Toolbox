using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class DatabaseDataHolder: ScriptableObject, IRunner, IClear
    {
        [SerializeField] private PropertiesDatabase properties;

        private List<SceneData> sceneDatas = new List<SceneData>();

        public PropertiesDatabase Properties => properties;

        public void Clear()
        {
            
        }

        public void Run()
        {
            
        }
    }

    [Serializable]
    public class SceneData
    {
        public string sceneName;
        public Dictionary<string, object> objectsData = new Dictionary<string, object>();
    }

    [Serializable]
    public class SaveData
    {

    }
}
