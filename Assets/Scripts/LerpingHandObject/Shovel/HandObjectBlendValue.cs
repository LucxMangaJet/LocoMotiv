using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandObjectBlendValue
{
    public Vector3 Position;
    public Quaternion Rotatation;

    public HandObjectBlendValue(HandObjectTarget baseTarget, HandObjectTarget overrideTarget)
    {
        Position = Vector3.Lerp(baseTarget.transform.position, overrideTarget.transform.position, overrideTarget.Opacity);
        Rotatation = Quaternion.Lerp(baseTarget.transform.rotation, overrideTarget.transform.rotation, overrideTarget.Opacity);
    }
}
