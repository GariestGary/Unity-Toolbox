using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class ScenePool : MonoCached
    {
        [SerializeField] private List<PoolData> pools;

        private bool initialized = false;

        protected override void Rise()
        {
            if (initialized) return;

            pools.ForEach(Pooler.TryAddPool);

            initialized = true;
        }

        protected override void Destroyed()
        {
            pools.ForEach(p => Pooler.TryRemovePool(p.tag));
        }
    }
}
