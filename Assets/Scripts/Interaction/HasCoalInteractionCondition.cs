using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasCoalInteractionCondition : InteractionConditionBase
{
    [SerializeField] bool inverted;
    public override bool IsMet()
    {
        return ShovelCoal.HasCoal != inverted;
    }
}
