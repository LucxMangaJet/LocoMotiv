using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelBlendValue
{
    public Vector3 Position;
    public Quaternion Rotatation;

    public ShovelBlendValue(ShovelTarget baseTarget, ShovelTarget overrideTarget)
    {
        Position = Vector3.Lerp(baseTarget.transform.position, overrideTarget.transform.position, overrideTarget.Opacity);
        Rotatation = Quaternion.Lerp(baseTarget.transform.rotation, overrideTarget.transform.rotation, overrideTarget.Opacity);
    }
}
