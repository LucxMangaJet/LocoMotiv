using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackMeshCreator : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] TrackSection section;
    [SerializeField, Expandable] TrackSectionConfiguration configuration;
    public TrackSectionConfiguration Configuration => configuration;
    public void UpdateMesh()
    {
        TrackMeshCreationResult[] creationResult = configuration.CreateMesh(transform, section);

        List<Material> materials = new List<Material>();
        List<CombineInstance> merge = new List<CombineInstance>();

        foreach (TrackMeshCreationResult result in creationResult)
        {
            CombineInstance instance = new CombineInstance();
            instance.mesh = result.Mesh;
            instance.transform = result.Transform.localToWorldMatrix;
            merge.Add(instance);
            materials.Add(result.Material);
        }

        Mesh merged = new Mesh();
        merged.CombineMeshes(merge.ToArray(), mergeSubMeshes: false);

        meshFilter.mesh = merged;
        meshRenderer.sharedMaterials = materials.ToArray();
    }
}
