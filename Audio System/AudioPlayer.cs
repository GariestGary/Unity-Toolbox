using UnityEngine;
using UnityEngine.Audio;

namespace VolumeBox.Toolbox
{
    public class AudioPlayer: ResourcesToolWrapper<AudioPlayer, AudioPlayerDataHolder>
    {
        [SerializeField] private Transform audioSourcesRoot;
        [SerializeField] private AudioSource defaultAudioSource;

        public override string GetDataPath()
        {
            return SettingsData.audioPlayerResourcesDataPath;
        }

        protected override void PostLoadRun()
        {
            Data.Run();

            foreach (var album in Data.Albums)
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

        protected override void Clear()
        {
            Data?.Clear();
        }

        public static void Play(string source, string id, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            Instance.Data.Play(source, id, volume, pitch, loop, playType);
        }

        public static void Play(string formattedId, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            Instance.Data.PlayFormatted(formattedId, volume, pitch, loop, playType);
        }

        public void StopAudio(string source)
        {
            Instance.Data.StopAudio(source);
        }

        public static void StopAll()
        {
            Instance.Data.StopAll();
        }
        
        public void AddAlbum(string albumName, AudioMixerGroup mixerGroup = null, AudioSource source = null)
        {
            Instance.Data.AddAlbum(albumName, defaultAudioSource, mixerGroup, source);
        }

        public void AddClipToAlbum(string clipID, string albumName, AudioClip clip)
        {
            Instance.Data.AddClipToAlbum(clipID, albumName, clip);
        }
    }
}