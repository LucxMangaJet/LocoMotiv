using System;
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
    static MoveInputOverride moveInputOverride = null;

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
        Vector2 moveInput = InputModule.GetMoveInput();
        if (moveInputOverride != null)
            moveInputOverride.Input(moveInput);
        else
            moveModule.Move(moveInput);

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position, "Player");
    }
#endif

    public static void SetRotateLock(PlayerRotateLock _lockType, bool _on)
    {
        if (_on)
            s_Rotatelock |= _lockType;
        else
            s_Rotatelock &= ~_lockType;
    }

    public static void SetMoveInputOverride(MoveInputOverride _moveInputOverride)
    {
        moveInputOverride = _moveInputOverride;
    }
}

public class MoveInputOverride
{
    public System.Action<Vector2> InputAction;
    public void Input(Vector2 moveInput)
    {
        InputAction?.Invoke(moveInput);
    }
}
