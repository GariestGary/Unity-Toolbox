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

        private void OnEnable()
        {
            m_useSaves = serializedObject.FindProperty("useSaves");
            m_stateProvider = serializedObject.FindProperty("stateProvider");
            m_fileHandler = serializedObject.FindProperty("fileHandler");
            m_slotsCount = serializedObject.FindProperty("saveSlotsCount");
            m_slots = serializedObject.FindProperty("saveSlots");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use Saves");
            GUILayout.FlexibleSpace();
            m_useSaves.boolValue = EditorGUILayout.Toggle(m_useSaves.boolValue, GUILayout.Width(25));

            EditorGUILayout.EndHorizontal();




            EditorGUILayout.EndVertical();
        }
    }
}
