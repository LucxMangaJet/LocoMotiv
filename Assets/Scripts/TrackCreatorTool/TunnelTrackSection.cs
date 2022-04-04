using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TunnelTrackSection : TrackSectionConfiguration
{
    [SerializeField, Expandable] RegularTrackSection regularBase;
    [SerializeField, Range(-5f, 5f)] float tunnelHeightOffset;
    [SerializeField, Range(0, 10f)] float tunnelHeight;
    [SerializeField, Range(1f, 5f)] private float tunnelTileLengt = 2f;
    [SerializeField, Range(1f, 10f)] private float tunnelWidth = 5f;
    [SerializeField, Range(0f, 1f)] private float tunnelCornerBevel = 0.25f;

    [SerializeField] Mesh tunnelEntranceArch;
    [SerializeField] Mesh tunnelEntranceBlock;


    [SerializeField] Mesh tunnelExitArch;
    [SerializeField] Mesh tunnelExitBlock;

    [SerializeField] Material tunnelMat, tuunelBlockMat, tunnelArchMat;

    public override bool IsTunnel => true;

    public override TrackMeshCreationResult[] CreateMesh(Transform transform, TrackSection section)
    {

        List<TrackMeshCreationResult> results = new List<TrackMeshCreationResult>(regularBase.CreateMesh(transform, section));

        if (!CheckTunnel(section.Route.GetPrevious(section)))
        {
            results.Add(new TrackMeshCreationResult()
            {
                Mesh = tunnelEntranceBlock,
                Transform = section.StartTransform,
                Material = tuunelBlockMat
            });

            results.Add(new TrackMeshCreationResult()
            {
                Mesh = tunnelEntranceArch,
                Transform = section.StartTransform,
                Material = tunnelArchMat
            });
        }

        if (!CheckTunnel(section.Route.GetNext(section)))
        {
            results.Add(new TrackMeshCreationResult()
            {
                Mesh = tunnelExitArch,
                Transform = section.EndTransform,
                Material = tunnelArchMat
            });


            results.Add(new TrackMeshCreationResult()
            {
                Mesh = tunnelExitBlock,
                Transform = section.EndTransform,
                Material = tuunelBlockMat
            });
        }

        results.Add(new TrackMeshCreationResult()
        {
            Mesh = CreateTunnelMesh(transform, section),
            Transform = transform,
            Material = tunnelMat
        });

        return results.ToArray();
    }

    private bool CheckTunnel(TrackSection trackSection)
    {
        if (trackSection == null) return false;
        return trackSection.IsTunnel;
    }

    private Mesh CreateTunnelMesh(Transform transform, TrackSection section)
    {
        int pointMax = Mathf.RoundToInt(section.Length / tunnelTileLengt);

        int vertsPerPoint = 6;
        int trisPerPoint = (vertsPerPoint) * 2 * 3;
        Vector3[] verts = new Vector3[(pointMax + 1) * vertsPerPoint];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int[] tris = new int[trisPerPoint * pointMax];

        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i <= pointMax; i++)
        {
            float t = (float)i / pointMax;
            TrackPoint point = section.CalculateTrackPointAtT(t, raw: false);

            Vector3 baseHeight = new Vector3(0, tunnelHeightOffset, 0);
            Vector3 topHeight = new Vector3(0, tunnelHeight, 0);
            Vector3 bevel = new Vector3(0, (tunnelWidth * 0.5f * tunnelCornerBevel), 0);

            verts[vertIndex] = point.Position + point.Right * tunnelWidth * 0.5f + baseHeight;
            verts[vertIndex + 1] = point.Position + point.Right * tunnelWidth * 0.5f + baseHeight + topHeight - bevel;
            verts[vertIndex + 2] = point.Position + point.Right * tunnelWidth * 0.5f * (1 - tunnelCornerBevel) + baseHeight + topHeight;
            verts[vertIndex + 3] = point.Position - point.Right * tunnelWidth * 0.5f * (1 - tunnelCornerBevel) + baseHeight + topHeight;
            verts[vertIndex + 4] = point.Position - point.Right * tunnelWidth * 0.5f + baseHeight + topHeight - bevel;
            verts[vertIndex + 5] = point.Position - point.Right * tunnelWidth * 0.5f + baseHeight;

            float y = i;

            float tunnelWidthAndHeight = tunnelWidth * 2f + tunnelHeight;

            uvs[vertIndex] = new Vector2(0, i);
            uvs[vertIndex + 1] = new Vector2(tunnelHeight, i);
            uvs[vertIndex + 2] = new Vector2(tunnelHeight + tunnelWidth * 0.5f * tunnelCornerBevel, i);
            uvs[vertIndex + 3] = new Vector2(tunnelHeight + tunnelWidth * (1 - (0.5f * tunnelCornerBevel)), i);
            uvs[vertIndex + 4] = new Vector2(tunnelHeight + tunnelWidth, i);
            uvs[vertIndex + 5] = new Vector2(tunnelHeight * 2 + tunnelWidth, i);

            normals[vertIndex] = Vector3.left;
            normals[vertIndex + 1] = Vector3.left;
            normals[vertIndex + 2] = Vector3.down;
            normals[vertIndex + 3] = Vector3.down;
            normals[vertIndex + 4] = Vector3.right;
            normals[vertIndex + 5] = Vector3.right;


            if (i < pointMax)
            {
                for (int triOffset = 0; triOffset < vertsPerPoint - 1; triOffset++)
                {
                    DrawFaceTris(vertsPerPoint, tris, vertIndex, triIndex, triOffset);
                }

                DrawFloorFaceTris(vertsPerPoint, tris, vertIndex, triIndex);
            }

            vertIndex += vertsPerPoint;
            triIndex += trisPerPoint;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.normals = normals;
        return mesh;
    }

    private static void DrawFloorFaceTris(int vertsPerPoint, int[] tris, int vertIndex, int triIndex)
    {
        tris[triIndex + (5 * 6)] = vertIndex + (vertsPerPoint - 1);
        tris[triIndex + (5 * 6) + 1] = vertIndex + vertsPerPoint + (vertsPerPoint -1);
        tris[triIndex + (5 * 6) + 2] = vertIndex;

        tris[triIndex + (5 * 6) + 3] = vertIndex;
        tris[triIndex + (5 * 6) + 4] = vertIndex + vertsPerPoint + (vertsPerPoint - 1);
        tris[triIndex + (5 * 6) + 5] = vertIndex + vertsPerPoint;
    }
}
