#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(DatabaseDataHolder))]
    public class DatabaseEditor : UnityEditor.Editor
    {
        private SerializedProperty m_database;

        private Vector2 _currentScrollPosition;

        private Color buttonColor = new Color(0.8705882352941176f, 0.3450980392156863f, 0.3450980392156863f);

        private void OnEnable()
        {
            m_database = serializedObject.FindProperty("properties");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_database);

            GUILayout.Space(5);

            _currentScrollPosition = GUILayout.BeginScrollView(_currentScrollPosition);

            if (m_database.objectReferenceValue == null)
            {
                GUILayout.Label("Create new database by right clicking in project window and then Create -> Toolbox -> Properties Database");
            }
            else
            {
                var databaseObject = new SerializedObject(m_database.objectReferenceValue);

                databaseObject.Update();

                var props = databaseObject.FindProperty("properties");

                for (int i = 0; i < props.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    EditorGUILayout.PropertyField(props.GetArrayElementAtIndex(i));

                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = buttonColor;

                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25), GUILayout.ExpandHeight(true)))
                    {
                        props.DeleteArrayElementAtIndex(i);
                    }

                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(5);
                }


                if (GUILayout.Button(EditorGUIUtility.IconContent("CreateAddNew"), GUILayout.Height(30)))
                {
                    props.InsertArrayElementAtIndex(Mathf.Clamp(props.arraySize - 1, 0, int.MaxValue));
                }

                databaseObject.ApplyModifiedProperties();
            }


            GUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif