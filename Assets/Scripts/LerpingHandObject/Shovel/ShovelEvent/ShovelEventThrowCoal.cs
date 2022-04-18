using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelEventThrowCoal : ShovelEventBase
{
    [SerializeField] ShovelCoal shovel;
    public override ShovelEventType Type => ShovelEventType.ThrowCoal;

    public override void ExecuteEvent() => shovel.SetCoal(false);
}
