using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PersueTargetState persueTargetState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (StateID != EnemyStates.CombatStance)
            StateID = EnemyStates.CombatStance;
        //Check for attack range
        //Potentially circle player or walk around them
        //If in attack range return attack state
        //If we are in a cool down after attacking, return this and continue circling the player
        //If the player runs out of range return PersueTarget state
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        HandleRotationTowardsTarget(enemyManager);

        if (enemyManager.isPerformingAction)
        {
            enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        }

        if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange)
        {
            return attackState;
        }
        else if (distanceFromTarget > enemyManager.maximumAttackRange)
        {
            return persueTargetState;
        }
        else
        {
            return this;
        }        
    }

    private void HandleRotationTowardsTarget(EnemyManager enemyManager)
    {
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed * Time.deltaTime);
        }
    }
}
