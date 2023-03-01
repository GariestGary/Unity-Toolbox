using System;
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
        var method = typeof(Messager).GetMethod("Subscribe", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        method = method.MakeGenericMethod(messageType.Type);
        Action<Message> callback = (m) => ReceivedEvent.Invoke();
        method.Invoke(Messager.Instance, new object[] { callback });
    }
}
