    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionStateMachine // Or you can type: using LocomotionStateMachine
{
    public class Forwarding : LocomotionState
    {
        //public float MoveDuration = 0.001f;
        //public float Distance = 3f;
        float pre_play_y;
        private void Start()
        {
            pre_play_y = Player.transform.position.y;
        }
        public override void StateAction()
        {
            base.StateAction();
            Debug.Log("Forwarding");
            if (Player.transform.position.y - pre_play_y > 0.03f) GameManager.instance.player_up_down = 2;
            else if (pre_play_y - Player.transform.position.y > 0.03f) GameManager.instance.player_up_down = 1;
            else GameManager.instance.player_up_down = 0;
            pre_play_y = Player.transform.position.y;
            //Player.transform.position += Player.transform.forward * Distance;
            EMSControl.instance.CallEMS_Locomotion();
        }
    }
}
