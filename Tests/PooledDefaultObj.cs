using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Tests
{
    internal class PooledDefaultObj: MonoCached, IPooled
    {
        public string compare;
        
        public void OnSpawn()
        {
            compare = "data";
        }
    }
}