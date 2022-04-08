using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeCurveBehaviour : MonoBehaviour
{
    [SerializeField] Transform pointer;
    [SerializeField] AnimationCurve percentToPointerAngleCurve;

    public void SetPercent(float _percent)
    {
        pointer.localRotation = Quaternion.Euler(percentToPointerAngleCurve.Evaluate(_percent), 0, 0);
    }
}
