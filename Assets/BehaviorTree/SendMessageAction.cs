using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SendMessage", story: "[Tatget] [Message]", category: "Action", id: "3a791ff712ddb8ddbeed1f881f862de3")]
public partial class SendMessageAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Tatget;
    [SerializeReference] public BlackboardVariable<string> Message;

    protected override Status OnStart()
    {
        if(Tatget.Value == null)
        {
            Debug.LogWarning("SendMessageAction: Target is null.");
            return Status.Failure;
        }
        if(Message.Value == null)
        {
            Debug.LogWarning("SendMessageAction: Message is null.");
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        if(Tatget.Value == null) {
            Debug.LogWarning("SendMessageAction: Target is null.");
            return Status.Failure;
        }
        if(Message.Value == null) {
            Debug.LogWarning("SendMessageAction: Message is null.");
            return Status.Failure;
        }
        CustomEvent.Trigger(Tatget.Value, Message.Value);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

