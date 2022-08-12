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
        Saver.Instance.SetFileHandler((WindowsFileHandler)ScriptableObject.CreateInstance(typeof(WindowsFileHandler)));
        Saver.Instance.SetStateProvider((MockStateProvider)ScriptableObject.CreateInstance(typeof(MockStateProvider)));
        Saver.Instance.SaveSlotsCount = 3;
        Saver.Instance.Run();

        for (var i = 0; i < 3; i++)
        {
            Saver.Instance.LoadCurrentSlot();
            (Saver.Instance.StateProvider as MockStateProvider).data = "Test " + i;
            Saver.Instance.Save();
            Saver.Instance.LoadCurrentSlot();
            Assert.AreEqual("Test " + i, Saver.Instance.CurrentSlot.state);
        }
        
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
