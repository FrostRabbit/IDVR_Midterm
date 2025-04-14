using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEnemyScript : MonoBehaviour
{
    public EnemyCore enemy;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@collision.transform.tag: " + collision.transform.tag);
    }

}
