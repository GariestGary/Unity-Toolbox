using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(SceneDropdownAttribute))]
    public class SceneDropdownPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty m_Property;
        private SceneAdvancedDropdown m_Dropdown;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelRect, label);
            var fieldRect = position;
            fieldRect.x += labelRect.width + 2;
            fieldRect.width -= labelRect.width + 2;

            if(property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(fieldRect, "Field is not a string type");
                return;
            }

            m_Property = property;

            if(m_Dropdown == null)
            {
                m_Dropdown = new SceneAdvancedDropdown(new AdvancedDropdownState(), OnSceneSelectedCallback);
            }

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            m_Property.serializedObject.Update();
            
            if(!property.stringValue.IsValuable())
            {
                property.stringValue = SceneAdvancedDropdown.GetFormattedScenesList()[0];
                m_Property.serializedObject.ApplyModifiedProperties();
            }

            if(GUI.Button(fieldRect, new GUIContent(property.stringValue), EditorStyles.popup))
            {
                m_Dropdown.Show(fieldRect);
            }

            if(EditorGUI.EndChangeCheck())
            {
            }

            EditorGUI.EndProperty();
        }

        private void OnSceneSelectedCallback(string sceneName)
        {
            m_Property.serializedObject.Update();
            m_Property.stringValue = sceneName;
            m_Property.serializedObject.ApplyModifiedProperties();
        }
    }
}
