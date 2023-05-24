using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class AudioPlayer: ResourcesToolWrapper<AudioPlayer, AudioPlayerDataHolder>
    {
        [SerializeField] private Transform audioSourcesRoot;

        public override string GetDataPath()
        {
            return SettingsData.audioPlayerResourcesDataPath;
        }

        protected override void PostLoadRun()
        {
            Data.Run();

            foreach (var album in Data.Albums)
            {
                var obj = new GameObject($"{album.albumName} Audio Source", typeof(AudioSource));
                obj.transform.SetParent(audioSourcesRoot);
                album.source = obj.GetComponent<AudioSource>();
            }
        }

        protected override void Clear()
        {
            Data.Clear();
        }

        public static void Play(string source, string id, float volume = 1, float pitch = 1, bool loop = false, bool oneShot = false)
        {
            Instance.Data.Play(source, id, volume, pitch, loop, oneShot);
        }

        public static void Play(string source, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, bool oneShot = false)
        {
            Instance.Data.Play(source, clip, volume, pitch, loop, oneShot);
        }

        public static void Play(AudioSource source, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, bool oneShot = false)
        {
            Instance.Data.Play(source, clip, volume, pitch, loop, oneShot);
        }

        public void StopAudio(string source)
        {
            Instance.Data.StopAudio(source);
        }
    }
}