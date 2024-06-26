using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    public class TestPrebuild: IPrebuildSetup
    {
        public void Setup()
        {
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
