using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    public class UpdaterTests
    {
        [UnityTest]
        public IEnumerator TimeScaleTest()
        {
            var testGO = new GameObject("Updater Test");
            var foo = testGO.AddComponent<Foo>();
            Updater.InitializeObject(testGO);

            Updater.TimeScale = 0.5f;

            yield return null;

            Assert.AreEqual(Updater.Delta, foo.Delta);
        }

        [UnityTest]
        public IEnumerator TimeIntervalTest()
        {
            var testGO = new GameObject("Updater Test");
            var foo = testGO.AddComponent<Foo>();
            Updater.InitializeObject(testGO);

            foo.Interval = 1;

            yield return null;

            Assert.AreEqual(true, foo.counter < 1);

            yield return new WaitForSeconds(1);

            Assert.AreEqual(true, foo.counter > 1);
        }

        [UnityTest]
        public IEnumerator IgnoreTimeScaleTest()
        {
            var testGO = new GameObject("Updater Test");
            var foo = testGO.AddComponent<Foo>();
            Updater.InitializeObject(testGO);

            foo.IgnoreTimeScale = true;
            yield return null;
            Updater.TimeScale = 0;
            yield return null;
            Assert.AreEqual(true, foo.Delta > 0);
            foo.IgnoreTimeScale = false;
            yield return null;
            Assert.AreEqual(true, foo.Delta == 0);
        }

        private class Foo: MonoCached
        {
            public float Delta => delta;
            public float counter = 0;

            protected override void Tick()
            {
                counter += delta;
            }
        }
    }
}
