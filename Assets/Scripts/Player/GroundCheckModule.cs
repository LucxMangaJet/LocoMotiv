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

    private RaycastHit lastHit;
    private bool groundedThisFrame;

    public bool GroundedThisFrame => groundedThisFrame;
    public Transform LastHitTransform => lastHit.transform;

    public void Update()
    {
        groundedThisFrame = Physics.SphereCast(origin.position + offset, distance, Vector3.down, out lastHit, 0.1f, mask);
    }

}
