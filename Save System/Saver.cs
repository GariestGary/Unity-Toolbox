using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Saver: ResourcesToolWrapper<Saver, SaverDataHolder>
    {
        public PropertiesDatabase Database => Data.Database;

        public override string GetDataPath()
        {
            return SettingsData.saverResourcesDataPath;
        }

        protected override void Clear()
        {
            Data.Clear();
        }

        protected override void PostLoadRun()
        {
            Data.Run();
        }
    }
}
