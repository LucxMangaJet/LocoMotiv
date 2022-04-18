using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShovelEventType
{
    GetCoal,
    ThrowCoal,
}

public class ShovelEventTrigger : MonoBehaviour
{
    public void Trigger(ShovelEventType shovelEvent)
    {
        ShovelEventBase.Trigger(shovelEvent);
    }
}
