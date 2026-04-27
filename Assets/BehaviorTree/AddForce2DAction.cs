using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Add Force 2D", story: "[Add] [Force] 2D", category: "Action", id: "78919a5a7ba66414faeae193b8c6fb7b")]
public partial class AddForce2DAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector2> Add;
    [SerializeReference] public BlackboardVariable<Rigidbody2D> Force;

    protected override Status OnStart()
    {
        if (Force.Value == null)
        {
            LogFailure("No target rigidbody2D assigned.");
            return Status.Failure;
        }

        Force.Value.AddForce(Add.Value,ForceMode2D.Impulse);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

