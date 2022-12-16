using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.Events;
using VolumeBox.Toolbox;

public class MessageReceiver: MonoCached
{
    [SerializeField][Inherits(typeof(Message), IncludeAdditionalAssemblies = new[] { "Assembly-CSharp" })] private TypeReference messageType;

    public UnityEvent ReceivedEvent;

    protected override void Rise()
    {
        Messager.Instance.Subscribe(messageType.Type, ReceivedEvent.Invoke);
    }
}
