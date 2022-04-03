using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelAnimationInteractable : InteractableBase
{
    [SerializeField] Animator animator;
    [SerializeField] ShovelTarget target;
    public override bool CanInteract => !animator.enabled && base.CanInteract;

    public void Finished()
    {
        animator.enabled = false;
    }
    public override void Interact()
    {
        base.Interact();
        ShovelTarget.SetOverride(target);
        animator.enabled = true;
    }
}
