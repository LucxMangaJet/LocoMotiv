using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerRotateLock
{
    None,
    Lever = 1 << 0
}

public class PlayerController : MonoBehaviour
{
    static PlayerRotateLock s_Rotatelock = PlayerRotateLock.None;

    [SerializeField] public InputModule InputModule;
    [SerializeField] public MoveModule moveModule;
    [SerializeField] GravityModule gravityModule;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] GroundCheckModule groundCheckModule;
    [SerializeField] PlayerInteractionModule interactionModule;


    public void Awake()
    {
        s_Rotatelock = PlayerRotateLock.None;
    }

    private void Update()
    {
        moveModule.Move(InputModule.GetMoveInput());

        if (s_Rotatelock == PlayerRotateLock.None)
            mouseLook.Look(InputModule.GetMouseInput());

        groundCheckModule.Update();

        if (groundCheckModule.GroundedThisFrame)
        {
            transform.parent = groundCheckModule.LastHitTransform;
            gravityModule.ResetToGround();
        }
        else
            gravityModule.ApplyGravity();

        interactionModule.Update();

        if (interactionModule.IsInteracting)
        {
            if (InputModule.InteractionInputReleased())
                interactionModule.StopInteracting();
        }
        else if (interactionModule.CanInteract && InputModule.InteractionInputPressed())
            interactionModule.StartInteracting();
    }

    public static void SetRotateLock(PlayerRotateLock _lockType, bool _on)
    {
        if (_on)
            s_Rotatelock |= _lockType;
        else
            s_Rotatelock &= ~_lockType;
    }
}
