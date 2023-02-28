using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float CameraMovementX;//mouseX
    public float CameraMovementY;//mouseY

    public bool b_Btn_Input;//b_Input by xbox standards
    public bool a_Btn_Input;//A button by xbox standards
    public bool y_Btn_Input;//y button by xbox standards
    public bool x_Btn_Input;//x button by xbox standards
    public bool RB_Btn_Input;//RB_Input by xbox standards
    public bool RT_Btn_Input;//RT_Input by xbox standards
    public bool LB_Btn_Input;//LB_Input by xbox standards
    public bool LT_Btn_Input;//LT_Input by xbox standards
    public bool Dpad_Up_Input;//Dpad_UP_Input by xbox standards
    public bool Dpad_Down_Input;//Dpad_Down_Input by xbox standards
    public bool Dpad_Left_Input;//Dpad_Left_Input by xbox standards
    public bool Dpad_Right_Input;//Dpad_Right_Input by xbox standards
    public bool Start_btn_Input;//Dpad_Right_Input by xbox standards
    public bool RightJoystick_btn_Input;//Right Joystick button Input by xbox standards
    public bool LeftJoystick_btn_Input;//Right Joystick button Input by xbox standards
    public bool TwoHandWeapon_btn_Input;//RB + Y by xbox standards

    public bool LockOnToTheLeft;//Right Joystick to the left while locking an enemy, Input by xbox standards
    public bool LockOnToTheRight;//Right Joystick to the right while locking an enemy, Input by xbox standards

    public Vector2 MouseWheel;

    public bool dodgeFlag;//rollFlag
    public bool climbDropFlag;
    public bool climbJumpFlag;
    public bool twoHandedFlag;
    public bool sprintFlag;
    public bool lockOnFlag;
    public bool comboFlag;
    public bool useConsumable;
    public float dodgeInputTimer;
    public float climbDropInputTimer;

    private PlayerControls playerControls;
    private PlayerCombatHandler playerCombatHandler;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private UIManager uIManager;
    private CameraHandler cameraHandler;
    private WeaponSlotManager weaponSlotManager;
    private PlayerAnimController playerAnim;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Start()
    {
        playerCombatHandler = GetComponentInChildren<PlayerCombatHandler>();
        playerInventory = GetComponent<PlayerInventory>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        playerAnim = GetComponentInChildren<PlayerAnimController>();
        cameraHandler = CameraHandler._instance;
        uIManager = playerManager.UIManager;
    }

    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += playerControls => movementInput = playerControls.ReadValue<Vector2>();
            playerControls.PlayerMovement.Look.performed += playerControls => cameraInput = playerControls.ReadValue<Vector2>();            
            playerControls.PlayerActions.RightHandAction.performed += i => RB_Btn_Input = true;
            playerControls.PlayerActions.RightHandSubAction.performed += i => RT_Btn_Input = true;
            playerControls.PlayerActions.LeftHandAction.performed += i => LB_Btn_Input = true;
            playerControls.PlayerActions.LeftHandSubAction.performed += i => LT_Btn_Input = true;
            playerControls.PlayerQuickSlots.DpadRight.performed += i => Dpad_Right_Input = true;
            playerControls.PlayerQuickSlots.DpadLeft.performed += i => Dpad_Left_Input = true;
            playerControls.PlayerActions.Interact.performed += i => a_Btn_Input = true;
            playerControls.PlayerActions.InteractToggle.performed += i => Dpad_Down_Input = true;
            playerControls.PlayerActions.UseConsumable.performed += i => x_Btn_Input = true;
            playerControls.PlayerActions.Jump.performed += i => y_Btn_Input = true;
            playerControls.PlayerActions.LockOn.performed += i => RightJoystick_btn_Input = true;
            playerControls.PlayerActions.Menu.performed += i => Start_btn_Input = true;
            playerControls.UIMenuAction.QuitMenu.performed += i => Start_btn_Input = true;
            playerControls.PlayerMovement.LeftLockOnTarget.performed += i => LockOnToTheLeft = true;
            playerControls.PlayerMovement.RightLockOnTarget.performed += i => LockOnToTheRight = true;
            playerControls.PlayerActions.TwoHandWeapon.performed += i => TwoHandWeapon_btn_Input = true;
        }
        playerControls.PlayerActions.Enable();
        playerControls.PlayerMovement.Enable();
        playerControls.PlayerQuickSlots.Enable();
        if (playerManager!=null)
        {
            if (playerManager.interactingObject != null)//I need to unbind the interacting player from the interacted object to allow other players to interact again, this will work in the meantime but implementing an interface would be better
            {
                if (playerManager.interactingObject.GetComponent<FurnaceController>() != null)
                {
                    FurnaceController furnaceController = playerManager.interactingObject.GetComponent<FurnaceController>();
                    furnaceController.InteractingPlayer = null;
                }
                if (playerManager.interactingObject.GetComponent<WeaponDisplayerManager>() != null)
                {
                    WeaponDisplayerManager weaponDisplayerManager = playerManager.interactingObject.GetComponent<WeaponDisplayerManager>();
                    weaponDisplayerManager.InteractingPlayer = null;
                }
                playerManager.interactingObject = null;//If movement controls are enabled again it meant the player is no longer interacting with any object
            } 
        }
    }

    public void OnDisable()
    {
        cameraInput = Vector2.zero;
        movementInput.x = 0;
        movementInput.y = 0;
        playerControls.PlayerActions.Disable();
        playerControls.PlayerMovement.Disable();
        playerControls.PlayerQuickSlots.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMoveInput(delta);
        HandleDodgeInput(delta);
        HandleItemChangeInput(delta);
        HandleClimbDropAndJumpInput(delta);
        HandleQuickSlotInput();
        HandleConsumable();
        HandleStartButton();
        HandleLockOnInput();
        if (playerManager.currentState != PlayerState.CLIMBING && playerManager.currentState != PlayerState.SWIMMING)
        {
            HandleCanceledInput();
            HandleTwoHandInput();
            HandleAttackInput();
        }
    }

    private void HandleMoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        CameraMovementX = cameraInput.x;
        CameraMovementY = cameraInput.y;
    }

    public void HandleConsumable()
    {
        useConsumable = x_Btn_Input;
    }

    private void HandleDodgeInput(float delta)
    {
        b_Btn_Input = playerControls.PlayerActions.DodgeRun.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        sprintFlag = b_Btn_Input;
        if (b_Btn_Input)
        {
            dodgeInputTimer += delta;            
        }
        else
        {
            if (dodgeInputTimer > 0 && dodgeInputTimer < 0.5f && !lockOnFlag)
            {
                sprintFlag = false;
                dodgeFlag = true;
            }

            dodgeInputTimer = 0;
        }
    }

    private void HandleClimbDropAndJumpInput(float delta)
    {
        if (playerControls.PlayerActions.Jump.IsPressed() && playerManager.isClimbing)
        {
            climbDropInputTimer += delta;
            if (climbDropInputTimer > 0.5f)
            {
                climbDropFlag = true;
                climbJumpFlag = false;
                climbDropInputTimer = 0;
            }
        }
        else if(!playerControls.PlayerActions.Jump.IsPressed() && playerManager.isClimbing)
        {
            if (climbDropInputTimer > 0 && climbDropInputTimer < 0.5f)
            {
                climbDropFlag = false;
                climbJumpFlag = true;
            }
            else
            {
                climbJumpFlag = false;
                climbDropFlag = false;
            }
            climbDropInputTimer = 0;
        }
        else
        {
            climbJumpFlag = false;
            climbDropFlag = false;
        }
    }
    private void HandleAttackInput()
    {
        if (RB_Btn_Input && !RT_Btn_Input)
        {
            if (playerAnim.anim.GetBool("isBlocking"))//Only cancel the block when is blocking otherwise the animator loses wich hand was being used mid animation
            {
                playerCombatHandler.CancelBlock();
            }
            playerCombatHandler.HandleRBAction();
        }

        if (RT_Btn_Input)
        {
            if (playerAnim.anim.GetBool("isBlocking"))
            {
                playerCombatHandler.CancelBlock();
            }
            playerCombatHandler.HandleRTAction();
        }

        if (LB_Btn_Input && !LT_Btn_Input)
        {
            if (playerAnim.anim.GetBool("isBlocking"))
            {
                playerCombatHandler.CancelBlock();
            }
            playerCombatHandler.HandleLBAction();
        }

        if (LT_Btn_Input)
        {
            if (playerAnim.anim.GetBool("isBlocking"))
            {
                playerCombatHandler.CancelBlock();
            }
            playerCombatHandler.HandleLTAction();
        }
    }

    public void HandleCanceledInput()
    {
        if (playerManager.isAiming)
        {
            if (playerAnim.anim.GetBool("arrowInHand"))
            {
                switch (playerStats.Handedness)
                {
                    case Handedness.RightHanded:
                        if (playerControls.PlayerActions.RightHandAction.WasReleasedThisFrame())
                            playerCombatHandler.ShootArrow(false);

                        if (playerControls.PlayerActions.RightHandSubAction.WasReleasedThisFrame() && !Mouse.current.rightButton.isPressed)
                            playerCombatHandler.ShootArrow(true);
                        break;
                    case Handedness.LeftHanded:
                        if (playerControls.PlayerActions.LeftHandAction.WasReleasedThisFrame())
                            playerCombatHandler.ShootArrow(false);

                        if (playerControls.PlayerActions.LeftHandSubAction.WasReleasedThisFrame() && !Mouse.current.leftButton.isPressed)
                            playerCombatHandler.ShootArrow(true);
                        break;
                    case Handedness.Ambidextrous:
                        if (playerControls.PlayerActions.RightHandAction.WasReleasedThisFrame())
                            playerCombatHandler.ShootArrow(false);

                        if (playerControls.PlayerActions.RightHandSubAction.WasReleasedThisFrame() && !Mouse.current.rightButton.isPressed)
                            playerCombatHandler.ShootArrow(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (playerControls.PlayerActions.RightHandAction.WasReleasedThisFrame())
                    playerAnim.anim.SetBool("isAiming", false);

                if (playerControls.PlayerActions.RightHandSubAction.WasReleasedThisFrame() && !Mouse.current.rightButton.isPressed)
                    playerAnim.anim.SetBool("isAiming", false);

                if (playerControls.PlayerActions.LeftHandAction.WasReleasedThisFrame())
                    playerAnim.anim.SetBool("isAiming", false);

                if (playerControls.PlayerActions.LeftHandSubAction.WasReleasedThisFrame() && !Mouse.current.leftButton.isPressed)
                    playerAnim.anim.SetBool("isAiming", false);
            }
        }

        if (playerManager.isBlocking)
        {
            switch (playerStats.Handedness)
            {
                case Handedness.RightHanded:
                    if (playerControls.PlayerActions.LeftHandAction.WasReleasedThisFrame())
                        playerCombatHandler.CancelBlock();

                    if (playerControls.PlayerActions.LeftHandSubAction.WasReleasedThisFrame() && !Mouse.current.leftButton.isPressed)
                        playerCombatHandler.CancelBlock();
                    break;
                case Handedness.LeftHanded:
                    if (playerControls.PlayerActions.RightHandAction.WasReleasedThisFrame())
                        playerCombatHandler.CancelBlock();

                    if (playerControls.PlayerActions.RightHandSubAction.WasReleasedThisFrame() && !Mouse.current.rightButton.isPressed)
                        playerCombatHandler.CancelBlock();
                    break;
                case Handedness.Ambidextrous:
                    if (playerControls.PlayerActions.LeftHandAction.WasReleasedThisFrame())
                        playerCombatHandler.CancelBlock();

                    if (playerControls.PlayerActions.LeftHandSubAction.WasReleasedThisFrame() && !Mouse.current.leftButton.isPressed)
                        playerCombatHandler.CancelBlock();
                    break;
                default:
                    break;
            }
        }

        if (playerManager.isDigging)
        {
            if (playerControls.PlayerActions.RightHandAction.WasReleasedThisFrame())
                playerCombatHandler.CancelDigging();

            if (playerControls.PlayerActions.RightHandSubAction.WasReleasedThisFrame() && !Mouse.current.rightButton.isPressed)
                playerCombatHandler.CancelDigging();

            if (playerControls.PlayerActions.LeftHandAction.WasReleasedThisFrame())
                playerCombatHandler.CancelDigging();

            if (playerControls.PlayerActions.LeftHandSubAction.WasReleasedThisFrame() && !Mouse.current.leftButton.isPressed)
                playerCombatHandler.CancelDigging();
        }
    }

    private void HandleLockOnInput()
    {
        if (RightJoystick_btn_Input && lockOnFlag == false)
        {
            RightJoystick_btn_Input = false;                        
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
                cameraHandler.ChangeCameraMode(CameraMode.LockOnCamera);
                cameraHandler.lockOnCamera.LookAt = cameraHandler.currentLockOnTarget.GetComponentInChildren<LockOnReticle>(true).transform;
                LockOnReticle lockOnReticle = cameraHandler.currentLockOnTarget.GetComponentInChildren<LockOnReticle>(true);
                if (lockOnReticle != null)
                {
                    lockOnReticle.gameObject.SetActive(true);
                }                
            }
        }
        else if (lockOnFlag && LockOnToTheLeft)
        {
            LockOnToTheLeft = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockOnTarget != null)
            {
                cameraHandler.ChangeLockOnReticle(true);
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockOnTarget;
                cameraHandler.lockOnCamera.LookAt = cameraHandler.currentLockOnTarget.GetComponentInChildren<LockOnReticle>(true).transform;
            }
        }
        else if (lockOnFlag && LockOnToTheRight)
        {
            LockOnToTheRight = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockOnTarget != null)
            {
                cameraHandler.ChangeLockOnReticle(false);
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockOnTarget;
                cameraHandler.lockOnCamera.LookAt = cameraHandler.currentLockOnTarget.GetComponentInChildren<LockOnReticle>(true).transform;
            }
        }
        else if (RightJoystick_btn_Input && lockOnFlag)
        {
            RightJoystick_btn_Input = false;
            cameraHandler.CancelLockOn();
        }
    }

    private void HandleQuickSlotInput()
    {
        if (playerManager.isInteracting)//TODO: if the player is intreacting hold the action and do it when it finishes interacting
            return;

        if (Dpad_Right_Input)
        {
            wasTwoHandingWeapon();
            playerInventory.ChangeRightWeapon();
        }
        else if (Dpad_Left_Input)
        {
            wasTwoHandingWeapon();
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleTwoHandInput()
    {
        if (playerManager.isInteracting)
            return;

        if (TwoHandWeapon_btn_Input)
        {
            TwoHandWeapon_btn_Input = false;
            twoHandedFlag = !twoHandedFlag;

            playerCombatHandler.HandleTwoHandingWeapon();
        }
    }

    public void wasTwoHandingWeapon()
    {
        if (playerManager.isTwoHandingWeapon)
        {
            twoHandedFlag = false;
            weaponSlotManager.UnsheathWeapon();
        }
    }

    private void HandleItemChangeInput(float delta)
    {
        playerControls.UIMenuAction.ChangeItem.performed += i => MouseWheel = i.ReadValue<Vector2>();
        if (MouseWheel.y>0)
        {
            //Debug.Log("Para arriba");
        }
        else if (MouseWheel.y<0)
        {
            //Debug.Log("Para abajo");
        }
    }
    private void HandleWeaponChangeInput(float delta)
    {
        playerControls.UIMenuAction.ChageWeapon.performed += i => MouseWheel = i.ReadValue<Vector2>();
        if (MouseWheel.y > 0)
        {
            Debug.Log("Para la derecha");
        }
        else if (MouseWheel.y < 0)
        {
            Debug.Log("Para la iquierda");
        }
    }

    public void HandleStartButton()
    {
        if (Start_btn_Input && GameController._instance.state != GameState.InMenu)
        {
            OnDisable();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameController._instance.ChangeGameState(GameState.InMenu);
            EnableMenuControls();
            uIManager.OpenInGameMenu();
            uIManager.playerUI.SetActive(false);
        }
        else if (Start_btn_Input && GameController._instance.state == GameState.InMenu)
        {
            if (uIManager.OpenWindows.Count > 1)
            {
                uIManager.RemoveOpenWindow();
                uIManager.OpenPreviousWindow();
            }
            else if(uIManager.OpenWindows.Count <=1)
            {
                uIManager.RemoveOpenWindow();
                CloseUI();
                //DisableMenuControls();
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
                //GameController._instance.ChangeGameState(GameState.InGame);
                //OnEnable();
                //uIManager.CloseAllUIWindows();
                //uIManager.playerUI.SetActive(true);
            }
        }
    }

    public void OpenUI()
    {
        OnDisable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameController._instance.ChangeGameState(GameState.InMenu);
        EnableMenuControls();
        uIManager.playerUI.SetActive(false);
    }

    public void CloseUI()
    {
        DisableMenuControls();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameController._instance.ChangeGameState(GameState.InGame);
        GameController._instance.CheckIfSettingsChanged();
        OnEnable();
        uIManager.CloseAllUIWindows();
        uIManager.playerUI.SetActive(true);
        GetComponentInChildren<PlayerInteraction>().UpdateInteractables();
    }

    public void DisableMenuControls()
    {
        playerControls.UIMenuAction.Disable();
    }

    public void EnableMenuControls()
    {
        playerControls.UIMenuAction.Enable();
    }
}
