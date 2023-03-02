using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class MonoCachedTests
{
    [UnityTest]
    public IEnumerator MonoCachedActiveStateProcessTest()
    {
        var parent = new GameObject("Parent", typeof(MonoCachedComponent)).GetComponent<MonoCachedComponent>();
        var child = new GameObject("Child", typeof(MonoCachedComponent)).GetComponent<MonoCachedComponent>();

        child.transform.SetParent(parent.transform);

        //check default, not process if inactive, not process if inactive in hierarchy
        parent.DisableGameObject();
        Assert.AreEqual(true, parent.Paused);
        Assert.AreEqual(true, child.Paused);
        parent.EnableGameObject();

        //check if process if inactive
        parent.ProcessIfInactiveSelf = true;
        child.ProcessIfInactiveSelf = true;
        parent.DisableGameObject();
        Assert.AreEqual(true, child.Paused);
        Assert.AreEqual(false, parent.Paused);
        parent.EnableGameObject();
        child.ProcessIfInactiveInHierarchy = true;
        parent.DisableGameObject();
        Assert.AreEqual(false, child.Paused);
        parent.EnableGameObject();
        parent.ProcessIfInactiveSelf = false;
        child.ProcessIfInactiveSelf = false;
        child.ProcessIfInactiveInHierarchy = false;

        //check if process if inactive in hierarchy
        child.ProcessIfInactiveInHierarchy = true;
        parent.DisableGameObject();
        Assert.AreEqual(true, parent.Paused);
        Assert.AreEqual(false, child.Paused);
        parent.ProcessIfInactiveInHierarchy = false;
        child.ProcessIfInactiveInHierarchy = false;

        yield return null;
    }
}
