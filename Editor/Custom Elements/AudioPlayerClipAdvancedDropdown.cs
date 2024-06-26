using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    public class AudioPlayerClipAdvancedDropdown: AdvancedDropdown
    {
        private Action<string> m_OnClipSelectedCallback;
        private Dictionary<string, string[]> m_Albums;

        public AudioPlayerClipAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {
        }

        public AudioPlayerClipAdvancedDropdown(AdvancedDropdownState state, Dictionary<string, string[]> albums, Action<string> onClipSelectedCallback) : base(state)
        {
            m_Albums = albums;
            m_OnClipSelectedCallback = onClipSelectedCallback;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Audio Clips");

            for (int i = 0; i < m_Albums.Keys.Count; i++)
            {
                var album = m_Albums.ElementAt(i);

                if (album.Value.Length > 0)
                {
                    var albumRoot = new AdvancedDropdownItem(album.Key);

                    for (int j = 0; j < album.Value.Length; j++)
                    {
                        albumRoot.AddChild(new AlbumAdvancedDropDownItem(album.Value[j], album.Key, album.Value[j]));
                    }

                    root.AddChild(albumRoot);
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            m_OnClipSelectedCallback?.Invoke((item as AlbumAdvancedDropDownItem).AudioClipFormatted);
        }

    }

    public class AlbumAdvancedDropDownItem: AdvancedDropdownItem
    {
        private string m_AlbumName;
        private string m_ClipName;

        public string AudioClipFormatted => $"{m_AlbumName}/{m_ClipName}";

        public AlbumAdvancedDropDownItem(string name) : base(name)
        {
        }

        public AlbumAdvancedDropDownItem(string name, string albumName, string clipName) : base(name)
        {
            m_AlbumName = albumName;
            m_ClipName = clipName;
        }
    }
}
