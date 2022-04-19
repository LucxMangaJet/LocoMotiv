using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatReducer : MonoBehaviour
{
    [SerializeField] TrainController controller;

    public void ReduceOverheat()
    {
        controller.ReduceOverheat();
    }
}
