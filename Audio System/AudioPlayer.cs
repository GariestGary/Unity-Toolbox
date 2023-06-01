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

            var obj = new GameObject("Default Audio Source");
            obj.transform.SetParent(audioSourcesRoot);
            var defaultSource = obj.AddComponent<AudioSource>();

            foreach (var album in Data.Albums)
            {
                if(album.useSeparateSource)
                {
                    var newSourceObj = new GameObject($"{album.albumName} Audio Source");
                    newSourceObj.transform.SetParent(audioSourcesRoot);
                    album.source = newSourceObj.AddComponent<AudioSource>();
                }
                else
                {
                    album.source = defaultSource;
                }
            }
        }

        protected override void Clear()
        {
            Data.Clear();
        }

        public static void Play(string source, string id, float volume = 1, float pitch = 1, bool loop = false, PlayType playType = PlayType.ONE_SHOT)
        {
            Instance.Data.Play(source, id, volume, pitch, loop, playType);
        }

        public void StopAudio(string source)
        {
            Instance.Data.StopAudio(source);
        }
    }
}