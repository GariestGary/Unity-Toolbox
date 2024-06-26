using System;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [System.Serializable]
    public class Subscriber
    {
        private Type type;
        private Action<object> callback;
        private GameObject bindedObject;
        private bool hasBind;
        private bool keep;

        public Type Type => type;
        public Action<object> Callback => callback;
        public GameObject BindedObject => bindedObject;
        public bool HasBind => hasBind;
        public bool Keep => keep;

        public Subscriber(Type type, Action<object> callback, GameObject bindedObject = null, bool keep = false)
        {
            this.bindedObject = bindedObject;
            this.callback = callback;
            this.type = type;
            this.keep = keep;
            this.hasBind = bindedObject != null;
        }

    }
}