using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    CameraHandler cameraHandler;
    Transform cameraObject;
    InputHandler inputHandler;
    PlayerManager playerManager;
    PlayerInteraction playerInteraction;
    UIManager uIManager;
    InteractionIdentifier cachedInteractionIdentifier;

    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public PlayerAnimController playerAnimControl;

    public Rigidbody playerRigidbody;
    public GameObject normalCamera;

    [Header("Ground and Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    float groundDirectionRayDistance = 0.05f;
    [SerializeField]
    float minimumDistanceToCancelClimb = 0.3f;

    public LayerMask ignoreForGroundCheck;

    public float inAirTimer;
    public bool jumped;
    bool firstImpulse = false;

    [Header("Movement Stats")]
    [SerializeField]
    float walkingSpeed = 3;
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    float rotationSpeed = 10;
    [SerializeField]
    float fallingSpeed = 45;
    [SerializeField]
    float knockOffForce = 5;//Force added to the player so it doesnt gets stuck at the edge of things when falling
    [SerializeField]
    float jumpHeight = 5;
    [SerializeField]
    float jumpDistance = 5;
    [SerializeField]
    float slopeLimit = 60;
    [SerializeField]
    float slopeSlideSpeed = 1;
    [SerializeField]
    float slopeAngle;
    [SerializeField]
    float climbSpeed = 1;
    [SerializeField]
    float swimSpeed = 1;

    RaycastHit slopeHit;

    public CapsuleCollider characterCollider;
    public CapsuleCollider characterColliderBlocker;

    public Transform OverHeadClimbCheckPos;
    public Transform GetDownLedgeCheckPos;

    public Transform TopClimbCheckPos;
    public Transform MiddleClimbCheckPos;
    public Transform BottomClimbCheckPos;

    bool OverHeadClimbCheck;
    bool TopClimbCheck;
    bool MiddleClimbCheck;
    bool BottomClimbCheck;
    bool BackClimbJumpCheck;
    bool ClimbDownCheck;

    RaycastHit TopHit, MiddleHit, BottomHit, GroundHit, BackClimbJumpHit;
    Vector3 normalVector;
    Vector3 targetPosition;

    //[SerializeField]
    //FLOW.FlowTrigger flowTrigger;

    [SerializeField]
    float testVariable=1;

    bool alreadyRotating = false;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        playerAnimControl = GetComponentInChildren<PlayerAnimController>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        cameraHandler = CameraHandler._instance;
        uIManager = playerManager.UIManager;
        cameraObject = Camera.main.transform;
        myTransform = transform;
        playerAnimControl.Initialize();

        playerManager.isGrounded = true;
        //ignoreForGroundCheck = (1 << 6) & (1 << 14) & (1 << 15) & (1 << 16);

        Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
    }

    private void FixedUpdate()
    {
        if (jumped)
        {
            if (playerManager.isClimbing)
            {
                CalcClimbBackJump();
            }
            else
            {
                CalcJump();
            }
        }
    }    

    private void HandleRotation(float delta)
    {
        if (inputHandler.lockOnFlag)
        {
            if (inputHandler.sprintFlag || inputHandler.dodgeFlag)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                targetDirection.Normalize();
                targetDirection.y = 0;
                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 rotationDirection = moveDirection;
                rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
        }
        else
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }        
    }

    public void HandleMovement(float delta)
    {        
        if (inputHandler.dodgeFlag)
            return;

        if (playerManager.isInteracting)
            return;

        if (playerManager.isJumping)
            return;

        if (playerManager.isInAir)        
            return;

        if (playerManager.isClimbing)
            return;

        if (playerManager.isSwiming)
            return;

        RaycastHit hit;
        Debug.DrawRay(GetDownLedgeCheckPos.position, -transform.forward * .5f, Color.green, 0.1f, false);
        if (Physics.Raycast(GetDownLedgeCheckPos.position, -transform.forward, out hit, .5f, ignoreForGroundCheck) && hit.transform.tag == "climbableSurface")
        {
            if (!uIManager.interactionMessagePrompt.activeSelf)
            {
                uIManager.interactionMessagePrompt.GetComponentInChildren<TMPro.TMP_Text>(true).text = "E: Climb down";
                cachedInteractionIdentifier = hit.transform.gameObject.GetComponent<InteractionIdentifier>();
                playerInteraction.interactables.Add(cachedInteractionIdentifier);
                uIManager.interactionMessagePrompt.SetActive(true);
            }
        }
        else
        {
            if (uIManager.interactionMessagePrompt.activeSelf)
            {
                if (uIManager.interactionMessagePrompt.GetComponentInChildren<TMPro.TMP_Text>(true).text == "E: Climb down")
                {
                    playerInteraction.interactables.Remove(cachedInteractionIdentifier);
                    cachedInteractionIdentifier = null;
                    uIManager.interactionMessagePrompt.SetActive(false);
                }
            }            
        }

        Vector3 origin = characterCollider.transform.position;
        if(Physics.Raycast(origin, transform.forward, out hit, 1, ignoreForGroundCheck) && hit.transform.tag == "climbableSurface")
        {
            float climbSteepnes = Vector3.Angle(hit.normal, Vector3.up);
            if (climbSteepnes > 59)
                return;
        }

        playerManager.ChangePlayerState(PlayerState.WALKING);

        playerAnimControl.canRotate = true;
        playerAnimControl.anim.SetBool("canRotate", true);

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();        

        float speed = movementSpeed;

        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
        {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if (inputHandler.moveAmount < 0.5)
            {
                moveDirection *= walkingSpeed;
                playerManager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }            
        }

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        playerRigidbody.velocity = projectedVelocity;

        if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
        {
            playerAnimControl.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal);
        }        
        else
        {
            playerAnimControl.UpdateAnimatorValues(inputHandler.moveAmount, 0);
        }

        if (playerAnimControl.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleDodgeAndRunning(float delta)
    {
        if (playerManager.isInteracting)
            return;

        if (playerManager.isClimbing)
            return;

        if (inputHandler.dodgeFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                playerAnimControl.playTargetAnimation("Rolling", true, false);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                playerAnimControl.playTargetAnimation("Backstep", true, false);
            }
        }
    }

    public void HandleSlopes(float delta)
    {
        if (playerManager.isInteracting)
            return;

        if (playerManager.isClimbing)
            return;

        if (playerManager.isJumping)
            return;

        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideSpeed = movementSpeed + slopeSlideSpeed + delta;

        moveDirection = slopeDirection * -slideSpeed;
        moveDirection.y = moveDirection.y - slopeHit.point.y;
        if (playerManager.isSliding)
        {
            playerManager.isInteracting = true;
        }            
        else
        {
            playerManager.isInteracting = false;
        }            
    }

    public bool CheckSlopeSteepnes()
    {
        if (!playerManager.isGrounded || playerManager.isClimbing)
            return false;

        if (playerManager.isSwiming)
            return false;

        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, -Vector3.up, out slopeHit,minimumDistanceNeededToBeginFall))
        {
            slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if (slopeAngle > slopeLimit)
            {
                playerManager.isSliding = true;
                return true;
            }                
        }
        playerManager.isSliding = false;
        return false;
    }

    public float GetSlopeSteepnes()
    {
        if (!playerManager.isGrounded || playerManager.isClimbing)
            return 0;

        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, -Vector3.up, out slopeHit, minimumDistanceNeededToBeginFall))
        {
            slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            return slopeAngle;
        }
        return 0;
    }

    public float GetClimbSteepnes()
    {
        RaycastHit hit;
        Vector3 origin = playerRigidbody.transform.position;

        Debug.DrawRay(origin, -Vector3.up * 1.1F, Color.white, 3, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, 2))
        {
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            return slopeAngle;
        }
        return 0;
    }

    public void HandleClimbing(float delta)
    {
        if (playerManager.isInteracting)
            return;

        ClimbCheck();

        if (TopClimbCheck && BottomClimbCheck)
        {
            float BottomClimbSteepnes = Vector3.Angle(BottomHit.normal, Vector3.up);
            float GroundClimbSteepnes = Vector3.Angle(GroundHit.normal, Vector3.up);

            if (BottomClimbSteepnes <= 45 && GroundClimbSteepnes <= 45)
            {
                if (playerManager.isClimbing)
                {
                    Debug.Log(BottomClimbSteepnes + "," + GroundClimbSteepnes);
                    CancelClimb();
                    playerManager.ChangePlayerState(PlayerState.INTERSTATE);
                    return;
                }
                else
                    return;
            }
        }

        if (playerManager.isClimbing)
        {
            if (TopClimbCheck && BottomClimbCheck)//This also needs a cone of vision check in wich the player starts the climb
            {
                playerRigidbody.AddForce(transform.forward * 10, ForceMode.Force);//Adds a continous force forward so the player is always sticking to the wall
                Vector2 input = SquareToCircle(new Vector2(inputHandler.horizontal, inputHandler.vertical));

                if (inputHandler.moveAmount > 0 && inputHandler.sprintFlag)
                {
                    transform.forward = -MiddleHit.normal;
                    //rigidbody.position = Vector3.Lerp(rigidbody.position, hit.point + hit.normal *.51f, 10 * delta);
                    playerRigidbody.velocity = transform.TransformDirection(input) * climbSpeed*2;
                    playerAnimControl.anim.speed = 2;
                }
                else
                {
                    transform.forward = -MiddleHit.normal;
                    //rigidbody.position = Vector3.Lerp(rigidbody.position, hit.point + hit.normal *.51f, 10 * delta);
                    playerRigidbody.velocity = transform.TransformDirection(input) * climbSpeed;
                    playerAnimControl.anim.speed = 1;
                }

                if (!OverHeadClimbCheck)
                {                    
                    ClimbUpLedge();
                }

                playerAnimControl.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal);

                if (ClimbDownCheck && inputHandler.vertical < 0)
                {
                    StartCoroutine("RotatePlayerGradually");
                    playerAnimControl.playTargetAnimation("Climb Drop", true);
                    CancelClimb();
                }
            }
            else if (playerManager.isClimbing)
            {
                if (!jumped)//If its not in the middle of a jump
                {
                    Debug.Log(TopClimbCheck + "," + BottomClimbCheck);
                    CancelClimb();
                }
            }
        }
        else
        {
            if (BottomClimbCheck && TopClimbCheck)
            {
                Vector2 input = SquareToCircle(new Vector2(inputHandler.horizontal, inputHandler.vertical));

                if (playerManager.currentState != PlayerState.CLIMBING)
                {
                    playerManager.ChangePlayerState(PlayerState.CLIMBING);
                    playerManager.isInAir = false;
                    StartClimb(true);
                }

                //rigidbody.drag = 10;//without this sometimes mid climb the drag goes to 0, why?

                if (inputHandler.moveAmount > 0)
                {
                    transform.forward = -MiddleHit.normal;
                    //rigidbody.position = Vector3.Lerp(rigidbody.position, hit.point + hit.normal *.51f, 10 * delta);
                    playerRigidbody.velocity = transform.TransformDirection(input) * climbSpeed;
                }

                playerAnimControl.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal);
            }
        }
    }

    public void ClimbUpLedge()
    {
        if (!playerManager.isClimbing)
            return;

        playerAnimControl.anim.speed = 1;
        playerRigidbody.AddForce(transform.forward * 25, ForceMode.Impulse);
        playerRigidbody.velocity += new Vector3(0, 10, 0);
        playerAnimControl.playTargetAnimation("Climbing Up From Ledge", true);
    }
    public void ClimbDownLedge()
    {
        playerManager.ChangePlayerState(PlayerState.CLIMBING);
        playerAnimControl.playTargetAnimation("Climbing Down From Ledge", true);
        transform.Rotate(0, transform.rotation.y + 90, 0);//This is done because the animation leaves the player 90 degrees short of facin the wall
        StartClimb(false);
    }

    public void ClimbDropAndJump(float delta)
    {
        if (playerManager.isInteracting)
            return;

        if (playerManager.isClimbing == false)
            return;

        if (inputHandler.climbDropFlag && inputHandler.moveAmount == 0)
        {
            Vector3 origin = myTransform.position;
            origin.y += 1;
            RaycastHit hit;
            Physics.Raycast(origin, transform.forward, out hit, 1, ignoreForGroundCheck);
            transform.forward = hit.normal;
            playerRigidbody.position = Vector3.Lerp(playerRigidbody.position, hit.point + hit.normal, delta);
            CancelClimb();
            /*
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall*2, ignoreForGroundCheck))//Needs more work or other anim
            {
                playerAnimControl.playTargetAnimation("ClimbDrop", true);
            }*/
            return;
        }
        else if (inputHandler.climbJumpFlag && inputHandler.moveAmount == 0)
        {
            if (BackClimbJumpCheck)
            {
                transform.Rotate(0, transform.rotation.y + 180, 0);
                playerManager.isSprinting = false;
                StartCoroutine("JumpingTimeEnded");
                playerManager.isJumping = true;
                playerAnimControl.anim.SetBool("isJumping", true);
                playerAnimControl.playTargetAnimation("Climbing Jump", true, false);
                jumped = true;
            }
            else//This needs a proper animation or other coroutine that handles the impulse because it looks weird
            {
                StartCoroutine("RotatePlayerGradually");
                playerManager.isSprinting = false;
                StartCoroutine("JumpingTimeEnded");
                playerManager.isJumping = true;
                playerAnimControl.anim.SetBool("isJumping", true);
                playerAnimControl.playTargetAnimation("Climb Drop", true);
                jumped = true;
                CancelClimb();
            }
        }
        else if (inputHandler.climbJumpFlag && inputHandler.moveAmount > 0)
        {
            if (inputHandler.vertical > 0)
            {
                playerAnimControl.playTargetAnimation("Climb Hop Up", true, false);
                playerRigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);//Forward impulse so the player doesnt get away from the wall
            }
            else if (inputHandler.horizontal > 0)
            {
                playerAnimControl.playTargetAnimation("Climb Hop Right", true, false);
            }
            else if (inputHandler.horizontal < 0)
            {
                playerAnimControl.playTargetAnimation("Climb Hop Left", true, false);
            }
        }
    }

    public void StartClimb(bool impulse)
    {
        if (impulse)
        {
            Debug.Log("impulso");
            playerRigidbody.AddForce(transform.forward * 75, ForceMode.VelocityChange);//50 magical number so it gets close to the wall
        }

        uIManager.interactionMessagePrompt.GetComponentInChildren<TMPro.TMP_Text>().text = "Hold space bar: Drop down";
        uIManager.interactionMessagePrompt.SetActive(true);

        playerAnimControl.canRotate = false;
        playerAnimControl.anim.SetBool("canRotate", false);
        playerRigidbody.useGravity = false;
        playerRigidbody.drag = 10;
    }
    public void CancelClimb()
    {
        Debug.Log("Climb canceled");
        playerManager.isClimbing = false;
        playerAnimControl.canRotate = true;
        playerAnimControl.anim.SetBool("canRotate", true);
        playerRigidbody.useGravity = true;
        playerRigidbody.drag = 1;
        playerAnimControl.anim.speed = 1;
        uIManager.interactionMessagePrompt.SetActive(false);
        playerManager.ChangePlayerState(PlayerState.INTERSTATE);
    }

    public Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1) ? input.normalized : input;
    }

    private void ClimbCheck()//Sometimes the checks gets false because the positions gets inside the walls/terrains, how could I prevent this?
    {
        OverHeadClimbCheck = false;
        TopClimbCheck = false;
        MiddleClimbCheck = false;
        BottomClimbCheck = false;
        BackClimbJumpCheck = false;
        ClimbDownCheck = false;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Debug.DrawRay(OverHeadClimbCheckPos.position, forward * 10, Color.white, 0.1f, false);
        if(Physics.Raycast(OverHeadClimbCheckPos.position, forward * 10, out RaycastHit hit, 2, ignoreForGroundCheck))
            OverHeadClimbCheck = true;

        Debug.DrawRay(TopClimbCheckPos.position, forward * 2, Color.white, 0.1f, false);
        if (Physics.Raycast(TopClimbCheckPos.position, forward, out TopHit, 2, ignoreForGroundCheck) && TopHit.transform.tag == "climbableSurface")//I would need to evaluate the hit.normal against the player forward to validate ta is facing "enough" the wall
            TopClimbCheck = true;

        Debug.DrawRay(MiddleClimbCheckPos.position, forward * 2, Color.white, 0.1f, false);
        if (Physics.Raycast(MiddleClimbCheckPos.position, forward, out MiddleHit, 2, ignoreForGroundCheck) && MiddleHit.transform.tag == "climbableSurface")
            MiddleClimbCheck = true;

        Debug.DrawRay(BottomClimbCheckPos.position, forward * 2, Color.white, 0.1f, false);
        if (Physics.Raycast(BottomClimbCheckPos.position, forward, out BottomHit, 2, ignoreForGroundCheck) && BottomHit.transform.tag == "climbableSurface")
            BottomClimbCheck = true;

        Debug.DrawRay(playerManager.lockOnTransform.position, -forward * 2.5f, Color.white, 0.1f, false);
        if (Physics.Raycast(playerManager.lockOnTransform.position, -forward, out BackClimbJumpHit, 2.5f, ignoreForGroundCheck) && BackClimbJumpHit.transform.tag == "climbableSurface")
            BackClimbJumpCheck = true;

        Debug.DrawRay(transform.position, -transform.up * minimumDistanceToCancelClimb, Color.red, 0.1f, false);
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit atGroundLevel, minimumDistanceToCancelClimb, ignoreForGroundCheck))
            ClimbDownCheck = true;

        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;
        //Debug.DrawRay(origin, -Vector3.up * 2, Color.white, 0.1f, false);
        //Physics.Raycast(origin, -Vector3.up, out GroundHit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck);
    }

    public void HandleSwimming(float delta)
    {
        Vector3 origin = characterCollider.transform.position;

        if (playerManager.isSwiming)
        {
            playerAnimControl.canRotate = true;
            playerAnimControl.anim.SetBool("canRotate", true);

            playerManager.isGrounded = true;
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
                        
            if (inputHandler.moveAmount > 0)
            {
                moveDirection.y = inputHandler.CameraMovementY;
            }
            else
            {
                moveDirection.y = 0;
            }

            moveDirection.Normalize();

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
            {
                moveDirection *= swimSpeed * 2;
                playerAnimControl.anim.speed = 2;
            }
            else
            {
                playerAnimControl.anim.speed = 1;
                if (inputHandler.moveAmount < 0.5)
                {                    
                    moveDirection *= swimSpeed * 1.5f;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= swimSpeed;
                    playerManager.isSprinting = false;
                }
            }

            //Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            playerRigidbody.velocity = moveDirection;

            playerAnimControl.UpdateAnimatorValues(inputHandler.moveAmount, 0);            

            if (playerAnimControl.canRotate)
            {
                HandleRotation(delta);
            }
        }
    }

    public void StartSwim()
    {
        uIManager.interactionMessagePrompt.SetActive(false);
        Debug.Log("Entro en el agua");
        playerManager.isSwiming = true;
        playerManager.ChangePlayerState(PlayerState.SWIMMING);
    }

    public void EndSwim()
    {
        //if (!flowTrigger.Met)
        //{
        //    StartCoroutine("OutOfWaterCheck");
        //}
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        if (playerManager.isClimbing)
            return;

        if (playerManager.isSwiming)
            return;

        if (jumped)
            return;

        RaycastHit hit;
        //Vector3 origin = myTransform.position;//+ new Vector3(0, 0, -.5f);
        //origin.y += 1;

        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        playerManager.isGrounded = false;

        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }
        
        if (playerManager.isInAir && !jumped)
        {
            if (!firstImpulse)
            {
                playerRigidbody.AddForce(moveDirection * knockOffForce, ForceMode.Impulse);
                firstImpulse = true;
            }
            playerRigidbody.AddForce(-Vector3.up * fallingSpeed + playerRigidbody.velocity);    
        }        

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;
            if (playerManager.isInAir)
            {
                if (inAirTimer > 0.7f && GetSlopeSteepnes()<slopeLimit)
                {
                    Debug.Log("You were in air for " + inAirTimer);
                    playerAnimControl.playTargetAnimation("Land", true, false);
                    firstImpulse = false;
                }
                else
                {
                    Debug.Log("tambien esto?");
                    playerAnimControl.playTargetAnimation("Empty", false);
                    firstImpulse = false;
                }
                inAirTimer = 0;
                playerManager.isInAir = false;                
            }
        }
        else
        {
            if (playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if (playerManager.isInAir == false)
            {

                if (playerManager.isInteracting == false)
                {
                    if (playerManager.isClimbing)
                    {
                        CancelClimb();
                    }
                    playerManager.ChangePlayerState(PlayerState.FALLING);
                    playerAnimControl.playTargetAnimation("StartingToFall", true, false);
                }
                Vector3 vel = playerRigidbody.velocity;
                vel.Normalize();
                playerRigidbody.velocity = vel * (movementSpeed / 2);
                Debug.Log("aqui");
                playerManager.isInAir = true;
            }
        }

        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }
        
        if (playerManager.isInteracting || inputHandler.moveAmount > 0.1f)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
        }
        else
        {
            myTransform.position = targetPosition;
        }
    }

    public void HandleJump(float delta)
    {
        if (playerManager.isInteracting)
            return;

        if (CheckSlopeSteepnes())
            return;            

        if (inputHandler.y_Btn_Input && !Keyboard.current[Key.LeftCtrl].isPressed)
        {
            if(inputHandler.moveAmount == 0 && !playerManager.isClimbing)
            {
                //playerManager.isSprinting = false;
                //StartCoroutine("JumpingTimeEnded");
                //playerManager.isJumping = true;
                //playerAnimControl.anim.SetBool("isJumping", true);
                //playerAnimControl.playTargetAnimation("Jump", true);                
                playerAnimControl.playTargetAnimation("JumpStanding", true, false);
                //jumped = true;

            }
            if (inputHandler.moveAmount > 0 && !playerManager.isClimbing)
            {
                playerManager.isSprinting = false;
                StartCoroutine("JumpingTimeEnded");
                playerManager.isJumping = true;
                playerAnimControl.anim.SetBool("isJumping", true);
                playerAnimControl.playTargetAnimation("Jump", true, false);
                jumped = true;
            }
        }
    }

    public void CalcJump()
    {
        playerRigidbody.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        playerRigidbody.AddForce(transform.forward * jumpDistance, ForceMode.Impulse);
    }

    public void CalcClimbBackJump()
    {
        playerRigidbody.AddForce(transform.forward * jumpDistance, ForceMode.Impulse);
    }

    IEnumerator JumpingTimeEnded()
    {
        yield return new WaitForSeconds(.6f);
        playerManager.isJumping = false;
        jumped = false;
        playerAnimControl.anim.SetBool("isJumping", false);
    }

    IEnumerator OutOfWaterCheck()//Sometimes the trigger changes to unmet quickly this is to make sure the player its actually out of the water
    {
        yield return new WaitForSeconds(.5f);
        //if (!flowTrigger.Met)
        //{
        //    Debug.Log("Salio del agua");
        //    playerAnimControl.anim.speed = 1;//It makes sure the animations go back to their normal speed when the player leaves the water
        //    playerManager.isSwiming = false;
        //    //playerManager.ChangePlayerState(PlayerState.INTERSTATE);
        //}
    }

    IEnumerator RotatePlayerGradually()//When the animation is replaced for the final this will be not needed
    {
        if (alreadyRotating)
            yield return null;
        else
        {
            alreadyRotating = true;
            for (int i = 0; i < 45; i++)
            {
                transform.Rotate(0, 4, 0);
                yield return null;
            }
            alreadyRotating = false;
        }
    }

}
