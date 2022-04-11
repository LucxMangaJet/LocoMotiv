using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableLeverGear : InteractableBase
{
    [SerializeField] FloatGearPair[] pairs;
    [SerializeField, Range(0, 1)] float leverPosition;
    [SerializeField, Range(0, 1)] float leverTargetPosition;
    [SerializeField] GameObject interactionCamera;
    [SerializeField] Animator animator;
    [SerializeField] TrainController trainController;

    MoveInputOverride currentOverride;

    public override bool bIsHeldInteraction => true;

    public override void StartInteracting()
    {
        SetInteractingState(true);
        currentOverride = new MoveInputOverride();
        PlayerController.SetMoveInputOverride(currentOverride);
        currentOverride.InputAction += Move;
    }
    public override void StopInteracting()
    {
        SetInteractingState(false);
        currentOverride.InputAction -= Move;
        currentOverride = null;
        PlayerController.SetMoveInputOverride(null);

        FloatGearPair pair = GetClosest(leverTargetPosition);
        leverTargetPosition = pair.Value;
        trainController.SwitchGear(pair.Gear);
    }
    private FloatGearPair GetClosest(float leverTargetPosition)
    {
        return pairs.OrderBy(pair => Mathf.Abs(pair.Value - leverTargetPosition)).First(); ;
    }

    private void SetInteractingState(bool interacting)
    {
        interactionCamera.SetActive(interacting);
        PlayerController.SetRotateLock(PlayerRotateLock.Lever, interacting);
    }
    private void Move(Vector2 input)
    {
        if (input.y == 0)
        {
            FloatGearPair pair = GetClosest(leverTargetPosition);
            leverTargetPosition = pair.Value;
        }
        else
        {
            leverTargetPosition = Mathf.Clamp(leverTargetPosition + input.y * Time.deltaTime, 0, 1);
        }
    }
    private void Update()
    {
        leverPosition = Mathf.MoveTowards(leverPosition, leverTargetPosition, Time.deltaTime);
        animator.SetFloat("position", leverPosition);
    }
}

[System.Serializable]
public class FloatGearPair
{
    public TrainGear Gear;
    public float Value;
}
