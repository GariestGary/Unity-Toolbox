using System;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(SettingsData))]
    public class SettingsDataEditor: Editor
    {
        private SerializedProperty resolveAtPlayProperty;
        //private SettingsData = 

        public override void OnInspectorGUI()
        {
            if(resolveAtPlayProperty == null)
            {
                resolveAtPlayProperty = serializedObject.FindProperty("AutoResolveScenesAtPlay");
            }

            serializedObject.Update();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Auto Resolve Scenes At Play");
            GUILayout.FlexibleSpace();
            EditorGUILayout.PropertyField(resolveAtPlayProperty, GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Width(25));
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(25);

            DrawDefaultInspector();
        }
    }
}