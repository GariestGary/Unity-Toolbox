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
            Instance.SubscribeKeeping<SceneUnloadedMessage>(_ => ClearSceneSubscribers());
        }

        public void ClearSceneSubscribers()
        {
            sceneSubscribers.Clear();
        }

        public void ClearKeepingSubscribers()
        {
	        subscribers.Clear();
        }

        public void RemoveSubscriber(Subscriber subscriber)
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
        

        public Subscriber SubscribeKeeping<T>(Action<T> next) where T: Message
        {
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback);
			subscribers.Add(sub);
			return sub;
        }

        public Subscriber SubscribeKeeping(Type type, Action next)
        {
	        Action<object> callback = args => next();
	        var sub = new Subscriber(type, callback);
	        subscribers.Add(sub);
	        return sub;
        }
        

		public Subscriber Subscribe<T>(Action<T> next) where T: Message
		{
			Action<object> callback = args => next((T)args);
			var sub = new Subscriber(typeof(T), callback);
            sceneSubscribers.Add(sub);
            return sub;
		}

		public Subscriber Subscribe(Type type, Action next)
		{
            Action<object> callback = args => next();
            var sub = new Subscriber(type, callback);
            sceneSubscribers.Add(sub);
            return sub;
		}


		public void Send<T>() where T: Message
		{
			var message = (T)Activator.CreateInstance(typeof(T));

			Send(message);
        }

		public void Send<T>(T message) where T: Message
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

    }
}
