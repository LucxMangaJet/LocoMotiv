using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : LerpingHandObject
{
    [SerializeField, Range(0, 1)] float fillValue = 0f;
    [SerializeField] MeshRenderer renderer;
    Material material;

    public static Bucket Instance;

    public bool IsFull => fillValue > 0.5f;

    protected override void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        base.Awake();
    }

    private void Start()
    {
        Material[] materials = renderer.materials;
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
        BucketTarget.ChangeOverride += OnChangeOverride;
    }
    private void OnDisable()
    {
        BucketTarget.ChangeOverride -= OnChangeOverride;
    }

    private void OnChangeOverride(BucketTarget target)
    {
        OverrideTarget = target;
    }
}
