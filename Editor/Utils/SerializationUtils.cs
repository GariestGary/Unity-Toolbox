using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    public static class SerializationUtils
    {
        public static object GetValue(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                FieldInfo field = type.GetField(path);
                
                if(field != null)
                {
                    obj = field.GetValue(obj);
                }
            }
            return obj;
        }
    }
}
