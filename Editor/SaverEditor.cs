using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(SaverDataHolder))]
    public class SaverEditor : Editor
    {
        private SerializedProperty m_useSaves;
        private SerializedProperty m_stateProvider;
        private SerializedProperty m_fileHandler;
        private SerializedProperty m_slotsCount;
        private SerializedProperty m_slots;

        private Vector2 currentScrollPosition;
        private const float LABEL_SIZE = 125;

        private void OnEnable()
        {
            m_useSaves = serializedObject.FindProperty("useSaves");
            m_stateProvider = serializedObject.FindProperty("stateProvider");
            m_fileHandler = serializedObject.FindProperty("fileHandler");
            m_slotsCount = serializedObject.FindProperty("saveSlotsCount");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use Saves", GUILayout.Width(LABEL_SIZE));
            m_useSaves.boolValue = EditorGUILayout.Toggle(m_useSaves.boolValue, GUILayout.Width(25));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("State Provider", GUILayout.Width(LABEL_SIZE));
            EditorGUILayout.PropertyField(m_stateProvider, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Handler", GUILayout.Width(LABEL_SIZE));
            EditorGUILayout.PropertyField(m_fileHandler, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Slots Count", GUILayout.Width(LABEL_SIZE));
            m_slotsCount.intValue = EditorGUILayout.IntField(m_slotsCount.intValue, GUILayout.Width(25));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }


    }
}
