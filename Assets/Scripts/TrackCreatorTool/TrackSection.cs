using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEditor;

public class TrackSection : MonoBehaviour
{
    [SerializeField] Transform start, end, middle;
    [SerializeField, ReadOnly] float length;
    [SerializeField, ReadOnly] AnimationCurve tDistanceCleanup;
    [SerializeField] public TrackMeshCreator MeshCreator;
    [SerializeField] public TrackRoute Route;
    public float Length => length;
    public Transform StartTransform => start;
    public Transform EndTransform => end;
    public Transform MidTransform => middle;

    public bool IsTunnel => MeshCreator.Configuration.IsTunnel;


    private void OnDrawGizmos()
    {
        float segmentCount = 100f;
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / segmentCount;

            TrackPoint point = CalculateTrackPointAtT(t, raw: false);

            Gizmos.DrawLine(point.Position, point.Position + point.Right);
        }

        float sampleT = ((float)EditorApplication.timeSinceStartup * 10f % length) / length;
        TrackPoint samplePos = CalculateTrackPointAtT(sampleT, raw: false);
        Gizmos.DrawWireSphere(samplePos.Position, 1f);
    }

    public void UpdateLengthAndSamples()
    {
        float estimatedLength = Vector3.Distance(start.position, middle.position) + Vector3.Distance(end.position, middle.position);
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
    }

    public void ChangeStartPosition(Vector3 newTargetPosition) => StartTransform.position = newTargetPosition;

    public void ChangeMidPosition(Vector3 newMidPosition) => MidTransform.position = newMidPosition;

    public void ChangeEndPosition(Vector3 newEndPosition) => EndTransform.position = newEndPosition;


    public TrackPoint CalculateTrackPointAtT(float t, bool raw = true)
    {
        float time = raw ? t : tDistanceCleanup.Evaluate(t);

        Vector3 abPos = Vector3.Lerp(start.position, middle.position, time);
        Vector3 bcPos = Vector3.Lerp(middle.position, end.position, time);
        Vector3 acPos = Vector3.Lerp(abPos, bcPos, time);

        Vector3 abRight = Vector3.Lerp(start.right, middle.right, time);
        Vector3 bcRight = Vector3.Lerp(middle.right, end.right, time);
        Vector3 acRight = Vector3.Lerp(abRight, bcRight, time);

        Vector3 abForward = Vector3.Lerp(start.forward, middle.forward, time);
        Vector3 bcForward = Vector3.Lerp(middle.forward, end.forward, time);
        Vector3 acForward = Vector3.Lerp(abForward, bcForward, time);

        return new TrackPoint() { Position = acPos, Right = acRight, Forward = acForward };
    }
}

public class TrackPoint
{
    public Vector3 Position;
    public Vector3 Right;
    public Vector3 Forward;

    public Quaternion Rotation => Quaternion.LookRotation(Forward, Vector3.Cross(Forward, Right));
}
