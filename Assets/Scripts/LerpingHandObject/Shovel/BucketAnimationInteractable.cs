using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketAnimationInteractable : InteractableBase
{
    [SerializeField] Animator animator;
    [SerializeField] BucketTarget target;
    [SerializeField] bool requireFullBucket;
    public override bool CanInteract => !animator.enabled && base.CanInteract && requireFullBucket == Bucket.Instance.IsFull;

    public override bool bIsHeldInteraction => false;

    public void Finished()
    {
        animator.enabled = false;
    }
    public override void StartInteracting()
    {
        base.StartInteracting();
        BucketTarget.SetOverride(target);
        animator.enabled = true;
    }
}
