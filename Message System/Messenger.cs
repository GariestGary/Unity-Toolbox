using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Messenger: ToolWrapper<Messenger>
	{
		private static List<Subscriber> subscribers = new List<Subscriber>();

		public static List<Subscriber> Subscribers => subscribers;

        protected override void Run()
        {
            Subscribe<SceneUnloadedMessage>(m => CheckSceneSubscribers(m.SceneName));
        }

		private static void CheckSceneSubscribers(string scene)
		{
			var sceneSubs = subscribers.Where(x => x.BindedObject != null && x.BindedObject.scene.name == scene).ToList();

			for (int i = 0; i < sceneSubs.Count; i++)
			{
				subscribers.Remove(sceneSubs[i]);
			}
		}

        public static void ClearKeepingSubscribers()
        {
	        subscribers.Clear();
        }

        public static void RemoveSubscriber(Subscriber subscriber)
        {
	        if(subscriber == null) return;
	        
	        Subscriber sub = subscribers.FirstOrDefault(x => x == subscriber);

	        if (sub == null)
	        {
		        sub = subscribers.FirstOrDefault(x => x == subscriber);

		        if (sub != null)
		        {
			        subscribers.Remove(sub);
		        }
	        }
	        else
	        {
		        subscribers.Remove(sub);
	        }
        }

		public static Subscriber Subscribe<T>(Action<T> next, GameObject bind = null) where T: Message
		{
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback, bind);
            subscribers.Add(sub);
            return sub;
		}

		public static void Send<T>() where T: Message
		{
			var message = (T)Activator.CreateInstance(typeof(T));

			Send(message);
        }

		public static void Send<T>(T message) where T: Message
		{
			if(message == null)
			{
				message = (T)Activator.CreateInstance(typeof(T));
			}

			var receivers = subscribers.Where(x => x.Type == message.GetType());
			receivers = receivers.Concat(subscribers.Where(x => x.Type == message.GetType()));

			receivers.ToList().ForEach(x =>
			{
				x.Callback.Invoke(message);
			});
        }

		protected override void Clear()
		{
			subscribers?.Clear();
			subscribers = null;
		}
	}
}
