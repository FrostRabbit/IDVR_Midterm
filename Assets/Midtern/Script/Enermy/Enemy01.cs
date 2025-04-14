using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class Enemy01 : EnemyCore
{
    private NavMeshAgent agent;
    private PlayerDetector playerDetector;
    private bool is_detected_player = false;
    private Vector3 startPoint;
    public float wanderRadius;
    private bool find_path = false;
    public override void SetUp()
    {
        agent = GetComponent<NavMeshAgent>();

        playerDetector = GetComponent<PlayerDetector>();
        //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@ playerDetector: " + playerDetector.name);
        startPoint = transform.position;
        base.SetUp();
    }
    public override void Move()
    {
        base.Move();
        is_detected_player = playerDetector.CanDetectPlayer();
        if (is_detected_player)
        {
            find_path = true;
            if (playerDetector.CanAttackPlayer())
            {
                // atk
                //Debug.Log("@@@@@@@@@@@@@@@@@@ ATK");
                ATK_Animation();
                agent.SetDestination(playerDetector.Player.transform.position);
            }
            else
            {
                //Debug.Log("@@@@@@@@@@@@@@@@@@ Findddd");
                defualt_Animation();
                agent.SetDestination(playerDetector.Player.transform.position);
                
            }
        }
        else
        {
            if (find_path)
            {
                find_path = false;
                agent.ResetPath();
            }
            
            //Debug.Log("@@@@@@@@@@@@@@@@@@ !agent.hasPath"+ agent.hasPath);
            if (HasReachedDestination() || !agent.hasPath) //agent.pathStatus = NavMeshPathStatus.PathInvalid
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += startPoint;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                Vector3 finalPosition = hit.position;

                agent.SetDestination(finalPosition);
            }
        }
        bool HasReachedDestination()
        {
            return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }

    }

    public override void Behurt(int damage)
    {
        agent.SetDestination(playerDetector.Player.transform.position);
        Hurt_Animation();
        base.Behurt(damage);
    }

    public override void Die()
    {
        Death_Animation();
        base.Die();
    }
}
