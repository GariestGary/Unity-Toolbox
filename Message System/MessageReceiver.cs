using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace VolumeBox.Toolbox
{
    public class MessageReceiver: MonoCached
    {
        [SerializeReference, SerializeField] private Message messageType;

        public UnityEvent ReceivedEvent;
        private Subscriber sub;

        protected override void Rise()
        {
            if(messageType == null)
            {
                return;
            }
            
            var method = typeof(Messenger).GetMethods()
                .FirstOrDefault(m => m.Name == nameof(Messenger.Subscribe) && m.IsGenericMethod);
            method = method.MakeGenericMethod(messageType?.GetType());

            void Callback(Message m) => ReceivedEvent.Invoke();
            sub = method.Invoke(Messenger.Instance, new object[] {(Action<Message>) Callback, gameObject, false}) as Subscriber;
        }

        protected override void Destroyed()
        {
            Messenger.RemoveSubscriber(sub);
        }
    }
}
