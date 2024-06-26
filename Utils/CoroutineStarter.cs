using System.Collections;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Util for starting coroutine from non-MonoBehaviour classes
    /// </summary>
    public class CoroutineStarter : MonoSingleton<CoroutineStarter>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
