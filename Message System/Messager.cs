using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
	public class Messager: Singleton<Messager>, IRunner
	{
		private List<Subscriber> subs = new List<Subscriber>();
		private List<Subscriber> levelSubs = new List<Subscriber>();

		public void Run()
		{
			Subscribe(Message.SCENE_UNLOADED, _ => levelSubs.Clear());
		}
		
		public void Subscribe(Message id, Action<object> next)
		{
			subs.Add(new Subscriber(id, next));
		}

		public void SubscribeForLevel(Message id, Action<object> next)
		{
			levelSubs.Add(new Subscriber(id, next));
		}

		public void Send(Message id, object data = null)
		{
			List<Subscriber> listeners = subs.Where(x => x.id == id).ToList();
			listeners.Concat(levelSubs.Where(x => x.id == id));

			for (var i = 0; i < listeners.Count; i++)
			{
				listeners[i].react.Invoke(data);	
			}
		}
	}
}
