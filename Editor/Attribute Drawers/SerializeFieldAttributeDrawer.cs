using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(TypeReferenceAttribute))]
    public class SerializeFieldAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if(property.propertyType == SerializedPropertyType.ManagedReference)
            {
                return new SerializeReferenceField(property);
            }
            else
            {
                return base.CreatePropertyGUI(property);
            }
        }
    }
}
