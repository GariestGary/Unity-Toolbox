#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(ScenePool))]
    public class ScenePoolEditor : UnityEditor.Editor
    {
        private GUISkin m_Skin;

        private SerializedProperty m_poolsList;
        private SerializedProperty m_RunType;
        private string searchValue;
        private Vector2 currentScrollPos;
        private float labelsWidth = 110;

        private Dictionary<int, string> m_RunTypes = new Dictionary<int, string>()
        {
            {0, "On Rise"},
            {1, "On Ready"},
            {2, "Manual"}
        };

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            m_poolsList = serializedObject.FindProperty("m_Pools");
            m_RunType = serializedObject.FindProperty("m_RunType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initialization type");
            if(EditorGUILayout.DropdownButton(new GUIContent(m_RunTypes[m_RunType.intValue]), FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("On Rise"), m_RunType.intValue == 0, OnRunTypeSelected, 0);
                menu.AddItem(new GUIContent("On Ready"), m_RunType.intValue == 1, OnRunTypeSelected, 1);
                menu.AddItem(new GUIContent("On Manual"), m_RunType.intValue == 2, OnRunTypeSelected, 2);
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            PoolerEditor.DrawSearchHeader(ref searchValue, m_poolsList, ref currentScrollPos.y);
            EditorGUI.indentLevel++;

            EditorGUILayout.Space(5);

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            currentScrollPos = EditorGUILayout.BeginScrollView(currentScrollPos);

            for (int i = 0; i < m_poolsList.arraySize; i++)
            {
                var pool = m_poolsList.GetArrayElementAtIndex(i);

                if (searchValue.IsValuable())
                {
                    if (pool.FindPropertyRelative("tag").stringValue.ToLower().Contains(searchValue.ToLower()))
                    {
                        PoolerEditor.DrawElement(pool, m_poolsList, i, labelsWidth, ResourcesUtils.GetOrLoadAsset(m_Skin, "toolbox_styles.guiskin"));
                    }
                }
                else
                {
                    PoolerEditor.DrawElement(pool, m_poolsList, i, labelsWidth, ResourcesUtils.GetOrLoadAsset(m_Skin, "toolbox_styles.guiskin"));
                }

                GUILayout.Space(3);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void OnRunTypeSelected(object userdata)
        {
            EditorGUI.BeginChangeCheck();
            m_RunType.intValue = (int)userdata;
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif