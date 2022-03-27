using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeBehaviour : MonoBehaviour
{
    [SerializeField] Transform pointer;
    [SerializeField] AnimationCurve preassureToPointerAngleCurve;

    public void SetPercent(float _percent)
    {
        pointer.localRotation = Quaternion.Euler(preassureToPointerAngleCurve.Evaluate(_percent), 0, 0);
    }
}
