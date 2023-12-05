using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(PoolAttribute))]
    public class PoolPropertyDrawer : PropertyDrawer
    {
        private PoolerDataHolder m_PoolerDataHolder;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }
    }
}
