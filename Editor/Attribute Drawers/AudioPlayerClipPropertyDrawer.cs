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
        private Dictionary<string, string[]> m_AlbumClipsRelations;
        private string[] m_Albums;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidateClips();

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
            
            if(m_Albums == null || m_Albums.Length <= 0)
            {
                albumIndex = 0;
                albumLabel.width = position.width - labelRect.width;
                property.stringValue = string.Empty;
                EditorGUI.LabelField(albumLabel, "There is no albums");
                EditorGUI.EndChangeCheck();
                return;
            }

            EditorGUI.LabelField(albumLabel, "Album");

            //Property
            var albumRect = position;
            albumRect.x = albumLabel.x + albumLabel.width + 5;
            albumRect.width = halfPropertyWidth - albumLabel.width;

            if(albumIndex < 0)
            {
                albumIndex = 0;
            }

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
            
            var clipsArray = new string[0];

            if(m_AlbumClipsRelations.ContainsKey(albumName))
            {
                clipsArray = m_AlbumClipsRelations[albumName];
            }

            if(clipsArray == null || clipsArray.Length <= 0)
            {
                clipIndex = 0;
                clipLabel.width = position.width - labelRect.width - halfPropertyWidth;
                EditorGUI.LabelField(clipLabel, "There is no clips");
                property.stringValue = string.Join("/", albumName, string.Empty);
                EditorGUI.EndChangeCheck();
                return;
            }

            EditorGUI.LabelField(clipLabel, "Clip");

            //Property
            var clipRect = position;
            clipRect.x = clipLabel.x + clipLabel.width;
            clipRect.width = halfPropertyWidth - clipLabel.width - 10;

            if(clipIndex < 0)
            {
                clipIndex = 0;
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

                IsClipsChanged = false;
            }
        }
    }
}
