using System;
using System.Diagnostics;
using System.Reflection;

namespace VolumeBox.Toolbox
{
    [System.Serializable]
    public class Subscriber
    {
        private Type type;
        private Action<object> callback;
        private string relatedSceneName;

        public Type Type => type;
        public Action<object> Callback => callback;
        public string RelatedSceneName => relatedSceneName;

        public Subscriber(Type type, Action<object> callback, string relatedSceneName = null)
        {
            this.relatedSceneName = relatedSceneName;
            this.callback = callback;
            this.type = type;
        }

    }
}