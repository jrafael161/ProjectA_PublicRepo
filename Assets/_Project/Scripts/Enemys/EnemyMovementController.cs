using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimControler enemyAnim;

    [Header("Ground and Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    float groundDirectionRayDistance = 0.05f;
    [SerializeField]
    float minimumDistanceToCancelClimb = 0.3f;
    [SerializeField]
    float fallingSpeed = 45;
    [SerializeField]
    float slopeLimit = 60;
    [SerializeField]
    float knockOffForce = 5;//Force added to the player so it doesnt gets stuck at the edge of things when falling

    public LayerMask ignoreForGroundCheck;

    public float inAirTimer;
    bool firstImpulse = false;

    [Header("Movement Stats")]
    [SerializeField]
    float walkingSpeed = 3;
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;

    Vector3 normalVector;
    Vector3 targetPosition;

    public CapsuleCollider characterCollider;
    public CapsuleCollider characterColliderBlocker;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnim = GetComponentInChildren<EnemyAnimControler>();

        Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        RaycastHit hit;
        //Vector3 origin = myTransform.position;//+ new Vector3(0, 0, -.5f);
        //origin.y += 1;

        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        enemyManager.isGrounded = false;

        if (Physics.Raycast(origin, transform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (enemyManager.isInAir)
        {
            if (!firstImpulse)
            {
                enemyManager.enemyRigidBody.AddForce(moveDirection * knockOffForce, ForceMode.Impulse);
                firstImpulse = true;
            }
            enemyManager.enemyRigidBody.AddForce(-Vector3.up * fallingSpeed + enemyManager.enemyRigidBody.velocity);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = transform.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            enemyManager.isGrounded = true;
            targetPosition.y = tp.y;
            if (enemyManager.isInAir)
            {
                if (inAirTimer > 0.7f && GetSlopeSteepnes() < slopeLimit)
                {
                    Debug.Log("You were in air for " + inAirTimer);
                    enemyAnim.playTargetAnimation("Land", true, false);
                    firstImpulse = false;
                }
                else
                {
                    Debug.Log("tambien esto?");
                    enemyAnim.playTargetAnimation("Empty", false);
                    firstImpulse = false;
                }
                inAirTimer = 0;
                enemyManager.isInAir = false;
                enemyManager.isInteracting = false;
                enemyManager.navMeshAgent.transform.SetPositionAndRotation(transform.position,transform.rotation);
            }
        }
        else
        {
            if (enemyManager.isGrounded)
            {
                enemyManager.isGrounded = false;
            }

            if (enemyManager.isInAir == false)
            {
                if (enemyManager.isInteracting == false)
                {
                    enemyManager.isInteracting = true;
                    enemyAnim.playTargetAnimation("StartingToFall", true, false);
                }
                Vector3 vel = enemyManager.enemyRigidBody.velocity;
                vel.Normalize();
                enemyManager.enemyRigidBody.velocity = vel * (movementSpeed / 2);
                Debug.Log("aqui");
                enemyManager.isInAir = true;
            }
        }

        if (enemyManager.isGrounded)
        {
            if (enemyManager.isInteracting)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
            }
        }

        if (enemyManager.isInteracting)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    public float GetSlopeSteepnes()
    {
        if (!enemyManager.isGrounded)
            return 0;

        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, -Vector3.up, out RaycastHit slopeHit, minimumDistanceNeededToBeginFall))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            return slopeAngle;
        }
        return 0;
    }
}
