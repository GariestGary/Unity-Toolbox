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
    [UnityTest]
    public IEnumerator PoolerSpawnObjectTest()
    {
        GameObject pooledGO = new GameObject("Pooler Test");
        Pooler.TryAddPool("Test pool", pooledGO, 3);

        GameObject sp = Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
        sp.name = "Pooler Test 1";
        Pooler.DespawnOrDestroy(sp);
        Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
        Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);

        Assert.AreEqual
        (
            true,
            Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity).name.Contains("Pooler Test 1")
        );

        yield return null;
    }
}
