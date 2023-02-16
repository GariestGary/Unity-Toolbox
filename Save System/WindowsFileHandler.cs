using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CreateAssetMenu(fileName = "Windows File Handler", menuName = "VolumeBox/Toolbox/File Handlers/Windows")]
    public class WindowsFileHandler : PlatformFileHandler
    {   
        public override object LoadBinary(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("File in " + path + " doesn't exist");
                return null;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None
                );
                object data = bf.Deserialize(fs);
                fs.Dispose();
                return data;
            }
            catch(Exception e)
            {
                Debug.LogError("File at " + path + " deserialize failed, Message: " + e.Message);
                return null;
            }
        }

        public override bool SaveBinary(object data, string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                FileStream fs = new FileStream(
                    path, 
                    FileMode.OpenOrCreate, 
                    FileAccess.ReadWrite, 
                    FileShare.None
                );
                bf.Serialize(fs, data);
                fs.Dispose();

                return true;
            }
            catch(Exception e)
            {
                Debug.LogError("Failed to save file at " + path + " Message: " + e.Message);
                return false;
            }
        }

        public override SaveSlot LoadGameSlot(int id)
        {
            return (SaveSlot)LoadBinary(GetSlotPath(id));
        }

        public override bool SaveGameSlot(SaveSlot slot)
        {
            return SaveBinary(slot, GetSlotPath(slot.id));
        }
    }
}
