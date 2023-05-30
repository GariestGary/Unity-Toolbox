using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
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

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchValue = GUILayout.TextField(searchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchValue = "";
                GUI.FocusControl(null);
            }

            if(GUILayout.Button("Add", GUILayout.Width(50)))
            {
                m_poolsList.InsertArrayElementAtIndex(0);
            }

            GUILayout.EndHorizontal();

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
        }

        private void DrawElement(SerializedProperty property, SerializedProperty list, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            EditorGUILayout.BeginHorizontal();

            var tag = property.FindPropertyRelative("tag");

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, tag.stringValue, true);

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                if (EditorUtility.DisplayDialog("Confirm delete", $"Are you sure want to delete {tag.stringValue} pool?", "Yes", "Cancel"))
                {
                    GUI.backgroundColor = oldColor;
                    list.DeleteArrayElementAtIndex(index);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }

            if(!property.isExpanded)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }   
            
            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();



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



            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
