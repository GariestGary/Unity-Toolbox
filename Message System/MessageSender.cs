using UnityEngine;
using System;
using System.Collections.Generic;
using TypeReferences;

namespace VolumeBox.Toolbox
{
    public class MessageSender: MonoCached
    {
        [SerializeField] private List<MessageToSend> _messages;

        private int _prevMessagesCount = 1;

        [ContextMenu("Send")]
        public void Send()
        {
            foreach (var message in _messages)
            {
                Messenger.Send(message.CurrentTypeInstance);
            }
        }

        private void OnValidate()
        {
#if !ODIN_INSPECTOR
            if (_messages == null) return;

            if (_messages.Count > _prevMessagesCount)
            {
                _messages[_messages.Count - 1].CurrentTypeInstance = null;
            }

            foreach (var message in _messages)
            {
                if (message.CurrentTypeInstance == null || message.CurrentTypeInstance.GetType() != message.MessageType.Type)
                {
                    if (message.MessageType == null)
                    {
                        message.CurrentTypeInstance = null;
                    }
                    else
                    {
                        if(message.MessageType.Type == null)
                        {
                            continue;
                        }
                        message.CurrentTypeInstance = Activator.CreateInstance(message.MessageType.Type) as Message;
                    }
                }
            }

            _prevMessagesCount = _messages.Count;
#endif
        }

        [Serializable]
        private class MessageToSend
        {
#if !ODIN_INSPECTOR
            [SerializeField, Inherits(typeof(Message), ShowAllTypes = true)] private TypeReference messageType;
#endif

            [SerializeReference] private Message currentTypeInstance;

#if !ODIN_INSPECTOR
            public TypeReference MessageType => messageType;
#endif
            public Message CurrentTypeInstance
            {
                get { return currentTypeInstance; }
                set { currentTypeInstance = value; }
            }
        }
    }
}