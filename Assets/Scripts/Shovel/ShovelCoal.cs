using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelCoal : MonoBehaviour
{
    [SerializeField] ParticleSystem coalParticleEffect;
    [SerializeField] bool hasCoal;

    ParticleSystem.CollisionModule collisionModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule;

    private void Awake()
    {
        coalParticleEffect = Instantiate(coalParticleEffect, transform);

        collisionModule = coalParticleEffect.collision;
        emissionModule = coalParticleEffect.emission;
        forceOverLifetimeModule = coalParticleEffect.forceOverLifetime;
    }

    internal void SetCoal(bool coal)
    {
        hasCoal = coal;
        if (coal) coalParticleEffect.Emit(10);
        collisionModule.dampenMultiplier = coal ? 1 : 0;
        emissionModule.rateOverTime = coal ? 2 : 0;
        forceOverLifetimeModule.enabled = !coal;


    }
}
