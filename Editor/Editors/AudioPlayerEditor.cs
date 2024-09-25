#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(AudioPlayerDataHolder))]
    public class AudioPlayerEditor: UnityEditor.Editor
    {
        [SerializeField] private GUISkin m_Skin;

        private SerializedProperty m_albums;
        private Vector2 currentScrollPosition;
        private string albumSearchValue;

        public static float LabelSize = 110;
        public static Color RedButtonColor = new Color(0.8705882352941176f, 0.3450980392156863f, 0.3450980392156863f);

        private void OnEnable()
        {
            m_albums = serializedObject.FindProperty("albums");
        }

        public void DrawIMGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            albumSearchValue = GUILayout.TextField(albumSearchValue, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                albumSearchValue = "";
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add Album", GUILayout.Width(80), GUILayout.ExpandHeight(true)))
            {
                if(m_albums.arraySize <= 0)
                {
                    m_albums.InsertArrayElementAtIndex(0);
                }
                else
                {
                    m_albums.InsertArrayElementAtIndex(m_albums.arraySize - 1);
                }

                m_albums.GetArrayElementAtIndex(m_albums.arraySize - 1).FindPropertyRelative("albumName").stringValue = string.Empty;
                currentScrollPosition.y = float.MaxValue;

                AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
            }

            if(GUILayout.Button(EditorGUIUtility.IconContent("d_PreMatQuad"), GUILayout.Width(20), GUILayout.ExpandHeight(true)))
            {
                AudioUtils.StopAllPreviewClips();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();


            if(m_albums.arraySize > 0)
            {
                EditorGUILayout.LabelField("Albums:", m_Skin.GetStyle("Label"));
                
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
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition);

            GUILayout.Space(5);

            for (int i = 0; i < m_albums.arraySize; i++)
            {
                var album = m_albums.GetArrayElementAtIndex(i);

                if(albumSearchValue.IsValuable())
                {
                    if(album.FindPropertyRelative("albumName").stringValue.ToLower().Contains(albumSearchValue.ToLower()))
                    {
                        DrawAlbum(album, m_albums, i, m_Skin, RedButtonColor, LabelSize);
                    }
                }
                else
                {
                    DrawAlbum(album, m_albums, i, m_Skin, RedButtonColor, LabelSize);
                }
                GUILayout.Space(3);
            }

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void SetExpandedStateForAll(bool value)
        {
            for (int i = 0; i < m_albums.arraySize; i++)
            {
                var album = m_albums.GetArrayElementAtIndex(i);
                album.isExpanded = value;
            }
        }

        public static void DrawAlbum(SerializedProperty property, SerializedProperty list, int index, GUISkin skin, Color removeButtonColor, float labelSize)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(3);
            var oldSkin = GUI.skin;
            GUI.skin = skin;
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUILayout.Space(3);
            GUI.skin = oldSkin;

            EditorGUILayout.BeginHorizontal(GUILayout.Height(25));

            GUILayout.Space(5);
            var albumName = property.FindPropertyRelative("albumName");
            
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, albumName.stringValue, true, skin.GetStyle("Foldout"));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = removeButtonColor;

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25), GUILayout.ExpandHeight(true)))
            {
                if(EditorUtility.DisplayDialog("Confirm delete", $"Are you sure want to delete {albumName.stringValue} album?", "Yes", "Cancel"))
                {
                    GUI.backgroundColor = oldColor;
                    list.DeleteArrayElementAtIndex(index);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
                    return;
                }
            }
            GUI.backgroundColor = oldColor;

            GUILayout.Space(5);
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

                var prevAlbumName = albumName.stringValue;
                albumName.stringValue = EditorGUILayout.TextField(albumName.stringValue);
                if(prevAlbumName != albumName.stringValue)
                {
                    AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
                }

                GUILayout.Space(25);

                var useSeparateSource = property.FindPropertyRelative("useSeparateSource");

                EditorGUILayout.LabelField("Use Separate Audio Source", GUILayout.Width(labelSize + 50));
                useSeparateSource.boolValue = EditorGUILayout.Toggle(useSeparateSource.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));

                GUILayout.Space(25);

                if (GUILayout.Button("Add Clip", GUILayout.Width(75)))
                {
                    m_clips.InsertArrayElementAtIndex(0);
                    var newClip = m_clips.GetArrayElementAtIndex(0);
                    newClip.isExpanded = false;
                    newClip.FindPropertyRelative("id").stringValue = string.Empty;
                    newClip.FindPropertyRelative("volume").floatValue = 1;
                    AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
                }

                EditorGUILayout.EndHorizontal();
                
                if(useSeparateSource.boolValue)
                {
                    GUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Mixer Group", GUILayout.Width(labelSize));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("mixerGroup"), GUIContent.none);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginVertical();

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();


                if(m_clips.arraySize > 0)
                {
                    EditorGUILayout.LabelField("Clips:", skin.GetStyle("Label"));
                    
                    if (GUILayout.Button("Expand All", GUILayout.Width(100)))
                    {
                        for (int j = 0; j < m_clips.arraySize; j++)
                        {
                            var clip = m_clips.GetArrayElementAtIndex(j);

                            clip.isExpanded = true;
                        }
                    }

                    if (GUILayout.Button("Collapse All", GUILayout.Width(100)))
                    {
                        for (int j = 0; j < m_clips.arraySize; j++)
                        {
                            var clip = m_clips.GetArrayElementAtIndex(j);

                            clip.isExpanded = false;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(1);

                for (int i = 0; i < m_clips.arraySize; i++)
                {
                    DrawClip(m_clips.GetArrayElementAtIndex(i), m_clips, i, skin, removeButtonColor, labelSize);
                    GUILayout.Space(3);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(3);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawClip(SerializedProperty property, SerializedProperty list, int index, GUISkin skin, Color removeButtonColor, float labelSize)
        {
            EditorGUILayout.BeginHorizontal();
            var oldSkin = GUI.skin;
            GUI.skin = skin;
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUI.skin = oldSkin;
            GUILayout.Space(3);
            EditorGUILayout.BeginHorizontal(GUILayout.Height(25));
            GUILayout.Space(3);
            var clipId = property.FindPropertyRelative("id");
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, clipId.stringValue, true, skin.GetStyle("Foldout"));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            var clip = property.FindPropertyRelative("clip");
            var volume = property.FindPropertyRelative("volume");
            var clipValue = clip.objectReferenceValue as AudioClip;

            if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton On"), GUILayout.Width(25), GUILayout.ExpandHeight(true)))
            {
                if(!AudioPlayer.HasInstance)
                {
                    EditorUtility.DisplayDialog("AudioPlayer missing!", "AudioPlayer instance needed for clips preview doesn't exist among currently opened scenes. You can open MAIN scene from 'Toolbox/Open MAIN Scene' that already has AudioPlayer", "OK");
                }
                else
                {
                    if (clipValue != null)
                    {
                        AudioPlayer.Instance.DefaultAudioSource.Stop();
                        var prevVolume = AudioPlayer.Instance.DefaultAudioSource.volume;
                        AudioPlayer.Instance.DefaultAudioSource.volume = volume.floatValue;
                        AudioPlayer.Instance.DefaultAudioSource.PlayOneShot(clipValue);
                    }
                }


            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_PreMatQuad"), GUILayout.Width(25), GUILayout.ExpandHeight(true)))
            {
                AudioUtils.StopAllPreviewClips();
            }

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = removeButtonColor;

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(25), GUILayout.ExpandHeight(true)))
            {
                GUI.backgroundColor = oldColor;
                list.DeleteArrayElementAtIndex(index);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
                return;
            }
            GUI.backgroundColor = oldColor;

            GUILayout.Space(3);
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
                var prevClipId = clipId.stringValue;
                clipId.stringValue = EditorGUILayout.TextField(clipId.stringValue);
                
                if(clipValue != null)
                {
                    if(GUILayout.Button("Set As Clip", GUILayout.Width(80)))
                    {
                        clipId.stringValue = clipValue.name;
                    }
                }

                if(prevClipId != clipId.stringValue)
                {
                    AudioPlayerClipPropertyDrawer.IsClipsChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                //clip reference draw
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Audio Clip", GUILayout.Width(labelSize));
                EditorGUILayout.PropertyField(clip, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clip Volume", GUILayout.Width(labelSize));
                volume.floatValue = EditorGUILayout.Slider(volume.floatValue, 0, 1);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                GUILayout.Space(8);

                EditorGUILayout.EndVertical();

                var previewSize = 75;
                var preview = AssetPreview.GetAssetPreview(clip.objectReferenceValue);
                GUILayout.Label(preview, GUILayout.Width(previewSize), GUILayout.Height(previewSize));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
            GUILayout.Space(3);
            EditorGUILayout.EndHorizontal();
        }

        private void OnDisable()
        {
            AudioUtils.StopAllPreviewClips();
        }
    }
}
#endif