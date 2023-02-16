using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class CoroutineStarter : Singleton<CoroutineStarter>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
