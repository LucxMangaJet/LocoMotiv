using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Train.Feedback.Modules;
using System;
using NaughtyAttributes;
using FMOD.Studio;

namespace Train.Feedback
{
    public class TrainEnvironmentFeedbackController : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float enginePressurePercent;
        [SerializeField, Range(0, 100)] float trainSpeed;
        [SerializeField, Range(0, 1)] float fuelPercentage;
        [SerializeField] AnimationCurve speedToStrokeDurationCurve;
        [SerializeField, ReadOnly] float strokeDuration;
        [SerializeField] PressureRelease pressureRelease;
        [SerializeField] ChimneySmoke chimneySmoke;
        [SerializeField] WhistleBehaviour whistle;

        [SerializeField] AnimatorValueFeedbackModule engineFire;
        [SerializeField] AnimatorValueFeedbackModule engineFuel;

        [SerializeField] GaugeBehaviour pressureGauge;
        [SerializeField] GaugeBehaviour fuelGauge;
        [SerializeField] GaugeBehaviour speedGauge;
        [SerializeField] float maxGaugeSpeed = 150;

        public float PressurePercent { set => enginePressurePercent = value; }
        public float FuelPercent { set => fuelPercentage = value; }
        public float Speed { set => trainSpeed = value; }

        private void Update()
        {
            strokeDuration = speedToStrokeDurationCurve.Evaluate(trainSpeed);
            pressureRelease.Update(enginePressurePercent, strokeDuration, Time.deltaTime);
            chimneySmoke.Update(enginePressurePercent, strokeDuration, Time.deltaTime);
            engineFire.SetPercent(enginePressurePercent);
            engineFuel.SetPercent(fuelPercentage);
            pressureGauge.SetPercent(enginePressurePercent);
            fuelGauge.SetPercent(fuelPercentage);
            speedGauge.SetPercent(trainSpeed / maxGaugeSpeed);
        }

        [Button]
        public void UseWhistle()
        {
            whistle.Use(enginePressurePercent);
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

        internal void SetPercent(float value)
        {
            animator.SetFloat("value", value);
        }
    }

    [System.Serializable]
    public class PressureRelease : ParticleSystemFeedbackModule
    {
        [SerializeField] FMODUnity.EventReference audioEvent; 
        EventInstance audioInstance;

        [SerializeField] AnimationCurve enginePressureToParticleCountCurve;
        [SerializeField] AnimationCurve enginePressureToParticleSpeedCurve;

        float time;

        internal void Update(float enginePressurePercentage, float strokeDuration, float deltaTime)
        {
            time += deltaTime;

            if (time > strokeDuration)
            {
                time = 0;

                audioInstance = FMODUnity.RuntimeManager.CreateInstance(audioEvent);
                EventDescription eventDescription;
                audioInstance.getDescription(out eventDescription);
                PARAMETER_DESCRIPTION parameterDescription;
                eventDescription.getParameterDescriptionByName("pressure", out parameterDescription);
                PARAMETER_ID parameterId = parameterDescription.id;
                audioInstance.setParameterByID(parameterId, enginePressurePercentage);
                audioInstance.start();

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
    public class ChimneySmoke: ParticleSystemFeedbackModule
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
