using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Tests
{
    public class TravelerTests
    {
        [UnityTest, PrebuildSetup(typeof(TestPrebuild))]
        public IEnumerator SceneManagementTest()
        {
            var test1 = "Test 1";
            var test2 = "Test 2";

            yield return Traveler.LoadScene(test1).ToCoroutine();

            Assert.AreEqual(true, Traveler.IsSceneOpened(test1));
            Assert.AreEqual(true, SceneManager.GetSceneByName(test1).isLoaded);


            yield return Traveler.UnloadScene(test1).ToCoroutine();

            Assert.AreEqual(false, SceneManager.GetSceneByName(test1).isLoaded);
            Assert.AreEqual(false, Traveler.IsSceneOpened(test1));

            var list = new List<UniTask>();

            list.Add(Traveler.LoadScene(test1));
            list.Add(Traveler.LoadScene(test2));
            list.Add(Traveler.UnloadScene(test1));
            list.Add(Traveler.UnloadScene(test2));

            yield return UniTask.WhenAll(list).ToCoroutine();

            Assert.AreEqual(true, SceneManager.GetSceneByName(test1).isLoaded);
            Assert.AreEqual(true, Traveler.IsSceneOpened(test1));
            Assert.AreEqual(true, SceneManager.GetSceneByName(test2).isLoaded);
            Assert.AreEqual(true, Traveler.IsSceneOpened(test2));

            yield return null;
        }
    }
}