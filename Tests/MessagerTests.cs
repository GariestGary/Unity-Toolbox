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

        Messager.Instance.Subscribe(Message.HIT_PLAYER, _ => React());
        Messager.Instance.Send(Message.HIT_PLAYER);

        Assert.AreEqual("Reacted", message);

        yield return null;
    }

    private void React()
    {
        message = "Reacted";
    }
}
