using System;
using System.Collections;
using BeauRoutine;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
    public static class ToolboxExtensions
    {
        public static void Resolve(this object mono)
        {
            Resolver.Instance.Inject(mono);
        }

        public static Routine StartManual(this IEnumerator coroutine)
        {
            return Routine.StartManual(coroutine);
        }
    }
}