using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace VolumeBox.Toolbox.Tests
{
    internal class TestPrebuild: IPrebuildSetup, IPostBuildCleanup
    {
        private bool _PreviousAutoplayResolve;

        public void Cleanup()
        {
            StaticData.Settings.AutoResolveScenesAtPlay = _PreviousAutoplayResolve;
        }

        public void Setup()
        {
            _PreviousAutoplayResolve = StaticData.Settings.AutoResolveScenesAtPlay;
            StaticData.Settings.AutoResolveScenesAtPlay = false;
            var container = Object.Instantiate(Resources.Load<GameObject>("Toolbox Container"));
            container.GetComponent<ToolboxEntry>().InitializeComponents();
        }
    }
}
