using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class UpdateModifier: ScriptableObject
    {
        public virtual void Initialize()
        {

        }

        public abstract float Modify(float delta, MonoCached mono);
    }
}
