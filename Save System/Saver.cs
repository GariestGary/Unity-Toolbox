using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Saver: Singleton<Saver>, IRunner
    {
        public bool useSaves;

        [Required]
        [EnableIf("useSaves")]
        [SerializeField] private StateProvider stateProvider;

        [Required]
        [EnableIf("useSaves")]
        [SerializeField] private PlatformFileHandler fileHandler;

        [EnableIf("useSaves")]
        [MinValue(1)][MaxValue(10)][SerializeField] private int saveSlotsCount = 1;

        public StateProvider StateProvider => stateProvider;
        public PlatformFileHandler FileHandler => fileHandler;

        public int SaveSlotsCount
        {
            get
            {
                return saveSlotsCount;
            }
            set
            {
                if (value < 1)
                {
                    saveSlotsCount = 1;
                }
                else if (value > 10)
                {
                    saveSlotsCount = 10;
                }
                else
                {
                    saveSlotsCount = value;
                }
            }
        }

        [ReadOnly][SerializeField] private List<SaveSlot> saveSlots;

        private SaveSlot currentSlot;

        public SaveSlot CurrentSlot => currentSlot;

        public void Run()
        {
            if (!useSaves) return;

            fileHandler.Run();
            ResolveSlots();
            Messager.Instance.Subscribe<SaveGameMessage>(_ => Save());
        }

        public void SetFileHandler(PlatformFileHandler handler)
        {
            fileHandler = handler;
        }

        public void SetStateProvider(StateProvider provider)
        {
            stateProvider = provider;
        }

        private void ResolveSlots()
        {
            if (!useSaves) return;

            saveSlots = new List<SaveSlot>();

            for (var i = 0; i < saveSlotsCount; i++)
            {
                SaveSlot slot = fileHandler.LoadGameSlot(i);

                if (slot == null)
                {
                    slot = new SaveSlot(null, i);
                    fileHandler.SaveGameSlot(slot);
                }

                saveSlots.Add(slot);
            }
        }

        public void CaptureCurrentState()
        {
            if (!useSaves) return;

            if (currentSlot == null) return;

            currentSlot.state = stateProvider.CaptureCurrentState();
        }

        public void Save()
        {
            if (!useSaves) return;

            if (currentSlot == null) return;

            CaptureCurrentState();
            fileHandler.SaveGameSlot(currentSlot);
        }

        public void LoadCurrentSlot()
        {
            if (!useSaves) return;

            if (currentSlot == null) return;

            stateProvider.RestoreCurrentState(currentSlot.state);
        }

        public SaveSlot GetSlot(int id)
        {
            if (id >= saveSlotsCount || id < 0) return null;

            SaveSlot slot = saveSlots.FirstOrDefault(x => x.id == id);
            return slot;
        }

        public SaveSlot SelectSlot(int id)
        {
            SaveSlot s = GetSlot(id);

            if (s != null)
            {
                currentSlot = s;
            }

            return s;
        }
    }

    public class SaveGameMessage: Message { }
}