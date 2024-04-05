using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer: PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new SceneField(property.displayName);
            field.BindProperty(property);
            return field;
        }
    }
}
