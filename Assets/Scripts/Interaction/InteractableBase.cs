using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    bool CanInteract { get; }

    bool bIsHeldInteraction { get; }

    void StartInteracting();

    void StopInteracting();

    void SetHoveredState(HoveredState _hovered);
}

public enum HoveredState
{
    NotHovered,
    Hovered,
    HoveredButNotInteractable,
    BeingUsed
}


public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] InteractionConditionBase[] conditions;

    private HoveredState hoveredState = HoveredState.NotHovered;
    public System.Action<HoveredState> HoveredStateChanged;
    public System.Action Interacted;
    public virtual bool CanInteract => AllConditionsMet();

    public abstract bool bIsHeldInteraction { get; }

    private bool AllConditionsMet()
    {
        if (conditions == null)
            return true;

        foreach (InteractionConditionBase condition in conditions)
        {
            if (condition != null && !condition.IsMet()) return false;
        }

        return true;
    }

    public virtual void StartInteracting()
    {
        Interacted?.Invoke();
    }

    public virtual void StopInteracting()
    {

    }

    public void SetHoveredState(HoveredState _newState)
    {
        if (_newState != hoveredState)
            HoveredStateChanged?.Invoke(_newState);

        hoveredState = _newState;
    }
}