using System;
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
    [SerializeField] InteractionConditionBase[] conditions;

    private bool interactableBefore = false;
    public System.Action<bool> ChangeIsInteractable;
    public System.Action Interacted;
    public virtual bool CanInteract => AllConditionsMet();

    private bool AllConditionsMet()
    {
        foreach (InteractionConditionBase condition in conditions)
        {
            if (condition != null && !condition.IsMet()) return false;
        }

        return true;
    }

    public virtual void Interact()
    {
        Interacted?.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        if (interactable != interactableBefore)
            ChangeIsInteractable?.Invoke(interactable);

        interactableBefore = interactable;
    }
}
