using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class TrainCheckpoint : MonoBehaviour
{
    [SerializeField] TrackRoute route;
    [SerializeField] float distance;

    public float DistanceAlongTrack => distance;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        if (route != null)
        {
            var sample = route.SampleTrackpoint(distance);

            if (sample != null)
            {
                transform.position = sample.Position;
                transform.rotation = sample.Rotation;
            }
        }

        UnityEditor.Handles.Label(transform.position, name);
    }

#endif

}
