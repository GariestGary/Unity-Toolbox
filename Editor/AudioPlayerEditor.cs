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

        private Color buttonColor = new Color(0.8705882352941176f, 0.3450980392156863f, 0.3450980392156863f);

        private void OnEnable()
        {
            m_albums = serializedObject.FindProperty("albums");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            albumSearchValue = GUILayout.TextField(albumSearchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                albumSearchValue = "";
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add Album", GUILayout.Width(80), GUILayout.ExpandHeight(true)))
            {
                m_albums.InsertArrayElementAtIndex(m_albums.arraySize - 1);
                m_albums.GetArrayElementAtIndex(m_albums.arraySize - 1).FindPropertyRelative("albumName").stringValue = string.Empty;
                currentScrollPosition.y = float.MaxValue;
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition);

            GUILayout.Space(5);

            for (int i = 0; i < m_albums.arraySize; i++)
            {
                var album = m_albums.GetArrayElementAtIndex(i);

                if(albumSearchValue.IsValuable())
                {
                    if(album.FindPropertyRelative("albumName").stringValue.ToLower().Contains(albumSearchValue.ToLower()))
                    {
                        DrawAlbum(album, m_albums, i);
                    }
                }
                else
                {
                    DrawAlbum(album, m_albums, i);
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAlbum(SerializedProperty property, SerializedProperty list, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));

            EditorGUILayout.BeginHorizontal(GUILayout.Height(15));

            var albumName = property.FindPropertyRelative("albumName");

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, albumName.stringValue, true);

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;

            EditorGUILayout.BeginVertical(GUILayout.Width(25));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25)))
            {
                if(EditorUtility.DisplayDialog("Confirm delete", $"Are you sure want to delete {albumName} album?", "Yes", "Cancel"))
                {
                    GUI.backgroundColor = oldColor;
                    list.DeleteArrayElementAtIndex(index);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            GUI.backgroundColor = oldColor;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (property.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);

                EditorGUILayout.BeginVertical();
                var m_clips = property.FindPropertyRelative("clips");

                GUILayout.Space(8);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Album Name", GUILayout.Width(labelSize));

                albumName.stringValue = EditorGUILayout.TextField(albumName.stringValue);

                GUILayout.Space(25);

                var useSeparateSource = property.FindPropertyRelative("useSeparateSource");

                EditorGUILayout.LabelField("Use Separate Audio Source", GUILayout.Width(labelSize + 50));
                useSeparateSource.boolValue = EditorGUILayout.Toggle(useSeparateSource.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));

                GUILayout.Space(25);

                if (GUILayout.Button("Add Clip", GUILayout.Width(75)))
                {
                    m_clips.InsertArrayElementAtIndex(0);
                    m_clips.GetArrayElementAtIndex(0).FindPropertyRelative("id").stringValue = string.Empty;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();

                GUILayout.Space(5);

                for (int i = 0; i < m_clips.arraySize; i++)
                {
                    DrawClip(m_clips.GetArrayElementAtIndex(i), m_clips, i);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawClip(SerializedProperty property, SerializedProperty list, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));

            EditorGUILayout.BeginHorizontal(GUILayout.Height(15));

            var clipId = property.FindPropertyRelative("id");
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, clipId.stringValue, true);

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;

            EditorGUILayout.BeginVertical(GUILayout.Width(15));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25)))
            {
                GUI.backgroundColor = oldColor;
                list.DeleteArrayElementAtIndex(index);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }
            GUI.backgroundColor = oldColor;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if(property.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);

                EditorGUILayout.BeginVertical();

                GUILayout.Space(8);

                //clip id draw
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clip ID", GUILayout.Width(labelSize));
                clipId.stringValue = EditorGUILayout.TextField(clipId.stringValue);
                EditorGUILayout.EndHorizontal();

                //clip reference draw
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Audio Clip", GUILayout.Width(labelSize));
                var clip = property.FindPropertyRelative("clip");
                EditorGUILayout.PropertyField(clip, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
