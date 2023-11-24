#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(InspectableType<>), true)]
    public class InspectableTypeDrawer : PropertyDrawer {
        Type[] _derivedTypes;
        GUIContent[] _optionLabels;
        int _selectedIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var storedProperty = property.FindPropertyRelative("qualifiedName");
            string qualifiedName = storedProperty.stringValue;

            if (_optionLabels == null) {
                Initialize(property, storedProperty);
            } else if (_selectedIndex == _derivedTypes.Length) {
                if (qualifiedName != "null") UpdateIndex(storedProperty);
            } else {
                if (qualifiedName != _derivedTypes[_selectedIndex].AssemblyQualifiedName) UpdateIndex(storedProperty);
            }

            var propLabel = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            _selectedIndex = EditorGUI.Popup(position, propLabel, _selectedIndex, _optionLabels);

            if (EditorGUI.EndChangeCheck()) {            
                storedProperty.stringValue = _selectedIndex < _derivedTypes.Length ? _derivedTypes[_selectedIndex].AssemblyQualifiedName : "null";
            }
            EditorGUI.EndProperty();
        }

        static Type[] FindAllDerivedTypes(Type baseType)
        {
            var basic = Assembly.Load("Assembly-CSharp")
                .GetTypes()
                .Where(t => 
                    t != baseType && 
                    !t.IsAbstract && 
                    baseType.IsAssignableFrom(t)).ToArray<Type>();
            return baseType.Assembly
                .GetTypes()
                .Where(t =>
                    t != baseType &&
                    !t.IsAbstract &&
                    baseType.IsAssignableFrom(t)
                    ).ToArray<Type>().Concat(basic).ToArray<Type>();
        }

        void Initialize(SerializedProperty property, SerializedProperty stored) {       

            var baseTypeProperty = property.FindPropertyRelative("baseTypeName");
            var baseType = Type.GetType(baseTypeProperty.stringValue);

            _derivedTypes = FindAllDerivedTypes(baseType);

            if (_derivedTypes.Length == 0) {
                _optionLabels = new[] { new GUIContent($"No types derived from {baseType.Name} found.")};
                return;
            }
            
            _optionLabels = new GUIContent[_derivedTypes.Length+1];
            for (int i = 0; i < _derivedTypes.Length; i++) {
                _optionLabels[i] = new GUIContent(_derivedTypes[i].Name);
            }
            _optionLabels[_derivedTypes.Length] = new GUIContent("null");
            Array.Sort(_optionLabels, (x,y) => 
            {
                if(x.text == "null")
                {
                    return 1;
                }
                else
                {
                    return String.Compare(x.text, y.text);
                }
            });
            UpdateIndex(stored);
        }

        void UpdateIndex(SerializedProperty stored) {
            string qualifiedName = stored.stringValue;
            
            for (int i = 0; i < _derivedTypes.Length; i++)
            {
                if (_derivedTypes[i].AssemblyQualifiedName == qualifiedName)
                {
                    _selectedIndex = i;
                    return;
                }
            }
            _selectedIndex = _derivedTypes.Length;
            stored.stringValue = "null";
        }
    }
}
#endif