#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VolumeBox.Toolbox;
using System;

public class ToolboxSettingsEditorWindow: EditorWindow
{
    private ToolboxSettings settings;
    private SerializedObject settingsObject;

    private SerializedProperty resolveAtPlayProperty;

    [MenuItem("Toolbox/Settings")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<ToolboxSettingsEditorWindow>();
        wnd.titleContent = new GUIContent("Toolbox Settings");
    }

    private void CreateGUI()
    {
        settings = Resources.Load<ToolboxSettings>("Toolbox Settings");

        settingsObject = settings == null ? null : new SerializedObject(settings);

        if (settingsObject == null) return;

        resolveAtPlayProperty = settingsObject.FindProperty("autoResolveScenesAtPlay");
    }

    private void OnGUI()
    {
        if (settingsObject == null) return;

        settingsObject.Update();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Auto Resolve Scenes At Play");
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(resolveAtPlayProperty, GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        settingsObject.ApplyModifiedProperties();
    }
}
#endif