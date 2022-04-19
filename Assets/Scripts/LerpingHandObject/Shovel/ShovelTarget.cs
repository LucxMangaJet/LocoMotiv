using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelTarget : HandObjectTarget
{
    public static System.Action<ShovelTarget> ChangeOverrideShovel;

    internal static void SetOverride(ShovelTarget target)
    {
        ChangeOverrideShovel?.Invoke(target);
    }
}