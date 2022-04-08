using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeShaderBehaviour : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] int materialIndex;
    Material material;

    private void Start()
    {
        Material[] materials = meshRenderer.materials;
        material = new Material(materials[materialIndex]);
        materials[materialIndex] = material;
        meshRenderer.materials = materials;
    }

    public void SetValue(float _percent)
    {
        material.SetFloat("value", _percent);
    }
}
