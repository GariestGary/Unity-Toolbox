using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class TestPrebuild: IPrebuildSetup
{
    public void Setup()
    {
        var msg = Messenger.Instance;
        var pool = Pooler.Instance;
        var trvl = Traveler.Instance;
        msg.RunInternal();
        pool.RunInternal();
        trvl.RunInternal();
    }
}
