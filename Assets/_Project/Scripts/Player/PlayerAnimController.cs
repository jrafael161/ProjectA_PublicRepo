using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : AnimatorController
{
    //InputHandler inputHandler;
    PlayerMovementController playerMovement;
    PlayerEffectsManager playerEffectsManager;

    int vertical;
    int horizontal;
    //public bool canRotate;

    Quaternion _originalRotation;
    Vector3 _offset;

    public void Initialize()
    {
        if (playerManager == null)
        {
            playerManager = GetComponentInParent<PlayerManager>();
        }        
        anim = GetComponent<Animator>();
        //inputHandler = GetComponentInParent<InputHandler>();
        playerMovement = GetComponentInParent<PlayerMovementController>();
        playerEffectsManager = GetComponentInParent<PlayerEffectsManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
    {
        float v = 0;
        float h = 0;

        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }

        if (h == 0 && v == 0)
        {
            playerManager.EnableCharacterColliderBlocker();
        }
        else
        {
            playerManager.DisableCharacterColliderBlocker();
        }

        if (playerManager.isSprinting )
        {
            v = 1.5f;
            //h = horizontalMovement;
        }

        if (playerManager.isClimbing)
        {
            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 2.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 3;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = 2.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = 2;
            }
            else
            {
                v = 2.5f;
            }

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -0.5f;
            }
            else
            {
                h = 0;
            }
        }

        if (playerManager.isSwiming)
        {            
            if(verticalMovement != 0 || horizontalMovement !=0)
            {
                v = -2f;
            }
            else
            {
                v = -1.5f;
            }

            h = 0;
        }
        
        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    //public void CanRotate()
    //{
    //    canRotate = true;
    //}

    //public void StopRotation()
    //{
    //    canRotate = false;
    //}

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo",true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    public void EnableInvulnerability()
    {
        anim.SetBool("isInvulnerable", true);
    }

    public void DisableInvulnerability()//This is not being applied because the animation is being cut before finishing
    {
        anim.SetBool("isInvulnerable", false);
    }

    public void MovePlayerTransform()//Used for the hop up animation
    {
        playerManager.GetComponent<Rigidbody>().AddForce(transform.up * 70, ForceMode.Impulse);
    }

    public void UpFromLedge()
    {
        Debug.Log("up");
        CapsuleCollider capsuleCollider;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        //playerMovement.transform.position = capsuleCollider.transform.position + new Vector3(0, -1.5f, 0);//magical offset so the player is tp to the correct place, when the tp takes place the player collider also moves how could i stop the movement?
    }

    public void DownFromLedge()
    {
        CapsuleCollider capsuleCollider;        
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();        
        transform.parent.Rotate(0, transform.parent.rotation.y + 70, 0);
        canRotate = false;
        anim.SetBool("canRotate", canRotate);
        Debug.Log("down");        
        playerMovement.transform.position = capsuleCollider.transform.position + new Vector3(0, -1.7f, 0);//magical offset so the player is tp to the correct place
    }

    public void Test1()//Functions that are called by animations events need to be in the sabe gameobject as the animation controller, they acan be in another script but in the same parent object
    {
        Debug.Log("Test");
    }

    public void SpawnArrow()
    {
        PlayerInventory playerInventory = GetComponentInParent<PlayerInventory>();
        if (anim.GetBool("isUsingLeftHand"))
        {
            GameObject loadedArrow = Instantiate(playerInventory.ammoSlot.loadItemModel, playerInventory._weaponSlotManager.rightHandSlot.transform);
            //ANIMATE the bow string being pulled
            playerEffectsManager.ArrowModel = loadedArrow;
            if (loadedArrow.GetComponent<ElementsInteractionHandler>() != null)
            {
                if (loadedArrow.GetComponent<WeaponEffectsManager>() != null)
                {
                    loadedArrow.GetComponent<WeaponEffectsManager>().UpdateItemFX(playerInventory._weaponSlotManager.leftHandSlot.currentWeapon);
                }

                if (playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponent<WeaponEffectsManager>() != null)
                {
                    loadedArrow.GetComponent<ElementsInteractionHandler>().SetElementType(playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementType);
                    loadedArrow.GetComponent<ElementsInteractionHandler>().SetElementLevel(playerInventory._weaponSlotManager.leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementLevel);
                }
            }
        }
        else
        {
            GameObject loadedArrow = Instantiate(playerInventory.ammoSlot.loadItemModel, playerInventory._weaponSlotManager.leftHandSlot.transform);
            //ANIMATE the bow string being pulled
            playerEffectsManager.ArrowModel = loadedArrow;
            if (loadedArrow.GetComponent<ElementsInteractionHandler>() != null)
            {
                if (loadedArrow.GetComponent<ElementsInteractionHandler>() != null)
                {
                    if (loadedArrow.GetComponent<WeaponEffectsManager>() != null)
                    {
                        loadedArrow.GetComponent<WeaponEffectsManager>().UpdateItemFX(playerInventory._weaponSlotManager.rightHandSlot.currentWeapon);
                    }

                    if (playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>() != null)
                    {
                        loadedArrow.GetComponent<ElementsInteractionHandler>().SetElementType(playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementType);
                        loadedArrow.GetComponent<ElementsInteractionHandler>().SetElementLevel(playerInventory._weaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponEffectsManager>().elementLevel);
                    }
                }
            }
        }
    }

    private void OnAnimatorMove()
    {        
        if (playerManager.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        var animationInfo = (anim.GetCurrentAnimatorClipInfo(0));
        //playerMovement.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        if (!animationInfo[0].clip.name.ToLower().Contains("climb"))
        {
            deltaPosition.y = 0;
        }
        Vector3 velocity = deltaPosition / delta;
        playerMovement.playerRigidbody.velocity = velocity;        
    }
}
