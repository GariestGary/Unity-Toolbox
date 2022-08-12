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
        Pooler.Instance.AddPool("Test pool", pooledGO, 3, false);

        GameObject sp = Pooler.Instance.Spawn("Test pool", Vector3.zero, Quaternion.identity);
        sp.name = "Pooler Test 1";
        Pooler.Instance.Despawn(sp);
        Pooler.Instance.Spawn("Test pool", Vector3.zero, Quaternion.identity);
        Pooler.Instance.Spawn("Test pool", Vector3.zero, Quaternion.identity);

        Assert.AreEqual("Pooler Test 1", Pooler.Instance.Spawn("Test pool", Vector3.zero, Quaternion.identity).name);

        yield return null;
    }
}
