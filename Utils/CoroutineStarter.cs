using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Util for starting coroutine from non-MonoBehaviour classes
    /// </summary>
    public class CoroutineStarter : Singleton<CoroutineStarter>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
