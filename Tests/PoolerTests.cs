using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class PoolerTests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
    public IEnumerator PoolerSpawnObjectTest()
    {
        Pooler.Instance.RunInternal();
        GameObject pooledGO = new GameObject("Pooler Test");
        Pooler.TryAddPool("Test pool", pooledGO, 3);

        GameObject sp = Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
        sp.name = "Pooler Test 1";
        Pooler.DespawnOrDestroy(sp);
        Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity).name = "Pooler Test 2";
        Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity).name = "Pooler Test 3";

        Assert.AreEqual
        (
            true,
            Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity).name.Contains("Pooler Test")
        );

        yield return null;
    }

    [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
    public IEnumerator PoolerGCTest()
    {
        GameObject test = new GameObject();
        Pooler.Instance.RunInternal();
        Pooler.TryAddPool("Test pool", test, 5);

        Assert.AreEqual(5, Pooler.GetPoolObjectsCount("Test pool"));

        var objList = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            objList.Add(Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity));
        }

        foreach (var obj in objList)
        {
            Pooler.DespawnOrDestroy(obj);
        }    

        Assert.AreEqual(10, Pooler.GetPoolObjectsCount("Test pool"));

        Pooler.ForceGarbageCollectorWork();

        Assert.AreEqual(5, Pooler.GetPoolObjectsCount("Test pool"));

        yield return null;
    }
}
