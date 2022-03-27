using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFireVisualizer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float targetIntensity;
    float intensity;

    private void Update()
    {
        intensity = Mathf.MoveTowards(intensity, targetIntensity, Time.deltaTime);
        animator.SetFloat("intensity", intensity);
    }
}
