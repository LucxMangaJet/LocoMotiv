using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundCheckModule
{
    [SerializeField] Transform origin;
    [SerializeField] Vector3 offset;
    [SerializeField] LayerMask mask;
    [SerializeField] float distance;

    public bool IsDetecting()
    {
        return Physics.CheckSphere(origin.position + offset, distance, mask);
    }
}
