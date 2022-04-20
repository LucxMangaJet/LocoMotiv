using UnityEngine;
using Cinemachine;

public class FixedCameraPerspective : MonoBehaviour, ICameraPerspective
{
    [SerializeField] CinemachineVirtualCamera camera;

    public string PerspectiveName => $"Fixed Perspective \"{name}\"";

    public virtual CinemachineVirtualCamera UpdateCameraTarget()
    {
        return camera;
    }

}
