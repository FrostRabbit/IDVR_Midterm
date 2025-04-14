using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorCore : MonoBehaviour
{
    protected int HP;
    protected int ATK;
    public ParticleSystem deathEffect, hurtEffect, healEffect;
    protected float effectSize;
    protected Color color;
    public CreatorSO creator;
    protected int max_hp;
    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public virtual void SetUp()
    {
        ATK = creator.atk;
        HP = creator.health;
        max_hp = HP;
        effectSize = creator.explosion_size;
        //init_blood_scale = Blood_UI.transform.localScale.x;
        color = creator.color;
        if (HP <= 0) Destroy(transform.parent.gameObject);
    }
    public virtual void Move()
    {
        // time_lock += Time.deltaTime;
        //agent.SetDestination(player.transform.position);
    }

    public virtual void Behurt(int damage)
    {
        if (HP - damage > 0)
        {
            ParticleSystem df;
            Debug.Log("WWWWW!!!QQQ");
            if (damage >= 0)
            {
                df = Instantiate(hurtEffect, transform.position, Quaternion.identity);
                //df = Instantiate(hurtEffect, gameObject.transform.position, Quaternion.identity, Camera.main.gameObject.transform);
            }
            else df = Instantiate(healEffect, transform.position, Quaternion.identity);
            ParticleSystem.MainModule psmain = df.main;
            df.GetComponent<Renderer>().material.color = color;
            psmain.startColor = color;
            df.transform.localScale = new Vector3(effectSize * 0.3f, effectSize * 0.3f, 1);
            //UpdateBloodUI(damage);
            HP -= damage;
        }
        else Die();
    }

    /*private void UpdateBloodUI(int damage)
    {
        Blood_UI.transform.localScale -= Vector3.right * damage / max_hp * init_blood_scale;
    }*/

    public virtual void Die()
    {
        ParticleSystem df = Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        ParticleSystem.MainModule psmain = df.main;
        psmain.startColor = color;
        df.GetComponent<Renderer>().material.color = color;
        df.transform.localScale *= effectSize;

        /*AudioSource s = transform.GetComponent<AudioSource>();
        if (s == null) Debug.LogWarning("Sound: " + name + "not found!");
        else s.Play();*/

        Destroy(df.gameObject, 1);
        Destroy(gameObject);
    }
}
