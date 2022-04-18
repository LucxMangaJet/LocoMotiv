using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelEventGetCoal : ShovelEventBase
{
    [SerializeField] ShovelCoal shovel;
    public override ShovelEventType Type => ShovelEventType.GetCoal;

    public override void ExecuteEvent() => shovel.SetCoal(true);
}
