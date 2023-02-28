using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    EnemyWeaponSlotManager enemyWeaponSlotManager;

    [SerializeField]
    GameObject leftHandBone;
    [SerializeField]
    GameObject rightHandBone;
    public CombatStanceState combatStanceState;

    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (StateID != EnemyStates.Attacking)
            StateID = EnemyStates.Attacking;

        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        HandleRotationTowardsTarget(enemyManager);

        if (enemyManager.isPerformingAction)
        {
            ResetBlockedAttack();//Before moving to another state reset the blocked attack flag in animator to prevente from blocking other animations
            return combatStanceState;
        }
            

        if (currentAttack != null)
        {
            if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
            {
                return this;
            }
            else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
            {
                if (viewableAngle <= currentAttack.maximumAttackAngle && viewableAngle >= currentAttack.minimumAttackAngle)
                {
                    if (enemyManager.currentRecoveryTime <=0 && enemyManager.isPerformingAction == false)
                    {
                        if (enemyWeaponSlotManager == null)
                        {
                            enemyWeaponSlotManager = enemyManager.transform.GetComponentInChildren<EnemyWeaponSlotManager>();
                        }

                        if (!DetermineHandBeingUsed(enemyStats, enemyAnim))//If the player cannot use neither hand to attack either because it has no stamina or no valid weapon
                        {
                            return combatStanceState;
                        }

                        enemyAnim.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                        enemyAnim.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

                        ResetBlockedAttack();//Before doing another attack reset the blocked attack flag

                        enemyAnim.playEnemyTargetAnimation(currentAttack.actionAnimation, true);
                        enemyManager.isPerformingAction = true;
                        enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                        currentAttack = null;
                        return combatStanceState;
                    }
                }
            }
        }
        else
        {
            GetNewAttack(enemyManager);
        }

        return combatStanceState;
    }

    private bool DetermineHandBeingUsed(EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        if (enemyWeaponSlotManager.leftHandWeapon != null && !enemyWeaponSlotManager.leftHandWeapon.isUnarmed)//This is going to give problems if enemys can attack barehand
        {
            if (enemyStats.currentStamina < enemyWeaponSlotManager.leftHandWeapon.baseStamina * enemyWeaponSlotManager.leftHandWeapon.lightAttackMultiplier)
                return false;
            else
                enemyAnim.anim.SetBool("isUsingLeftHand",true);
        }
        else if (enemyWeaponSlotManager.rightHandWeapon != null && !enemyWeaponSlotManager.rightHandWeapon.isUnarmed)
        {
            if (enemyStats.currentStamina < enemyWeaponSlotManager.rightHandWeapon.baseStamina * enemyWeaponSlotManager.rightHandWeapon.lightAttackMultiplier)
                return false;
            else
                enemyAnim.anim.SetBool("isUsingRightHand", true);
        }
        //else//what if they have two weapons
        //{
        //    return false;
        //}

        return true;
    }

    private void ResetBlockedAttack()
    {
        if (enemyWeaponSlotManager.leftHandWeapon != null && !enemyWeaponSlotManager.leftHandWeapon.isUnarmed)//This is going to give problems if enemys can attack barehand
        {
            DamageCollider damageCollider = leftHandBone.GetComponentInChildren<DamageCollider>();
            if (damageCollider != null)
            {
                damageCollider.ResetBlockedAttackFlag();
            }
        }
        else if (enemyWeaponSlotManager.rightHandWeapon != null && !enemyWeaponSlotManager.rightHandWeapon.isUnarmed)
        {
            DamageCollider damageCollider = rightHandBone.GetComponentInChildren<DamageCollider>();
            if (damageCollider != null)
            {
                damageCollider.ResetBlockedAttackFlag();
            }
        }
        else
        {
            Debug.LogWarning("Not clear which hand is using to attack");
        }
    }

    private void GetNewAttack(EnemyManager enemyManager)
    {
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore);
        int temporaryScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    if (currentAttack != null)
                        return;

                    temporaryScore += enemyAttackAction.attackScore;

                    if (temporaryScore > randomValue)
                    {
                        currentAttack = enemyAttackAction;
                    }
                }
            }
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
