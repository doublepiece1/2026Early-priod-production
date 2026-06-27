using Kounosuke;
using UnityEngine;


namespace Kounosuke
{
    public class CollitionGoal : ColitionEventBase
    {
        protected override void CollitionEvent()
        {
            Debug.Log("collition Goal");
        }
    }
}
