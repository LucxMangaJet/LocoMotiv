using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShovelEventBase : MonoBehaviour
{
    public static System.Action<ShovelEventType> Trigger;
    public abstract ShovelEventType Type { get; }
    public abstract void ExecuteEvent();
    private void OnEnable()
    {
        Trigger += OnTrigger;
    }

    private void OnDisable()
    {
        Trigger -= OnTrigger;
    }

    private void OnTrigger(ShovelEventType type)
    {
        Debug.Log("OnTrigger: " + type);

        if (type == Type)
            ExecuteEvent();
    }
}
