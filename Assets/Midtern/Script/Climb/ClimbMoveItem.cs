using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;

public class ClimbMoveItem : MonoBehaviour
{
    GameManager gm;
    UnityChanControlScriptWithRgidBody player;
    Vector3 movement_amount;
    Vector3 pre_pos;
    bool is_selected = false;
    // 這個 script 會需要讓 climbing 全部使用 local position， 但是使用 local postion ，在player rotate 情況下，會導致移動距離不對，所以暫時沒有用到這個
    // Start is called before the first frame update
    void Start()
    {
        player = UnityChanControlScriptWithRgidBody.instance;
        gm = GameManager.instance;
        pre_pos = transform.localPosition;
    }

    public void isSelected()
    {
        // gm.DetermineHandState();
        pre_pos = transform.localPosition;
        is_selected = true;
    }

    public void unSelected()
    {
        //gm.DetermineHandState();
        if (gm.is_left_hand_grab || gm.is_right_hand_grab || gm.is_left_hand_controller_grab || gm.is_right_hand_controller_grab) return;
        is_selected = false;
    }

    public void isMove()
    {
        // gm.DetermineHandState();
        if (!is_selected) return;
        player.is_climbing = true;
        movement_amount = transform.localPosition - pre_pos;
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@ movement_amount: " + movement_amount);
        player.IsClimbing(movement_amount*500, true);
        pre_pos = transform.localPosition;
    }

    public void isRelease()
    {
        //gm.DetermineHandState();
        if (gm.is_left_hand_grab || gm.is_right_hand_grab || gm.is_left_hand_controller_grab || gm.is_right_hand_controller_grab) return;
        player.is_climbing = false;
    }
}
