using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TrainGear : ScriptableObject
{
    [SerializeField] public AnimationCurve forceOverSpeedCurve;
    [SerializeField, Range(0f, 1f)] public float beatsPerUnit;
    private int[] dirs = new int[] { -1, 1 };
    [SerializeField, Dropdown("dirs")] public int directionMultiplier = 1;
    [SerializeField, Range(0f,1f)] public float GravityScale = 1f;
}
