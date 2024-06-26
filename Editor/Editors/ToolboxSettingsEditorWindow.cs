#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public class ToolboxSettingsEditorWindow: EditorWindow
    {
        [SerializeField] private Texture2D m_MainSettingsIcon;
        [SerializeField] private Texture2D m_PoolerIcon;
        [SerializeField] private Texture2D m_AudioPlayerIcon;
        [SerializeField] private Texture2D m_DatabaseIcon;
        [SerializeField] private GUISkin m_TabSkin;

        private PoolerDataHolder poolerDataHolder;
        private AudioPlayerDataHolder audioPlayerDataHolder;

        private SettingsDataEditor settingsEditor;
        private PoolerEditor poolerEditor;
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

            settingsEditor = (SettingsDataEditor)UnityEditor.Editor.CreateEditor(StaticData.Settings);
            poolerEditor = (PoolerEditor)UnityEditor.Editor.CreateEditor(poolerDataHolder);
            audioEditor = (AudioPlayerEditor)UnityEditor.Editor.CreateEditor(audioPlayerDataHolder);
        }

        private void OnGUI()
        {
            switch (DrawToolbar())
            {
                case 0:
                    settingsEditor?.DrawIMGUI();
                    break;
                case 1:
                    poolerEditor?.DrawIMGUI();
                    break;
                case 2:
                    audioEditor?.DrawIMGUI();
                    break;
                default:
                    break;
            }
        }

        private int DrawToolbar()
        {
            if(StaticData.HasSettings)
            {
                if(poolerDataHolder == null || audioPlayerDataHolder == null)
                {
                    InitEditors();
                }

                var oldSkin = GUI.skin;
                GUI.skin = m_TabSkin;
                selectedTab = GUILayout.Toolbar(selectedTab, new GUIContent[] 
                {
                    new GUIContent("Main Settings", m_MainSettingsIcon), 
                    new GUIContent("Pooler", m_PoolerIcon), 
                    new GUIContent("Audio Player", m_AudioPlayerIcon), 
                }, m_TabSkin.GetStyle("Tab"), GUILayout.Height(40));
                GUI.skin = oldSkin;
//
                return selectedTab;
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

                return -1;
            }
        }

        private void OnLostFocus()
        {
            AudioUtils.StopAllPreviewClips();
        }
    }
}
#endif
