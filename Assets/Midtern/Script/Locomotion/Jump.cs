using LocomotionStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;

namespace LocomotionStateMachine
{
    public class Jump : LocomotionState
    {
        public override void StateAction()
        {
            Debug.Log("Jump");
            // Player.GetComponent<Rigidbody>().AddForce(Vector3.up * Player.GetComponent<UnityChanControlScriptWithRgidBody>().jumpPower, ForceMode.VelocityChange);
        }
    }
}

