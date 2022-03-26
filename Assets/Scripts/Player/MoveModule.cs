using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveModule
{
    [SerializeField] Transform transform;
    [SerializeField] CharacterController characterController;
    [SerializeField] float moveSpeed;

    public System.Action<float> MoveEvent;


    // Update is called once per frame
    public void Move(Vector2 input)
    {
        Vector3 move = transform.right * input.x  + transform.forward * input.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        float speed = move.magnitude;
        MoveEvent?.Invoke(speed);
    }
}
