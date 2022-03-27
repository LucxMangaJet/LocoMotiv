using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    [SerializeField] ShovelTarget baseTarget;
    ShovelTarget overrideTarget;

    private void Awake()
    {
        overrideTarget = baseTarget;
    }

    private void OnEnable()
    {
        ShovelTarget.ChangeOverrideShovel += OnChangeOverrideShovel;
    }
    private void OnDisable()
    {
        ShovelTarget.ChangeOverrideShovel -= OnChangeOverrideShovel;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ShovelBlendValue value = new ShovelBlendValue(baseTarget, overrideTarget);
        transform.SetPositionAndRotation(value.Position, value.Rotatation);
    }

    private void OnChangeOverrideShovel(ShovelTarget target)
    {
        overrideTarget = target;
    }
}
