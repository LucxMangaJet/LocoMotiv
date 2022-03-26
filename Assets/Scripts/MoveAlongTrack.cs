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

    private CurveSample curveSample;

    public float CurrentDistance => currentDistance;
    public CurveSample CurveSample => curveSample;

    private void Start()
    {
        currentDistance = spline.GetProjectionSample(transform.position).distanceInCurve;
    }

    private void Update()
    {
        currentDistance += Time.deltaTime * speed;

        if (currentDistance > spline.Length)
            currentDistance %= spline.Length;

        curveSample = MoveToDistance(transform, currentDistance);
    }

    public CurveSample MoveToDistance(Transform _target, float _distance)
    {
        _distance %= spline.Length;
        if (_distance < 0)
            _distance += spline.Length;

        var sample = spline.GetSampleAtDistance(_distance);
        _target.position = sample.location;
        _target.rotation = sample.Rotation;

        return sample;
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    [Button]
    private void SnapToTracks()
    {
        MoveToDistance(transform, spline.GetProjectionSample(transform.position).distanceInCurve);
    }
}
