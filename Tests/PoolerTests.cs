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
            GameObject pooledGO = new GameObject("Pooler Test");
            Toolbox.Pooler.TryAddPool("Test pool", pooledGO, 3);

            var test = Toolbox.Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Toolbox.Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Toolbox.Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            Toolbox.Pooler.DespawnOrDestroy(test);

            var obj = Toolbox.Pooler.Spawn("Test pool", Vector3.zero, Quaternion.identity);
            
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
            GameObject pooledGO = new GameObject("Pooler Test Nest");
            Toolbox.Pooler.TryAddPool("Test pool Nest", pooledGO, 3);

            var nest1 = Toolbox.Pooler.Spawn("Test pool Nest");
            var nest2 = Toolbox.Pooler.Spawn("Test pool Nest", null, nest1.transform);
            var nest3 = Toolbox.Pooler.Spawn("Test pool Nest", null, nest2.transform);


            Assert.AreEqual(nest2.transform, nest3.transform.parent);
            Assert.AreEqual(nest1.transform, nest2.transform.parent);
            Toolbox.Pooler.DespawnOrDestroy(nest1);
            Assert.AreEqual(3, Toolbox.Pooler.GetPoolObjectsCount("Test pool Nest"));

            yield return null;
        }

        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator PoolerGCTest()
        {
            GameObject test = new GameObject();
            Toolbox.Pooler.TryAddPool("Test pool GC", test, 5);

            Assert.AreEqual(5, Toolbox.Pooler.GetPoolObjectsCount("Test pool GC"));

            var objList = new List<GameObject>();

            for (int i = 0; i < 10; i++)
            {
                objList.Add(Toolbox.Pooler.Spawn("Test pool GC", Vector3.zero, Quaternion.identity));
            }

            foreach (var obj in objList)
            {
                Toolbox.Pooler.DespawnOrDestroy(obj);
            }    

            Assert.AreEqual(10, Toolbox.Pooler.GetPoolObjectsCount("Test pool GC"));

            Toolbox.Pooler.ForceGarbageCollector();

            Assert.AreEqual(5, Toolbox.Pooler.GetPoolObjectsCount("Test pool GC"));

            yield return null;
        }
    }
}
