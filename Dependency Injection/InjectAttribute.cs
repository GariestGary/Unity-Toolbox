using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class InjectAttribute : Attribute
    {
        private string _id = "";

        public string ID => _id;

        public InjectAttribute(string id)
        {
            _id = id;
        }

        public InjectAttribute() { }
    }
}
