using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(AudioPlayerClipAttribute))]
    public class AudioPlayerClipPropertyDrawer : PropertyDrawer
    {
        public static bool IsClipsChanged { get; set; }

        private AudioPlayerDataHolder m_AudioPlayerDataHolder;
        private Dictionary<string, string[]> m_AlbumClipsRelations;
        private string[] m_Albums;
        private AudioPlayerClipAdvancedDropdown m_Dropdown;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidateClips(property);
            
            var labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelRect, label);

            var dropdownRect = position;
            dropdownRect.width -= labelRect.width + 2;
            dropdownRect.x += labelRect.width + 2;
            
            if(m_Albums.Length <= 0)
            {
                EditorGUI.LabelField(dropdownRect, "There is no albums", EditorStyles.popup);
                return;
            }

            ValidateProperty(property);

            EditorGUI.BeginChangeCheck();

            if(m_Dropdown == null)
            {
                m_Dropdown = new AudioPlayerClipAdvancedDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), m_AlbumClipsRelations, clip => OnClipSelectedCallback(clip, property));
            }

            var splits = property.stringValue.Split("/");

            var albumName = splits.Length > 0 ? splits[0] : string.Empty;
            var clipName = splits.Length > 1 ? splits[1] : string.Empty;

            if(GUI.Button(dropdownRect, string.Format($"Album: {albumName} | Clip: {clipName}"), EditorStyles.popup))
            {
                m_Dropdown.Show(dropdownRect);
            }

            EditorGUI.EndChangeCheck();
        }

        private void OnClipSelectedCallback(string formattedClip, SerializedProperty property)
        {
            property.serializedObject.Update();
            property.stringValue = formattedClip;
            property.serializedObject.ApplyModifiedProperties();
        }

        private void ValidateProperty(SerializedProperty property)
        {
            string clip;
            string album;

            if (!property.stringValue.IsValuable())
            {
                (album, clip) = GetDefaultAlbumAndClip();
            }
            else
            {
                var splits = property.stringValue.Split("/");

                switch (splits.Length)
                {
                    case 0:
                        (album, clip) = GetDefaultAlbumAndClip();
                        break;

                    case 1:
                        album = splits[0];

                        if (!AlbumExists(album))
                        {
                            album = m_Albums[0];
                        }

                        clip = GetDefaultClipOfAlbum(album);
                        break;

                    case 2:
                        album = splits[0];
                        clip = splits[1];

                        if (!AlbumExists(album))
                        {
                            album = m_Albums[0];
                        }

                        if (!ClipExistsInAlbum(album, clip))
                        {
                            clip = GetDefaultClipOfAlbum(album);
                        }

                        break;

                    default:
                        (album, clip) = GetDefaultAlbumAndClip();
                        break;
                }
            }

            property.stringValue = string.Join("/", album, clip);
        }

        private (string, string) GetDefaultAlbumAndClip()
        {
            var album = m_Albums[0];
            var clip = GetDefaultClipOfAlbum(album);
            return (album, clip);
        }

        private string GetDefaultClipOfAlbum(string album)
        {
            if(!AlbumExists(album))
            {
                return string.Empty;
            }

            var clips = m_AlbumClipsRelations[album];
            return clips.Length > 0 ? clips[0] : string.Empty;
        }

        private bool AlbumExists(string album)
        {
            return m_Albums.Contains(album);
        }

        private bool ClipExistsInAlbum(string album, string clip)
        {
            var albumExists = AlbumExists(album);

            if(albumExists)
            {
                return m_AlbumClipsRelations[album].Contains(clip);
            }
            else
            {
                return false;
            }
        }

        private void ValidateClips(SerializedProperty property)
        {
            if (m_AudioPlayerDataHolder == null)
            {
                m_AudioPlayerDataHolder = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            }

            if (IsClipsChanged || m_Albums == null)
            {
                m_AlbumClipsRelations = new Dictionary<string, string[]>();

                if(m_AudioPlayerDataHolder == null)
                {
                    return;
                }

                m_Albums = m_AudioPlayerDataHolder.Albums.ConvertAll(c => c.albumName).ToArray();

                foreach(var album in m_AudioPlayerDataHolder.Albums)
                {
                    m_AlbumClipsRelations.Add(album.albumName, album.clips.ConvertAll(c => c.id).ToArray());
                }

                m_Dropdown = new AudioPlayerClipAdvancedDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), m_AlbumClipsRelations, clip => OnClipSelectedCallback(clip, property));
                IsClipsChanged = false;
            }
        }
    }
}
