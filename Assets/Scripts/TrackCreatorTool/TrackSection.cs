using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEditor;

public class TrackSection : MonoBehaviour
{
    [SerializeField] Transform start, end;
    [SerializeField] Vector3 startTangent, endTangent;
    [SerializeField] float startTangentHeightOffset, endTangentHeightOffset;
    [SerializeField, ReadOnly] float length;
    [SerializeField, ReadOnly] AnimationCurve tDistanceCleanup;
    [SerializeField] public TrackMeshCreator MeshCreator;
    [SerializeField] public TrackRoute Route;
    public float Length => length;
    public Transform StartTransform => start;
    public Transform EndTransform => end;
    public Vector3 StartTangent => startTangent;
    public Vector3 EndTangent => endTangent;

    public bool IsTunnel => MeshCreator.Configuration.IsTunnel;

    public void UpdateLengthAndSamplesAndMesh()
    {
        float estimatedLength = Vector3.Distance(start.position, end.position);
        float samplesPerUnit = 10f;
        float caculatedLength = 0f;

        int sampleCount = Mathf.RoundToInt(estimatedLength * samplesPerUnit);
        Vector3 lastPos = start.position;

        for (int i = 1; i <= sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            Vector3 pos = CalculateTrackPointAtT(t).Position;

            caculatedLength += Vector3.Distance(lastPos, pos);
            lastPos = pos;
        }

        length = caculatedLength;

        caculatedLength = 0f;
        samplesPerUnit = 0.1f;
        sampleCount = Mathf.RoundToInt(estimatedLength * samplesPerUnit);

        Vector3 lastPos2 = start.position;
        tDistanceCleanup = AnimationCurve.Linear(0, 0, 1, 1);
        tDistanceCleanup.AddKey(0f, 0f);
        for (int i = 1; i <= sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            Vector3 pos = CalculateTrackPointAtT(t).Position;
            caculatedLength += Vector3.Distance(lastPos2, pos);

            float cleanT = t + ((caculatedLength / length) - t) * -1;

            tDistanceCleanup.AddKey(t, cleanT);

            lastPos2 = pos;
        }

        startTangent = start.position + start.forward * Length / 3f;
        endTangent = end.position + end.forward * -Length / 3f;

        MeshCreator.UpdateMesh();
    }

    public TrackPoint CalculateTrackPointAtT(float t, bool raw = true)
    {
        float time = raw ? t : tDistanceCleanup.Evaluate(t);

        float ti = 1 - time;
        float ti2 = ti * ti;
        float ti3 = ti2 * ti;

        float t2 = time * time;
        float t3 = t2 * time;

        Vector3 p0 = start.position;
        Vector3 p1 = startTangent;
        Vector3 p2 = endTangent;
        Vector3 p3 = end.position;

        Vector3 position = (ti3 * p0) + (3 * ti2 * time * p1) + (3 * ti * t2 * p2) + (t3 * p3);
        Vector3 tangent = (3 * ti2 * (p1 - p0)) + (6 * ti * time * (p2 - p1)) + (3 * t2 * (p3 - p2));

        return new TrackPoint() { Position = position, Right = Quaternion.LookRotation(tangent, Vector3.up) * Vector3.right, Tangent = tangent.normalized, IsTunnel = IsTunnel };
    }
}

public class TrackPoint
{
    public Vector3 Position;
    public Vector3 Right;
    public Vector3 Tangent;
    public Quaternion Rotation => Quaternion.LookRotation(Tangent, Vector3.Cross(Tangent, Right));
    public bool IsTunnel;
}
