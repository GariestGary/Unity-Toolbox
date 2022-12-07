using System;
using System.Collections.Generic;
using System.Linq;

namespace VolumeBox.Toolbox
{
    public class Messager: Singleton<Messager>, IRunner
    {
        private List<Subscriber> subscribers = new List<Subscriber>();
        private List<Subscriber> sceneSubscribers = new List<Subscriber>();

        public void Run()
        {
            Instance.SubscribeKeeping<SceneUnloadedMessage>(_ => ClearNullSubscribers());
        }

        private void ClearNullSubscribers()
        {
            sceneSubscribers.Clear();
        }

        public void SubscribeKeeping<T>(Action<T> next) where T: Message
        {
            Action<object> callback = args => next((T)args);

            subscribers.Add(new Subscriber(typeof(T), callback));
        }

        public void Subscribe<T>(Action<T> next) where T: Message
        {
            Action<object> callback = args => next((T)args);

            sceneSubscribers.Add(new Subscriber(typeof(T), callback));
        }

        public void Send<T>()
        {
            var message = (T)Activator.CreateInstance(typeof(T));

            Send(message);
        }

        public void Send<T>(T message)
        {
            if (message == null)
            {
                message = (T)Activator.CreateInstance(typeof(T));
            }

            var receivers = subscribers.Where(x => x.Type == message.GetType());
            receivers = receivers.Concat(sceneSubscribers.Where(x => x.Type == message.GetType()));

            receivers.ToList().ForEach(x =>
            {
                x.Callback.Invoke(message);
            });
        }
    }
}