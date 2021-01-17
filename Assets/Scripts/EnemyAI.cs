using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float enemyHealth = 100f;
    public NavMeshAgent agent;
    private Vector3 puloc;
    GameObject holdingKey;

    public Transform player;
    public Transform enemy;
    public bool hasKey = false;
    public LayerMask whatIsGround, whatIsPlayer, whatIsKey;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float throwForce;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public bool isCarried;
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

        if(enemyHealth <= 0f)
        {
            Destroy(enemy);
        }
        //Check for sight and attack range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        int keyCount = enemy.transform.childCount;

        if (keyCount == 0)
        {
            FindClosestKey();
        }

        else
        {
            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }        
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
            Debug.Log("Threw the key");

            holdingKey.GetComponent<ThrowObject>().EnemyThrowKey(enemy);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);   
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void FindClosestKey()
    {
        float distanceToClosestKey = Mathf.Infinity;
        GameObject closestKey = null;
        GameObject[] allKeys = GameObject.FindGameObjectsWithTag("Key");

        foreach (GameObject currentKey in allKeys)
        {
            float distanceToKey = (currentKey.transform.position - this.transform.position).sqrMagnitude;

            //bool isThrown = FindObjectOfType<ThrowObject>().catchBool;
            //ThrowObject keyScript = currentKey.GetComponent<ThrowObject>();
            //isCarried = keyScript.beingCarried;

            if (distanceToKey < distanceToClosestKey && isThrown == false)
            {
                distanceToClosestKey = distanceToKey;
                closestKey = currentKey;
            }
        }
        Debug.DrawLine (this.transform.position, closestKey.transform.position);
        agent.SetDestination(closestKey.transform.position);

        playerInKeyRange = Physics.CheckSphere(transform.position, keyRange, whatIsKey);

        if (playerInKeyRange)
        {
            //GameObject pickUpKey = GameObject.Find(closestKey);
            closestKey.GetComponent<ThrowObject>().EnemyPickUp(enemy);

            holdingKey = closestKey;

            hasKey = true;
        }
    }

    public void TakeDmg (float dmg)
    {
        enemyHealth =- dmg;
    }
}
