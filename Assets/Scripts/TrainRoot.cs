using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class TrainRoot : MonoBehaviour
{
    [SerializeField] MoveAlongTrack mover;
    [SerializeField] Transform[] segments;

    private float[] distances;
    private CurveSample[] samples;

    public CurveSample[] SegmentsSamples => samples;

    public Transform[] Segments => segments;

    private void Awake()
    {
        distances = new float[segments.Length];
        samples = new CurveSample[segments.Length];

        for (int i = 0; i < segments.Length; i++)
        {
            var target = i == 0 ? transform : segments[i - 1];

            distances[i] = (target.position - segments[i].position).magnitude;

        }
    }

    private void Update()
    {
        float distSum = 0;
        for (int i = 0; i < segments.Length; i++)
        {
            distSum += distances[i];

            float distance = (mover.CurrentDistance - distSum);
            Transform segment = segments[i];
            samples[i] = mover.MoveToDistance(segment, distance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (segments != null)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                var target = i == 0 ? transform : segments[i - 1];
                Gizmos.DrawLine(target.position, segments[i].position);
            }
        }
    }
}