using UnityEngine;
using UnityEditor;
using Alchemy.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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
