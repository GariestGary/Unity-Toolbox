using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
    }
}
