using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnArrow : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerEffectsManager playerEffectsManager = animator.GetComponentInParent<PlayerEffectsManager>();
        
        if (playerEffectsManager == null)
            return;

        if (!animator.GetBool("arrowInHand"))
        {
            Destroy(playerEffectsManager.ArrowModel);
        }
    }
}
