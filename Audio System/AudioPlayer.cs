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
        
        public AudioSource DefaultAudioSource => defaultAudioSource;

        public void Initialize(Messenger msg)
        {
            msg.Subscribe<PlayAudioMessage>(x => Play(x.albumName, x.clipID, x.volume, x.pitch, x.loop, x.playType), null, true);
            msg.Subscribe<StopAudioMessage>(x => StopAudio(x.albumName), null, true);
            msg.Subscribe<StopAllAudioMessage>(_ => StopAll());

            _Data = ResourcesUtils.ResolveScriptable<AudioPlayerDataHolder>(SettingsData.audioPlayerResourcesDataPath);
            
            foreach (var album in _Data.Albums)
            {
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

        public void Play(string source, string id, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var album = GetAlbum(source);

            if(album == null)
            {
                Debug.LogWarning($"Album named '{source}' not found");
                return;
            }
            
            var clipInfo = GetClip(album.clips, id);

            if (clipInfo == null)
            {
                Debug.LogWarning($"Clip named '{id}' not found");
                return;
            }

            Play(source, clipInfo.clip, volume < 0 ? clipInfo.volume : volume, pitch, loop, playType);
        }

        public void PlayFormatted(string formattedId, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            if (string.IsNullOrEmpty(formattedId))
            {
                return;
            }
            
            var split = formattedId.Split('/');

            if(split == null || split.Length < 2)
            {
                Debug.LogWarning($"Invalid formatted string {formattedId} in Audio Player. Make sure you separated album and clip id with \"\\\" symbol");
                return;
            }

            Play(split[0], split[1], volume, pitch, loop, playType);
        }

        public void Play(string source, AudioClip clip, float volume = -1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var album = GetAlbum(source);
            
            if (album == null || clip == null) return;

            Play(album.source, clip, volume, pitch, loop, playType);
        }

        public void Play(AudioSource source, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
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

                case PlayType.ONE_SHOT:
                    source.PlayOneShot(clip, volume);
                    break;

                case PlayType.NO_INTERRUPT:
                    if (source.isPlaying)
                    {
                        return;
                    }
                    else
                    {
                        source.Stop();
                        source.Play();
                    }
                    break;

                default:
                    source.PlayOneShot(clip, volume);
                    break;
            }
        }

        public void StopAudio(string source)
        {
            var album = GetAlbum(source);

            album?.source.Stop();
        }

        public void PauseAudio(string source)
        {
            var album = GetAlbum(source);
            
            album?.source.Pause();
        }

        public void PauseAll()
        {
            _Data.Albums.ForEach(a => a.source.Pause());
        }

        public void StopAll()
        {
            _Data.Albums.ForEach(a => a.source.Stop());
        }

        private AudioAlbum GetAlbum(string albumName)
        {
            return _Data.Albums.All(x => x.albumName != albumName) ? null : _Data.Albums.FirstOrDefault(a => a.albumName == albumName);
        }

        private AudioClipInfo GetClip(List<AudioClipInfo> list, string id)
        {
            var clips = list.Where(x => x.id == id).ToArray();

            return clips.Length switch
            {
                0 => null,
                1 => clips[0],
                _ => clips[UnityEngine.Random.Range(0, clips.Length)]
            };
        }

        public void AddAlbum(AudioAlbum album)
        {
            _Data.Albums.Add(album);
        }

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
            var album = GetAlbum(albumName);

            if (album == null)
            {
                Debug.LogError($"There is no album named {albumName} to add {clipID}");
                return;
            }
            
            album.clips.Add(new AudioClipInfo()
            {
                clip = clip, id =  clipID
            });
        }

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
        NO_INTERRUPT
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