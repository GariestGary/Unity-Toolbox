#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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

        [MenuItem("Toolbox/Settings", priority = 1)]
        public static void ShowMyEditor()
        {
            if(StaticData.HasSettings)
            {
                EditorWindow wnd = GetWindow<ToolboxSettingsEditorWindow>();
                wnd.titleContent = new GUIContent("Toolbox Settings");
            }
            else
            {
                InitialScreenWindow.OpenWindow();
            }
        }

        [MenuItem("Toolbox/Documentation", priority = 3)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://gariestgary.github.io/toolbox/about/");
        }

        [MenuItem("Toolbox/Open MAIN Scene", priority = 2)]
        public static void OpenMainScene()
        {
            if(EditorLoadUtils.IsMainSceneCorrectInBuild())
            {
                EditorLoadUtils.OpenMainScene();
            }
            else
            {
                Debug.LogError("There's an issue with MAIN scene, please open Toolbox Setup window to fix this");
            }
        }

        public static void CreateAssets()
        {
            ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
            ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            ResourcesUtils.ResolveScriptable<DatabaseDataHolder>(SettingsData.saverResourcesDataPath);
            StaticData.CreateSettings();
        }

        private void OnEnable()
        {
            InitEditors();
        }

        private void InitEditors()
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
            if(StaticData.HasSettings)
            {
                if(poolerDataHolder == null || audioPlayerDataHolder == null || saverDataHolder == null)
                {
                    InitEditors();
                }

                selectedTab = GUILayout.Toolbar(selectedTab, new GUIContent[] 
                {
                    new GUIContent("Main Settings", EditorGUIUtility.IconContent("d__Popup").image), 
                    new GUIContent("Pooler", EditorGUIUtility.IconContent("d_PreMatLight1").image), 
                    new GUIContent("Audio Player", EditorGUIUtility.IconContent("d_Profiler.Audio").image), 
                    new GUIContent("Database", EditorGUIUtility.IconContent("d_SaveAs").image)
                }, GUILayout.Height(35));

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
            else 
            {
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 20;
                EditorGUILayout.HelpBox("Toolbox requires settings data files to be created in Resources folder. Click button below to open setup window", MessageType.Info);
                if(GUILayout.Button("Open Setup Window", buttonStyle, GUILayout.Height(45)))
                {
                    InitialScreenWindow.OpenWindow();
                }
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

        private void OnLostFocus()
        {
            AudioUtils.StopAllPreviewClips();
        }
    }
}
#endif
