using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class MessengerTest
{
    private string message;

    [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
    public IEnumerator MessengerReactTest()
    {
        message = "null";

        Messenger.ClearSubscribers();
        
        Messenger.Subscribe<MockMessage>(x => React(x.message));
        Messenger.Send<MockMessage>();
        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
    public IEnumerator MessengerSceneHandleTest()
    {
        Messenger.ClearSubscribers();

        var obj = new GameObject("A");

        Subscriber subToRemove = Messenger.Subscribe<MockMessage>(m => React(m.message), obj);

        message = "null";

        Messenger.Send(new SceneUnloadedMessage(obj.scene.name));
        Messenger.Send<MockMessage>();

        Assert.AreEqual("null", message);

        Messenger.ClearSubscribers();

        message = "null";

        Subscriber subToStay = Messenger.Subscribe<MockMessage>(m => React(m.message));

        Messenger.Send(new SceneUnloadedMessage(obj.scene.name));
        Messenger.Send<MockMessage>();

        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
    public IEnumerator MessengerObjectBindTest()
    {
        Messenger.Instance.RunInternal();
        Pooler.Instance.RunInternal();

        Messenger.ClearSubscribers();

        var obj = new GameObject("A");

        message = "null";

        Messenger.Subscribe<MockMessage>(m => React(m.message), obj);

        Pooler.DespawnOrDestroy(obj);

        Messenger.Send<MockMessage>();
        Assert.AreEqual("null", message);
        yield return null;

        yield return null;
    }

    private void React(string test)
    {
        message = test;
    }

    [Serializable]
    public class MockMessage: Message
    {
        public string message = "Reacted";
    }
}
