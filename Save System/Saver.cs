using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Saver: ResourcesToolWrapper<Saver, SaverDataHolder>
    {
        public PlatformFileHandler FileHandler => Instance.Data.FileHandler;
        public StateProvider StateProvider => Instance.Data.StateProvider;
        public int SaveSlotsCount => Instance.Data.SaveSlotsCount;
        public bool UseSaves => Instance.Data.UseSaves;
        public SaveSlot CurrentSlot => Instance.Data.CurrentSlot;

        protected override void Clear()
        {
            Instance.Data.Clear();
        }

        public override string GetDataPath()
        {
            return SettingsData.saverResourcesDataPath;
        }

        protected override void PostLoadRun()
        {
            Instance.Data.Run();
        }

        public static void SetFileHandler(PlatformFileHandler handler)
        {
            Instance.Data.SetFileHandler(handler);
        }

        public static void SetStateProvider(StateProvider provider)
        {
            Instance.Data.SetStateProvider(provider);
        }

        public static void CaptureCurrentState()
        {
            Instance.Data.CaptureCurrentState();
        }

        public static void Save()
        {
            Instance.Data.Save();
        }

        public static void LoadCurrentSlot()
        {
            Instance.Data.LoadCurrentSlot();
        }

        public static SaveSlot GetSlot(int id)
        {
            return Instance.Data.GetSlot(id);
        }

        public static SaveSlot SelectSlot(int id)
        {
            return Instance.Data.SelectSlot(id);
        }
    }
}
