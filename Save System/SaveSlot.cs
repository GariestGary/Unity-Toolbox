using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [System.Serializable]
    public class SaveSlot
    {
        public object state;
        public int id;

        public SaveSlot(object state, int id)
        {
            this.state = state;
            this.id = id;
        }
    }
}
