using UnityEngine;
using Unity.VisualScripting;

public class AnimatorSendEvent : MonoBehaviour
{
    [SerializeField, Header("状態マシン")] private StateMachine stateMachine_;

    public void SendEvent(AnimationEvent animationEvent)
    {
        if (stateMachine_ != null)
        {
            stateMachine_.TriggerAnimationEvent(animationEvent);
        }
    }
}
