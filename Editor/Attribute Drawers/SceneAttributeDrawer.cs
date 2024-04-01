using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using VolumeBox.Toolbox;
using Alchemy.Editor;
using UnityEditor.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomAttributeDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer: AlchemyAttributeDrawer
    {
        public override void OnCreateElement()
        {
            var parent = TargetElement.parent;
            var field = new SceneField(SerializedProperty.displayName);
            field.BindProperty(SerializedProperty);
            parent.Remove(TargetElement);
            parent.Add(field);
        }
    }
}
