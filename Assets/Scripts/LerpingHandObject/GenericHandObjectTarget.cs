using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericHandObjectTarget<T> : HandObjectTarget
{
    public static System.Action<T> ChangeOverride;
    internal static void SetOverride(T target)
    {
        ChangeOverride?.Invoke(target);
    }
}
