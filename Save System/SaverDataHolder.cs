using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class SaverDataHolder: ScriptableObject, IRunner, IClear
    {
        [SerializeField] private PropertiesDatabase database;

        private List<SceneData> sceneDatas = new List<SceneData>();

        public PropertiesDatabase Database => database;

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
