#if UNITY_EDITOR
using Alchemy.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(SettingsData))]
    public class SettingsDataEditor: AlchemyEditor
    {
        [SerializeField] private VisualTreeAsset m_Document;

        public const float LABEL_WIDTH = 150;

        private SerializedProperty m_resolveAtPlay;
        private SerializedProperty m_timeScale;
        private SerializedProperty m_targetFrameRate;
        private SerializedProperty m_initialSceneName;
        private SerializedProperty m_initialSceneArgs;
        private SerializedProperty m_manualFadeOut;
        private SerializedProperty m_fadeOutDuration;

        private int selectedScene;
        private VisualElement m_Container;
        //private string[] scenesList;

        private void OnEnable()
        {
            m_resolveAtPlay = serializedObject.FindProperty("AutoResolveScenesAtPlay");
            m_timeScale = serializedObject.FindProperty("TimeScale");
            m_targetFrameRate = serializedObject.FindProperty("TargetFrameRate");
            m_initialSceneName = serializedObject.FindProperty("InitialSceneName");
            m_initialSceneArgs = serializedObject.FindProperty("InitialSceneArgs");
            m_manualFadeOut = serializedObject.FindProperty("ManualFadeOut");
            m_fadeOutDuration = serializedObject.FindProperty("FadeOutDuration");

            //scenesList = EditorBuildSettings.scenes.ToList().ConvertAll(x =>
            //{
            //    int pos = x.path.LastIndexOf("/") + 1;
            //    return x.path.Substring(pos, x.path.Length - pos).Replace(".unity", "");
            //}).ToArray();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var element = m_Document.Instantiate();
            element.Q<Slider>("timescale_slider").BindProperty(m_timeScale);
            element.Q<FloatField>("timescale_floatfield").BindProperty(m_timeScale);
            element.Q<Toggle>("resolve_at_play_toggle").BindProperty(m_resolveAtPlay);
            element.Q<SceneField>("scene_name_field").BindProperty(m_initialSceneName);
            var manualFadeOutToggle = element.Q<Toggle>("manual_fade_out_toggle");
            manualFadeOutToggle.BindProperty(m_manualFadeOut);
            element.Q<FloatField>("target_framerate_float").BindProperty(m_targetFrameRate);

            ObjectField initialSceneArgsField = new ObjectField(string.Empty);
            initialSceneArgsField.AddToClassList("field-grow");
            initialSceneArgsField.objectType = typeof(SceneArgs);
            initialSceneArgsField.allowSceneObjects = false;
            initialSceneArgsField.BindProperty(m_initialSceneArgs);
            element.Q<VisualElement>("initial_scene_args_field_container").Add(initialSceneArgsField);

            manualFadeOutToggle.RegisterValueChangedCallback<bool>(OnManualFadeOutToggleChanged);

            m_Container = element;
            return element;
        }

        private void OnManualFadeOutToggleChanged(ChangeEvent<bool> evt)
        {
            var fadeOutElement = m_Container.Q<VisualElement>("fade_out_duration_container");
            fadeOutElement.style.display = evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void CreateIMGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();


            //HEADER
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            EditorGUILayout.LabelField("Scene Management", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resolve Scenes On Play", GUILayout.Width(LABEL_WIDTH));
            m_resolveAtPlay.boolValue = EditorGUILayout.Toggle(m_resolveAtPlay.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            //HEADER
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            EditorGUILayout.LabelField("Timings", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time Scale", GUILayout.Width(LABEL_WIDTH));
            m_timeScale.floatValue = EditorGUILayout.Slider(m_timeScale.floatValue, 0, 5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Frame Rate", GUILayout.Width(LABEL_WIDTH));
            var oldFramerate = m_targetFrameRate.intValue;
            var newFramerate = EditorGUILayout.IntField(GUIContent.none, m_targetFrameRate.intValue);
            m_targetFrameRate.intValue = newFramerate;

            if(newFramerate < 0)
            {
                newFramerate = 0;
            }

            if(oldFramerate != newFramerate)
            {
                ENTRY.UpdateTargetFramerate(newFramerate);
            }

            GUILayout.EndHorizontal();

            var optionDataList = new List<string>();

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                optionDataList.Add(name);
            }

            EditorGUILayout.EndVertical();

            //HEADER
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            EditorGUILayout.LabelField("Initial Scene", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene Name", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(m_initialSceneName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene Args", GUILayout.Width(LABEL_WIDTH));
            EditorGUILayout.PropertyField(m_initialSceneArgs, GUIContent.none);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Manual Fade Out", GUILayout.Width(LABEL_WIDTH));
            m_manualFadeOut.boolValue = EditorGUILayout.Toggle(m_manualFadeOut.boolValue);
            EditorGUILayout.EndHorizontal();

            if(!m_manualFadeOut.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Fade Out Duration", GUILayout.Width(LABEL_WIDTH));
                m_fadeOutDuration.floatValue = EditorGUILayout.FloatField(m_fadeOutDuration.floatValue);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            serializedObject.ApplyModifiedProperties();
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif