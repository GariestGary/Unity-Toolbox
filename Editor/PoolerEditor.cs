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
            

            currentScrollPos = EditorGUILayout.BeginScrollView(currentScrollPos);

            for (int i = 0; i < m_poolsList.arraySize; i++)
            {
                DrawElement(i);
            }

            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElement(int index)
        {
            var pool = m_poolsList.GetArrayElementAtIndex(index);

            var tag = pool.FindPropertyRelative("tag");

            if (searchValue.IsValuable())
            {
                if (!tag.stringValue.ToLower().Contains(searchValue.ToLower()))
                {
                    return;
                }
            }

            EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pool Tag", GUILayout.Width(labelsWidth));
            tag.stringValue = EditorGUILayout.TextField(tag.stringValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefab", GUILayout.Width(labelsWidth));
            var pooledObj = pool.FindPropertyRelative("pooledObject");
            EditorGUILayout.PropertyField(pooledObj, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Pool Size", GUILayout.Width(labelsWidth));
            var initialSize = pool.FindPropertyRelative("initialSize");
            initialSize.intValue = EditorGUILayout.IntField(initialSize.intValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            var preview = AssetPreview.GetAssetPreview(pooledObj.objectReferenceValue);
            var size = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
            GUILayout.Label(preview, GUILayout.Width(size), GUILayout.Height(size));

            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(size)))
            {
                m_poolsList.DeleteArrayElementAtIndex(index);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }
    }
}
