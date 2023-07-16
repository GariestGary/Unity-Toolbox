using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Messenger: ToolWrapper<Messenger>
	{
		private List<Subscriber> subscribers = new List<Subscriber>();

		public static List<Subscriber> Subscribers => Instance.subscribers;

        protected override void Run()
        {
            Subscribe<SceneUnloadedMessage>(m => CheckSceneSubscribers(m.SceneName));
        }

		private static void CheckSceneSubscribers(string scene)
		{
			var sceneSubs = Subscribers.Where(x => x.BindedObject != null && x.BindedObject.scene.name == scene).ToList();

			for (int i = 0; i < sceneSubs.Count; i++)
			{
				Subscribers.Remove(sceneSubs[i]);
			}
		}

        public static void ClearKeepingSubscribers()
        {
	        Subscribers.Clear();
        }

        public static void RemoveSubscriber(Subscriber subscriber)
        {
	        if(subscriber == null) return;
	        
	        Subscriber sub = Subscribers.FirstOrDefault(x => x == subscriber);

	        if (sub == null)
	        {
		        sub = Subscribers.FirstOrDefault(x => x == subscriber);

		        if (sub != null)
		        {
			        Subscribers.Remove(sub);
		        }
	        }
	        else
	        {
		        Subscribers.Remove(sub);
	        }
        }

		public static Subscriber Subscribe<T>(Action<T> next, GameObject bind = null) where T: Message
		{
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback, bind);
            Subscribers.Add(sub);
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

			var receivers = Subscribers.Where(x => x.Type == message.GetType());

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
