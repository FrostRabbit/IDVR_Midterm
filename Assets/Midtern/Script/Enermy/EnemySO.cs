using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Midterm/Enemy")]
public class EnemySO : CreatorSO
{
    public int speed = 1;
    public float fire_rate = 1f;

    public bool is_bullet = false;
    public bool bullet_track = false;
    public int max_distance = 30;
    public void Print() 
    {
        Debug.Log(name + ": " + description);
    }
}
