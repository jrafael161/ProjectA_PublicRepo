using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public PlayerManager playerManager;
    public EnemyManager enemyManager;

    public Animator anim;
    public bool canRotate;
    public void playTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = true, bool mirrorAnim = false)
    {
        if(playerManager == null)
        {
            playerManager = GetComponentInParent<PlayerManager>();
        }        
        anim.applyRootMotion = isInteracting;
        anim.SetBool("canRotate", canRotate);
        anim.SetBool("isInteracting", isInteracting);
        anim.SetBool("isMirrored", mirrorAnim);
        anim.Play(targetAnim);
        //anim.CrossFade(targetAnim, 0.2f); Using this causes the step back animation to be out of sync
    }

    public void playEnemyTargetAnimation(string targetAnim, bool isInteracting)
    {
        if (enemyManager == null)
        {
            enemyManager = GetComponentInParent<EnemyManager>();
        }
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        enemyManager.isInteracting = isInteracting;
        anim.Play(targetAnim);
    }
}
