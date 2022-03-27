using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    bool CanInteract { get; }
    void Interact();
    void SetInteractable(bool interactable);
}

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    private bool interactableBefore = false;
    public System.Action<bool> ChangeIsInteractable;
    public abstract bool CanInteract { get; }
    public abstract void Interact();

    public void SetInteractable(bool interactable)
    {
        if (interactable != interactableBefore)
            ChangeIsInteractable?.Invoke(interactable);

        interactableBefore = interactable;
    }
}
