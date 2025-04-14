using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCore : CreatorCore
{
    public Animator animator;
    // public GameObject Blood_UI;
    // protected float init_blood_scale;
    protected float atk_rate = 0.5f;
    protected float speed;
    
    // lock trigger collision
    private float time_lock = 999;
    EnemySO enemySO;
    // Game Manager
    // GameManager gm;
    // Enemy Manager
    // EnemyManager em;
    // Start is called before the first frame update

    public override void SetUp()
    {
        base.SetUp();
        // game manager
        /*gm = GameManager.instance;
        if (gm == null)
        {
            Debug.LogError("there needs to be an GameManager in the scene");
        }*/
        /*
         // enemy manager
        em = EnemyManager.instance;
        if (em == null)
        {
            Debug.LogError("there needs to be an EnemyManager in the scene");
        }
        */
        enemySO = creator as EnemySO;
        speed = enemySO.speed;
        atk_rate = enemySO.fire_rate;
        if (HP <= 0) Destroy(transform.parent.gameObject);
    }
    public override void Move()
    {
        base.Move();    
        time_lock += Time.deltaTime;
        //agent.SetDestination(player.transform.position);
        
    }
    public virtual void ATK_Animation()
    {
        animator.SetBool("IsAttack", true);
    }
    public virtual void defualt_Animation()
    {
        animator.SetBool("IsAttack", false);
    }
    public virtual void Hurt_Animation()
    {
        animator.SetTrigger("IsHurt");
    }
    public virtual void Death_Animation()
    {
        animator.SetTrigger("Death");
    }
    // 需要自帶 collider
    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@collision.transform.tag: " + collision.transform.tag);
    }
    void OnTriggerEnter(Collider col)
    {
        if (time_lock > atk_rate)
        {
            //check if the enemy hit the player
            if (col.CompareTag("Player"))
            {
                //PlayerController.instance.Behurt(ATK);
                Debug.Log("@@@@@@@@@@@@@@@@@@ Player");
                //Die();
            }
            else if (col.CompareTag("Item"))
            {
                //PlayerController.instance.Behurt(ATK);
                col.GetComponent<CreatorCore>().Behurt(ATK);
                Debug.Log("@@@@@@@@@@@@@@@@@@ Item");
                //Die();
            }
            time_lock = 0;
        }

    }*/
}
