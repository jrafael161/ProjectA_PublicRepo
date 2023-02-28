using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponSlotManager : MonoBehaviour
{
    public WeaponItem rightHandWeapon;
    public WeaponItem leftHandWeapon;

    WeaponHolderSlot rightHandSlot;
    WeaponHolderSlot leftHandSlot;

    DamageCollider rightHandDamageCollider;
    DamageCollider leftHandDamageCollider;

    EnemyStats enemyStats;
    Animator animator;

    private void Start()
    {
        CreateNewInstancesOfItems();
        enemyStats = GetComponentInParent<EnemyStats>();
        animator = GetComponent<Animator>();

        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
        {
            if (weaponSlot.isLeftHand)
            {
                leftHandSlot = weaponSlot;
            }
            else if (weaponSlot.isRightHand)
            {
                rightHandSlot = weaponSlot;
            }
        }

        LoadWeaponsOnBothHands();
    }
    public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.currentWeapon = weapon;
            leftHandSlot.LoadWeaponModel(weapon);
            LoadWeaponsDamageCollider(true);
        }
        else
        {
            rightHandSlot.currentWeapon = weapon;
            rightHandSlot.LoadWeaponModel(weapon);
            LoadWeaponsDamageCollider(false);
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        if (rightHandWeapon != null)
        {
            LoadWeaponOnSlot(rightHandWeapon, false);
        }
        if (leftHandWeapon != null)
        {
            LoadWeaponOnSlot(leftHandWeapon, true);
        }
    }
    public void LoadWeaponsDamageCollider(bool isLeft)
    {
        if (isLeft)
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.InitializeWeaponDamage(leftHandWeapon);
            leftHandDamageCollider.theOneDoingDamage = GetComponentInParent<EnemyManager>().gameObject;
        }
        else
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.InitializeWeaponDamage(rightHandWeapon);
            rightHandDamageCollider.theOneDoingDamage = GetComponentInParent<EnemyManager>().gameObject;
        }
    }

    public void CreateNewInstancesOfItems()
    {
        if (rightHandWeapon != null)
        {
            WeaponItem weaponItem = Instantiate(rightHandWeapon);
            rightHandWeapon = null;
            rightHandWeapon = weaponItem;
        }

        if (leftHandWeapon != null)
        {
            WeaponItem weaponItem = Instantiate(leftHandWeapon);
            leftHandWeapon = null;
            leftHandWeapon = weaponItem;
        }
    }

    public void EnableHandDamageCollider()
    {
        if (animator.GetBool("isUsingLeftHand"))
        {
            leftHandDamageCollider.EnableDamageCollider();
        }
        else if (animator.GetBool("isUsingRightHand"))
        {
            rightHandDamageCollider.EnableDamageCollider();
        }
    }

    public void DisableHandDamageCollider()
    {
        if (animator.GetBool("isUsingLeftHand"))
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
        else if (animator.GetBool("isUsingRightHand"))
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
    }

    public void DrainStaminaLightAttack()
    {
        bool isLeft = animator.GetBool("isUsingLeftHand");
        if (isLeft)
        {
            enemyStats.ConsumeStamina(Mathf.RoundToInt(leftHandSlot.currentWeapon.baseStamina * leftHandSlot.currentWeapon.lightAttackMultiplier));
        }
        else
        {
            enemyStats.ConsumeStamina(Mathf.RoundToInt(rightHandSlot.currentWeapon.baseStamina * rightHandSlot.currentWeapon.lightAttackMultiplier));
        }
    }

    public void DrainStaminaHeavyAttack()
    {
        bool isLeft = animator.GetBool("isUsingLeftHand");
        if (isLeft)
        {
            enemyStats.ConsumeStamina(Mathf.RoundToInt(leftHandSlot.currentWeapon.baseStamina * leftHandSlot.currentWeapon.lightAttackMultiplier));
        }
        else
        {
            enemyStats.ConsumeStamina(Mathf.RoundToInt(rightHandSlot.currentWeapon.baseStamina * rightHandSlot.currentWeapon.lightAttackMultiplier));
        }
    }

    public void EnableCombo()
    {
        //anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        //anim.SetBool("canDoCombo", false);
    }

    public void TrySpawnWeaponEffect()
    {
        Debug.Log("Spawn enemy weapon effects");
        //Spawn enemy weapon effects
    }
}
