using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public GameObject theOneDoingDamage;
    Collider damageCollider;
    int currentWeaponDamage=0;
    [SerializeField]
    bool attackblocked = false;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void InitializeWeaponDamage(WeaponItem weaponItem)
    {
        currentWeaponDamage = weaponItem.weaponDamage;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "BlockCollider")
        {
            PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
            EnemyManager enemyManager = collision.GetComponentInParent<EnemyManager>();
            if (playerManager != null)
            {
                if (playerManager.isBlocking)
                {
                    PlayerStats playerStats = collision.GetComponentInParent<PlayerStats>();
                    if (playerStats.currentStamina > (float)currentWeaponDamage / 2)
                    {
                        playerStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        EnemyAnimControler enemyAnimControler = GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.anim.SetBool("attackWasBlocked",true);
                        enemyAnimControler.playEnemyTargetAnimation("AttackBlocked", true);
                        EnemyWeaponSlotManager enemyWeaponSlotManager = GetComponentInParent<EnemyWeaponSlotManager>();//Disables the damage collider because the attack animation is cut and doesnt disables it
                        enemyWeaponSlotManager.DisableHandDamageCollider();

                        PlayerAnimController playerAnimController = collision.GetComponentInParent<PlayerAnimController>();//The block collider needs to be a children of the gameobject where the AnimControlles is located
                        playerAnimController.playTargetAnimation("BlockedAttack", true);

                        attackblocked = true;//Reset blocked attack when a new attack is initiated
                    }
                    else
                    {
                        playerStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        EnemyWeaponSlotManager enemyWeaponSlotManager = GetComponentInParent<EnemyWeaponSlotManager>();
                        enemyWeaponSlotManager.DisableHandDamageCollider();

                        PlayerAnimController playerAnimController = collision.GetComponentInParent<PlayerAnimController>();
                        playerAnimController.playTargetAnimation("GuardBroken", true);

                        attackblocked = true;
                        collision.GetComponentInParent<PlayerCombatHandler>().CancelBlock();
                    }
                }
            }
            else if (enemyManager != null)
            {
                if (enemyManager.isBlocking)
                {
                    EnemyStats enemyStats = collision.GetComponentInParent<EnemyStats>();
                    if (enemyStats.currentStamina > (float)currentWeaponDamage / 2)
                    {
                        enemyStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        PlayerAnimController playerAnimController = GetComponentInParent<PlayerAnimController>();
                        playerAnimController.anim.SetBool("attackWasBlocked", true);
                        playerAnimController.playTargetAnimation("AttackBlocked", true);
                        WeaponSlotManager weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
                        weaponSlotManager.DisableHandDamageCollider();

                        EnemyAnimControler enemyAnimControler = collision.GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.playEnemyTargetAnimation("BlockedAttack", true);

                        attackblocked = true;
                    }
                    else
                    {
                        enemyStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        WeaponSlotManager weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
                        weaponSlotManager.DisableHandDamageCollider();

                        EnemyAnimControler enemyAnimControler = collision.GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.playEnemyTargetAnimation("GuardBroken", true);

                        attackblocked = true;
                        //collision.GetComponentInParent<EnemyAttackAction>().CancelBlock();
                    }
                }
            }
            return;
        }

        if (collision.tag == "Weapon")
        {
            PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
            EnemyManager enemyManager = collision.GetComponentInParent<EnemyManager>();
            if (playerManager != null)
            {
                if (playerManager.isBlocking)
                {
                    PlayerStats playerStats = collision.GetComponentInParent<PlayerStats>();
                    if (playerStats.currentStamina > (float)currentWeaponDamage / 2)
                    {
                        playerStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        EnemyAnimControler enemyAnimControler = collision.GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.anim.SetBool("attackWasBlocked", true);
                        enemyAnimControler.playEnemyTargetAnimation("AttackBlocked", true);
                        
                        PlayerAnimController playerAnimController = GetComponentInParent<PlayerAnimController>();
                        playerAnimController.playTargetAnimation("BlockedAttack", true);

                        attackblocked = true;
                    }
                    else
                    {
                        playerStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        PlayerAnimController playerAnimController = GetComponentInParent<PlayerAnimController>();
                        playerAnimController.playTargetAnimation("GuardBroken", true);

                        attackblocked = true;
                        collision.GetComponentInParent<PlayerCombatHandler>().CancelBlock();
                    }
                    return;
                }
            }
            else if (enemyManager != null)
            {
                if (enemyManager.isBlocking)
                {
                    EnemyStats enemyStats = collision.GetComponentInParent<EnemyStats>();
                    if (enemyStats.currentStamina > (float)currentWeaponDamage / 2)
                    {
                        enemyStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        PlayerAnimController playerAnimController = GetComponentInParent<PlayerAnimController>();
                        playerAnimController.anim.SetBool("attackWasBlocked", true);
                        playerAnimController.playTargetAnimation("AttackBlocked", true);

                        EnemyAnimControler enemyAnimControler = collision.GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.playEnemyTargetAnimation("BlockedAttack", true);

                        attackblocked = true;
                    }
                    else
                    {
                        enemyStats.ConsumeStamina(Mathf.FloorToInt((float)currentWeaponDamage / 2));

                        EnemyAnimControler enemyAnimControler = collision.GetComponentInParent<EnemyAnimControler>();
                        enemyAnimControler.playEnemyTargetAnimation("GuardBroken", true);

                        attackblocked = true;
                        //collision.GetComponentInParent<EnemyAttackAction>().CancelBlock();
                    }
                    return;
                }
            }
        }

        if (collision.tag == "Player" && !attackblocked)
        {
            PlayerStats playerStats = collision.GetComponentInParent<PlayerStats>();
            if (playerStats != null && collision.GetComponentInParent<PlayerManager>().gameObject != theOneDoingDamage)
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }

        if (collision.tag == "Enemy" && !attackblocked)
        {
            EnemyStats enemyStats = collision.GetComponentInParent<EnemyStats>();
            if (enemyStats != null && collision.GetComponentInParent<EnemyManager>().gameObject != theOneDoingDamage)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }
    }

    public void ResetBlockedAttackFlag()
    {
        PlayerManager playerManager = GetComponentInParent<PlayerManager>();
        EnemyManager enemyManager = GetComponentInParent<EnemyManager>();
        if (playerManager != null)
        {
            PlayerAnimController playerAnimController = GetComponentInParent<PlayerAnimController>();
            playerAnimController.anim.SetBool("attackWasBlocked", false);
            WeaponSlotManager weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
            weaponSlotManager.DisableHandDamageCollider();
        }
        if (enemyManager != null)
        {
            EnemyAnimControler enemyAnimControler = GetComponentInParent<EnemyAnimControler>();
            enemyAnimControler.anim.SetBool("attackWasBlocked", false);
            EnemyWeaponSlotManager enemyWeaponSlotManager = GetComponentInParent<EnemyWeaponSlotManager>();
            enemyWeaponSlotManager.DisableHandDamageCollider();
        }
            
        attackblocked = false;
    }
}
