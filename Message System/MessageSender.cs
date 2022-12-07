using TypeReferences;
using UnityEngine;
using VolumeBox.Toolbox;
using System;
using System.Collections.Generic;

namespace VolumeBox.Toolbox
{
    public class MessageSender : MonoCached
    {
        [SerializeField] [Inherits(typeof(Message))] private TypeReference messageType;

        [SerializeReference] private Message currentTypeInstance;

        [SerializeField] public Message CurrentTypeInstance => currentTypeInstance;

        [SerializeField] private MessagesContainer container = new MessagesContainer();

        public void InitContainer()
        {
            if(container == null)
            {
                container = new MessagesContainer();
            }
        }

        public void Send()
        {
            object message = container.GetType().GetField(currentTypeInstance.GetType().Name).GetValue(container);
            Messager.Instance.Send(message as Message);
        }

        private void OnValidate()
        {

            if (currentTypeInstance == null || currentTypeInstance.GetType() != messageType.Type)
            {
                if(messageType == null || messageType.Type == null)
                {
                    currentTypeInstance = null;
                }
                else
                {
                    currentTypeInstance = Activator.CreateInstance(messageType.Type) as Message;
                }
            }
        }
    }
}

public class TestMessage: Message
{
    public GameObject obj;
    public float a;
    public Vector3 s;
    public List<SceneBindingMessage> msg;
}
