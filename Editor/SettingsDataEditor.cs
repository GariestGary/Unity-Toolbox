#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(SettingsData))]
    public class SettingsDataEditor: UnityEditor.Editor
    {
        public const float LABEL_WIDTH = 150;

        private SerializedProperty m_resolveAtPlay;
        private SerializedProperty m_timeScale;
        private SerializedProperty m_targetFrameRate;
        private SerializedProperty m_initialSceneName;
        private SerializedProperty m_initialSceneArgs;
        private SerializedProperty m_manualFadeOut;
        private SerializedProperty m_fadeOutDuration;

        private int selectedFramerate;
        private int selectedScene;

        private void OnEnable()
        {
            m_resolveAtPlay = serializedObject.FindProperty("AutoResolveScenesAtPlay");
            m_timeScale = serializedObject.FindProperty("TimeScale");
            m_targetFrameRate = serializedObject.FindProperty("TargetFrameRate");
            m_initialSceneName = serializedObject.FindProperty("InitialSceneName");
            m_initialSceneArgs = serializedObject.FindProperty("InitialSceneArgs");
            m_manualFadeOut = serializedObject.FindProperty("ManualFadeOut");
            m_fadeOutDuration = serializedObject.FindProperty("FadeOutDuration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resolve Scenes On Play", GUILayout.Width(LABEL_WIDTH));
            m_resolveAtPlay.boolValue = EditorGUILayout.Toggle(m_resolveAtPlay.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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

            GUILayout.BeginHorizontal();
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
        }
    }
}
#endif