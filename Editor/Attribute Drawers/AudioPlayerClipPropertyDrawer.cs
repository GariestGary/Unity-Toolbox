using System;
using System.Collections;
using System.Collections.Generic;
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
        private Dictionary<string, string[]> m_AlbumClipsRelations = new();
        private string[] m_Albums = new string[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidateClips();

            if(m_AudioPlayerDataHolder == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginChangeCheck();

            var currentString = property.stringValue;
            var (albumIndex, clipIndex) = Parse(currentString);

            EditorGUI.LabelField(position, label);

            var labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth + 2;
            var halfPropertyWidth = (position.width - labelRect.width) * 0.5f;

            #region ALBUM
            //Label
            var albumLabel = position;
            albumLabel.x = labelRect.width;
            albumLabel.width = 40;

            EditorGUI.LabelField(albumLabel, "Album");

            //Property
            var albumRect = position;
            albumRect.x = albumLabel.x + albumLabel.width + 5;
            albumRect.width = halfPropertyWidth - albumLabel.width;

            albumIndex = EditorGUI.Popup(albumRect, albumIndex, m_Albums);
            
            var albumName = string.Empty;
            
            if(albumIndex >= 0 && albumIndex < m_Albums.Length)
            {
                albumName = m_Albums[albumIndex];
            }
            #endregion

            #region CLIP
            //Label
            var clipLabel = position;
            clipLabel.x = albumRect.x + albumRect.width + 5;
            clipLabel.width = 40;

            EditorGUI.LabelField(clipLabel, "Clip");

            //Property
            var clipRect = position;
            clipRect.x = clipLabel.x + clipLabel.width;
            clipRect.width = halfPropertyWidth - clipLabel.width - 10;

            var clipsArray = new string[0];

            if(m_AlbumClipsRelations.ContainsKey(albumName))
            {
                clipsArray = m_AlbumClipsRelations[albumName];
            }

            clipIndex = EditorGUI.Popup(clipRect, clipIndex, clipsArray);

            var clipName = string.Empty;

            if(clipIndex >= 0 && clipIndex < clipsArray.Length)
            {
                clipName = clipsArray[clipIndex];
            }
            #endregion

            property.stringValue = string.Join("/", albumName, clipName);   
        }

        private (int, int) Parse(string text)
        {
            var splits = text.Split("/");

            if(splits == null || splits.Length < 2)
            {
                return (0, 0);
            }

            var albumIndex = Array.IndexOf(m_Albums, splits[0]);
            var clipIndex = -1;
            if (albumIndex > -1)
            {
                clipIndex = Array.IndexOf(m_AlbumClipsRelations[splits[0]], splits[1]);
            }

            return (albumIndex, clipIndex);
        }

        private void ValidateClips()
        {
            if (IsClipsChanged || m_AudioPlayerDataHolder == null)
            {
                m_AlbumClipsRelations.Clear();

                if(m_AudioPlayerDataHolder == null)
                {
                    m_AudioPlayerDataHolder = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
                }

                if(m_AudioPlayerDataHolder == null)
                {
                    return;
                }

                m_Albums = m_AudioPlayerDataHolder.Albums.ConvertAll(c => c.albumName).ToArray();

                foreach(var album in m_AudioPlayerDataHolder.Albums)
                {
                    m_AlbumClipsRelations.Add(album.albumName, album.clips.ConvertAll(c => c.id).ToArray());
                }

                IsClipsChanged = false;
            }
        }
    }
}
