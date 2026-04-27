using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntil", story: "[IsSuccess]", category: "Flow", id: "cccbcca0786c98502c97308f837ed6a2")]
public partial class WaitUntilModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<bool> IsSuccess;

    protected override Status OnStart()
    {
        Debug.Log("WaitUntilModifier: OnStart called, initializing IsSuccess to false.");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsSuccess.Value) {
            Debug.Log("WaitUntilModifier: IsSuccess is true, returning Success.");
            return Status.Success;
        }
        Debug.Log("WaitUntilModifier: IsSuccess is false, returning Waiting.");
        return Status.Waiting;
    }
}

