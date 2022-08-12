using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public interface IPooled
    {
        void OnSpawn(object data);
    }
}
