using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericLerpingHandObject<T> : LerpingHandObject
{
    private void OnEnable()
    {
        GenericHandObjectTarget<T>.ChangeOverride += OnChangeOverride;
    }
    private void OnDisable()
    {
        GenericHandObjectTarget<T>.ChangeOverride -= OnChangeOverride;
    }

    private void OnChangeOverride(T target)
    {
        OverrideTarget = target as HandObjectTarget;
    }
}
