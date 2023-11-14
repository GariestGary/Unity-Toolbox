#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(ScenePool))]
    public class ScenePoolEditor : UnityEditor.Editor
    {
        private SerializedProperty m_poolsList;
        private string searchValue;
        private Vector2 currentScrollPos;
        private float labelsWidth = 110;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            m_poolsList = serializedObject.FindProperty("pools");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel--;
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
                        PoolerEditor.DrawElement(pool, m_poolsList, i, labelsWidth);
                    }
                }
                else
                {
                    PoolerEditor.DrawElement(pool, m_poolsList, i, labelsWidth);
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

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