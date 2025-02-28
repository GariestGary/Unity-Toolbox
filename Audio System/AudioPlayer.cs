using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace VolumeBox.Toolbox
{
    public class AudioPlayer: MonoBehaviour
    {
        [SerializeField] private Transform audioSourcesRoot;
        [SerializeField] private AudioSource defaultAudioSource;

        private AudioPlayerDataHolder _Data;
        private List<AudioAlbumController> _AlbumControllers = new();
        
        public AudioSource DefaultAudioSource => defaultAudioSource;

        public void Initialize(Messenger msg)
        {
            msg.Subscribe<PlayAudioMessage>(x => Play(x.albumName, x.clipID, x.volume, x.pitch, x.loop, x.playType), null, true);
            msg.Subscribe<StopAudioMessage>(x => StopAudio(x.albumName), null, true);
            msg.Subscribe<StopAllAudioMessage>(_ => StopAll());

            _Data = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            
            foreach (var album in _Data.Albums)
            {
                var controller = new AudioAlbumController(album);
                _AlbumControllers.Add(controller);
                
                if(album.useSeparateSource)
                {
                    var newSourceObj = new GameObject($"{album.albumName} Audio Source");
                    newSourceObj.transform.SetParent(audioSourcesRoot);
                    album.source = newSourceObj.AddComponent<AudioSource>();
                    album.source.outputAudioMixerGroup = album.mixerGroup;
                }
                else
                {
                    album.source = defaultAudioSource;
                }
            }
        }

        protected void Clear()
        {
            _Data.Albums.ForEach(a => a.source = null);
        }

        public AudioAlbumController Play(string source, string id, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var controller = GetAlbumController(source);

            if(controller == null)
            {
                Debug.LogWarning($"Album named '{source}' not found");
                return null;
            }
            
            controller.Play(id, volume, pitch, loop, playType);
            return controller;
        }

        public AudioAlbumController PlayFormatted(string formattedId, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var clipInfo = TryParseClipId(formattedId);

            if (clipInfo == null)
            {
                return null;
            }

            return Play(clipInfo.Value.album, clipInfo.Value.clip, volume, pitch, loop, playType);
        }

        public static (string album, string clip)? TryParseClipId(string formattedId)
        {
            if (string.IsNullOrEmpty(formattedId))
            {
                return null;
            }
            
            var split = formattedId.Split('/');
            
            if(split == null || split.Length < 2)
            {
                Debug.LogWarning($"Invalid formatted string {formattedId} in Audio Player. Make sure you separated album and clip id with \"\\\" symbol");
                return null;
            }
            
            return (split[0], split[1]);
        }

        public void Play(string source, AudioClip clip, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var controller = GetAlbumController(source);
            
            if (controller == null || clip == null) return;

            controller.Play(clip, volume, pitch, loop, playType);
        }

        public static void Play(AudioSource source, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            if (source == null || clip == null)
            {
                return;
            }

            source.loop = loop;
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;

            switch (playType)
            {
                case PlayType.STOP_THEN_PLAY:
                    source.Stop();
                    source.Play();
                    break;
                
                case PlayType.PLAY_DEFAULT:
                    source.Play();
                    break;

                case PlayType.ONE_SHOT:
                    source.PlayOneShot(clip, volume);
                    break;

                case PlayType.NO_INTERRUPT:
                    if (source.isPlaying)
                    {
                        return;
                    }

                    source.Stop();
                    source.Play();
                    break;

                default:
                    source.PlayOneShot(clip, volume);
                    break;
            }
        }

        public void StopAudio(string source)
        {
            var controller = GetAlbumController(source);

            controller?.Stop();
        }

        public void PauseAudio(string source)
        {
            var controller = GetAlbumController(source);

            controller?.Pause();
        }

        public void PauseAll()
        {
            _AlbumControllers.ForEach(c => c.Pause());
        }

        public void StopAll()
        {
            _AlbumControllers.ForEach(c => c.Stop());
        }

        public AudioAlbumController GetAlbumController(string albumName)
        {
            return _AlbumControllers.FirstOrDefault(a => a.AlbumName == albumName);
        }

        /// <summary>
        /// Adds album. Not recommended to use it in playmode
        /// </summary>
        public void AddAlbum(AudioAlbum album)
        {
            _Data.Albums.Add(album);
        }

        /// <summary>
        /// Adds album. Not recommended to use it in playmode
        /// </summary>
        public void AddAlbum(string albumName, AudioSource defaultSource, AudioMixerGroup mixerGroup = null, AudioSource source = null)
        {
            _Data.Albums.Add(new AudioAlbum()
            {
                albumName = albumName,
                mixerGroup = mixerGroup,
                source = source == null ? defaultSource : source
            });
        }

        public void AddClipToAlbum(string clipID, string albumName, AudioClip clip)
        {
            var controller = GetAlbumController(albumName);

            if (controller == null)
            {
                Debug.LogError($"There is no album named {albumName} to add {clipID}");
                return;
            }
            
            controller.AddClip(new AudioClipInfo()
            {
                clip = clip, id =  clipID
            });
        }

        /// <summary>
        /// Removes album. Not recommended to use it in playmode
        /// </summary>
        public bool TryRemoveAlbum(AudioAlbum album)
        {
            if(_Data.Albums.Contains(album))
            {
                _Data.Albums.Remove(album);
                return true;
            }

            return false;
        }
    }
    
    public enum PlayType
    {
        STOP_THEN_PLAY,
        ONE_SHOT,
        NO_INTERRUPT,
        PLAY_DEFAULT,
        NONE,
    }
    
    [Serializable]
    public class PlayAudioMessage : Message
    {
        public string albumName;
        public string clipID;
        public float volume = 1;
        public float pitch = 1;
        public bool loop = false;
        public PlayType playType = PlayType.ONE_SHOT;
    }

    [Serializable]
    public class StopAudioMessage : Message
    {
        public string albumName;
    }

    public class StopAllAudioMessage : Message { }
}