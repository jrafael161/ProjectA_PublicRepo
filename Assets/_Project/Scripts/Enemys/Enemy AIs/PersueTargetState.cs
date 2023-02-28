using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersueTargetState : State
{
    public CombatStanceState combatStanceState;
    public FallingState fallingState;
    public IddleState iddleState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (StateID != EnemyStates.Pursuing)
            StateID = EnemyStates.Pursuing;

        //Chase the target
        //If within attack range, return combat stance state
        //If target is out of range, retun this state and continue to chase the target
        if (enemyManager.isPerformingAction)
        {
            enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }

        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position,enemyManager.transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

        if (distanceFromTarget > enemyManager.maximumAttackRange)
        {
            enemyAnim.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }

        HandleRotationTowardsTarget(enemyManager);

        enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
        enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

        if (enemyManager.currentTarget.currentHealth <=0)
        {
            return iddleState;
        }
        else if (distanceFromTarget <= enemyManager.maximumAttackRange)
        {
            return combatStanceState;
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
