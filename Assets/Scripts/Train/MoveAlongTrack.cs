using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using NaughtyAttributes;

public class MoveAlongTrack : MonoBehaviour
{
    [SerializeField] private TrackRoute track;

    [SerializeField] private float speed = 1;

    private float currentDistance;

    private TrackPoint curveSample = new TrackPoint();

    public float CurrentDistance => currentDistance;
    public TrackPoint CurveSample => curveSample;

    private void Start()
    {
        var checkpoints = GameObject.FindObjectsOfType<TrainCheckpoint>();

        currentDistance = 0;

        if (checkpoints.Length > 0)
            currentDistance = checkpoints[0].DistanceAlongTrack;
    }

    private void Update()
    {
        currentDistance += Time.deltaTime * speed;

        if (currentDistance > track.Length)
            currentDistance %= track.Length;

        curveSample = MoveToDistance(transform, currentDistance);
    }

    public TrackPoint SampleAtOffset(float _distance)
    {
        _distance = (CurrentDistance + _distance) % track.Length;
        if (_distance < 0)
            _distance += track.Length;

        return track.SampleTrackpoint(_distance);
    }

    public TrackPoint MoveToDistance(Transform _target, float _distance)
    {
        _distance %= track.Length;
        if (_distance < 0)
            _distance += track.Length;

        var sample = track.SampleTrackpoint(_distance);
        _target.position = sample.Position;
        _target.rotation = sample.Rotation;

        return sample;
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
}
