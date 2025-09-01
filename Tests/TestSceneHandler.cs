using Cysharp.Threading.Tasks;

namespace VolumeBox.Toolbox.Tests
{
    internal class TestSceneHandler: SceneHandler<TestSceneArgs>
    {
        protected override async UniTask SetupSceneAsync(TestSceneArgs args)
        {
            TravelerTests.compare = args.TestString;
        }

        protected override async UniTask UnloadSceneAsync()
        {
            TravelerTests.compare = "unloaded";
        }
    }

    internal class TestSceneArgs : SceneArgs
    {
        public string TestString;
    }
}