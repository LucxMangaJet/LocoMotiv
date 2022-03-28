using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEnvironmentValueAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    internal void SetPercent(float percent)
    {
        animator.SetFloat("value", percent);
    }
}
