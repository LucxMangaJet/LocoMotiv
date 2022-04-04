using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using System;

public class TrainSegmentsMover : MonoBehaviour
{
    [SerializeField] MoveAlongTrack mover;
    [SerializeField] private float distanceBetweenWagons;

    private TrainSegment[] segments;
    public TrainSegment[] Segments => segments;

    private void Awake()
    {
        segments = GetComponentsInChildren<TrainSegment>();
    }

    private void Update()
    {
        float distSum = 0;
        for (int i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];

            var frontTipSample = mover.SampleAtOffset(-distSum);

            distSum += segment.Length;

            var backTipSample = mover.SampleAtOffset(-distSum);

            segment.SetPosition(frontTipSample.Position, backTipSample.Position);

            distSum += distanceBetweenWagons;
        }
    }
}