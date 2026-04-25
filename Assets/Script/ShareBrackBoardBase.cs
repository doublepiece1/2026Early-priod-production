using Unity.Behavior;
using UnityEngine;


namespace Kounosuke {
    public class ShareBrackBoardBase : MonoBehaviour
    {
        [SerializeField] private Blackboard Blackboard;

        public Blackboard GetGraph()
        {
            if(Blackboard != null)
            {
                return Blackboard;
            }
            return null;
        }
    }
}