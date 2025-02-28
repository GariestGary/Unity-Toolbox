using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class AudioAlbumController
    {
        public AudioAlbum Album { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool IsStopped { get; private set; }
        public PlayType CurrentPlayType { get; private set; }
        public float CurrentPlayTime { get; private set; }
        public AudioClip CurrentPlayingClip { get; private set; }

        public event Action StartedPlaying;
        public event Action StoppedPlaying;
        public event Action Paused;
        public event Action EndedPlaying;


        private float _CurrentClipDuration;
        private Coroutine _PlaybackCoroutine;

        public AudioAlbumController(AudioAlbum album)
        {
            Album = album;
            IsPaused = false;
            IsPlaying = false;
            IsStopped = true;
            CurrentPlayTime = -1;
            CurrentPlayType = PlayType.NONE;
        }

        public AudioClipInfo GetClip(string clipId)
        {
            return Album.clips.FirstOrDefault(c => c.id == clipId);
        }

        public void Play(string clipId, float volume, float pitch, bool loop, PlayType playType)
        {
            var clipInfo = GetClip(clipId);

            if (clipInfo == null)
            {
                Debug.LogWarning($"Clip with ID '{clipId}' not found");
                return;
            }

            Play(clipInfo.clip, volume, pitch, loop, playType);
        }
        
        public void Play(AudioClip clip, float volume, float pitch, bool loop, PlayType playType)
        {
            AudioPlayer.Play(Album.source, clip, volume, pitch, loop, playType);
            IsPlaying = true;
            IsStopped = false;
            IsPaused = false;
            CurrentPlayTime = clip.length;
            CurrentPlayType = playType;
            CurrentPlayingClip = clip;
            CoroutineStarter.Invoke(nameof(HandlePlayback));
        }

        public void Stop()
        {
            Album.source.Stop();
            IsStopped = true;
            IsPlaying = false;
            IsPaused = false;
            CoroutineStarter.Stop(nameof(HandlePlayback));
            StoppedPlaying?.Invoke();
        }

        public void Pause()
        {
            Album.source.Pause();
            IsPaused = true;
            IsPlaying = false;
            IsStopped = false;
            Paused?.Invoke();
        }

        private IEnumerator HandlePlayback()
        {
            if (CurrentPlayingClip == null)
            {
                yield break;
            }
            
            while (CurrentPlayTime < _CurrentClipDuration)
            {
                if (CurrentPlayingClip == null)
                {
                    break;
                }
                
                if (IsPlaying)
                {
                    CurrentPlayTime += Time.unscaledDeltaTime;
                }
                
                yield return null;
            }
            
            HandleClipEnd();
        }

        private void HandleClipEnd()
        {
            if (CurrentPlayingClip != null)
            {
                EndedPlaying?.Invoke();
            }
        }

        public void AddClip(AudioClipInfo audioClipInfo)
        {
            Album.clips.Add(audioClipInfo);
        }
    }
}
