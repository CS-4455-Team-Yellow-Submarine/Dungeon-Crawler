using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcState : MonoBehaviour
{
    private const string IDLE_ANIMATION_BOOL = "Idle";
    private const string ATTACK_ANIMATION_BOOL = "Attack";
    private Animator animator;
    #region
    public void AnimateIdle()
    {
        Animate(IDLE_ANIMATION_BOOL);
    }
    public void AnimateAttack()
    {
        Animate(ATTACK_ANIMATION_BOOL);
    }
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
        Animate(IDLE_ANIMATION_BOOL);
    }
    private void Animate(string boolName)
    {
        DisableOtherAnimations(animator, boolName);
        animator.SetBool(boolName, true);
    }

    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name != animation)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }
}