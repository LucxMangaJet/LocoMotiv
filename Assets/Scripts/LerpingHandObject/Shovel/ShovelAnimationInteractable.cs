using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelAnimationInteractable : InteractableBase
{
    [SerializeField] Animator animator;
    [SerializeField] ShovelTarget target;
    public override bool CanInteract => !animator.enabled && base.CanInteract;

    public override bool bIsHeldInteraction => false;

    public void Finished()
    {
        animator.enabled = false;
    }
    public override void StartInteracting()
    {
        base.StartInteracting();
        ShovelTarget.SetOverride(target);
        animator.enabled = true;
    }
}
