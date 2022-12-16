using TypeReferences;
using UnityEngine;
using VolumeBox.Toolbox;
using System;
using System.Collections.Generic;

public class MessageSender : MonoCached
{
    [SerializeField] private List<MessageToSend> _messages;

    public void Send()
    {
        foreach (var message in _messages)
        {
            Messager.Instance.Send(message.CurrentTypeInstance);
        }
    }

    private void OnValidate()
    {
        foreach (var message in _messages)
        {
            if (message.CurrentTypeInstance == null || message.CurrentTypeInstance.GetType() != message.MessageType.Type)
            {
                if(message.MessageType.Type == null)
                {
                    message.CurrentTypeInstance = null;
                }
                else
                {
                    message.CurrentTypeInstance = Activator.CreateInstance(message.MessageType.Type) as Message;
                }
            }
        }
        
        
    }

    [Serializable]
    private class MessageToSend
    {
        [SerializeField] [Inherits(typeof(Message), IncludeAdditionalAssemblies = new[] { "Assembly-CSharp" })] private TypeReference messageType;

        [SerializeReference] private Message currentTypeInstance;

        public TypeReference MessageType => messageType;
        public Message CurrentTypeInstance
        {
            get { return currentTypeInstance; }
            set { currentTypeInstance = value; }
        }
    }
}


