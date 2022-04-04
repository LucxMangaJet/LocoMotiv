using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteTraveler : MonoBehaviour
{
    [SerializeField] TrackRoute currentRoute;
    [SerializeField] float currentDistanceOnRoute = 1f;
    [SerializeField, Range(0f,10f)] float speed = 1f;


    // Update is called once per frame
    void Update()
    {
        if (currentRoute == null) return;

        currentDistanceOnRoute += speed * Time.deltaTime;
        TrackPoint sample1 = currentRoute.SampleTrackpoint(currentDistanceOnRoute - 1f);
        TrackPoint sample2 = currentRoute.SampleTrackpoint(currentDistanceOnRoute + 1f);

        if (sample1 == null || sample2 == null) return;

        transform.position = Vector3.Lerp(sample1.Position, sample2.Position, 0.5f);
        transform.forward = (sample2.Position - sample1.Position).normalized;
    }
}
