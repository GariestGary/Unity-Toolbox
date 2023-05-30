using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(AudioPlayerDataHolder))]
    public class AudioPlayerEditor: Editor
    {
        private SerializedProperty m_albums;
        private Vector2 currentScrollPosition;
        private float labelSize = 110;
        private string albumSearchValue;

        private void OnEnable()
        {
            m_albums = serializedObject.FindProperty("albums");
        }

        public override void OnInspectorGUI()
        {
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition);

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            albumSearchValue = GUILayout.TextField(albumSearchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                albumSearchValue = "";
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add", GUILayout.Width(50)))
            {
                m_albums.InsertArrayElementAtIndex(0);
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < m_albums.arraySize; i++)
            {
                var album = m_albums.GetArrayElementAtIndex(i);

                if(albumSearchValue.IsValuable())
                {
                    if(album.FindPropertyRelative("albumName").stringValue.ToLower().Contains(albumSearchValue.ToLower()))
                    {
                        DrawAlbum(album);
                    }
                }
                else
                {
                    DrawAlbum(album);
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawAlbum(SerializedProperty property)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.FindPropertyRelative("albumName").stringValue, true);

            if (property.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Album Name", GUILayout.Width(labelSize));
                var name = property.FindPropertyRelative("albumName");
                name.stringValue = EditorGUILayout.TextField(name.stringValue);
                EditorGUILayout.EndHorizontal();

                var m_clips = property.FindPropertyRelative("clips");

                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
                albumSearchValue = GUILayout.TextField(albumSearchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));
                if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                {
                    albumSearchValue = "";
                    GUI.FocusControl(null);
                }

                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    m_clips.InsertArrayElementAtIndex(0);
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();

                for (int i = 0; i < m_clips.arraySize; i++)
                {
                    DrawClip(m_clips.GetArrayElementAtIndex(i));
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawClip(SerializedProperty property)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));

            EditorGUILayout.LabelField(property.FindPropertyRelative("id").stringValue);

            EditorGUILayout.EndVertical();
        }
    }
}
