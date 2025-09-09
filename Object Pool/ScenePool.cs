using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class ScenePool : MonoCached
    {
        [SerializeField] private List<PoolData> m_Pools;

        private bool m_Initialized = false;

        private List<Pool> m_CurrentPools = new List<Pool>();

        public List<PoolData> Pools => m_Pools;

        protected override void Rise()
        {
            InitializePools();
        }

        public void InitializePools()
        {
            if (m_Initialized) return;

            m_CurrentPools = new List<Pool>();

            foreach (var pool in m_Pools) 
            {
                m_CurrentPools.Add(Toolbox.Pooler.TryAddPool(pool));
            }

            m_Initialized = true;
        }

        protected override void Destroyed()
        {
            if(Toolbox.HasInstance)
            {
                m_CurrentPools.ForEach(p => Toolbox.Pooler.TryRemovePool(p));
            }
        }
    }
}
