using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimationCurve moveSpeedToAnimatorValueCurve;
    private void OnEnable()
    {
        playerController.moveModule.MoveEvent += OnMove;
    }

    private void OnDisable()
    {
        playerController.moveModule.MoveEvent -= OnMove;
    }

    private void OnMove(float speed)
    {
        animator.SetFloat("speed", moveSpeedToAnimatorValueCurve.Evaluate(speed));
    }
}
