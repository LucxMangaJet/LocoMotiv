using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;

public class CameraController : MonoBehaviour
{
    ICameraPerspective currentPerspective;
    new CinemachineVirtualCamera camera;
    private int currentPerspectiveIndex;

    List<ICameraPerspective> perspectives;

    private float lastPerspectiveChangeStamp;

    private void Awake()
    {
        perspectives = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ICameraPerspective>().ToList();

        currentPerspectiveIndex = perspectives.FindIndex(x => x is MouseLook);

        if (currentPerspectiveIndex < 0)
            currentPerspectiveIndex = 0;

        currentPerspective = perspectives[currentPerspectiveIndex];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentPerspectiveIndex = (currentPerspectiveIndex + 1) % perspectives.Count;
            currentPerspective = perspectives[currentPerspectiveIndex];
            lastPerspectiveChangeStamp = Time.time;
        }

        var cam = currentPerspective.UpdateCameraTarget();

        if (cam != camera)
        {
            if (camera != null)
                camera.Priority = 0;
            cam.Priority = 100;
            camera = cam;
        }
    }

    private void OnGUI()
    {
        if (Time.time - lastPerspectiveChangeStamp < 3)
        {
            var oldSize = GUI.skin.label.fontSize;
            GUI.color = Color.black;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(100, Screen.height - 50, 2000, 50), currentPerspective.PerspectiveName);
            GUI.skin.label.fontSize = oldSize;
        }
    }
}

public interface ICameraPerspective
{
    string PerspectiveName { get; }
    CinemachineVirtualCamera UpdateCameraTarget();

}
