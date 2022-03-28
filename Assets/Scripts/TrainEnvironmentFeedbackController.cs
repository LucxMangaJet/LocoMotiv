using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Train.Feedback.Modules;
using System;
using NaughtyAttributes;

namespace Train.Feedback
{
    public class TrainEnvironmentFeedbackController : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float enginePressurePercentage;
        [SerializeField, Range(0, 100)] float trainSpeed;
        [SerializeField] AnimationCurve speedToStrokeDurationCurve;
        [SerializeField, ReadOnly] float strokeDuration;
        [SerializeField] PressureReleaseBySpeedFeedback pressureRelease;


        private void Update()
        {
            strokeDuration = speedToStrokeDurationCurve.Evaluate(trainSpeed);
            pressureRelease.Update(enginePressurePercentage, strokeDuration, Time.deltaTime);
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
    public class PressureReleaseBySpeedFeedback : ParticleSystemFeedbackModule
    {
        [SerializeField] AnimationCurve enginePressureToParticleCountCurve;
        [SerializeField] AnimationCurve enginePressureToParticleSpeedCurve;

        float time;

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

                    var velo = system.velocityOverLifetime;
                    velo.z = new ParticleSystem.MinMaxCurve(speed / -4f, speed / -2f);

                    system.Emit(Mathf.RoundToInt(enginePressureToParticleCountCurve.Evaluate(enginePressurePercentage)));
                }
            }

        }
    }
}
