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
        public IEnumerator NestedSpawnObjectsTest()
        {
            Pooler.Instance.RunInternal();
            GameObject pooledGO = new GameObject("Pooler Test Nest");
            Pooler.TryAddPool("Test pool Nest", pooledGO, 3);

            var nest1 = Pooler.Spawn("Test pool Nest");
            var nest2 = Pooler.Spawn("Test pool Nest", null, nest1.transform);
            var nest3 = Pooler.Spawn("Test pool Nest", null, nest2.transform);


            Assert.AreEqual(nest2.transform, nest3.transform.parent);
            Assert.AreEqual(nest1.transform, nest2.transform.parent);
            Pooler.DespawnOrDestroy(nest1);
            Assert.AreEqual(3, Pooler.GetPoolObjectsCount("Test pool Nest"));

            yield return null;
        }

            [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator PoolerGCTest()
        {
            GameObject test = new GameObject();
            Pooler.Instance.RunInternal();
            Pooler.TryAddPool("Test pool GC", test, 5);

            Assert.AreEqual(5, Pooler.GetPoolObjectsCount("Test pool GC"));

            var objList = new List<GameObject>();

            for (int i = 0; i < 10; i++)
            {
                objList.Add(Pooler.Spawn("Test pool GC", Vector3.zero, Quaternion.identity));
            }

            foreach (var obj in objList)
            {
                Pooler.DespawnOrDestroy(obj);
            }    

            Assert.AreEqual(10, Pooler.GetPoolObjectsCount("Test pool GC"));

            Pooler.ForceGarbageCollectorWork();

            Assert.AreEqual(5, Pooler.GetPoolObjectsCount("Test pool GC"));

            yield return null;
        }
    }
}
