using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableFeedbackOutline : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    Material material;
    InteractableBase interactable;

    private void Awake()
    {
        interactable = GetComponent<InteractableBase>();
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;
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
        material.SetFloat("_Outline", isInteractable ? 1.25f : 0);
    }
}
