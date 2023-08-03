#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(PoolerDataHolder))]
    public class PoolerEditor : Editor
    {
        private PoolerDataHolder pooler;
        private SerializedProperty m_poolsList;
        private string searchValue;
        private Vector2 currentScrollPos;
        private float labelsWidth = 110;

        private Color buttonColor = new Color(0.8705882352941176f, 0.3450980392156863f, 0.3450980392156863f);

        private void OnEnable()
        {
            if(target == null)
            {
                return;
            }

            pooler = (PoolerDataHolder)target;
            m_poolsList = serializedObject.FindProperty("poolsList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchValue = EditorGUILayout.TextField(searchValue, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                searchValue = "";
                GUI.FocusControl(null);
            }

            if(GUILayout.Button("Add Pool", GUILayout.Width(80), GUILayout.ExpandHeight(true)))
            {
                m_poolsList.InsertArrayElementAtIndex(0);
                m_poolsList.GetArrayElementAtIndex(0).FindPropertyRelative("tag").stringValue = string.Empty;
                currentScrollPos.y = 0;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical();
            currentScrollPos = EditorGUILayout.BeginScrollView(currentScrollPos);

            for (int i = 0; i < m_poolsList.arraySize; i++)
            {
                var pool = m_poolsList.GetArrayElementAtIndex(i);

                if (searchValue.IsValuable())
                {
                    if (pool.FindPropertyRelative("tag").stringValue.ToLower().Contains(searchValue.ToLower()))
                    {
                        DrawElement(pool, m_poolsList, i);
                    }
                }
                else
                {
                    DrawElement(pool, m_poolsList, i);
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void DrawElement(SerializedProperty property, SerializedProperty list, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));



            EditorGUILayout.BeginHorizontal(GUILayout.Height(15));

            var tag = property.FindPropertyRelative("tag");

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, tag.stringValue, true);

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;

            EditorGUILayout.BeginVertical(GUILayout.Width(25));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25)))
            {
                if (EditorUtility.DisplayDialog("Confirm delete", $"Are you sure want to delete {tag.stringValue} pool?", "Yes", "Cancel"))
                {
                    GUI.backgroundColor = oldColor;
                    list.DeleteArrayElementAtIndex(index);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();


            GUI.backgroundColor = oldColor;
            
            if(property.isExpanded)
            {
                EditorGUILayout.Space(8);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space(20);

                EditorGUILayout.BeginVertical();


                //Tag draw
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Pool Tag", GUILayout.Width(labelsWidth));
                tag.stringValue = EditorGUILayout.TextField(tag.stringValue);

                EditorGUILayout.EndHorizontal();

                //Prefab draw
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Prefab", GUILayout.Width(labelsWidth));
                var pooledObj = property.FindPropertyRelative("pooledObject");
                EditorGUILayout.PropertyField(pooledObj, GUIContent.none);

                EditorGUILayout.EndHorizontal();

                //Pool size draw
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Initial Pool Size", GUILayout.Width(labelsWidth));
                var initialSize = property.FindPropertyRelative("initialSize");
                initialSize.intValue = EditorGUILayout.IntField(initialSize.intValue);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.EndVertical();


                var preview = AssetPreview.GetAssetPreview(pooledObj.objectReferenceValue);
                var size = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
                GUILayout.Label(preview, GUILayout.Width(size), GUILayout.Height(size));

                EditorGUILayout.EndHorizontal();

            }   
            
            EditorGUILayout.EndVertical();
        }
    }
}
#endif