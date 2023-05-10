using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CreateAssetMenu(menuName = "VolumeBox/Toolbox/Settings")]
    public class ToolboxSettings : ScriptableObject
    {
        public bool autoResolveScenesAtPlay = true;
    }
}
