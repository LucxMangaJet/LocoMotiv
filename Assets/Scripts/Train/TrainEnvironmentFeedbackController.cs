using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Train.Feedback.Modules;
using System;
using NaughtyAttributes;
using FMODUnity;

namespace Train.Feedback
{
    public class TrainEnvironmentFeedbackController : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float enginePressurePercent;
        [SerializeField, Range(0, 1)] float beatsPerUnit;
        [SerializeField, Range(0, 100)] float trainSpeed;
        [SerializeField, Range(0, 1)] float fuelPercentage;
        [SerializeField, Range(-10, 10)] float slope;
        [SerializeField, Range(0, 1)] float overheatingPercent;
        [SerializeField] AnimationCurve pressureToOverheatingFeedback;
        [SerializeField] AnimationCurve speedToStrokeDurationCurve;
        [SerializeField, ReadOnly] float strokeDuration;
        [SerializeField] bool lidOpen;
        [SerializeField, Range(0, 1)] float tunnel;

        [SerializeField] PressureRelease pressureRelease;
        [SerializeField] ChimneySmoke chimneySmoke;
        [SerializeField] WhistleBehaviour whistle;
        [SerializeField] StudioEventEmitter railsEmitter;

        [SerializeField] EngineFire engineFire;
        [SerializeField] AnimatorValueFeedbackModule engineFuel;

        [SerializeField] GaugeCurveBehaviour pressureGauge;
        [SerializeField] GaugeCurveBehaviour fuelGauge;
        [SerializeField] GaugeCurveBehaviour speedGauge;
        [SerializeField] GaugeCurveBehaviour slopeGauge;
        [SerializeField] float maxGaugeSpeed = 150;

        [SerializeField] GameObject explosionEffects;

        [SerializeField] MeshRenderer[] overheatingRenderers;
        private Material[] overheatingMaterials;

        [SerializeField, ReadOnly] float beatsPerSecond;

        public float PressurePercent { set => enginePressurePercent = value; }
        public float FuelPercent { set => fuelPercentage = value; }
        public float Speed { set => trainSpeed = Mathf.Abs(value); }
        public float Slope { set => slope = Mathf.Clamp(value, -9.9f, 9.9f); }
        public float BeatsPerUnit { set => beatsPerUnit = value; }
        public bool Tunnel { set => tunnel = value ? 1 : 0; }
        public float Overheat { set => overheatingPercent = value; }

        private void Start()
        {
            overheatingMaterials = new Material[overheatingRenderers.Length];
            for (int i = 0; i < overheatingRenderers.Length; i++)
            {
                overheatingMaterials[i] = new Material(overheatingRenderers[i].material);
                overheatingRenderers[i].material = overheatingMaterials[i];
            }
        }
        private void Update()
        {
            beatsPerSecond = beatsPerUnit * trainSpeed;
            strokeDuration = speedToStrokeDurationCurve.Evaluate(trainSpeed);
            pressureRelease.Update(enginePressurePercent, Mathf.Max(1f / beatsPerSecond, 0.1f), Time.deltaTime);
            chimneySmoke.Update(enginePressurePercent, strokeDuration, Time.deltaTime);
            engineFire.Update(enginePressurePercent, lidOpen);
            engineFuel.SetPercent(fuelPercentage);
            pressureGauge.SetPercent(enginePressurePercent);
            fuelGauge.SetPercent(fuelPercentage);
            float speedPercent = trainSpeed / maxGaugeSpeed;
            speedGauge.SetPercent(speedPercent);
            slopeGauge.SetPercent(slope);

            foreach (Material material in overheatingMaterials) material.SetFloat("_heat", overheatingPercent);

            float currentlyOverheating = overheatingPercent * pressureToOverheatingFeedback.Evaluate(enginePressurePercent);

            whistle.SetOverheat(currentlyOverheating);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("overheat", currentlyOverheating);

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("trainSpeed", speedPercent);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("beatsPerSecond", Mathf.Clamp(beatsPerSecond, 0.25f, 10f));
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("pressure", enginePressurePercent);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("slope", slope);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("tunnel", tunnel);
        }

        [Button]
        public void UseWhistle()
        {
            whistle.Use(enginePressurePercent);
        }

        [Button]
        public void Explode()
        {
            explosionEffects.SetActive(true);
        }
    }
}

namespace Train.Feedback.Modules
{
    public abstract class ParticleSystemFeedbackModule
    {
        [SerializeField] protected ParticleSystem[] particleSystems;
    }

    [System.Serializable]
    public class AnimatorValueFeedbackModule
    {
        [SerializeField] protected Animator animator;

        public void SetPercent(float value)
        {
            animator.SetFloat("value", value);
        }
    }

    [System.Serializable]
    public class EngineFire : AnimatorValueFeedbackModule
    {
        [SerializeField] StudioEventEmitter eventEmitter;

        internal void Update(float enginePressurePercent, bool lidOpen)
        {
            SetPercent(enginePressurePercent);
            eventEmitter.SetParameter("lidOpen", lidOpen ? 1 : 0);
        }
    }

    [System.Serializable]
    public class PressureRelease : ParticleSystemFeedbackModule
    {
        [SerializeField] StudioEventEmitter eventEmitter;

        [SerializeField] AnimationCurve enginePressureToParticleCountCurve;
        [SerializeField] AnimationCurve enginePressureToParticleSpeedCurve;

        float time;

        internal void Update(float enginePressurePercentage, float strokeDuration, float deltaTime)
        {
            time += deltaTime;

            if (time > strokeDuration)
            {
                time = 0;
                eventEmitter.Play();

                foreach (ParticleSystem system in particleSystems)
                {
                    float speed = enginePressureToParticleSpeedCurve.Evaluate(enginePressurePercentage);

                    var main = system.main;
                    main.startSpeed = new ParticleSystem.MinMaxCurve(speed / 2f, speed);

                    var velo = system.velocityOverLifetime;
                    velo.z = new ParticleSystem.MinMaxCurve(speed / -4f, speed / -2f);

                    system.Emit(Mathf.RoundToInt(enginePressureToParticleCountCurve.Evaluate(enginePressurePercentage)));
                }
            }

        }
    }

    [System.Serializable]
    public class ChimneySmoke : ParticleSystemFeedbackModule
    {
        float time;
        [SerializeField] AnimationCurve enginePressureToParticleSpeedCurve;
        [SerializeField] float particleAmountFromStrokeDurationMultiplier = 16f;

        internal void Update(float enginePressurePercentage, float strokeDuration, float deltaTime)
        {
            time += deltaTime;

            if (time > strokeDuration)
            {
                time = 0;
                foreach (ParticleSystem system in particleSystems)
                {
                    float speed = enginePressureToParticleSpeedCurve.Evaluate(enginePressurePercentage);

                    var main = system.main;
                    main.startSpeed = new ParticleSystem.MinMaxCurve(speed / 2f, speed);

                    system.Emit(Mathf.RoundToInt(strokeDuration * particleAmountFromStrokeDurationMultiplier));
                }
            }

        }
    }
}
