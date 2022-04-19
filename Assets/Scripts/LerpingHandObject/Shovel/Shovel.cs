using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : LerpingHandObject
{
    private void OnEnable()
    {
        ShovelTarget.ChangeOverride += OnChangeOverride;
    }
    private void OnDisable()
    {
        ShovelTarget.ChangeOverride -= OnChangeOverride;
    }

    private void OnChangeOverride(ShovelTarget target)
    {
        OverrideTarget = target;
    }
}
