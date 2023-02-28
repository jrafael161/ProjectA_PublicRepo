using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushState : State
{
    public bool isSleeping;
    public float detectionRadiusWhileSleeping = 5;
    public string sleepAnimation;
    public string wakeAnimation;

    public LayerMask detectionLayer;

    public PersueTargetState persueTargetState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (StateID != EnemyStates.Ambush)
            StateID = EnemyStates.Ambush;

        if (enemyAnim.anim.GetBool("gotHurt"))
            isSleeping = false;

        if (isSleeping && enemyManager.isInteracting == false)
        {
            enemyAnim.playEnemyTargetAnimation(sleepAnimation, true);
        }

        Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadiusWhileSleeping, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponentInParent<CharacterStats>();
            if (characterStats != null && colliders[i].tag != "Enemy")//This needs to be changed if enemies can attack each other when they are hurt by another enemies, mayby change their tag to hostile or state
            {
                Vector3 targetDirection = characterStats.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    enemyManager.currentTarget = characterStats;
                    isSleeping = false;
                    enemyAnim.playEnemyTargetAnimation(wakeAnimation, true);
                }
            }
        }

        if (enemyManager.currentTarget != null)
        {
            return persueTargetState;
        }
        else
        {
            return this;
        }
    }
}
