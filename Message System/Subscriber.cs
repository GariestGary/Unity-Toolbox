using System;
using System.Diagnostics;
using System.Reflection;

namespace VolumeBox.Toolbox
{
    public class Subscriber
    {
        private Type type;
        private Action<object> callback;

        public Type Type => type;
        public Action<object> Callback => callback;

        public Subscriber(Type type, Action<object> callback)
        {
            this.callback = callback;
            this.type = type;
        }

    }
}