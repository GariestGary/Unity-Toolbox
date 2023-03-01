using System;
using System.Collections;
using System.Collections.Generic;
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
