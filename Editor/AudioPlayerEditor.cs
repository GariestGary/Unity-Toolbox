using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(AudioPlayerDataHolder))]
    public class AudioPlayerEditor : Editor
    {
        private SerializedProperty m_albums;
        private Vector2 currentScrollPosition;

        private void OnEnable()
        {
            m_albums = serializedObject.FindProperty("albums");
        }

        public override void OnInspectorGUI()
        {
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition);

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < m_albums.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"));
                


                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawAlbum(SerializedProperty property)
        {

        }
    }
}
