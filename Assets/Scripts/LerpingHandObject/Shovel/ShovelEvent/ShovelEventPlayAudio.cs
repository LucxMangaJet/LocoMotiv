using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ShovelEventPlayAudio : ShovelEventBase
{
    [SerializeField] ShovelEventType type;
    public override ShovelEventType Type => type;

    [SerializeField] StudioEventEmitter eventEmitter;

    public override void ExecuteEvent()
    {
        eventEmitter.Play();
    }
}
