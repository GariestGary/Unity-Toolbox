using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class ScenePool : MonoCached
    {
        [SerializeField] private List<PoolData> pools;

        private bool _initialized = false;

        protected override void OnActivate()
        {
            if (_initialized) return;

            pools.ForEach(p => Pooler.Instance.TryAddPool(p));

            _initialized = true;
        }

        protected override void Destroyed()
        {
            pools.ForEach(p => Pooler.Instance.TryRemovePool(p.tag));
        }
    }
}
