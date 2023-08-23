#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Editor
{
    public class ToolboxSettingsEditorWindow: EditorWindow
    {
        private PoolerDataHolder poolerDataHolder;
        private AudioPlayerDataHolder audioPlayerDataHolder;
        private DatabaseDataHolder saverDataHolder;

        private SettingsDataEditor settingsEditor;
        private PoolerEditor poolerEditor;
        private DatabaseEditor saverEditor;
        private AudioPlayerEditor audioEditor;

        private int selectedTab;

        [MenuItem("Toolbox/Settings")]
        public static void ShowMyEditor()
        {
            EditorWindow wnd = GetWindow<ToolboxSettingsEditorWindow>();
            wnd.titleContent = new GUIContent("Toolbox Settings");
        }

        private void OnEnable()
        {
            poolerDataHolder = ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
            audioPlayerDataHolder = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            saverDataHolder = ResourcesUtils.ResolveScriptable<DatabaseDataHolder>(SettingsData.saverResourcesDataPath);

            settingsEditor = (SettingsDataEditor)UnityEditor.Editor.CreateEditor(StaticData.Settings);
            poolerEditor = (PoolerEditor)UnityEditor.Editor.CreateEditor(poolerDataHolder);
            audioEditor = (AudioPlayerEditor)UnityEditor.Editor.CreateEditor(audioPlayerDataHolder);
            saverEditor = (DatabaseEditor)UnityEditor.Editor.CreateEditor(saverDataHolder);
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
            settingsEditor?.OnInspectorGUI();
        }

        private void PoolerGUI()
        {
            
            poolerEditor?.OnInspectorGUI();
        }

        private void AudioPlayerGUI()
        {
            
            audioEditor?.OnInspectorGUI();
        }

        private void SaverGUI()
        {
            
            saverEditor?.OnInspectorGUI();
        }
    }
}
#endif
