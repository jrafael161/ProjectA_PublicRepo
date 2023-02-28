using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour
{
    public string targetBool;//this refers to the action to be updated ex: isInteracting -> false
    public bool status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(targetBool, status);
        //TODO: deactivate the "running" state so no other animations than the intended can go on x2 speed
    }
}
