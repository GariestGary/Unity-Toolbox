using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(TypeDropdownAttribute))]
    public class TypeDropdownPropertyDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(position, "A");
            EditorGUI.EndProperty();
        }
    }
}
