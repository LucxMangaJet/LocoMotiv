using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelAnimationInteractable : InteractableBase
{
    [SerializeField] Animator animator;
    [SerializeField] ShovelTarget target;
    public override bool CanInteract => !animator.enabled;

    public void Finished()
    {
        animator.enabled = false;
    }
    public override void Interact()
    {
        ShovelTarget.SetOverride(target);
        animator.enabled = true;
    }
}
