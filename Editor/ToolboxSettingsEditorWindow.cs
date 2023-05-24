#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VolumeBox.Toolbox;
using System;

namespace VolumeBox.Toolbox
{
    
    public class ToolboxSettingsEditorWindow: EditorWindow
    {
        private PoolerDataHolder poolerDataHolder;
        private AudioPlayerDataHolder audioPlayerDataHolder;
        private SaverDataHolder saverDataHolder;

        private int selectedTab;

        [MenuItem("Toolbox/Settings")]
        public static void ShowMyEditor()
        {
            EditorWindow wnd = GetWindow<ToolboxSettingsEditorWindow>();
            wnd.titleContent = new GUIContent("Toolbox Settings");
        }

        private void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] {"Main Settings", "Pooler", "Audio Player", "Saver" });

            switch (selectedTab)
            {
                case 0:
                    MainSettingsGUI();
                    break;
                case 1:
                    PoolerGUI();
                    break;
                case 2:
                    AudioPlayerGUI();
                    break;
                case 3:
                    SaverGUI();
                    break;
            }
        }

        private void MainSettingsGUI()
        {
            var settingsEditor = SettingsDataEditor.CreateEditor(StaticData.Settings);
            
            settingsEditor.OnInspectorGUI();
        }

        private void PoolerGUI()
        {
            if (poolerDataHolder == null)
            {
                poolerDataHolder = ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
            }

            var poolerEditor = PoolerEditor.CreateEditor(poolerDataHolder);
            poolerEditor.OnInspectorGUI();
        }

        private void AudioPlayerGUI()
        {
            if(audioPlayerDataHolder == null)
            {
                audioPlayerDataHolder = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            }

            var audioEditor = AudioPlayerEditor.CreateEditor(audioPlayerDataHolder);
            audioEditor.OnInspectorGUI();
        }

        private void SaverGUI()
        {
            if(saverDataHolder == null)
            {
                saverDataHolder = ResourcesUtils.ResolveScriptable<SaverDataHolder>(SettingsData.saverResourcesDataPath);
            }

            var saverEditor = SaverEditor.CreateEditor(saverDataHolder);
            saverEditor.OnInspectorGUI();
        }
    }
}
#endif
