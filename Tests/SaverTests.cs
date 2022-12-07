using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VolumeBox.Toolbox;

public class SaverTests
{
    [UnityTest]
    public IEnumerator WindowsSaveSlotTest()
    {
        Saver.Instance.useSaves = true;

        Saver.Instance.SetFileHandler((WindowsFileHandler)ScriptableObject.CreateInstance(typeof(WindowsFileHandler)));
        Saver.Instance.SetStateProvider((MockStateProvider)ScriptableObject.CreateInstance(typeof(MockStateProvider)));
        Saver.Instance.Run();
        Saver.Instance.SelectSlot(0);
        Saver.Instance.LoadCurrentSlot();
        (Saver.Instance.StateProvider as MockStateProvider).data = "Test";
        Saver.Instance.Save();
        Saver.Instance.LoadCurrentSlot();
        Assert.AreEqual("Test", Saver.Instance.CurrentSlot.state);

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
