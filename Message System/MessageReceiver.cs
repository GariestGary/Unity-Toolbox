using System;
using UnityEngine;
using UnityEngine.Events;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox
{
    public class MessageReceiver: MonoCached
    {
        [SerializeField] private InspectableType<Message> messageType;

        public UnityEvent ReceivedEvent;
        private Subscriber sub;

        protected override void Rise()
        {
            var method = typeof(Messenger).GetMethod("Subscribe", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            method = method.MakeGenericMethod(messageType.Type);
            Action<Message> callback = (m) => ReceivedEvent.Invoke();
            sub = method.Invoke(Messenger.Instance, new object[] { callback }) as Subscriber;
        }

        protected override void Destroyed()
        {
            Messenger.RemoveSubscriber(sub);
        }
    }
}
