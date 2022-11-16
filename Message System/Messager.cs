using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeBox.Toolbox;

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

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void SubscribeKeeping<T>(Action<T> next)
        {
			Action<object> callback = args => next((T)args);

			subscribers.Add(new Subscriber(typeof(T), callback));
		}

		public void Subscribe<T>(Action<T> next)
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
			if(message == null)
			{
				message = (T)Activator.CreateInstance(typeof(T));
			}

			var receivers = subscribers.Where(x => x.Type == message.GetType()).ToList();

			receivers.ForEach(x =>
			{
				x.Callback.Invoke(message);
			});
        }

    }
}
