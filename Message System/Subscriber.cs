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

        public Type Type => type;
        public Action<object> Callback => callback;
        public GameObject BindedObject => bindedObject;

        public Subscriber(Type type, Action<object> callback, GameObject bindedObject = null)
        {
            this.bindedObject = bindedObject;
            this.callback = callback;
            this.type = type;
        }

    }
}