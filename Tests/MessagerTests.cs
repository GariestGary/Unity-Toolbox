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

        Messager.Instance.SubscribeKeeping<MockMessage>(x => React(x.message));
        Messager.Instance.Send<MockMessage>();
        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MessagerSceneHandleTest()
    {
        Subscriber subToRemove = Messager.Instance.Subscribe<MockMessage>(m => React(m.message), "Scene to Unload");
        Subscriber subToStay = Messager.Instance.Subscribe<MockMessage>(m => React(m.message), "Scene to Stay");

        Messager.Instance.Send(new SceneUnloadedMessage("Scene to Unload"));

        Assert.AreEqual(true, Messager.Instance.SceneSubscribers.Any(x => x == subToStay));
        Assert.AreEqual(false, !Messager.Instance.SceneSubscribers.Any(x => x == subToRemove));

        yield return null;
    }

    private void React(string test)
    {
        message = test;
    }

    [Serializable]
    public class MockMessage : Message
    {
        public string message = "Reacted";
    }
}
