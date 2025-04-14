using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LocomotionStateMachine
{
    public class Lock : LocomotionState
    {
        // Start is called before the first frame update
        public override void StateAction()
        {
            Debug.Log("Lock");
        }
    }
}
