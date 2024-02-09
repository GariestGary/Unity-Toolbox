using System;
using System.Linq;
using TypeReferences;
using UnityEngine;
using UnityEngine.Events;

namespace VolumeBox.Toolbox
{
    public class MessageReceiver: MonoCached
    {
#if ODIN_INSPECTOR
        [SerializeReference] private Message messageType;
#else
        [SerializeField]
        [Inherits(typeof(Message), ShowAllTypes = true)]
        private TypeReference messageType;
#endif

        public UnityEvent ReceivedEvent;
        private Subscriber sub;

        protected override void Rise()
        {
#if ODIN_INSPECTOR
            if (messageType == null)
            {
                return;
            }
#else
            if(messageType.Type == null)
            {
                return;
            }
#endif
            
            //var method = typeof(Messenger).GetMethod("Subscribe", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var method = typeof(Messenger).GetMethods()
                .FirstOrDefault(m => m.Name == nameof(Messenger.Subscribe) && m.IsGenericMethod);
#if ODIN_INSPECTOR
            method = method.MakeGenericMethod(messageType?.GetType());
#else
            method = method.MakeGenericMethod(messageType.Type);
#endif
            void Callback(Message m) => ReceivedEvent.Invoke();
            sub = method.Invoke(Messenger.Instance, new object[] {(Action<Message>) Callback, gameObject, false}) as Subscriber;
        }

        protected override void Destroyed()
        {
            Messenger.RemoveSubscriber(sub);
        }
    }
}
