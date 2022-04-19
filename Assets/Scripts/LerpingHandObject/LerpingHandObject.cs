using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpingHandObject : MonoBehaviour
{
    [SerializeField] HandObjectTarget baseTarget;
    [SerializeField] protected HandObjectTarget OverrideTarget;

    protected virtual void Awake()
    {
        OverrideTarget = baseTarget;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        HandObjectBlendValue value = new HandObjectBlendValue(baseTarget, OverrideTarget);
        transform.SetPositionAndRotation(value.Position, value.Rotatation);
    }
}
