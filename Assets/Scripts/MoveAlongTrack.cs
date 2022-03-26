using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using NaughtyAttributes;

public class MoveAlongTrack : MonoBehaviour
{
    [SerializeField] private Spline spline;

    [SerializeField] private float speed = 1;

    private float currentDistance;

    private void Start()
    {
        currentDistance = spline.GetProjectionSample(transform.position).distanceInCurve;
    }

    private void Update()
    {
        currentDistance += Time.deltaTime * speed;

        if (currentDistance > spline.Length)
            currentDistance %= spline.Length;

        MoveToDistance(currentDistance);
    }

    private void MoveToDistance(float _distance)
    {
        var sample = spline.GetSampleAtDistance(_distance);
        transform.position = sample.location;
        transform.rotation = sample.Rotation;
    }

    [Button]
    private void SnapToTracks()
    {
        MoveToDistance(spline.GetProjectionSample(transform.position).distanceInCurve);
    }
}
