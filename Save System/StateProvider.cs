using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class StateProvider : ScriptableObject
    {
        public abstract object CaptureCurrentState();
        public abstract void RestoreCurrentState(object data);
    }
}
