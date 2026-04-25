using Kounosuke;
using System;
using UnityEngine;
using Unity.Behavior;


namespace Kounosuke {
    public class SendTrueEventGettor : AnimationEventGettorBase
    {
        [SerializeField] private BlackboardVariable<bool> IsSuccess;

        void Start()
        {

        }

        void Update()
        {

        }

        public override void GetEvent()
        {
            if (IsSuccess != null) {
                IsSuccess.Value = true;
            }
        }
    }

}