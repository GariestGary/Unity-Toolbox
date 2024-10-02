using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    public class UpdaterTests
    {
        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator TimeScaleTest()
        {
            var testGO = new GameObject("Timescale Test");
            var foo = testGO.AddComponent<Foo>();
            Updater.InitializeObject(testGO);

            Updater.TimeScale = 0.5f;

            yield return null;

            Assert.AreEqual(Updater.Delta, foo.Delta);
        }

        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator TimeIntervalTest()
        {
            var testGO = new GameObject("Time Interval Test");
            var foo = testGO.AddComponent<Foo>();
            Updater.InitializeObject(testGO);
            Updater.TimeScale = 1;

            foo.Interval = 1;

            yield return null;

            Assert.AreEqual(true, foo.counter < 1);

            yield return new WaitForSeconds(1);
            Debug.Log(foo.counter);
            Assert.AreEqual(true, foo.counter >= 1);
        }

        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator IgnoreTimeScaleTest()
        {
            var testGO = new GameObject("Ignore Timescale Test");
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
    }
}
