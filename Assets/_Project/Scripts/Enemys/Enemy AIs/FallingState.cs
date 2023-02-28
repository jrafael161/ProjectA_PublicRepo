using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : State
{
    public PersueTargetState persueTargetState;
    float groundDetectionRayStartPoint = 0.5f;
    float minimumDistanceNeededToBeginFall = 1f;
    float groundDirectionRayDistance = 0.05f;
    float slopeLimit = 60;
    public float inAirTimer;
    bool firstImpulse = false;
    public LayerMask ignoreForGroundCheck;
    Vector3 normalVector;
    Vector3 targetPosition;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim)
    {
        RaycastHit hit;
        //Vector3 origin = myTransform.position;//+ new Vector3(0, 0, -.5f);
        //origin.y += 1;

        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        enemyManager.isGrounded = false;

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
                if (inAirTimer > 0.7f && GetSlopeSteepnes(enemyManager) < slopeLimit)
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
                return persueTargetState;
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
                Vector3 vel = enemyManager.enemyRigidBody.velocity;
                vel.Normalize();
                enemyManager.enemyRigidBody.velocity = vel * (1 / 2);
                Debug.Log("aqui");
                enemyManager.isInAir = true;
            }

            return this;
        }

        if (enemyManager.isGrounded)
        {
            if (enemyManager.isInteracting || enemyManager.enemyRigidBody.velocity.magnitude > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
            else
            {
            transform.position = targetPosition;
            }
        }

        if (enemyManager.isInteracting || enemyManager.enemyRigidBody.velocity.magnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
        }
        else
        {
            transform.position = targetPosition;
        }

        return this;
    }

    public float GetSlopeSteepnes(EnemyManager enemyManager)
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
