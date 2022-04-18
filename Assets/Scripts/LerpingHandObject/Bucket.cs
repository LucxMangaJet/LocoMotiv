using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : LerpingHandObject
{
    [SerializeField, Range(0,1)] float fillValue = 0f;
    [SerializeField] MeshRenderer renderer;
    Material material;

    private void Start()
    {
        Material []  materials = renderer.materials;
        material = new Material(materials[2]);
        materials[2] = material;
        renderer.materials = materials;
    }

    internal void SetFillValue(float fillValue)
    {
        this.fillValue = fillValue;
        material.SetFloat("_water_level", fillValue);
    }

    private void OnEnable()
    {
        BucketTarget.ChangeOverrideBucket += OnChangeOverride;
    }
    private void OnDisable()
    {
        BucketTarget.ChangeOverrideBucket -= OnChangeOverride;
    }

    private void OnChangeOverride(BucketTarget target)
    {
        OverrideTarget = target;
    }
}
