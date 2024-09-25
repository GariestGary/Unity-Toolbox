using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VolumeBox.Toolbox
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceExtended: MonoCached
    {
        private List<Action> _EndActions = new();
        private AudioSource _AudioSource;
        private bool _IsPlaying;
        private bool _IsPaused;

        public AudioSource Source => _AudioSource;

        public UnityEvent StartedPlaying;
        public UnityEvent PausedPlaying;
        public UnityEvent ResumedPlaying;
        public UnityEvent StoppedPlaying;

        public AudioClip clip
        {
            get
            {
                ValidateSource();
                return _AudioSource.clip;
            }
            set
            {
                ValidateSource();
                _AudioSource.clip = value;
            }
        }

        public float pitch
        {
            get
            {
                ValidateSource();
                return _AudioSource.pitch;
            }
            set
            {
                ValidateSource();
                _AudioSource.pitch = value;
            }
        }

        public float volume
        {
            get
            {
                ValidateSource();
                return _AudioSource.volume;
            }
            set
            {
                ValidateSource();
                _AudioSource.volume = value;
            }
        }

        private void ValidateSource()
        {
            if(_AudioSource == null)
            {
                _AudioSource = GetComponent<AudioSource>();
            }
        }
        
        public void Play()
        {
            StartedPlaying.Invoke();

            if(_AudioSource.clip != null)
            {
                _IsPlaying = true;
                _IsPaused = false;
                _AudioSource.Play();
            }
            else
            {
                StoppedPlaying.Invoke();
            }

        }
    }
}
