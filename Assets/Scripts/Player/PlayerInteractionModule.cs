using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInteractionModule
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactionDistance = 2f;

    IInteractable hovered;
    IInteractable activeInteractable;

    private bool canInteract;

    public bool IsInteracting => activeInteractable != null;

    public bool CanInteract => canInteract;

    public void Update()
    {
        Vector3 pos = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        IInteractable before = hovered;
        hovered = RaycastForInteractable(pos, dir, interactionDistance);

        if (before != null && before != hovered)
            before.SetHoveredState(HoveredState.NotHovered);

        bool canInteract = !IsInteracting && hovered != null && hovered.CanInteract;

        HoveredState state = (hovered != null && hovered == activeInteractable) ? HoveredState.BeingUsed : canInteract ? HoveredState.Hovered : HoveredState.HoveredButNotInteractable;

        hovered?.SetHoveredState(state);

        //Debug.DrawRay(pos, dir * interactionDistance, canInteract ? Color.green : Color.red);

        this.canInteract = canInteract;
    }

    private IInteractable RaycastForInteractable(Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, distance))
            return hit.collider.GetComponent<IInteractable>();

        return null;
    }

    public void StartInteracting()
    {
        hovered.StartInteracting();

        if (hovered.bIsHeldInteraction)
            activeInteractable = hovered;
    }

    public void StopInteracting()
    {
        activeInteractable.StopInteracting();
        activeInteractable = null;
    }
}