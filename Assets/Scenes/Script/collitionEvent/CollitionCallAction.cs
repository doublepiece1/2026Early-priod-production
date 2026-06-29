using UnityEngine;
using UnityEngine.Events;


namespace Kounosuke
{
    public class CollitionCallAction : ColitionEventBase
    {
        [SerializeField, Header("åƒÇ—èoÇ∑publicä÷êî")] private UnityEvent Action_;

        protected override void CollitionEvent()
        {
            Debug.Log("collition action");
            Action_?.Invoke();
        }
    }
}