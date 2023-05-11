#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VolumeBox.Toolbox;
using System;

public class ToolboxSettingsEditorWindow: EditorWindow
{
    private static ToolboxSettings settings;

    [MenuItem("Toolbox/Settings")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<ToolboxSettingsEditorWindow>();
        wnd.titleContent = new GUIContent("Toolbox Settings");
    }

    private void CreateGUI()
    {
        settings = Resources.Load<ToolboxSettings>("Toolbox Settings");
    }

    private void OnGUI()
    {
        var editor = Editor.CreateEditor(settings);
        editor.OnInspectorGUI();
        editor.Repaint();
    }
}
#endif