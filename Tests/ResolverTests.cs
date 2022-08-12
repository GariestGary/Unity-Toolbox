using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class ResolverTests
{
    [UnityTest]
    public IEnumerator ResolverInjectTest()
    {
        Instance inst = new Instance();

        Resolver.Instance.Run();
        Resolver.Instance.AddInstance(inst);

        GameObject test = new GameObject("Resolver Test");
        Dependency dep = test.AddComponent<Dependency>();

        Resolver.Instance.Inject(test);

        Assert.AreEqual(inst, dep.Check);
        
        yield return null;
    }

    private class Instance
    {

    }

    private class Dependency: MonoBehaviour
    {
        [Inject] private Instance instance; 

        public Instance Check => instance;
    }
}
