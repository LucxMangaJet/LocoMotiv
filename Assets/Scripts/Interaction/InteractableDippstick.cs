using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDippstick : InteractableBase
{
    [SerializeField] WaterDippstickTarget target;
    [SerializeField] Animator animator;
    public override bool bIsHeldInteraction => true;

    private void Start()
    {
        WaterDippstickTarget.ChangeOverride?.Invoke(target);
        target.Opacity = 1f;
    }

    public override void StartInteracting()
    {
        base.StartInteracting();
        animator.enabled = true;
        animator.Play("CheckEngineWaterDippstick");
    }

    public override void StopInteracting()
    {
        base.StopInteracting();
        animator.Play("PutBackEngineWaterDippstick");
    }

    public void Finished()
    {
        animator.enabled = false;
    }
}
