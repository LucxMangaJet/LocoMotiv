using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputModule
{
    private const string INTERACT_INPUT = "Fire1";

    public Vector2 GetMouseInput()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public Vector2 GetMoveInput()
    {
        Vector2 axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (axis.magnitude > 1)
            return axis / axis.magnitude;

        return axis;
    }
    public bool InteractionInputPressed()
    {
        return Input.GetButtonDown(INTERACT_INPUT);
    }

    public bool InteractionInputReleased()
    {
        return Input.GetButtonUp(INTERACT_INPUT);
    }
}
