using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TrainGear : ScriptableObject
{
    [SerializeField] AnimationCurve forceOverSpeedCurve;
    [SerializeField, Range(0f, 1f)] float beatsPerUnit;
    private int[] dirs = new int[] { -1, 1 };
    [SerializeField, Dropdown("dirs")] int directionMultiplier = 1;
}
