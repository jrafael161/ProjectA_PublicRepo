using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    public EnemyData enemyTemplate;
    public EnemyHealthBarUI enemyHealthBarUI;
    EnemyMovementController enemyMovement;
    EnemyAnimControler enemyAnim;
    EnemyStats enemyStats;
    
    public State currentState;
    public CharacterStats currentTarget;
    public NavMeshAgent navMeshAgent;
    public Rigidbody enemyRigidBody;

    public bool isPerformingAction;
    public bool isInteracting;
    public bool isGrounded;
    public bool isInAir;

    public float rotationSpeed = 25;
    public float maximumAttackRange = 1.5f;

    [Header("A.I. Settings")]
    public float detectionRadius = 20;
    //Cone of view (FOV)
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;

    public float currentRecoveryTime = 0;

    public bool isBlocking;

    public float waitTimeBetweenStateChange = .5f;
    public float stateChangeTimer = 0;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovementController>();
        enemyAnim = GetComponentInChildren<EnemyAnimControler>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
        if (enemyHealthBarUI == null)
        {
            enemyHealthBarUI = GetComponentInChildren<EnemyHealthBarUI>();
        }
        isGrounded = true;
        enemyRigidBody.isKinematic = false;
        navMeshAgent.enabled = false;
        stateChangeTimer = Time.time;
    }

    private void Update()
    {
        HandleRecoveryTime();
        isInteracting = enemyAnim.anim.GetBool("isInteracting");
        enemyStats.RegenerateStamina();
    }

    private void FixedUpdate()
    {
        HandleStateMachine();
        enemyMovement.HandleFalling(Time.deltaTime, transform.forward);
    }

    private void HandleStateMachine()
    {
        if (currentState != null)
        {
            if (enemyStats.isDead)
            {
                currentState = null;
                return;
            }
            State nextState = currentState.Tick(this, enemyStats, enemyAnim);
            if (nextState != null)
            {
                if (Time.time - stateChangeTimer >= waitTimeBetweenStateChange)
                {
                    SwithToNextState(nextState);
                    stateChangeTimer = Time.time;
                    return;
                }
            }

        }
    }

    private void SwithToNextState(State state)
    {
        if (state.StateID == EnemyStates.Pursuing)
        {
            enemyMovement.characterColliderBlocker.gameObject.SetActive(false);
        }
        else
        {
            enemyMovement.characterColliderBlocker.gameObject.SetActive(true);
        }
        currentState = state;
    }

    private void HandleRecoveryTime()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }
        if (isPerformingAction)
        {
            if (currentRecoveryTime <= 0)
            {
                isPerformingAction = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
