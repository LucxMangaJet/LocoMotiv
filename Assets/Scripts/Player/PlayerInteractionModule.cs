using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInteractionModule
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactionDistance = 2f;

    IInteractable interactable;
    public bool CheckForCanInteract()
    {
        Vector3 pos = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;


        IInteractable before = interactable;
        interactable = RaycastForInteractable(pos, dir, interactionDistance);

        if (before != null && before != interactable)
            before.SetInteractable(false);

        bool canInteract = interactable != null && interactable.CanInteract;

        if (interactable != null)
            interactable.SetInteractable(canInteract);

        Debug.DrawRay(pos, dir * interactionDistance, canInteract ? Color.green : Color.red);

        return canInteract;
    }

    private IInteractable RaycastForInteractable(Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, distance))
            return hit.collider.GetComponent<IInteractable>();

        return null;
    }

    internal void Interact()
    {
        interactable.Interact();
    }
}
