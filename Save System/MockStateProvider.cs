using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CreateAssetMenu(fileName = "Mock State Provider", menuName = "VolumeBox/Toolbox/State Providers/Mock")]
    public class MockStateProvider : StateProvider
    {
        public string data = "Mock";

        public override object CaptureCurrentState()
        {
            return data;
        }

        public override void RestoreCurrentState(object data)
        {
            
        }
    }
}
