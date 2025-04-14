using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;
using System.Collections.ObjectModel;
public class Climbing : MonoBehaviour
{
    Vector3 right_hand_pre_pos;
    Vector3 left_hand_pre_pos;
    Vector3 right_hand_controller_pre_pos;
    Vector3 left_hand_controller_pre_pos;
    Vector3 movement_amount;
    Vector3 right_movement_amount;
    Vector3 left_movement_amount;
    UnityChanControlScriptWithRgidBody player;
    public float move_force = 10f;
    public GameObject right_hand;
    public GameObject left_hand;
    public GameObject right_hand_controller;
    public GameObject left_hand_controller;
    bool is_controller = false;
    bool is_selected = false;
    GameManager gm;
    private void Start()
    {
        right_hand_pre_pos = right_hand.transform.position;
        left_hand_pre_pos = left_hand.transform.position;

        right_hand_controller_pre_pos = right_hand_controller.transform.position;
        left_hand_controller_pre_pos = left_hand_controller.transform.position;
        player = UnityChanControlScriptWithRgidBody.instance;
        gm = GameManager.instance;
    }
    public void isSelected()
    {
        gm.DetermineHandState();
        right_hand_pre_pos = right_hand.transform.position;
        left_hand_pre_pos = left_hand.transform.position;

        right_hand_controller_pre_pos = right_hand_controller.transform.position;
        left_hand_controller_pre_pos = left_hand_controller.transform.position;

        is_selected = true;
    }
    public void unSelected()
    {
        gm.DetermineHandState();
        if (gm.is_left_hand_grab || gm.is_right_hand_grab || gm.is_left_hand_controller_grab || gm.is_right_hand_controller_grab) return;
        is_selected = false;
    }
    public void isMove()
    {
        gm.DetermineHandState();
        if (!is_selected) return;
        gm.player_grab = 1;
        player.is_climbing = true;

        if (gm.is_right_hand_grab) right_movement_amount = (right_hand.transform.position - right_hand_pre_pos) * move_force;
        if (gm.is_right_hand_controller_grab) right_movement_amount = (right_hand_controller.transform.position - right_hand_controller_pre_pos) * move_force;
        if (gm.is_left_hand_grab) left_movement_amount = (left_hand.transform.position - left_hand_pre_pos) * move_force;
        if (gm.is_left_hand_controller_grab) left_movement_amount = (left_hand_controller.transform.position - left_hand_controller_pre_pos) * move_force;

        if ((gm.is_right_hand_grab && gm.is_left_hand_grab) || (gm.is_right_hand_controller_grab && gm.is_left_hand_controller_grab))
        {
            if (right_movement_amount.magnitude > left_movement_amount.magnitude) movement_amount = right_movement_amount;
            else movement_amount = left_movement_amount;
            player.IsClimbing(movement_amount);
        }
        else if (gm.is_right_hand_grab || gm.is_right_hand_controller_grab)
        {
            movement_amount = right_movement_amount;
            player.IsClimbing(movement_amount);
        }
        else if(gm.is_left_hand_grab || gm.is_left_hand_controller_grab)
        {
            movement_amount = left_movement_amount;
            player.IsClimbing(movement_amount);
        }
        // Debug.Log("@@@@@@@@@@@@movement_amount" + movement_amount);
        
        
        right_hand_pre_pos = right_hand.transform.position;
        left_hand_pre_pos = left_hand.transform.position;
        right_hand_controller_pre_pos = right_hand_controller.transform.position;
        left_hand_controller_pre_pos = left_hand_controller.transform.position;
    }

    public void isRelease()
    {
        gm.DetermineHandState();
        if (gm.is_left_hand_grab || gm.is_right_hand_grab || gm.is_left_hand_controller_grab || gm.is_right_hand_controller_grab) return;
        Debug.Log("@@@@@@@@@@@@release");
        player.is_climbing = false;
        
    }
}
