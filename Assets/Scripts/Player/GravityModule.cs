using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GravityModule 
{
    Vector3 velocity;

    [SerializeField] CharacterController characterController;
    public void ApplyGravity()
    {
        velocity.y += (Physics.gravity.y * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    public void ResetToGround()
    {
        velocity = Vector3.zero;
    }
}
