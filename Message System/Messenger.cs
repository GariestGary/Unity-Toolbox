using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
	public class Messenger: ToolWrapper<Messenger>
	{
		private static List<Subscriber> subscribers = new List<Subscriber>();
		private static List<Subscriber> sceneSubscribers = new List<Subscriber>();

		public static List<Subscriber> Subscribers => subscribers;
		public static List<Subscriber> SceneSubscribers => sceneSubscribers;

        protected override void Run()
        {
            SubscribeKeeping<SceneUnloadedMessage>(m => CheckSceneSubscribers(m.SceneName));
        }

		private static void CheckSceneSubscribers(string scene)
		{
			var sceneSubs = sceneSubscribers.Where(x => x.RelatedSceneName == scene);

			foreach (var sub in sceneSubs)
			{
				sceneSubscribers.Remove(sub);
			}
		}

        public static void ClearSceneSubscribers()
        {
            sceneSubscribers.Clear();
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
		        sub = sceneSubscribers.FirstOrDefault(x => x == subscriber);

		        if (sub != null)
		        {
			        sceneSubscribers.Remove(sub);
		        }
	        }
	        else
	        {
		        subscribers.Remove(sub);
	        }
        }
        
        public static Subscriber SubscribeKeeping<T>(Action<T> next) where T: Message
        {
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback);
			subscribers.Add(sub);
			return sub;
        }

		public static Subscriber Subscribe<T>(Action<T> next, string relatedScene = null) where T: Message
		{
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback, relatedScene);
            sceneSubscribers.Add(sub);
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
			receivers = receivers.Concat(sceneSubscribers.Where(x => x.Type == message.GetType()));

			receivers.ToList().ForEach(x =>
			{
				x.Callback.Invoke(message);
			});
        }

		protected override void Clear()
		{
			subscribers.Clear();
			subscribers = null;
			sceneSubscribers.Clear();
			sceneSubscribers = null;
		}
	}
}
