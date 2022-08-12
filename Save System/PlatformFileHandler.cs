using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class PlatformFileHandler : ScriptableObject, IRunner
    {
        [SerializeField] protected string saveFileExtension = "snt";
        [SerializeField] protected string saveFilePrefix = "save";

        protected string savePath;

        public string SavePath => savePath;

        public abstract SaveSlot LoadGameSlot(int id);
        public abstract bool SaveGameSlot(SaveSlot slot);
        public abstract bool SaveBinary(object data, string path);
        public abstract object LoadBinary(string path);

        protected string GetSlotPath(int id)
        {
            return savePath + "\\" + saveFilePrefix + " - " + id + "." + saveFileExtension;
        }

        public virtual void Run()
        {
            SetSavePath();
        }

        protected virtual void SetSavePath()
        {
            savePath = Application.persistentDataPath;
        }
    }
}
