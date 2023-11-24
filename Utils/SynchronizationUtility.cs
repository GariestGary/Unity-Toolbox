#if TOOLBOX_TASKS_HANDLE
#if UNITY_EDITOR
using System.Threading;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Workaround for issue, when Task continues executing even after exited play mode
    /// </summary>
    [InitializeOnLoad]
    public static class SynchronizationUtility
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeFirstSceneLoad_Static()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    KillCurrentSynchronizationContext();
                }
            };
        }

        // Kills the current synchronization context after exiting play mode to avoid Tasks continuing to run.
        private static void KillCurrentSynchronizationContext()
        {
            var synchronizationContext = SynchronizationContext.Current;

            var constructor = synchronizationContext
                .GetType()
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int) }, null);

            if (constructor == null)
            {
                return;
            }

            object newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
        }
    }
}

#endif
#endif