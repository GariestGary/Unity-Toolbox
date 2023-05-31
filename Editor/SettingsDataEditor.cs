using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(SettingsData))]
    public class SettingsDataEditor: Editor
    {
        private float labelsWidth = 175;

        private SerializedProperty m_resolveAtPlay;
        private SerializedProperty m_timeScale;
        private SerializedProperty m_targetFrameRate;
        private SerializedProperty m_initialSceneName;
        private SerializedProperty m_initialSceneArgs;
        private SerializedProperty m_manualFadeOut;
        private SerializedProperty m_fadeOutDuration;

        private int selectedFramerate;
        private int selectedScene;

        private string[] frameRateOptions = new string[9] { "5", "10", "15", "30", "60", "75", "120", "144", "165" };

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

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Auto Resolve Scenes At Play", GUILayout.Width(labelsWidth));
            m_resolveAtPlay.boolValue = EditorGUILayout.Toggle(m_resolveAtPlay.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time Scale", GUILayout.Width(labelsWidth));
            m_timeScale.floatValue = EditorGUILayout.Slider(m_timeScale.floatValue, 0, 5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Frame Rate", GUILayout.Width(labelsWidth));
            selectedFramerate = EditorGUILayout.Popup(selectedFramerate, frameRateOptions);

            m_targetFrameRate.intValue = int.Parse(frameRateOptions[selectedFramerate]);

            GUILayout.EndHorizontal();
            var optionDataList = new List<string>();

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                optionDataList.Add(name);
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene", GUILayout.Width(labelsWidth));
            selectedScene = EditorGUILayout.Popup(selectedScene, optionDataList.ToArray());
            m_initialSceneName.stringValue = optionDataList[selectedScene];
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene Args", GUILayout.Width(labelsWidth));
            EditorGUILayout.PropertyField(m_initialSceneArgs, GUIContent.none);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Manual Fade Out", GUILayout.Width(labelsWidth));
            m_manualFadeOut.boolValue = EditorGUILayout.Toggle(m_manualFadeOut.boolValue);
            EditorGUILayout.EndHorizontal();

            if(!m_manualFadeOut.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Fade Out Duration", GUILayout.Width(labelsWidth));
                m_fadeOutDuration.floatValue = EditorGUILayout.FloatField(m_fadeOutDuration.floatValue);
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}