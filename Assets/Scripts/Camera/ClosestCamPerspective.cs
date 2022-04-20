using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestCamPerspective : MonoBehaviour, ICameraPerspective
{
    [SerializeField] CinemachineVirtualCamera[] cameras;
    [SerializeField] PlayerController player;

    [SerializeField] float maxDistance;
    [SerializeField] GameObject fallbackPerspectiveObject;

    ICameraPerspective fallbackPerspective;

    public string PerspectiveName => $"Closest Camera Perspective with Fallback: {fallbackPerspective.PerspectiveName}";

    private void Awake()
    {
        fallbackPerspective = fallbackPerspectiveObject.GetComponent<ICameraPerspective>();

        foreach (var item in cameras)
            item.LookAt = player.transform;
    }

    public CinemachineVirtualCamera UpdateCameraTarget()
    {
        float minDist = float.MaxValue;
        CinemachineVirtualCamera minCam = null;
        foreach (var camera in cameras)
        {
            var dist = (camera.transform.position - player.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                minCam = camera;
            }
        }

        if (minDist > maxDistance)
            return fallbackPerspective.UpdateCameraTarget();

        return minCam;
    }
}