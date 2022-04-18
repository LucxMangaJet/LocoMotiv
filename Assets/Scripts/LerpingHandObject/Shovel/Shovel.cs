using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : LerpingHandObject
{
    private void OnEnable()
    {
        ShovelTarget.ChangeOverrideShovel += OnChangeOverride;
    }
    private void OnDisable()
    {
        ShovelTarget.ChangeOverrideShovel -= OnChangeOverride;
    }

    private void OnChangeOverride(ShovelTarget target)
    {
        OverrideTarget = target;
    }
}
