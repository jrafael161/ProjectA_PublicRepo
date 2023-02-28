using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IddleState : State
{
    public PersueTargetState persueTargetState;
    public LayerMask detectionLayer;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (StateID != EnemyStates.Iddle)
            StateID = EnemyStates.Iddle;

        //Look for a potential target
        //Switch to the persue target state if target is found
        //If not return this state

        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponentInParent<CharacterStats>();
            
            if (characterStats != null)
            {
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    enemyManager.currentTarget = characterStats;
                }
            }
        }

        if (enemyManager.currentTarget.currentHealth <=0)
        {
            return this;
        }
        else if (enemyManager.currentTarget != null)
        {
            return persueTargetState;
        }
        else
        {
            return this;
        }        
    }
}
