using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class UpdaterTests
{
    [UnityTest]
    public IEnumerator TimeScaleTest()
    {
        var testGO = new GameObject("Updater Test");
        var foo = testGO.AddComponent<Foo>();
        Updater.Instance.InitializeObject(testGO);

        Updater.Instance.TimeScale = 0.5f;

        yield return null;

        Assert.AreEqual(Updater.Instance.Delta, foo.Delta);
    }

    [UnityTest]
    public IEnumerator TimeIntervalTest()
    {
        Resolver.Instance.Run();
        var testGO = new GameObject("Updater Test");
        var foo = testGO.AddComponent<Foo>();
        Updater.Instance.Run();
        Updater.Instance.InitializeObject(testGO);

        foo.Interval = 1;

        yield return null;

        Assert.AreEqual(true, foo.counter < 1);

        yield return new WaitForSeconds(1);

        Assert.AreEqual(true, foo.counter > 1);
    }

    private class Foo: MonoCached
    {
        public float Delta => delta;
        public float counter = 0;

        public override void Tick()
        {
            counter += delta;
        }
    }
}
