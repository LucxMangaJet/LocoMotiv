using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableFeedbackOutline : MonoBehaviour
{
    [SerializeField] Outline outline;
    InteractableBase interactable;

    private void Awake()
    {
        interactable = GetComponent<InteractableBase>();
        outline.enabled = false;
    }

    private void OnEnable()
    {
        if (interactable != null) interactable.HoveredStateChanged += OnChangeIsInteractable;
    }
    private void OnDisable()
    {
        if (interactable != null) interactable.HoveredStateChanged -= OnChangeIsInteractable;
    }
    private void OnChangeIsInteractable(HoveredState _state)
    {

        switch (_state)
        {
            case HoveredState.NotHovered:
                outline.enabled = false;
                break;
            case HoveredState.Hovered:
                outline.enabled = true;
                outline.OutlineColor = Color.white;
                break;
            case HoveredState.HoveredButNotInteractable:
                outline.enabled = true;
                outline.OutlineColor = new Color(1, 1, 1, 0.1f);
                break;
            case HoveredState.BeingUsed:
                outline.enabled = true;
                outline.OutlineColor = Color.yellow;
                break;
        }
    }
}