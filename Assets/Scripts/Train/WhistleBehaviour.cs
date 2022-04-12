using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WhistleBehaviour : MonoBehaviour
{
    [Header("DefaultWhistle")]
    [SerializeField] StudioEventEmitter eventEmitter;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AnimationCurve particleAmountOverWhistleDurationCurve;
    [SerializeField] AnimationCurve particleAmountOverPressureCurve;
    [SerializeField] AnimationCurve particleSpeedOverPressureCurve;
    [SerializeField] Transform armTransform;
    [SerializeField] AnimationCurve armXRotationCurve;

    [Header("Overheat")]
    [SerializeField] ParticleSystem overheatSmokeSystem;
    [SerializeField] ParticleSystem overheatDistortionSystem;
    ParticleSystem.EmissionModule overheatSmokeEmission, overheatDistortionEmission;
    ParticleSystem.MainModule overheatSmokeMain, overheatDistortionMain;
    [SerializeField] AnimationCurve overheatSpeedCurve;
    [SerializeField] AnimationCurve overheatAmountCurve;
    [SerializeField] AnimationCurve overheatDistortionAmountCurve;

    private void Awake()
    {
        overheatSmokeEmission = overheatSmokeSystem.emission;
        overheatDistortionEmission = overheatDistortionSystem.emission;
        overheatSmokeMain = overheatSmokeSystem.main;
        overheatDistortionMain = overheatDistortionSystem.main;
    }


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

    public void SetOverheat(float overheatingPercent)
    {
        overheatSmokeEmission.rateOverTime = new ParticleSystem.MinMaxCurve(overheatAmountCurve.Evaluate(overheatingPercent));
        overheatDistortionEmission.rateOverTime = new ParticleSystem.MinMaxCurve(overheatDistortionAmountCurve.Evaluate(overheatingPercent));
        float speed = overheatSpeedCurve.Evaluate(overheatingPercent);
        overheatSmokeMain.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.75f, speed * 1.25f);
    }
}
