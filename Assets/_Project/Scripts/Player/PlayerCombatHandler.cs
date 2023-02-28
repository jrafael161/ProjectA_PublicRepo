using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    PlayerAnimController playerAnim;
    PlayerManager playerManager;
    PlayerEffectsManager playerEffectsManager;
    PlayerStats playerStats;
    PlayerInventory playerInventory;
    InputHandler inputHandler;
    WeaponSlotManager weaponSlotManager;

    //[Header("Modification parameters")]
    //public BrushType brush = BrushType.Stalagmite;
    //public ActionType action = ActionType.Dig;
    //[Range(0, 7)] public int textureIndex=2;
    //[Range(0.5f, 10f)] public float size = 1;
    //[Range(0f, 1f)] public float opacity = 1;
    //[Tooltip("Enable to edit the terrain asynchronously and avoid impacting the frame rate too much.")]
    //public bool editAsynchronously = true;

    [Header("Attack Animations")]
    public string OneHand_LightAttack01 = "OneHandLightAttack01";
    public string OneHand_LightAttack02 = "OneHandLightAttack02";
    public string OneHand_HeavyAttack = "OneHandHeavyAttack01";
    public string TwoHand_LightAttack01 = "TwoHandLightAttack01";
    public string TwoHand_LightAttack02 = "TwoHandLightAttack02";
    public string TwoHand_HeavyAttack = "TwoHandHeavyAttack01";
    public string Block = "Block";

    public string lastAttack;
    public bool wasLastAttackMadeWithLeftHand;

    [SerializeField]
    GameObject _leftHandBone;
    [SerializeField]
    GameObject _rightHandBone;
    [SerializeField]
    Collider blockCollider;

    private void Start()
    {
        playerAnim = GetComponent<PlayerAnimController>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerEffectsManager = GetComponentInParent<PlayerEffectsManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        playerStats = GetComponentInParent<PlayerStats>();
        inputHandler = GetComponentInParent<InputHandler>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
        //diggerMasterRuntime = FindObjectOfType<DiggerMasterRuntime>();
        //diggerNavMeshMasterRuntime = FindObjectOfType<DiggerNavMeshRuntime>();
    }

    public void HandleRBAction()
    {
        if (playerManager.isTwoHandingWeapon)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    PerformRightWeaponAction(false);
                    break;
                case Handedness.LeftHanded:
                    PerformBlock(false);
                    break;
                case Handedness.Ambidextrous:
                    PerformRightWeaponAction(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            PerformRightWeaponAction(false);
        }
    }

    public void HandleLBAction()
    {
        if (playerManager.isTwoHandingWeapon)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    PerformBlock(true);
                    break;
                case Handedness.LeftHanded:
                    PerformLeftWeaponAction(false);
                    break;
                case Handedness.Ambidextrous:
                    PerformBlock(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            PerformLeftWeaponAction(false);
        }
    }

    public void HandleRTAction()
    {
        if (playerManager.isTwoHandingWeapon)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    PerformRightWeaponAction(true);
                    break;
                case Handedness.LeftHanded:
                    PerformBlock(false);
                    break;
                case Handedness.Ambidextrous:
                    PerformRightWeaponAction(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            PerformRightWeaponAction(true);
        }
    }

    public void HandleLTAction()
    {
        if (playerManager.isTwoHandingWeapon)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    PerformBlock(true);
                    break;
                case Handedness.LeftHanded:
                    PerformLeftWeaponAction(true);
                    break;
                case Handedness.Ambidextrous:
                    PerformBlock(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            PerformLeftWeaponAction(true);
        }
    }

    public void PerformRightWeaponAction(bool isTrigger)//maybe create an interface in weapon item so they can handle individually their actions?
    {
        if (playerManager.canDoCombo && !wasLastAttackMadeWithLeftHand)
        {
            inputHandler.comboFlag = true;
            playerAnim.anim.SetBool("isUsingRightHand", true);
            HandleWeaponCombo(playerInventory.rightHandWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;

            //if (playerManager.canDoCombo)
            //    return;

            if (playerInventory.rightHandWeapon.itemName.ToLower().Contains("pickaxe") && playerManager.isTwoHandingWeapon == false)
            {
                inputHandler.twoHandedFlag = true;
                HandleTwoHandingWeapon();
                return;
            }

            if (playerInventory.rightHandWeapon.weaponType == WeaponType.Bow && playerManager.isTwoHandingWeapon == false)
            {
                inputHandler.twoHandedFlag = true;
                HandleTwoHandingWeapon();
                return;
            }

            if (isTrigger)
            {
                playerAnim.anim.SetBool("isUsingRightHand", true);
                wasLastAttackMadeWithLeftHand = false;
                DamageCollider damageCollider = _rightHandBone.GetComponentInChildren<DamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.ResetBlockedAttackFlag();
                }
                HandleHeavyAttack(playerInventory.rightHandWeapon);
            }
            else
            {
                playerAnim.anim.SetBool("isUsingRightHand", true);
                wasLastAttackMadeWithLeftHand = false;
                DamageCollider damageCollider = _rightHandBone.GetComponentInChildren<DamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.ResetBlockedAttackFlag();
                }
                HandleLightAttack(playerInventory.rightHandWeapon);
            }
        }
    }

    public void PerformLeftWeaponAction(bool isTrigger)
    {
        if (playerManager.canDoCombo && wasLastAttackMadeWithLeftHand)
        {
            inputHandler.comboFlag = true;
            playerAnim.anim.SetBool("isUsingLeftHand", true);
            HandleWeaponCombo(playerInventory.leftHandWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;

            //if (playerManager.canDoCombo)
            //    return;

            if (playerInventory.leftHandWeapon.itemName.ToLower().Contains("pickaxe") && playerManager.isTwoHandingWeapon == false)
            {
                inputHandler.twoHandedFlag = true;
                HandleTwoHandingWeapon();
                return;
            }

            if (playerInventory.leftHandWeapon.weaponType == WeaponType.Bow && playerManager.isTwoHandingWeapon == false)
            {
                inputHandler.twoHandedFlag = true;
                HandleTwoHandingWeapon();
                return;
            }

            if (isTrigger)
            {
                playerAnim.anim.SetBool("isUsingLeftHand", true);
                wasLastAttackMadeWithLeftHand = true;
                DamageCollider damageCollider = _leftHandBone.GetComponentInChildren<DamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.ResetBlockedAttackFlag();
                }
                HandleHeavyAttack(playerInventory.leftHandWeapon);
            }
            else
            {
                playerAnim.anim.SetBool("isUsingLeftHand", true);
                wasLastAttackMadeWithLeftHand = true;
                DamageCollider damageCollider = _leftHandBone.GetComponentInChildren<DamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.ResetBlockedAttackFlag();
                }
                HandleLightAttack(playerInventory.leftHandWeapon);
            }
        }
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        playerAnim.anim.runtimeAnimatorController = weapon.weaponAnimController;

        if (playerStats.currentStamina < (weapon.baseStamina * weapon.lightAttackMultiplier))
            return;

        if (inputHandler.comboFlag)
        {
            playerAnim.anim.SetBool("canDoCombo", false);

            if (lastAttack == OneHand_LightAttack01)
            {
                playerAnim.playTargetAnimation(OneHand_LightAttack02, true, false);
            }
            else if (lastAttack == TwoHand_LightAttack01)
            {
                playerAnim.playTargetAnimation(TwoHand_LightAttack02, true, false);
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        playerAnim.anim.runtimeAnimatorController = weapon.weaponAnimController;

        if (playerStats.currentStamina < (weapon.baseStamina * weapon.lightAttackMultiplier))
            return;

        if (weapon.weaponType == WeaponType.Bow)
        {
            if (!playerAnim.anim.GetBool("isAiming"))
            {
                if (playerInventory.IsAmmoAvailable() >= 1)
                {
                    playerAnim.anim.SetBool("isAiming", true);
                    if (playerInventory.rightHandWeapon.weaponType == WeaponType.Bow)//if the shooting is being done with the right hand (cant use isUsingRightHand because is using two hands)
                    {
                        playerAnim.anim.SetBool("isUsingRightHand", true);
                    }
                    else
                    {
                        playerAnim.anim.SetBool("isUsingLeftHand", true);
                    }
                    playerAnim.playTargetAnimation("DrawArrow", false);
                }
                else
                {
                    //playerAnim.playTargetAnimation("noAmmoAvailable", false);
                }
            }
            return;
        }

        if (weapon.itemName.ToLower().Contains("pickaxe"))
        {
            HandlePickaxeAction(weapon);
            return;
        }

        if (playerManager.isTwoHandingWeapon)
        {
            playerAnim.playTargetAnimation(TwoHand_LightAttack01, true, false);
            lastAttack = TwoHand_LightAttack01;
        }
        else
        {
            playerAnim.playTargetAnimation(OneHand_LightAttack01, true, false);
            lastAttack = OneHand_LightAttack01;
        }
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        playerAnim.anim.runtimeAnimatorController = weapon.weaponAnimController;

        if (playerStats.currentStamina < (weapon.baseStamina * weapon.heavyAttackMultiplier))
            return;

        if (weapon.weaponType == WeaponType.Bow)
        {
            if (!playerAnim.anim.GetBool("isAiming"))
            {
                if (playerInventory.IsAmmoAvailable() >= 1)
                {
                    playerAnim.anim.SetBool("isAiming", true);
                    if (playerInventory.rightHandWeapon.weaponType == WeaponType.Bow)//if the shooting is being done with the right hand (cant use isUsingRightHand because is using two hands)
                    {
                        playerAnim.anim.SetBool("isUsingRightHand", true);
                    }
                    else
                    {
                        playerAnim.anim.SetBool("isUsingLeftHand", true);
                    }
                    playerAnim.playTargetAnimation("DrawArrow", false);
                }
                else
                {
                   //playerAnim.playTargetAnimation("noAmmoAvailable", false);
                }
            }
            return;
        }

        if (weapon.itemName.ToLower().Contains("pickaxe"))
        {
            HandlePickaxeAction(weapon);
            return;
        }

        if (playerManager.isTwoHandingWeapon)//should i use twoHandflag from playermanager instead?
        {
            playerAnim.playTargetAnimation(TwoHand_HeavyAttack, true, false);
            lastAttack = TwoHand_HeavyAttack;
        }
        else
        {
            playerAnim.playTargetAnimation(OneHand_HeavyAttack, true, false);
            lastAttack = OneHand_HeavyAttack;
        }
    }

    public void HandlePickaxeAction(WeaponItem pickaxe)
    {
        playerAnim.anim.SetBool("isDigging", true);
        Dig(pickaxe);
    }

    public void Dig(WeaponItem pickaxe)
    {
        //Debug.DrawRay(playerManager.lockOnTransform.transform.position, CameraHandler._instance.cameraTransform.forward, Color.black, Mathf.Infinity);
        //if (Physics.Raycast(playerManager.lockOnTransform.transform.position, CameraHandler._instance.cameraTransform.forward, out var hit, pickaxe.weaponRange))
        //{
        //    playerAnim.playTargetAnimation("PickaxeDig", false);
        //    diggerMasterRuntime.ModifyAsyncBuffured(hit.point, brush, action, textureIndex, opacity, size, 1, true);
        //    FLOW.FlowSimulation.DirtyGroundAll(playerManager.WorldCenter.position,250);
        //    diggerNavMeshMasterRuntime.UpdateNavMeshAsync();
        //}
    }

    public void CancelDigging()
    {
        playerAnim.anim.SetBool("isDigging", false);
    }

    public void ShootArrow(bool isHeavyAttack)
    {
        playerAnim.anim.SetBool("isAiming", false);

        if (isHeavyAttack)
        {
            ReadyDamagingArrow();
            playerAnim.playTargetAnimation(TwoHand_HeavyAttack, true, false);
            lastAttack = TwoHand_HeavyAttack;
        }
        else
        {
            if(playerAnim.anim.GetBool("bowFullyTensed"))
            {
                ReadyDamagingArrow();
                playerAnim.playTargetAnimation(TwoHand_LightAttack01, true, false);//Have a variation of the bow attakc when the bow is fully tensed
            }
            else
            {
                ReadyDamagingArrow();
                playerAnim.playTargetAnimation(TwoHand_LightAttack01, true, false);
            }
            lastAttack = TwoHand_LightAttack01;
        }
        playerAnim.anim.SetBool("arrowInHand", false);
    }

    private void ReadyDamagingArrow()
    {
        List<Item> ammoAvailable = playerInventory.itemsInventory.FindAll(x => x.itemName == playerInventory.ammoSlot.itemName);

        AmmoItem firedArrow = ammoAvailable[ammoAvailable.Count - 1] as AmmoItem;
        firedArrow.timesUsed += 1;
        Destroy(playerEffectsManager.ArrowModel);

        GameObject damagingArrow;
        if (playerAnim.anim.GetBool("isUsingLeftHand"))
        {
            damagingArrow = Instantiate(playerInventory.ammoSlot.damagingModel, playerInventory._weaponSlotManager.rightHandSlot.transform.position, CameraHandler._instance.cameraTransform.rotation);
            if (damagingArrow.GetComponent<ElementsInteractionHandler>() != null)
            {
                if (damagingArrow.GetComponent<WeaponEffectsManager>() != null)
                {
                    damagingArrow.GetComponent<WeaponEffectsManager>().UpdateItemFX(playerInventory._weaponSlotManager.leftHandSlot.currentWeapon);
                }

                if (playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>() != null)
                {
                    damagingArrow.GetComponent<ElementsInteractionHandler>().SetElementType(playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementType);
                    damagingArrow.GetComponent<ElementsInteractionHandler>().SetElementLevel(playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementLevel);
                    //Maybe the element level will be needed too
                }
            }
            //ANIMATE the bow string being released
        }
        else
        {
            damagingArrow = Instantiate(playerInventory.ammoSlot.damagingModel, playerInventory._weaponSlotManager.leftHandSlot.transform);
            if (damagingArrow.GetComponent<ElementsInteractionHandler>() != null)
            {
                if (damagingArrow.GetComponent<WeaponEffectsManager>() != null)
                {
                    damagingArrow.GetComponent<WeaponEffectsManager>().UpdateItemFX(playerInventory._weaponSlotManager.rightHandSlot.currentWeapon);
                }

                if (playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>() != null)
                {
                    damagingArrow.GetComponent<ElementsInteractionHandler>().SetElementType(playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementType);
                    damagingArrow.GetComponent<ElementsInteractionHandler>().SetElementLevel(playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementLevel);
                    //Maybe the element level will be needed too
                }
            }
            //ANIMATE the bow string being released
        }

        if (damagingArrow != null)
        {
            RangedProjectileDamageCollider rangedProjectileDamage = damagingArrow.GetComponent<RangedProjectileDamageCollider>();
            rangedProjectileDamage.ammoItem = firedArrow;
            playerInventory.RemoveItem(ammoAvailable[ammoAvailable.Count - 1]);
        }

        Rigidbody arrowRigidbody = damagingArrow.GetComponentInChildren<Rigidbody>();
        if (CameraHandler._instance.currentCamera == CameraMode.LockOnCamera)
        {
            damagingArrow.transform.LookAt(CameraHandler._instance.lockOnCamera.LookAt);
        }
        else
        {
            damagingArrow.transform.rotation = Quaternion.Euler(CameraHandler._instance.cameraTransform.eulerAngles.x, playerManager.lockOnTransform.eulerAngles.y, 0);
        }

        //FLOW.FlowSample damageArrowFlowSample = damagingArrow.GetComponentInChildren<FLOW.FlowSample>(true);
        //damageArrowFlowSample.Simulation = FindObjectOfType<FLOW.FlowSimulation>();
        
        arrowRigidbody.AddForce(damagingArrow.transform.forward * firedArrow.forwardVelocity);
        arrowRigidbody.AddForce(damagingArrow.transform.up * firedArrow.upwardVelocity);
        arrowRigidbody.useGravity = firedArrow.useGravity;
        arrowRigidbody.mass = firedArrow.ammoMass;
        damagingArrow.transform.parent = null;
    }

    public void PerformBlock(bool isLeftHand)
    {
        if (playerManager.isInteracting)
            return;

        playerAnim.anim.SetBool("isUsingLeftHand", false);
        playerAnim.anim.SetBool("isUsingRightHand", false);
        playerAnim.anim.SetBool("isAiming", false);
        playerAnim.anim.SetBool("arrowInHand", false);

        if (playerEffectsManager.ArrowModel != null)
        {
            Destroy(playerEffectsManager.ArrowModel);
        }

        if (isLeftHand)
        {
            playerAnim.anim.SetBool("isUsingRightHand", true);//This is backwards, meaning, when you two-hand a weapon you use your dominant hand to hold it, and in theory attack and block, but by the controls inputs you are using your other hand to block (in this case LT or LB), thats why the case is in the "isLeftHand" case, but we know the player should be using the right hand because the weapon is actually held with that hand
            if (playerInventory.rightHandWeapon.weaponType == WeaponType.Bow)//special case with the bow because to block yo use the contrary hand
            {
                playerAnim.playTargetAnimation(Block, false, true, true);
            }
            else
            {
                playerAnim.playTargetAnimation(Block, false);
            }
            //Load weapon HitBox to detect blocked attacks(very small, not ideal to detect the hit)
        }
        else
        {
            playerAnim.anim.SetBool("isUsingLeftHand", true);//This is backwards, meaning, when you two-hand a weapon you use your dominant hand to hold it, and in theory attack and block, but by the controls inputs you are using your other hand to block (in this case RT or RB), thats why the case is in the "isRighttHand" case, but we know the player should be using the left hand because the weapon is actually held with that hand
            playerAnim.playTargetAnimation(Block, false);
            //Load weapon HitBox to detect blocked attacks(very small, not ideal to detect the hit)
        }

        playerAnim.anim.SetBool("isBlocking", true);
        blockCollider.enabled = true;
    }

    public void CancelBlock()
    {
        playerAnim.anim.SetBool("isBlocking", false);
        playerAnim.anim.SetBool("isUsingLeftHand", false);
        playerAnim.anim.SetBool("isUsingRightHand", false);
        blockCollider.enabled = false;
        //hide weapon HitBox
    }

    public void HandleTwoHandingWeapon()
    {
        if (inputHandler.twoHandedFlag)
        {
            if (!playerInventory.rightHandWeapon.isUnarmed && playerInventory.leftHandWeapon.isUnarmed)
            {
                playerManager.isTwoHandingWeapon = true;
                return;
            }
            else if (!playerInventory.leftHandWeapon.isUnarmed && playerInventory.rightHandWeapon.isUnarmed)
            {
                playerManager.isTwoHandingWeapon = true;
                return;
            }
            else if (!playerInventory.leftHandWeapon.isUnarmed && !playerInventory.rightHandWeapon.isUnarmed)
            {
                switch (playerStats.Handedness)
                {
                    case Handedness.RightHanded:
                        weaponSlotManager.backSlot.currentWeapon = playerInventory.leftHandWeapon;
                        playerInventory.ChangeCurrentWeapon(weaponSlotManager.backSlot);
                        weaponSlotManager.SheathWeapon(true);
                        playerManager.isTwoHandingWeapon = true;
                        return;
                    case Handedness.LeftHanded:
                        weaponSlotManager.backSlot.currentWeapon = playerInventory.rightHandWeapon;
                        playerInventory.ChangeCurrentWeapon(weaponSlotManager.backSlot);
                        weaponSlotManager.SheathWeapon(false);
                        playerManager.isTwoHandingWeapon = true;
                        return;
                    case Handedness.Ambidextrous:
                        weaponSlotManager.backSlot.currentWeapon = playerInventory.leftHandWeapon;
                        playerInventory.ChangeCurrentWeapon(weaponSlotManager.backSlot);
                        weaponSlotManager.SheathWeapon(true);
                        playerManager.isTwoHandingWeapon = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                inputHandler.twoHandedFlag = false;//Can not 2 hand the weapon at hand
                return;
            }
        }
        else
        {
            playerManager.isTwoHandingWeapon = false;
            inputHandler.twoHandedFlag = false;
            if (weaponSlotManager.backSlot != null)
            {
                weaponSlotManager.UnsheathWeapon();
            }
        }
    }

    public void WeaponChangeCurrentActionCancel()
    {
        playerAnim.anim.SetBool("isAiming", false);
        playerAnim.anim.SetBool("arrowInHand", false);

        if (playerEffectsManager.ArrowModel != null)
        {
            Destroy(playerEffectsManager.ArrowModel);
        }

        playerAnim.anim.SetBool("isBlocking", false);

        playerAnim.anim.SetBool("isUsingLeftHand", false);
        playerAnim.anim.SetBool("isUsingRightHand", false);
    }
}
