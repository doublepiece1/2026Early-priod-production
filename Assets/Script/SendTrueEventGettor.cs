using Kounosuke;
using System;
using UnityEngine;
using Unity.Behavior;


namespace Kounosuke {
    public class SendTrueEventGettor : AnimationEventGettorBase
    {
        [SerializeField] private BehaviorGraphAgent Agent;

        void Start()
        {

        }

        void Update()
        {

        }

        public void Send(string eventName, bool value)
        {
            if (Agent != null)
            {
                var blackboard = Agent.BlackboardReference;
                blackboard.SetVariableValue<bool>(eventName, value);
                Debug.Log(blackboard.GetVariableValue<bool>(eventName, out bool After));
                Debug.Log("GetEvent called");
            }
        }
        public override void GetEvent( )
        {
           
        }
    }

}