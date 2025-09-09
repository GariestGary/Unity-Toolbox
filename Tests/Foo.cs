using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Tests
{
    internal class Foo: MonoCached
    {
        public float Delta => delta;
        public float counter = 0;

        protected override void Tick()
        {
            counter += delta;
        }
    }
}