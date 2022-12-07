using System.Collections;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class CoroutineStarter: Singleton<CoroutineStarter>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}