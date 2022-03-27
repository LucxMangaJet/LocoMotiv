using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreassureGaugeBehaviour : MonoBehaviour
{
    [SerializeField] Transform pointer;
    [SerializeField] AnimationCurve preassureToPointerAngleCurve;

    public void DisplayPreasure(float preasure)
    {
        pointer.localRotation = Quaternion.Euler(preassureToPointerAngleCurve.Evaluate(preasure), 0, 0);
    }
}
