using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class PlatformFileHandler
    {
        public abstract bool LoadData();
        public abstract bool SaveData();
    }
}
