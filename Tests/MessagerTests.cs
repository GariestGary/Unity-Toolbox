using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class MessagerTests
{
    private string message;

    [UnityTest]
    public IEnumerator MessagerReactTest()
    {
        message = "null";
            
        Messenger.Subscribe<MockMessage>(x => React(x.message));
        Messenger.Send<MockMessage>();
        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MessagerSceneHandleTest()
    {
        var obj = new GameObject("A");

        Subscriber subToRemove = Messenger.Subscribe<MockMessage>(m => React(m.message), obj);
        Subscriber subToStay = Messenger.Subscribe<MockMessage>(m => React(m.message));

        Messenger.Send(new SceneUnloadedMessage(obj.scene.name));

        Assert.AreEqual(true, Messenger.Subscribers.Any(x => x == subToStay));
        Assert.AreEqual(false, Messenger.Subscribers.Any(x => x == subToRemove));

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
