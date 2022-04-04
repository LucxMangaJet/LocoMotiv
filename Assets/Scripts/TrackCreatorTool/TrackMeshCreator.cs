using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackMeshCreator : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] TrackSection section;

    float tileLength = 4f, tileWidth = 5f;
    public void CreateMesh()
    {
        int pointMax = Mathf.RoundToInt(section.Legth / tileLength);
        Mesh gravel = CreateGravelBedMesh(pointMax);
        Mesh railsLeft = CreateRailMesh(pointMax, 0.8f);
        Mesh railsRight = CreateRailMesh(pointMax, -0.8f);
        Mesh bearers = CreateBearers(pointMax * 2);

        CombineInstance[] combines = new CombineInstance[2];
        combines[0].mesh = railsLeft;
        combines[0].transform = transform.localToWorldMatrix;
        combines[1].mesh = railsRight;
        combines[1].transform = transform.localToWorldMatrix;

        Mesh rails = new Mesh();
        rails.CombineMeshes(combines);

        combines = new CombineInstance[3];
        combines[0].mesh = rails;
        combines[0].transform = transform.localToWorldMatrix;
        combines[1].mesh = bearers;
        combines[1].transform = transform.localToWorldMatrix;
        combines[2].mesh = gravel;
        combines[2].transform = transform.localToWorldMatrix;

        Mesh merged = new Mesh();
        merged.CombineMeshes(combines, mergeSubMeshes: false);
        meshFilter.mesh = merged;
    }

    private Mesh CreateRailMesh(int pointMax, float xOffset)
    {
        int vertsPerPoint = 4;
        int trisPerPoint = (vertsPerPoint - 1) * 2 * 3;
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

            Vector3 up = new Vector3(0, 0.5f, 0);

            float railWidth = 0.4f;

            verts[vertIndex] = point.Position + point.Right * xOffset + point.Right * (railWidth * -0.5f) + up;
            verts[vertIndex + 1] = point.Position + point.Right * xOffset + point.Right * (railWidth * -0.5f) + up * 2;
            verts[vertIndex + 2] = point.Position + point.Right * xOffset + point.Right * (railWidth * 0.5f) + up * 2;
            verts[vertIndex + 3] = point.Position + point.Right * xOffset + point.Right * (railWidth * 0.5f) + up;

            float y = i;

            normals[vertIndex] = Vector3.left;
            normals[vertIndex + 1] = Vector3.Lerp(Vector3.left, Vector3.up, 0.5f);
            normals[vertIndex + 2] = Vector3.Lerp(Vector3.up, Vector3.right, 0.5f);
            normals[vertIndex + 3] = Vector3.right;

            if (i < pointMax)
            {
                for (int triOffset = 0; triOffset < vertsPerPoint - 1; triOffset++)
                {
                    DrawFaceTris(vertsPerPoint, tris, vertIndex, triIndex, triOffset);
                }
            }

            vertIndex += vertsPerPoint;
            triIndex += trisPerPoint;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.normals = normals;
        return mesh;
    }

    private Mesh CreateGravelBedMesh(int pointMax)
    {
        int vertsPerPoint = 4;
        int trisPerPoint = (vertsPerPoint - 1) * 2 * 3;
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

            Vector3 up = new Vector3(0, 0.5f, 0);

            verts[vertIndex] = point.Position - point.Right * tileWidth * 0.5f - up;
            verts[vertIndex + 1] = point.Position - point.Right * tileWidth * 0.4f + up;
            verts[vertIndex + 2] = point.Position + point.Right * tileWidth * 0.4f + up;
            verts[vertIndex + 3] = point.Position + point.Right * tileWidth * 0.5f - up;

            float y = i;

            uvs[vertIndex] = new Vector2(0, i);
            uvs[vertIndex + 1] = new Vector2(0.2f, i);
            uvs[vertIndex + 2] = new Vector2(0.8f, i);
            uvs[vertIndex + 3] = new Vector2(1f, i);

            normals[vertIndex] = Vector3.up;
            normals[vertIndex + 1] = Vector3.up;
            normals[vertIndex + 2] = Vector3.up;
            normals[vertIndex + 3] = Vector3.up;

            if (i < pointMax)
            {
                for (int triOffset = 0; triOffset < vertsPerPoint - 1; triOffset++)
                {
                    DrawFaceTris(vertsPerPoint, tris, vertIndex, triIndex, triOffset);
                }
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
    private Mesh CreateBearers(int pointMax)
    {
        int vertsPerPoint = 8;
        int trisPerPoint = 10 * 3;
        Vector3[] verts = new Vector3[(pointMax) * vertsPerPoint];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int[] tris = new int[trisPerPoint * pointMax];

        int vertIndex = 0;
        int triIndex = 0;

        float bearerWidth = 3f;
        float bearerThickness = 0.5f;

        for (int i = 0; i < pointMax; i++)
        {
            float t = ((float)i + 0.5f) / pointMax;
            TrackPoint point = section.CalculateTrackPointAtT(t, raw: false);

            Vector3 bottom = new Vector3(0, 0.5f, 0);
            Vector3 top = new Vector3(0, 0.75f, 0);

            verts[vertIndex] = point.Position - point.Right * bearerWidth * 0.5f + bottom - point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 1] = point.Position + point.Right * bearerWidth * 0.5f + bottom - point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 2] = point.Position + point.Right * bearerWidth * 0.5f + top - point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 3] = point.Position - point.Right * bearerWidth * 0.5f + top - point.Forward * bearerThickness * 0.5f;

            verts[vertIndex + 4] = point.Position - point.Right * bearerWidth * 0.5f + top + point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 5] = point.Position + point.Right * bearerWidth * 0.5f + top + point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 6] = point.Position + point.Right * bearerWidth * 0.5f + bottom + point.Forward * bearerThickness * 0.5f;
            verts[vertIndex + 7] = point.Position - point.Right * bearerWidth * 0.5f + bottom + point.Forward * bearerThickness * 0.5f;

            int[] triangles = {
                vertIndex +0, vertIndex +2, vertIndex +1, //face front
	            vertIndex +0, vertIndex +3, vertIndex +2,
                vertIndex +2, vertIndex +3, vertIndex +4, //face top
	            vertIndex +2, vertIndex +4, vertIndex +5,
                vertIndex +1, vertIndex +2, vertIndex +5, //face right
	            vertIndex +1, vertIndex +5, vertIndex +6,
                vertIndex +0, vertIndex +7, vertIndex +4, //face left
	            vertIndex +0, vertIndex +4, vertIndex +3,
                vertIndex +5, vertIndex +4, vertIndex +7, //face back
	            vertIndex +5, vertIndex +7, vertIndex +6,
            };

            for (int tIndex = 0; tIndex < triangles.Length; tIndex++)
            {
                tris[triIndex + tIndex] = triangles[tIndex];
            }

            vertIndex += vertsPerPoint;
            triIndex += trisPerPoint;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        return mesh;
    }

    private static void DrawFaceTris(int vertsPerPoint, int[] tris, int vertIndex, int triIndex, int triOffset)
    {
        tris[triIndex + (triOffset * 6)] = vertIndex + triOffset;
        tris[triIndex + (triOffset * 6) + 1] = vertIndex + triOffset + vertsPerPoint;
        tris[triIndex + (triOffset * 6) + 2] = vertIndex + triOffset + 1;

        tris[triIndex + (triOffset * 6) + 3] = vertIndex + triOffset + 1;
        tris[triIndex + (triOffset * 6) + 4] = vertIndex + triOffset + vertsPerPoint;
        tris[triIndex + (triOffset * 6) + 5] = vertIndex + triOffset + vertsPerPoint + 1;
    }
}
