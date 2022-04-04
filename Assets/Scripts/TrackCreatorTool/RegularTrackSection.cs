using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegularTrackSection : TrackSectionConfiguration
{
    [SerializeField, Range(1f,10f)] float tileLength = 4f;
    [SerializeField, Range(1f, 10f)] float tileWidth = 5f;
    [SerializeField, Range(-5f, 5f)] float heightOffset;
    [SerializeField, Range(0f, 2f)] float gravelBedHeight;


    [SerializeField, Range(0f, 2f)] float railDistance = 1.5f;
    [SerializeField, Range(0f, 2f)] float railHeight = 0.5f;
    [SerializeField, Range(0f, 1f)] float railWidth = 0.4f;

    [SerializeField, Range(1f, 5f)] int bearersPerSegment = 2;

    [SerializeField, Range(0f, 2f)] float bearerHeight = 0.5f;
    [SerializeField, Range(0f, 5f)] float bearerWidth = 3f;
    [SerializeField, Range(0f, 2f)] float bearerThickness = 0.5f;

    [SerializeField] Material railMat, bearerMat, gravelMat;

    public override TrackMeshCreationResult[] CreateMesh(Transform transform,TrackSection section)
    {
        int pointMax = Mathf.RoundToInt(section.Legnth / tileLength);

        Mesh railsLeft = CreateRailMesh(section, pointMax, railDistance / 2f);
        Mesh railsRight = CreateRailMesh(section, pointMax, -railDistance / 2f);

        CombineInstance[] combines = new CombineInstance[2];
        combines[0].mesh = railsLeft;
        combines[0].transform = transform.localToWorldMatrix;
        combines[1].mesh = railsRight;
        combines[1].transform = transform.localToWorldMatrix;

        Mesh rails = new Mesh();
        rails.CombineMeshes(combines);

        List<TrackMeshCreationResult> results = new List<TrackMeshCreationResult>();

        results.Add(new TrackMeshCreationResult() { Material = railMat, Mesh = rails, Transform = transform });
        results.Add(new TrackMeshCreationResult() { Material = gravelMat, Mesh = CreateGravelBedMesh(section, pointMax), Transform = transform });
        results.Add(new TrackMeshCreationResult() { Material = bearerMat, Mesh = CreateBearers(section, pointMax * bearersPerSegment), Transform = transform });

        return results.ToArray();
    }

    private Mesh CreateRailMesh(TrackSection section, int pointMax, float xOffset)
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

            Vector3 lowest = new Vector3(0, heightOffset + gravelBedHeight, 0);
            Vector3 highest = new Vector3(0, heightOffset + gravelBedHeight + railHeight, 0);

            verts[vertIndex] = point.Position + point.Right * xOffset + point.Right * (railWidth * -0.5f) + lowest;
            verts[vertIndex + 1] = point.Position + point.Right * xOffset + point.Right * (railWidth * -0.5f) + highest;
            verts[vertIndex + 2] = point.Position + point.Right * xOffset + point.Right * (railWidth * 0.5f) + highest;
            verts[vertIndex + 3] = point.Position + point.Right * xOffset + point.Right * (railWidth * 0.5f) + lowest;

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

    private Mesh CreateGravelBedMesh(TrackSection section, int pointMax)
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

            Vector3 baseHeight = new Vector3(0, heightOffset, 0);
            Vector3 gravelHeight = new Vector3(0, gravelBedHeight, 0);

            verts[vertIndex] = point.Position - point.Right * tileWidth * 0.5f + baseHeight;
            verts[vertIndex + 1] = point.Position - point.Right * tileWidth * 0.4f + baseHeight + gravelHeight;
            verts[vertIndex + 2] = point.Position + point.Right * tileWidth * 0.4f + baseHeight + gravelHeight;
            verts[vertIndex + 3] = point.Position + point.Right * tileWidth * 0.5f + baseHeight;

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
    private Mesh CreateBearers(TrackSection section, int pointMax)
    {
        int vertsPerPoint = 8;
        int trisPerPoint = 10 * 3;
        Vector3[] verts = new Vector3[(pointMax) * vertsPerPoint];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int[] tris = new int[trisPerPoint * pointMax];

        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < pointMax; i++)
        {
            float t = ((float)i + 0.5f) / pointMax;
            TrackPoint point = section.CalculateTrackPointAtT(t, raw: false);

            Vector3 bottom = new Vector3(0, heightOffset + gravelBedHeight, 0);
            Vector3 top = new Vector3(0, heightOffset + gravelBedHeight + bearerHeight, 0);

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
}
