#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public class ToolboxSettingsEditorWindow: EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_Document;
        [SerializeField] private Texture2D m_MainSettingsIcon;
        [SerializeField] private Texture2D m_PoolerIcon;
        [SerializeField] private Texture2D m_AudioPlayerIcon;
        [SerializeField] private Texture2D m_DatabaseIcon;
        [SerializeField] private GUISkin m_TabSkin;

        private PoolerDataHolder poolerDataHolder;
        private AudioPlayerDataHolder audioPlayerDataHolder;
        private DatabaseDataHolder saverDataHolder;

        private SettingsDataEditor settingsEditor;
        private PoolerEditor poolerEditor;
        private DatabaseEditor saverEditor;
        private AudioPlayerEditor audioEditor;

        private int selectedTab;
        private int prevSelectedTab;
        private VisualElement currentContent;

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

        private void CreateGUI()
        {
            rootVisualElement.Add(m_Document.Instantiate());
            var toolbar = new IMGUIContainer(() =>
            {
                selectedTab = DrawToolbar();
            });
            rootVisualElement.Add(toolbar);
        }

        private void OnGUI()
        {
            if (selectedTab == prevSelectedTab && currentContent != null)
            {
                return;
            }

            VisualElement content = new VisualElement();
            
            switch (selectedTab)
            {
                case 0:
                    content = MainSettingsGUI();
                    break;
                case 1:
                    content = PoolerGUI();
                    break;
                case 2:
                    content = AudioPlayerGUI();
                    break;
                case 3:
                    content = SaverGUI();
                    break;
                default:
                    break;
            }

            if(currentContent != null)
            {
                rootVisualElement.Remove(currentContent);
            }

            rootVisualElement.Add(content);

            currentContent = content;
            prevSelectedTab = selectedTab;
        }

        private int DrawToolbar()
        {
            if(StaticData.HasSettings)
            {
                if(poolerDataHolder == null || audioPlayerDataHolder == null || saverDataHolder == null)
                {
                    InitEditors();
                }

                GUI.skin = m_TabSkin;
                var selectedIndex = GUILayout.Toolbar(selectedTab, new GUIContent[] 
                {
                    new GUIContent("Main Settings", m_MainSettingsIcon), 
                    new GUIContent("Pooler", m_PoolerIcon), 
                    new GUIContent("Audio Player", m_AudioPlayerIcon), 
                    new GUIContent("Database", m_DatabaseIcon)
                }, m_TabSkin.GetStyle("Tab"), GUILayout.Height(40));
//
                return selectedIndex;
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

        private VisualElement MainSettingsGUI()
        {
            return settingsEditor?.CreateInspectorGUI();
        }

        private VisualElement PoolerGUI()
        {
            return poolerEditor?.CreateInspectorGUI();
        }

        private VisualElement AudioPlayerGUI()
        {
            
            return audioEditor?.CreateInspectorGUI();
        }

        private VisualElement SaverGUI()
        {
            
            return saverEditor?.CreateInspectorGUI();
        }

        private void OnLostFocus()
        {
            AudioUtils.StopAllPreviewClips();
        }
    }
}
#endif
