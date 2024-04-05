using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public static class Utils
    {
        public static string GetManagedReferenceFieldTypeName(this SerializedProperty property)
        {
            var typeName = property.managedReferenceFieldTypename;
            var splitIndex = typeName.IndexOf(' ');
            return typeName[(splitIndex + 1)..];
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var typeName = property.managedReferenceFieldTypename;
            var splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName[..splitIndex]);
            return assembly.GetType(typeName[(splitIndex + 1)..]);
        }

        public static object SetManagedReferenceType(this SerializedProperty property, Type type)
        {
            var obj = (type != null) ? Activator.CreateInstance(type) : null;
            property.managedReferenceValue = obj;
            return obj;
        }

        public static float CalculateLabelWidth(VisualElement element, VisualElement root)
        {
            // This code is a partial modification of the Label width calculation method actually used inside PropertyField.
            var num = root.resolvedStyle.paddingLeft;
            var num2 = 37f;
            var num3 = 123f;
            var num4 = element.GetFirstAncestorOfType<Foldout>() == null ? 0f : 15f;

            var width = root.resolvedStyle.width;
            var a = width * 0.45f - num2 - num - num4;
            var b = Mathf.Max(num3 - num - num4, 0f);

            return Mathf.Max(a, b) + 12f;
        }

        public static float CalculateFieldWidth(VisualElement element, VisualElement root)
        {
            var labelWidth = CalculateLabelWidth(element, root);
            return root.resolvedStyle.width - labelWidth - 27f;
        }
    }
}
