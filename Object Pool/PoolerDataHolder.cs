using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [Serializable]
    public class PoolerDataHolder : ScriptableObject
    {
        [SerializeField] private float m_GarbageCollectorWorkInterval = 10;
        [SerializeField] private List<PoolData> poolsList;

        public float GarbageCollectorWorkInterval => m_GarbageCollectorWorkInterval;
        public List<PoolData> PoolsList => poolsList;
    }

    [System.Serializable]
    public class PoolData
    {
        public string tag;
        public GameObject pooledObject;
        public int size;
    }
}



