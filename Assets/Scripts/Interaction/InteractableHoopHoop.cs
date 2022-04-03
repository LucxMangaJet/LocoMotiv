using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHoopHoop : InteractableBase
{
    [SerializeField] TrainController train;

    public override bool bIsHeldInteraction => true;

    public override void StartInteracting()
    {
        base.StartInteracting();

        train.EnableWhistling();
    }

    public override void StopInteracting()
    {
        base.StopInteracting();
        train.DisableWhistling();
    }
}