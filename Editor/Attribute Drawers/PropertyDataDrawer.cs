#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(Property))]
    public class PropertyDataDrawer : PropertyDrawer
    {
        private const float CONTAINER_HEIGHT = 25;
        private const float PROPERTY_HEIGHT = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = PROPERTY_HEIGHT;
            position.y += (CONTAINER_HEIGHT - PROPERTY_HEIGHT) * 0.5f;

            float remainedWidth = position.width;

            position.width = 20;
            EditorGUI.LabelField(position, "ID");

            position.x += position.width + 5;
            position.width = 150;
            var m_id = property.FindPropertyRelative("id");
            EditorGUI.PropertyField(position, m_id, GUIContent.none);

            position.x += position.width + 5;
            position.width = 35;
            EditorGUI.LabelField(position, "Type");

            position.x += position.width + 5;
            position.width = 100;
            var m_type = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(position, m_type, GUIContent.none);

            position.x += position.width + 5;
            position.width = remainedWidth - 325;

            switch (m_type.enumValueIndex)
            {
                case 0:
                    var m_bool = property.FindPropertyRelative("boolData");
                    m_bool.boolValue = EditorGUI.Toggle(position, m_bool.boolValue);
                    break;
                case 1:
                    var m_int = property.FindPropertyRelative("integerData");
                    m_int.intValue = EditorGUI.IntField(position, m_int.intValue);
                    break;
                case 2:
                    var m_float = property.FindPropertyRelative("floatData");
                    m_float.floatValue = EditorGUI.FloatField(position, m_float.floatValue);
                    break;
                case 3:
                    var m_vec2 = property.FindPropertyRelative("vector2Data");
                    m_vec2.vector2Value = EditorGUI.Vector2Field(position, GUIContent.none, m_vec2.vector2Value);
                    break;
                case 4:
                    var m_vec3 = property.FindPropertyRelative("vector3Data");
                    m_vec3.vector3Value = EditorGUI.Vector3Field(position, GUIContent.none, m_vec3.vector3Value);
                    break;
                case 5:
                    var m_vec4 = property.FindPropertyRelative("vector4Data");
                    m_vec4.vector4Value = EditorGUI.Vector4Field(position, GUIContent.none, m_vec4.vector4Value);
                    break;
                case 6:
                    var m_string = property.FindPropertyRelative("stringData");
                    m_string.stringValue = EditorGUI.TextField(position, m_string.stringValue);
                    break;
                default:
                    break;
            }

        }

        private bool IsType<T>(object data)
        {
            return data is T;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CONTAINER_HEIGHT;
        }
    }
}
#endif