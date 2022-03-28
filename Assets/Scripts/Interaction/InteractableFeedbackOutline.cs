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
        if (interactable != null) interactable.ChangeIsInteractable += OnChangeIsInteractable;
    }
    private void OnDisable()
    {
        if (interactable != null) interactable.ChangeIsInteractable -= OnChangeIsInteractable;
    }
    private void OnChangeIsInteractable(bool isInteractable)
    {
        outline.enabled = isInteractable;
    }
}
