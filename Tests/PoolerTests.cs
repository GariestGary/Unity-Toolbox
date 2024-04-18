using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    public class PoolerTests
    {
        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator PoolerSpawnObjectTest()
        {
            Pooler.Instance.RunInternal();
            GameObject pooledGO = new GameObject("Pooler Test");
            Pooler.TryAddPool("Test pool", pooledGO, 3);

            var test = Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Pooler.DespawnOrDestroy(test);

            var obj = Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            
            Assert.AreEqual
            (
                true,
                test == obj
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
}
