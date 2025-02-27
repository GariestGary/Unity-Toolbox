using System.Collections;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Util for starting coroutine from non-MonoBehaviour classes
    /// </summary>
    public class CoroutineStarter : MonoSingleton<CoroutineStarter>
    {
        public static Coroutine Invoke(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine Invoke(string methodName)
        {
            return Instance.StartCoroutine(methodName);
        }

        public static void Stop(IEnumerator coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static void Stop(string methodName)
        {
            Instance.StopCoroutine(methodName);
        }
    }
}
