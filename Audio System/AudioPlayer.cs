using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
    public class AudioPlayer : Singleton<AudioPlayer>, IRunner
    {
        [SerializeField] private AudioSource bgMusicSource;
        [SerializeField] private AudioSource uiSource;
        [SerializeField] private AudioSource atmosphereSource;
        [SerializeField] private AudioSource soundsSource;
        [Header("Audio Clips")]
        [AllowNesting]
        [SerializeField] private List<AudioClipInfo> music;
        [SerializeField] private List<AudioClipInfo> ui;
        [SerializeField] private List<AudioClipInfo> atmos;
        [SerializeField] private List<AudioClipInfo> sounds;

        [Inject] private Messager msg;

        public const float DefaultVolume = 1;

        public void Run()
        {
            msg.SubscribeKeeping<PlayMusicMessage>(x => PlayMusic(x.id, x.volume, x.loop));
            msg.SubscribeKeeping<PlayUIMessage>(x => PlayUI(x.id, x.volume, x.loop));
            msg.SubscribeKeeping<PlayAtmosMessage>(x => PlayAtmos(x.id, x.volume, x.loop));
            msg.SubscribeKeeping<PlaySoundMessage>(x => PlaySound(x.id, x.volume, x.loop));
            msg.SubscribeKeeping<StopMusicMessage>(_ => StopMusic());
        }

        public void PlaySound(string id, float volume = DefaultVolume, bool loop = false, float pitch = 1)
        {
            AudioClip clip = GetClip(sounds, id);

            PlaySound(clip, volume, loop, pitch);
        }

        public void PlaySound(AudioClip clip, float volume = DefaultVolume, bool loop = false, float pitch = 1)
        {
            PlayOneShot(soundsSource, clip, volume, pitch);
        }

        //ATMOSPHERE
        public void PlayAtmos(string id, float volume = DefaultVolume, bool loop = true)
        {
            AudioClip clip = GetClip(atmos, id);

            PlayAtmos(clip, volume, loop);
        }

        public void PlayAtmos(AudioClip clip, float volume = DefaultVolume, bool loop = true)
        {
            Play(atmosphereSource, clip, volume, loop);
        }

        //UI
        public void PlayUI(string id, float volume = DefaultVolume, bool loop = true)
        {
            AudioClip clip = GetClip(ui, id);
            PlayUI(clip, volume, loop);
        }

        public void PlayUI(AudioClip clip, float volume = DefaultVolume, bool loop = true)
        {
            PlayOneShot(uiSource, clip, volume);
        }

        //MUSIC
        public void PlayMusic(string id, float volume = DefaultVolume, bool loop = true)
        {
            AudioClip clip = GetClip(music, id);

            PlayMusic(clip, volume, loop);
        }

        public void PlayMusic(AudioClip clip, float volume = DefaultVolume, bool loop = true)
        {
            Play(bgMusicSource, clip, volume, loop);
        }

        public void StopMusic()
        {
            bgMusicSource.Pause();
        }

        public void PlaySoundOneShot(AudioClip clip, float volume = DefaultVolume, float pitch = 1)
        {
            PlayOneShot(soundsSource, clip, volume, pitch);
        }

        public void PlaySoundOneShot(string id, float volume = DefaultVolume, float pitch = 1)
        {
            PlayOneShot(soundsSource, GetClip(sounds, id), volume, pitch);
        }
        

        public void Play(AudioSource source, AudioClip clip, float volume = DefaultVolume, bool loop = false)
        {
            if (source == null || clip == null) return;

            source.Stop();

            source.loop = loop;
            source.clip = clip;
            source.volume = volume;

            source.Play();
        }

        public void PlayOneShot(AudioSource source, AudioClip clip, float volume = DefaultVolume, float pitch = 1)
        {
            if (source == null || clip == null) return;
            source.pitch = pitch;
            source.PlayOneShot(clip, volume);
        }

        
        private AudioClip GetClip(List<AudioClipInfo> list, string id)
        {
            AudioClipInfo[] clips = list.Where(x => x.id == id).ToArray();

            if(clips.Length == 0)
            {
                return null;
            }

            if(clips.Length == 1) 
            {
                return clips[0].clip;
            }

            return clips[UnityEngine.Random.Range(0, clips.Length)].clip;
        }

        public void SetOnOffMusic(bool active)
        {
            bgMusicSource.enabled = active;
        }
    }

    [Serializable]
    public class AudioClipInfo
    {
        public Sprite s;
        public AudioClip clip;
        public string id;
    }

    [Serializable]
    public class PlayMusicMessage: Message
    {
        public string id;
        public float volume = AudioPlayer.DefaultVolume;
        public bool loop = true;
    }

    [Serializable]
    public class PlayUIMessage: Message
    {
        public string id;
        public float volume = AudioPlayer.DefaultVolume;
        public bool loop = false;
    }

    [Serializable]
    public class PlayAtmosMessage: Message
    {
        public string id;
        public float volume = AudioPlayer.DefaultVolume;
        public bool loop = true;
    }

    [Serializable]
    public class PlaySoundMessage: Message
    {
        public string id;
        public float volume = AudioPlayer.DefaultVolume;
        public bool loop = false;
    }

    [Serializable]
    public class StopMusicMessage : Message
    {
        
    }
}