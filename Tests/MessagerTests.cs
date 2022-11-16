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
        Messager.Instance.Send<MockMessage>();// (new MockMessage() { message = "FFFFFFFF"});

        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    private void React(string test)
    {
        message = test;
    }

    public class MockMessage 
    {
        public string message = "Reacted";
    }
}
