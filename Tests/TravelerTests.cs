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
            var test1 = "Test1";
            var test2 = "Test2";
            
            yield return Toolbox.Traveler.LoadScene(test1).ToCoroutine();
            
            Assert.AreEqual(true, Toolbox.Traveler.IsSceneOpened(test1));
            Assert.AreEqual(true, SceneManager.GetSceneByName(test1).isLoaded);


            yield return Toolbox.Traveler.UnloadScene(test1).ToCoroutine();

            Assert.AreEqual(false, SceneManager.GetSceneByName(test1).isLoaded);
            Assert.AreEqual(false, Toolbox.Traveler.IsSceneOpened(test1));

            var list = new List<UniTask>
            {
                Toolbox.Traveler.LoadScene(test1),
                Toolbox.Traveler.LoadScene(test2),
                Toolbox.Traveler.UnloadScene(test1),
                Toolbox.Traveler.UnloadScene(test2)
            };

            yield return UniTask.WhenAll(list).ToCoroutine();

            Assert.AreEqual(true, SceneManager.GetSceneByName(test1).isLoaded);
            Assert.AreEqual(true, Toolbox.Traveler.IsSceneOpened(test1));
            Assert.AreEqual(true, SceneManager.GetSceneByName(test2).isLoaded);
            Assert.AreEqual(true, Toolbox.Traveler.IsSceneOpened(test2));

            yield return null;
        }
    }
}