using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntil", story: "[IsSuccess]", category: "Flow", id: "cccbcca0786c98502c97308f837ed6a2")]
public partial class WaitUntilModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<bool> IsSuccess;

    protected override Status OnStart()
    {
        IsSuccess.Value = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsSuccess.Value) {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

