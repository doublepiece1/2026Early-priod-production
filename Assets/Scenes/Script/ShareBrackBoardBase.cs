using Unity.Behavior;
using UnityEngine;


namespace Kounosuke {
    public class ShareBrackBoardBase : MonoBehaviour
    {
        [SerializeField] private BehaviorGraphAgent Agent;

        public BehaviorGraphAgent GetGraph()
        {
            if(Agent != null)
            {
                return Agent;
            }
            return null;
        }
    }
}