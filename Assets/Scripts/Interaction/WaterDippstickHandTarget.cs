using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDippstickHandTarget : WaterDippstickTarget
{
    public static WaterDippstickHandTarget Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
