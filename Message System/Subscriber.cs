using System;

namespace VolumeBox.Toolbox
{
    [System.Serializable]
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