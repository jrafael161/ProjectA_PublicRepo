using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    public WeaponItem unarmedWeapon;

    public WeaponHolderSlot leftHandSlot;//Need a way to unify this and the slots in the player inventory maybe event? to assign the weapon item both ways?
    public WeaponHolderSlot rightHandSlot;
    public WeaponHolderSlot backSlot;

    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;

    Animator animator;

    PlayerStats playerStats;
    PlayerInventory playerInventory;
    QuickSlotsUI quickSlotsUI;
    InputHandler inputHandler;
    PlayerManager playerManager;
    PlayerAnimController playerAnim;
    PlayerCombatHandler playerCombatHandler;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerStats = GetComponentInParent<PlayerStats>();
        quickSlotsUI = playerManager.UIManager.gameObject.GetComponentInChildren<QuickSlotsUI>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        playerAnim = GetComponentInChildren<PlayerAnimController>();
        playerCombatHandler = GetComponent<PlayerCombatHandler>();

        CreateUnarmedWeaponInstances();
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
            else if (weaponSlot.isBackSlot)
            {
                backSlot = weaponSlot;
            }
        }        
    }

    public void CreateUnarmedWeaponInstances()
    {
        if (unarmedWeapon != null)
        {
            WeaponItem weaponItem = Instantiate(unarmedWeapon);
            unarmedWeapon = null;
            unarmedWeapon = weaponItem;
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (weaponItem == null || weaponItem.itemID == unarmedWeapon.itemID)
        {
            weaponItem = unarmedWeapon;
        }

        if (!weaponItem.isUnarmed)//The unarmed weapon is used to fill the contrary hand when a two handed weapon stops being two handed
        {
            inputHandler.twoHandedFlag = false;
            playerManager.isTwoHandingWeapon = false;
        }

        if (weaponItem.weaponType == WeaponType.Bow)
        {
            if (isLeft && playerStats.Handedness == Handedness.LeftHanded)
            {
                inputHandler.twoHandedFlag = true;
                playerCombatHandler.HandleTwoHandingWeapon();
            }
            else if(!isLeft && playerStats.Handedness == Handedness.RightHanded)
            {
                inputHandler.twoHandedFlag = true;
                playerCombatHandler.HandleTwoHandingWeapon();
            }
            else if(!isLeft && playerStats.Handedness == Handedness.Ambidextrous)
            {
                inputHandler.twoHandedFlag = true;
                playerCombatHandler.HandleTwoHandingWeapon();
            }
            else
            {
                //If the bow is being equipped in the non dominant hand dont try to 2 hand it
            }
        }

        if (isLeft)
        {
            leftHandSlot.currentWeapon = weaponItem;
            playerInventory.ChangeCurrentWeapon(leftHandSlot);
            leftHandSlot.LoadWeaponModel(weaponItem);
            GetLeftWeaponDamageCollider(weaponItem);
            quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
            playerAnim.anim.runtimeAnimatorController = weaponItem.weaponAnimController;

            if (weaponItem.isUnarmed)
            {
                playerAnim.playTargetAnimation("Left Arm Empty", false);
            }
            else
            {
                playerAnim.playTargetAnimation("Left_Arm_Idle_01", false);
            }
            
        }
        else
        {
            rightHandSlot.currentWeapon = weaponItem;
            playerInventory.ChangeCurrentWeapon(rightHandSlot);
            rightHandSlot.LoadWeaponModel(weaponItem);
            GetRightWeaponDamageCollider(weaponItem);
            quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
            playerAnim.anim.runtimeAnimatorController = weaponItem.weaponAnimController;
            
            if (weaponItem.isUnarmed)
            {
                playerAnim.playTargetAnimation("Right Arm Empty", false);
            }
            else
            {
                playerAnim.playTargetAnimation("Right_Arm_Idle_01", false);
            }
        }
    }

    private void GetLeftWeaponDamageCollider(WeaponItem weaponItem)
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        leftHandDamageCollider.InitializeWeaponDamage(weaponItem);
        leftHandDamageCollider.theOneDoingDamage = playerManager.gameObject;
    }

    private void GetRightWeaponDamageCollider(WeaponItem weaponItem)
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        rightHandDamageCollider.InitializeWeaponDamage(weaponItem);
        rightHandDamageCollider.theOneDoingDamage = playerManager.gameObject;
    }

    public void EnableHandDamageCollider()
    {
        if (playerManager.isUsingLeftHand)
        {
            leftHandDamageCollider.EnableDamageCollider();
        }
        else if(playerManager.isUsingRightHand)
        {
            rightHandDamageCollider.EnableDamageCollider();
        }        
    }
    
    public void DisableHandDamageCollider()
    {
        if (playerManager.isUsingLeftHand)
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
        else if (playerManager.isUsingRightHand)
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
    }

    public void DrainStaminaLightAttack()
    {
        bool isLeft = animator.GetBool("isUsingLeftHand");
        if (isLeft)
        {
            playerStats.ConsumeStamina(Mathf.RoundToInt(leftHandSlot.currentWeapon.baseStamina * leftHandSlot.currentWeapon.lightAttackMultiplier));
        }
        else
        {
            playerStats.ConsumeStamina(Mathf.RoundToInt(rightHandSlot.currentWeapon.baseStamina * rightHandSlot.currentWeapon.lightAttackMultiplier));
        }
    }

    public void DrainStaminaHeavyAttack()
    {
        bool isLeft = animator.GetBool("isUsingLeftHand");
        if (isLeft)
        {
            playerStats.ConsumeStamina(Mathf.RoundToInt(leftHandSlot.currentWeapon.baseStamina * leftHandSlot.currentWeapon.lightAttackMultiplier));
        }
        else
        {
            playerStats.ConsumeStamina(Mathf.RoundToInt(rightHandSlot.currentWeapon.baseStamina * rightHandSlot.currentWeapon.lightAttackMultiplier));
        }
    }

    public void DigFromAnimation() 
    {
        if (playerManager.isUsingLeftHand)
        {
            playerCombatHandler.Dig(leftHandSlot.currentWeapon);
        }
        else if (playerManager.isUsingRightHand)
        {
            playerCombatHandler.Dig(rightHandSlot.currentWeapon);
        }
    }

    public void UnsheathWeapon()
    {
        backSlot.UnloadWeapon();
        if (backSlot.currentWeapon != null)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    LoadWeaponOnSlot(backSlot.currentWeapon, true);
                    break;
                case Handedness.LeftHanded:
                    LoadWeaponOnSlot(backSlot.currentWeapon, false);
                    break;
                case Handedness.Ambidextrous:
                    LoadWeaponOnSlot(backSlot.currentWeapon, true);
                    break;
                default:
                    break;
            }
            backSlot.currentWeapon = null;
            playerInventory.ChangeCurrentWeapon(backSlot);
            playerManager.isTwoHandingWeapon = false;
        }
    }

    public void SheathWeapon(bool isLeftWeapon)
    {
        if (isLeftWeapon)
        {
            playerAnim.playTargetAnimation("SheathLeftHand", true);

            //animator.Play("SheathLeftHand");
        }
        else
        {
            playerAnim.playTargetAnimation("SheathRightHand", true);
            //animator.Play("SheathRightHand");
        }
        playerAnim.playTargetAnimation("Left Arm Empty", false);
        playerAnim.playTargetAnimation("Right Arm Empty", false);
        StartCoroutine("SheathWeaponWait", isLeftWeapon);
    }

    public void TrySpawnWeaponEffect()
    {
        if (playerManager.isUsingLeftHand)
        {
            if (leftHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>() != null)
            {
                if ((int)leftHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>().elementLevel > 1)
                {
                    GameObject WeaponEffect = Instantiate(leftHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>().WeaponSlashFX, playerManager.lockOnTransform.position, playerManager.transform.rotation);
                    WeaponEffect.SetActive(true);
                    Destroy(WeaponEffect, 10);
                }
            }
        }
        else if (playerManager.isUsingRightHand)
        {
            if (rightHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>() != null)
            {
                if ((int)rightHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>().elementLevel > 1)
                {
                    GameObject WeaponEffect = Instantiate(rightHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>().WeaponSlashFX, playerManager.lockOnTransform.position, playerManager.transform.rotation);
                    WeaponEffect.SetActive(true);
                    Destroy(WeaponEffect, 10);
                }
            }
        }
    }

    IEnumerator SheathWeaponWait(bool isLeftWeapon)
    {
        yield return new WaitForSeconds(.3f);
        if (isLeftWeapon)
        {
            leftHandSlot.UnloadWeapon();
            leftHandSlot.currentWeapon = unarmedWeapon;
            playerInventory.ChangeCurrentWeapon(leftHandSlot);
        }
        else
        {
            rightHandSlot.UnloadWeapon();
            rightHandSlot.currentWeapon = unarmedWeapon;
            playerInventory.ChangeCurrentWeapon(rightHandSlot);
        }
        backSlot.LoadWeaponModel(backSlot.currentWeapon);
    }

    public void Test2()//Functions that are called by animations events need to be in the sabe gameobject as the animation controller, they acan be in another script but in the same parent object
    {
        Debug.Log("Test");
    }

}
