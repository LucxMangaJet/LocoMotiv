using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackSectionConfiguration : ScriptableObject
{
    public abstract TrackMeshCreationResult[] CreateMesh(Transform transform, TrackSection section);
    protected static void DrawFaceTris(int vertsPerPoint, int[] tris, int vertIndex, int triIndex, int triOffset)
    {
        tris[triIndex + (triOffset * 6)] = vertIndex + triOffset;
        tris[triIndex + (triOffset * 6) + 1] = vertIndex + triOffset + vertsPerPoint;
        tris[triIndex + (triOffset * 6) + 2] = vertIndex + triOffset + 1;

        tris[triIndex + (triOffset * 6) + 3] = vertIndex + triOffset + 1;
        tris[triIndex + (triOffset * 6) + 4] = vertIndex + triOffset + vertsPerPoint;
        tris[triIndex + (triOffset * 6) + 5] = vertIndex + triOffset + vertsPerPoint + 1;
    }
}
