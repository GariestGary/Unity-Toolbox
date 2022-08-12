using System;
using System.Collections;
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
    }
}