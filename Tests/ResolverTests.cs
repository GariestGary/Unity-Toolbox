using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class ResolverTests
{
    // [UnityTest]
    // public IEnumerator ResolverInjectTest()
    // {
    //     Instance inst = new Instance();
    //
    //     Resolver.Instance.Run();
    //     Resolver.Instance.AddInstance(inst);
    //
    //     GameObject test = new GameObject("Resolver Test");
    //     Dependency dep = test.AddComponent<Dependency>();
    //
    //     test.Resolve();
    //
    //     Assert.AreEqual(inst, dep.Instance);
    //     
    //     yield return null;
    // }
    //
    // private class Instance
    // {
    //
    // }
    //
    // private class Dependency: MonoBehaviour
    // {
    //     [Inject] public Instance Instance;
    // }
}
