using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WhistleBehaviour : MonoBehaviour
{
    [SerializeField] StudioEventEmitter eventEmitter;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AnimationCurve particleAmountOverWhistleDurationCurve;
    [SerializeField] AnimationCurve particleAmountOverPressureCurve;
    [SerializeField] AnimationCurve particleSpeedOverPressureCurve;

    [SerializeField] Transform armTransform;
    [SerializeField] AnimationCurve armXRotationCurve;

    Coroutine useWhistleCoroutine;

    internal void Use(float enginePressurePercent)
    {
        if (useWhistleCoroutine != null)
            StopCoroutine(useWhistleCoroutine);

        useWhistleCoroutine = StartCoroutine(UseWhistleRoutine(enginePressurePercent));

        eventEmitter.SetParameter("pressure", enginePressurePercent);
        eventEmitter.Play();
    }

    IEnumerator UseWhistleRoutine(float enginePressurePercent)
    {
        float t = 0;
        float duration = particleAmountOverWhistleDurationCurve.keys[particleAmountOverWhistleDurationCurve.keys.Length - 1].time;

        var emit = particleSystem.emission;
        var main = particleSystem.main;

        while (t <= duration)
        {
            float whistleGap = particleAmountOverWhistleDurationCurve.Evaluate(t);
            int amount = Mathf.RoundToInt(whistleGap * particleAmountOverPressureCurve.Evaluate(enginePressurePercent));
            emit.rateOverTime = new ParticleSystem.MinMaxCurve(amount);

            float speed = whistleGap * particleSpeedOverPressureCurve.Evaluate(enginePressurePercent);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed / 2f, speed);

            armTransform.localRotation = Quaternion.Euler(armXRotationCurve.Evaluate(t/duration), 0,0);

            yield return null;
            t += Time.deltaTime;
        }

        emit.rateOverTime = new ParticleSystem.MinMaxCurve(0);
        useWhistleCoroutine = null;
    }
}
