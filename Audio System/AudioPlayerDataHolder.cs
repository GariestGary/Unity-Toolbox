using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
    public class AudioPlayerDataHolder : ScriptableObject, IRunner, IClear
    {
        [SerializeField] private List<AudioAlbum> albums;

        public List<AudioAlbum> Albums => albums;

        [Inject] private Messenger msg;

        public void Run()
        {
            Messenger.SubscribeKeeping<PlayAudioMessage>(x => Play(x.albumName, x.clipID, x.volume, x.pitch, x.loop, x.playType));
            Messenger.SubscribeKeeping<StopAudioMessage>(x => StopAudio(x.albumName));
        }

        public void Play(string source, string id, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            var album = GetAlbum(source);

            var clip = GetClip(album.clips, id);

            if (clip == null) return;

            Play(source, clip, volume, pitch, loop, playType);
        }

        public void Play(string source, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
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

            switch (playType)
            {
                case PlayType.STOP_THEN_PLAY:
                    source.Stop();
                    source.loop = loop;
                    source.clip = clip;
                    source.volume = volume;
                    source.pitch = pitch;
                    source.Play();
                    break;
                case PlayType.ONE_SHOT:
                    source.PlayOneShot(clip, volume);
                    break;
                case PlayType.NO_INTERRUPT:
                    if (source.isPlaying) return;
                    else
                    {
                        source.Stop();
                        source.loop = loop;
                        source.clip = clip;
                        source.volume = volume;
                        source.pitch = pitch;
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

        private AudioAlbum GetAlbum(string albumName)
        {
            return albums.All(x => x.albumName != albumName) ? null : albums.FirstOrDefault(a => a.albumName == albumName);
        }

        private AudioClip GetClip(IEnumerable<AudioClipInfo> list, string id)
        {
            var clips = list.Where(x => x.id == id).ToArray();

            return clips.Length switch
            {
                0 => null,
                1 => clips[0].clip,
                _ => clips[UnityEngine.Random.Range(0, clips.Length)].clip
            };
        }

        public void Clear()
        {
            albums.ForEach(a => a.source = null);
        }
    }

    public enum PlayType
    {
        STOP_THEN_PLAY,
        ONE_SHOT,
        NO_INTERRUPT
    }

    [Serializable]
    public class AudioAlbum
    {
        [SerializeField]
        public string albumName;
        [HideInInspector, SerializeField] public AudioSource source;
        [SerializeField] public List<AudioClipInfo> clips;
    }
    
    [Serializable]
    public class AudioClipInfo
    {
        public string id;
        public AudioClip clip;
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
}