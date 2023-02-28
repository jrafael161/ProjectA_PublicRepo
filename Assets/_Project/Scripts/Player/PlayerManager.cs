using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    WALKING,
    CLIMBING,
    SWIMMING,
    SLIDING,
    FALLING,
    INTERACTING,
    DEAD,
    INTERSTATE
}

public class PlayerManager : CharacterManager
{
    InputHandler inputHandler;
    Animator animator;
    PlayerMovementController playerMovement;
    PlayerStats playerStats;
    PlayerInventory playerInventory;
    PlayerInteraction playerInteraction;
    PlayerAnimController playerAnimController;

    public UIManager UIManager;
    //public UIManager UIManager { set { uiManager = value; } get { return uiManager; } } [SerializeField] UIManager uiManager;

    public GameObject interactingObject;

    public Transform WorldCenter;

    public SkinnedMeshRenderer playerSkin;
    public Material[] kinSkins;

    bool areWeaponsHidden;

    public PlayerState currentState;

    [Header ("Player Flags ")]
    public bool isInteracting;
    public bool isSprinting;
    public bool isInAir;
    public bool isJumping;
    public bool isGrounded;
    public bool isSliding;
    public bool isClimbing;
    public bool isSwiming;
    public bool isInvulnerable;
    public bool canDoCombo;
    public bool isAiming;
    public bool isDigging;
    public bool isUsingLeftHand;
    public bool isUsingRightHand;
    public bool isUsingBothHands;
    public bool isTwoHandingWeapon;
    public bool isBlocking;
    public bool isUnderground;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();        
        animator = GetComponentInChildren<Animator>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        playerMovement = GetComponent<PlayerMovementController>();
        playerStats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();
        playerAnimController = GetComponentInChildren<PlayerAnimController>();
        WorldCenter = GameObject.Find("WorldCenter").GetComponent<Transform>();

        //ChangePlayerState(PlayerState.INTERSTATE);
    }

    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = animator.GetBool("isInteracting");
        canDoCombo = animator.GetBool("canDoCombo");
        isUsingLeftHand = animator.GetBool("isUsingLeftHand");
        isUsingRightHand = animator.GetBool("isUsingRightHand");
        isBlocking = animator.GetBool("isBlocking");
        isInvulnerable = animator.GetBool("isInvulnerable");
        isAiming = animator.GetBool("isAiming");
        isDigging = animator.GetBool("isDigging");
        
        animator.SetBool("isInAir",isInAir);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isTwoHanding", isTwoHandingWeapon);

        playerStats.RegenerateStamina();
        inputHandler.TickInput(delta);
        playerAnimController.canRotate = animator.GetBool("canRotate");

        playerMovement.HandleDodgeAndRunning(delta);
        playerMovement.ClimbDropAndJump(delta);
        playerMovement.HandleJump(delta);
        playerInteraction.ChecForPlayerAction();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if (playerMovement.CheckSlopeSteepnes())
        {
            playerMovement.HandleSlopes(delta);
        }            
        else
        {
            playerMovement.HandleMovement(delta);
            playerMovement.HandleClimbing(delta);
            playerMovement.HandleSwimming(delta);
            playerMovement.HandleFalling(delta, playerMovement.moveDirection); 
        }        
    }

    private void LateUpdate()
    {
        inputHandler.dodgeFlag = false;
        inputHandler.RB_Btn_Input = false;
        inputHandler.RT_Btn_Input = false;
        inputHandler.LB_Btn_Input = false;
        inputHandler.LT_Btn_Input = false;

        inputHandler.Dpad_Up_Input = false;
        inputHandler.Dpad_Down_Input = false;
        inputHandler.Dpad_Left_Input = false;
        inputHandler.Dpad_Right_Input = false;

        inputHandler.a_Btn_Input = false;
        inputHandler.y_Btn_Input = false;
        inputHandler.x_Btn_Input = false;
        inputHandler.MouseWheel = Vector2.zero;

        inputHandler.Start_btn_Input = false;

        float delta = Time.deltaTime;

        if (isInAir && !isClimbing)
        {
            playerMovement.inAirTimer += Time.deltaTime;
        }
    }   

    public void ChangePlayerState(PlayerState state)
    {
        if (isInteracting && state != PlayerState.DEAD)
            return;

        if (currentState == state)
            return;

        Debug.Log(currentState + "," + state);

        switch (state)
        {
            case PlayerState.WALKING:
                transform.rotation.Set(0, transform.rotation.y, transform.rotation.z, transform.rotation.w);//Sets the rotation in X to 0 so the player is always upright when starts walking
                DisableCharacterColliderBlocker();
                if (areWeaponsHidden)
                    playerInventory.UnhideWeapons();
                currentState = state;
                break;
            case PlayerState.CLIMBING:
                DisableCharacterColliderBlocker();
                playerInventory.HideWeapons();
                isClimbing = true;
                isGrounded = true;
                areWeaponsHidden = true;
                currentState = state;
                break;
            case PlayerState.SWIMMING:
                DisableCharacterColliderBlocker();
                playerInventory.HideWeapons();
                areWeaponsHidden = true;
                currentState = state;
                break;
            case PlayerState.SLIDING:
                EnableCharacterColliderBlocker();
                currentState = state;
                break;
            case PlayerState.FALLING:
                EnableCharacterColliderBlocker();
                currentState = state;
                break;
            case PlayerState.DEAD:
                EnableCharacterColliderBlocker();
                currentState = state;
                inputHandler.OnDisable();
                inputHandler.DisableMenuControls();
                break;
            case PlayerState.INTERACTING:
                EnableCharacterColliderBlocker();
                playerInventory.HideWeapons();
                areWeaponsHidden = true;
                currentState = state;
                break;
            default:
                break;
        }
    }

    public void EnableCharacterColliderBlocker()
    {
        playerMovement.characterColliderBlocker.gameObject.SetActive(true);
    }

    public void DisableCharacterColliderBlocker()
    {
        playerMovement.characterColliderBlocker.gameObject.SetActive(false);
    }

    public void SetPlayerSkin(int playerkin)
    {
        Renderer skinRenderer = playerSkin.GetComponent<Renderer>();
        skinRenderer.sharedMaterial = kinSkins[playerkin];
    }
}
