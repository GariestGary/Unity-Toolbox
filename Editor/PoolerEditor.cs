#if UNITY_EDITOR
using Alchemy.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(PoolerDataHolder))]
    public class PoolerEditor : AlchemyEditor
    {
        [SerializeField] private VisualTreeAsset m_Document;
        [SerializeField] private VisualTreeAsset m_PoolDocument;
        [SerializeField] private GUISkin m_Skin;

        private SerializedProperty m_poolsList;
        private SerializedProperty m_poolGCInterval;
        private string searchValue;
        private Vector2 currentScrollPos;
        private float labelsWidth = 110;

        private static Color buttonColor = new Color(0.8705882352941176f, 0.3450980392156863f, 0.3450980392156863f);

        private void OnEnable()
        {
            if(target == null)
            {
                return;
            }

            m_poolsList = serializedObject.FindProperty("poolsList");
            m_poolGCInterval = serializedObject.FindProperty("m_GarbageCollectorWorkInterval");
        }

        public override VisualElement CreateInspectorGUI()
        {
            var element = new IMGUIContainer(() => 
            {
                CreateIMGUI();
            });

            return element;
        }

        public void CreateIMGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawSearchHeader(ref searchValue, m_poolsList, ref currentScrollPos.y);
            EditorGUILayout.Space(3);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GC Collect Interval", GUILayout.Width(SettingsDataEditor.LABEL_WIDTH));
            var interval = EditorGUILayout.FloatField(m_poolGCInterval.floatValue);
            interval = Mathf.Clamp(interval, 0.5f, float.MaxValue);
            m_poolGCInterval.floatValue = interval;
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();

            if(m_poolsList.arraySize > 0)
            {
                if(GUILayout.Button("Expand All"))
                {
                    SetExpandedStateForAll(true);
                }

                if(GUILayout.Button("Collapse All"))
                {
                    SetExpandedStateForAll(false);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            currentScrollPos = EditorGUILayout.BeginScrollView(currentScrollPos);

            for (int i = 0; i < m_poolsList.arraySize; i++)
            {
                var pool = m_poolsList.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(4);
                if (searchValue.IsValuable())
                {
                    if (pool.FindPropertyRelative("tag").stringValue.ToLower().Contains(searchValue.ToLower()))
                    {
                        DrawElement(pool, m_poolsList, i, labelsWidth, m_Skin);
                    }
                }
                else
                {
                    DrawElement(pool, m_poolsList, i, labelsWidth, m_Skin);
                }
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(3);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void SetExpandedStateForAll(bool value)
        {
            for (int i = 0; i < m_poolsList.arraySize; i++)
            {
                m_poolsList.GetArrayElementAtIndex(i).isExpanded = value;
            }
        }

        public static void DrawSearchHeader(ref string searchValue, SerializedProperty poolsList, ref float currentScrollPosY)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchValue = EditorGUILayout.TextField(searchValue, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                searchValue = "";
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add Pool", GUILayout.Width(80), GUILayout.ExpandHeight(true)))
            {
                poolsList.InsertArrayElementAtIndex(0);
                poolsList.GetArrayElementAtIndex(0).FindPropertyRelative("tag").stringValue = string.Empty;
                currentScrollPosY = 0;
                PoolerTagPropertyDrawer.IsPoolsChanged = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void DrawElement(SerializedProperty property, SerializedProperty list, int index, float labelsWidth, GUISkin skin = null)
        {
            var oldSkin = GUI.skin;
            if(skin != null)
            {
                GUI.skin = skin;
            }

            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUILayout.Space(4);
            GUI.skin = oldSkin;
            EditorGUILayout.BeginHorizontal(GUILayout.Height(15));
            GUILayout.Space(4);
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
                    PoolerTagPropertyDrawer.IsPoolsChanged = true;
                    return;
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
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
                var prevTag = tag.stringValue;
                tag.stringValue = EditorGUILayout.TextField(tag.stringValue);

                if(prevTag != tag.stringValue)
                {
                    PoolerTagPropertyDrawer.IsPoolsChanged = true;
                }

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
                var settedValue = EditorGUILayout.IntField(initialSize.intValue);
                settedValue = Mathf.Clamp(settedValue, 1, int.MaxValue);

                initialSize.intValue = settedValue;

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.EndVertical();


                var preview = AssetPreview.GetAssetPreview(pooledObj.objectReferenceValue);
                var size = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
                GUILayout.Label(preview, GUILayout.Width(size), GUILayout.Height(size));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(4);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif