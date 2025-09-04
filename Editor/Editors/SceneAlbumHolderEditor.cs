#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(SceneAlbumsHolder))]
    public class SceneAlbumHolderEditor : UnityEditor.Editor
    {
        private GUISkin m_Skin;

        private SerializedProperty _AlbumsList;
        private string _AlbumSearchValue;
        private Vector2 _CurrentScrollPosition;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            _AlbumsList = serializedObject.FindProperty("_Albums");
        }

        private void SetExpandedStateForAll(bool value)
        {
            for (int i = 0; i < _AlbumsList.arraySize; i++)
            {
                var album = _AlbumsList.GetArrayElementAtIndex(i);
                album.isExpanded = value;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            EditorGUI.indentLevel--;
            _AlbumSearchValue = GUILayout.TextField(_AlbumSearchValue, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                _AlbumSearchValue = "";
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add Album", GUILayout.Width(80), GUILayout.ExpandHeight(true)))
            {
                if (_AlbumsList.arraySize <= 0)
                {
                    _AlbumsList.InsertArrayElementAtIndex(0);
                }
                else
                {
                    _AlbumsList.InsertArrayElementAtIndex(_AlbumsList.arraySize - 1);
                }

                _AlbumsList.GetArrayElementAtIndex(_AlbumsList.arraySize - 1).FindPropertyRelative("albumName").stringValue = string.Empty;
                _CurrentScrollPosition.y = float.MaxValue;

                AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_PreMatQuad"), GUILayout.Width(20), GUILayout.ExpandHeight(true)))
            {
                AudioUtils.StopAllPreviewClips();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            EditorGUI.indentLevel++;
            if (_AlbumsList.arraySize > 0)
            {
                EditorGUILayout.LabelField("Albums:", ResourcesUtils.GetOrLoadAsset(m_Skin, "toolbox_styles.guiskin").GetStyle("Label"));

                if (GUILayout.Button("Expand All"))
                {
                    SetExpandedStateForAll(true);
                }

                if (GUILayout.Button("Collapse All"))
                {
                    SetExpandedStateForAll(false);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            _CurrentScrollPosition = EditorGUILayout.BeginScrollView(_CurrentScrollPosition);

            GUILayout.Space(5);

            for (int i = 0; i < _AlbumsList.arraySize; i++)
            {
                var album = _AlbumsList.GetArrayElementAtIndex(i);

                if (_AlbumSearchValue.IsValuable())
                {
                    if (album.FindPropertyRelative("albumName").stringValue.ToLower().Contains(_AlbumSearchValue.ToLower()))
                    {
                        AudioPlayerEditor.DrawAlbum(album, _AlbumsList, i, ResourcesUtils.GetOrLoadAsset(m_Skin, "toolbox_styles.guiskin"), AudioPlayerEditor.RedButtonColor, AudioPlayerEditor.LabelSize, true);
                    }
                }
                else
                {
                    AudioPlayerEditor.DrawAlbum(album, _AlbumsList, i, ResourcesUtils.GetOrLoadAsset(m_Skin, "toolbox_styles.guiskin"), AudioPlayerEditor.RedButtonColor, AudioPlayerEditor.LabelSize, true);
                }
                GUILayout.Space(3);
            }

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif