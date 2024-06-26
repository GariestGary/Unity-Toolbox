using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Messenger: ToolWrapper<Messenger>
	{
		private List<Subscriber> subscribers = new();

        protected override void Run()
        {
            Subscribe<SceneUnloadedMessage>(m => CheckSceneSubscribers(m.SceneName), null, true);
			Subscribe<GameObjectRemovedMessage>(m => CheckRemovedObject(m.Obj), null, true);
        }

		private static void CheckSceneSubscribers(string scene)
		{
			Instance.subscribers.RemoveAll(x => x.HasBind && x.BindedObject == null);

			var sceneSubs = Instance.subscribers.Where(x => x.BindedObject != null && x.BindedObject.scene.name == scene).ToList();

			foreach (var sub in sceneSubs)
			{
				RemoveSubscriber(sub);
			}
		}

		private static void CheckRemovedObject(GameObject obj)
		{
			var bindedSub = Instance.subscribers.FirstOrDefault(x => x.HasBind && x.BindedObject == obj);

			if(bindedSub is not null)
			{
				RemoveSubscriber(bindedSub);
			}
		}

        public static void ClearSubscribers()
        {
			Instance.subscribers.RemoveAll(s => !s.Keep);
        }

        public static void RemoveSubscriber(Subscriber subscriber)
        {
	        if(subscriber == null) return;

	        if (Instance.subscribers == null || Instance.subscribers.Count <= 0)
	        {
		        return;
	        }

	        if(Instance.subscribers.Contains(subscriber))
			{
				Instance.subscribers.Remove(subscriber);
			}
        }

        public static void RemoveSubscribers(IEnumerable<Subscriber> subscribers)
        {
	        foreach (var subscriber in subscribers)
	        {
		        RemoveSubscriber(subscriber);
	        }
        }

		public static Subscriber Subscribe<T>(Action<T> next, GameObject bind = null, bool keep = false) where T: Message
		{
			var sub = new Subscriber(typeof(T), Callback, bind, keep);
            Instance.subscribers.Add(sub);
            return sub;
            void Callback(object args) => next((T)args);
		}

		public static Subscriber Subscribe<T>(Action next, GameObject bind = null, bool keep = false) where T : Message
		{
			var sub = new Subscriber(typeof(T), Callback, bind, keep);
			Instance.subscribers.Add(sub);
			return sub;
			void Callback(object args) => next();
		}

		public static void Send<T>() where T: Message
		{
			var message = (T)Activator.CreateInstance(typeof(T));

			Send(message);
        }

		public static void Send<T>(T message) where T: Message
		{
			message ??= (T)Activator.CreateInstance(typeof(T));

			var receivers = Instance.subscribers.Where(x => x.Type == message.GetType()).ToList();

			for (int i = 0; i < receivers.Count(); i++)
			{
				var receiver = receivers[i];
				
				try
				{
					if(receiver.HasBind && (receiver.BindedObject == null))
					{
						RemoveSubscriber(receiver);
						return;
					}
				}
				catch
				{
					RemoveSubscriber(receiver);
					return;

				}

				receiver.Callback.Invoke(message);
			}
			
			foreach (var receiver in receivers)
			{
				
			}
        }

		protected override void Clear()
		{
			subscribers?.Clear();
			subscribers = null;
		}
	}
}
