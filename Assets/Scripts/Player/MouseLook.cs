using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform handOriginTransform;
    [SerializeField] float handOriginRotationMultiplier = 0.5f;
    float xRotation = 0f;
    [SerializeField] float mouseSensitivity = 100f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    public void Look(Vector2 input)
    {
        Vector2 mouse = input * mouseSensitivity;

        xRotation -= mouse.y * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        handOriginTransform.localRotation = Quaternion.Euler(xRotation * handOriginRotationMultiplier, 0f, 0f);

        playerTransform.Rotate(Vector3.up * mouse.x * Time.deltaTime);
    }
}
