using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCore : CreatorCore
{
    protected float atk_rate = 0.5f;
    // lock trigger collision
    private float time_lock = 999;
    Vector3 pre_pos;
    private float is_move_timer = 999;
    GameManager gm;
    // Start is called before the first frame update

    public override void SetUp()
    {
        base.SetUp();
        pre_pos = transform.position;
        gm = GameManager.instance;
    }
    public override void Move()
    {
        base.Move();
        time_lock += Time.deltaTime;
        is_move_timer += Time.deltaTime;
        if(is_move_timer > 1f)
        {
            is_move_timer = 0;
            if(Mathf.Abs((transform.position - pre_pos).magnitude) < 0.01f) transform.gameObject.tag = "Untagged";
            pre_pos = transform.position;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            //PlayerController.instance.Behurt(ATK);
            col.GetComponent<GetEnemyScript>().enemy.Behurt(ATK);
            Debug.Log("@@@@@@@@@@@@@@@@@@ Enemy");
            Die();
        }
        if (col.CompareTag("PlayerHand"))
        {
            gm.DetermineHandState();
            transform.gameObject.tag = "Item";
            if(creator.name == "RockL") gm.player_grab = 4;
            else if(creator.name == "RockM") gm.player_grab = 3;
            else gm.player_grab = 2;
        }
        time_lock = 0;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(transform.tag == "Item") { 
            if(collision.transform.GetComponent<GetEnemyScript>() != null)
            {
                collision.transform.GetComponent<GetEnemyScript>().enemy.Behurt(ATK);
                Die();
            }
        }
    }
}
