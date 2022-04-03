using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class IntertactableFeedbackPlaySound : MonoBehaviour
{
    [SerializeField] StudioEventEmitter eventEmitter;
    InteractableBase interactable;
    private void OnEnable()
    {
        interactable = GetComponent<InteractableBase>();
        if (interactable != null) interactable.Interacted += OnInteracted;
    }

    private void OnDisable()
    {
        if (interactable != null) interactable.Interacted -= OnInteracted;
    }

    private void OnInteracted()
    {
        eventEmitter?.Play();
    }
}
