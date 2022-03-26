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

        interactable = RaycastForInteractable(pos, dir, interactionDistance);

        bool camInteract = interactable != null && interactable.CanInteract;

        Debug.DrawRay(pos, dir* interactionDistance, camInteract ? Color.green: Color.red);

        return camInteract;
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
