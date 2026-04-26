using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitBool", story: "[Bool]", category: "Action/Blackboard", id: "a54b5decb1848aa5fdb635a4e2486450")]
public partial class WaitBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Bool;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(!Bool.Value)
        {
            Debug.Log("WaitBoolAction: Bool is false, returning Waiting.");
            return Status.Waiting;
        }
        return Status.Success;
    }

}

