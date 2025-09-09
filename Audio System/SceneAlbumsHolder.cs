using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class SceneAlbumsHolder : MonoCached
    {
        [SerializeField] private List<AudioAlbum> _Albums;

        public List<AudioAlbum> Albums => _Albums;

        protected override void Rise()
        {
            foreach(var album in _Albums)
            {
                Toolbox.AudioPlayer.AddAlbum(album);
            }
        }

        protected override void Destroyed()
        {
            foreach(var album in _Albums)
            {
                Toolbox.AudioPlayer.TryRemoveAlbum(album);
            }
        }
    }
}
