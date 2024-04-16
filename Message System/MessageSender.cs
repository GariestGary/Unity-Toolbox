using UnityEngine;
using System;
using System.Collections.Generic;

namespace VolumeBox.Toolbox
{
    public class MessageSender: MonoCached
    {
        [SerializeField] private List<MessageToSend> _messages;

        [ContextMenu("Send")]
        public void Send()
        {
            foreach (var message in _messages)
            {
                Messenger.Send(message.Message);
            }
        }

        [Serializable]
        private class MessageToSend
        {
            [SerializeReference, SerializeField] private Message message;

            public Message Message
            {
                get { return message; }
                set { message = value; }
            }
        }
    }
}