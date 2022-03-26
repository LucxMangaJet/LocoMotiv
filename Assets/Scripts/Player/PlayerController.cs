using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public InputModule InputModule;
    [SerializeField] public MoveModule moveModule;
    [SerializeField] GravityModule gravityModule;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] GroundCheckModule groundCheckModule;

    private void Update()
    {
        moveModule.Move(InputModule.GetMoveInput());
        mouseLook.Look(InputModule.GetMouseInput());

        if (!groundCheckModule.IsDetecting())
            gravityModule.ApplyGravity();
        else
            gravityModule.ResetToGround();
    }
}
