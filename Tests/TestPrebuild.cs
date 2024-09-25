using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    public class TestPrebuild: IPrebuildSetup, IPostBuildCleanup
    {
        private bool _PreviousAutoplayResolve;

        public void Cleanup()
        {
            StaticData.Settings.AutoResolveScenesAtPlay = _PreviousAutoplayResolve;
        }

        public void Setup()
        {
            _PreviousAutoplayResolve = StaticData.Settings.AutoResolveScenesAtPlay;
            StaticData.Settings.AutoResolveScenesAtPlay = false;
            var msg = Messenger.Instance;
            var pool = Pooler.Instance;
            var trvl = Traveler.Instance;
            var upd = Updater.Instance;
            upd.RunInternal();
            msg.RunInternal();
            pool.RunInternal();
            trvl.RunInternal();
        }
    }
}
