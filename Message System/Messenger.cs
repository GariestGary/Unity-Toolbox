using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Messenger: ToolWrapper<Messenger>
	{
		private List<Subscriber> subscribers = new List<Subscriber>();

        protected override void Run()
        {
            Subscribe<SceneUnloadedMessage>(m => CheckSceneSubscribers(m.SceneName), null, true);
			Subscribe<GameObjectRemovedMessage>(m => CheckRemovedObject(m.Obj), null, true);
        }

		private static void CheckSceneSubscribers(string scene)
		{
			Instance.subscribers.RemoveAll(x => x.HasBind && x.BindedObject == null);

			var sceneSubs = Instance.subscribers.Where(x => x.BindedObject != null && x.BindedObject.scene.name == scene).ToList();

			for (int i = 0; i < sceneSubs.Count; i++)
			{
				RemoveSubscriber(sceneSubs[i]);
			}
		}

		private static void CheckRemovedObject(GameObject obj)
		{
			var bindedSub = Instance.subscribers.Where(x => x.HasBind && x.BindedObject == obj).FirstOrDefault();

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

		public static Subscriber Subscribe<T>(Action<T> next, GameObject bind = null, bool keep = false) where T: Message
		{
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback, bind, keep);
            Instance.subscribers.Add(sub);
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

			var receivers = Instance.subscribers.Where(x => x.Type == message.GetType());

			receivers.ToList().ForEach(x =>
			{
				try
				{
					if(x.HasBind && (x.BindedObject == null))// || x.BindedObject.transform == ))
					{
						RemoveSubscriber(x);
						return;
					}
				}
				catch
				{
					RemoveSubscriber(x);
					return;

				}

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
