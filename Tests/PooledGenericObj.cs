using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Tests
{
    internal class PooledGenericObj : MonoCached, IPooled<TestData>
    {
        public string compare = "null";

        public void OnSpawn(TestData data)
        {
            compare = data.TestString;
        }
    }

    internal class TestData
    {
        public string TestString;
    }
}
