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

        Messager.Instance.Subscribe(Message.MOCK, _ => React());
        Messager.Instance.Send(Message.MOCK);

        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    private void React()
    {
        message = "Reacted";
    }
}
