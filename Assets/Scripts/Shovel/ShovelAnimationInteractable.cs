using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }

    void Interact();
}

public class ShovelAnimationInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] Animator animator;
    [SerializeField] ShovelTarget target;

    public bool CanInteract => !animator.enabled;

    public void Interact()
    {
        ShovelTarget.SetOverride(target);
        animator.enabled = true;
    }

    public void Finished()
    {
        animator.enabled = false;
    }
}
