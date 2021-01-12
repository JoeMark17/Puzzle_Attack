using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 puloc;

    public Transform player;
    public Transform enemy;
    public bool hasKey = false;
    public LayerMask whatIsGround, whatIsPlayer, whatIsKey;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    public float sightRange, attackRange, keyRange;
    public bool playerInSightRange, playerInAttackRange, playerInKeyRange;

    private void Awake() 
    {
        Debug.Log("Awaken");
        player = GameObject.Find("Player").transform;    
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() 
    {
        //Check for sight and attack range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //If game object = piece and that transfrom is greater than 1f (so it is not bouncing off piece on the ground - then bounce back)
        // if (keyInBounceRange && key.transform.position.y > -18.29f ) 
        // {
        //     enemy.AddForce (-transform.forward * 1000f * Time.deltaTime);
        // }

        FindClosestKey();

        // if (!playerInSightRange && !playerInAttackRange) Patrolling();
        // if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        // if (playerInAttackRange && playerInSightRange) AttackPlayer();

        
    }
    
    private void Patrolling()
    {
        Debug.Log("Patrolling");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached.
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        Debug.Log("Seach Walk");
        //Calculate random point in range.
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3 (transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        Debug.Log("Chase Player");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack Player");
        //Make sure enemy doesn't move.
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ////Attack code here
            //Will be melee attack

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void FindClosestKey()
    {
        float distanceToClosestKey = Mathf.Infinity;
        GameObject closestKey = null;
        GameObject[] allKeys = GameObject.FindGameObjectsWithTag("Key");

        foreach (GameObject currentKey in allKeys)
        {
            float distanceToKey = (currentKey.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToKey < distanceToClosestKey)
            {
                distanceToClosestKey = distanceToKey;
                closestKey = currentKey;
            }
        }
        Debug.DrawLine (this.transform.position, closestKey.transform.position);
        agent.SetDestination(closestKey.transform.position);

        //float pickupKeyDistance = Vector3.Distance (player.position, closestKey.transform.position);
        //Debug.Log(pickupKeyDistance);

        playerInKeyRange = Physics.CheckSphere(transform.position, keyRange, whatIsKey);

        if (playerInKeyRange)
        {
            Debug.Log("Pick Up Key!");

            ////This will call the function in the throw object script so the key wil be a child of the enemy.
            //EnemyKeyGrab();

            hasKey = true;
        }
    }
}
