using NaughtyAttributes;
using UnityEngine;
using Train.Feedback;

public class TrainController : Singleton<TrainController>
{
    [Header("Settings")]

    [Foldout("Settings"), SerializeField] float segmentMass;
    [Foldout("Settings"), SerializeField] float rootMass;

    [Foldout("Settings"), SerializeField] float rollingResistanceCoefficient = 1;
    [Foldout("Settings"), SerializeField] float airDragCoefficient = 1;
    [Foldout("Settings"), SerializeField] float turnResistanceCoefficient = 1;

    [Foldout("Settings"), SerializeField] float wagonCrossSectionArea = 1;
    [Foldout("Settings"), SerializeField] float airDensity = 1;

    [Header("Balance")]

    [Foldout("Balance"), SerializeField] float fuelAddedPerShovel = 10;
    [Foldout("Balance"), SerializeField] float maxFuel = 100;
    [Foldout("Balance"), SerializeField] float maxPressure = 1000;
    [Foldout("Balance"), SerializeField] float breakingForce = 100000;

    [Foldout("Balance"), SerializeField] AnimationCurve fuelConsumptionOverAmount;
    [Foldout("Balance"), SerializeField] AnimationCurve maxPressureOverFuelAmount;
    [Foldout("Balance"), SerializeField] AnimationCurve pressureBuildupOverFuelAmount;
    [Foldout("Balance"), SerializeField] AnimationCurve pressureToForceCurve;
    [Foldout("Balance"), SerializeField] AnimationCurve overheatToForceCurve;
    [Foldout("Balance"), SerializeField] float pressureToForceCurveMultiplyer;
    [Foldout("Balance"), SerializeField] float pressureReleaseByWhistle;
    [Foldout("Balance"), SerializeField] AnimationCurve pressureReleaseOverSpeed;
    [Foldout("Balance"), SerializeField] AnimationCurve pressureReleaseOverPressure;
    [Foldout("Balance"), SerializeField] AnimationCurve pressureReleaseOverOverheat;
    [Foldout("Balance"), SerializeField] AnimationCurve rollingResistanceOverSpeedMultiplierCurve;
    [Foldout("Balance"), SerializeField] AnimationCurve overheatPerSecondOverPressure;

    [Header("Control")]
    [Foldout("Control"), Range(0, 1), SerializeField] float throttle = 1;
    [Foldout("Control"), Range(0, 1), SerializeField] float breaks = 0;
    [Foldout("Control"), SerializeField] bool bCheatAlwaysMaxPressure;

    [Header("Debug")]

    [Foldout("Debug"), SerializeField]
    Force _forceMultipliers;
    [Foldout("Debug"), SerializeField, ProgressBar("relative force", 10, EColor.Green)]
    private float _force = 0;
    private float _gravity = 0;
    [Foldout("Debug"), SerializeField, ProgressBar("positive gravity", 10, EColor.Green)]
    private float _gravityPos = 0f;
    [Foldout("Debug"), SerializeField, ProgressBar("negative gravity", 10, EColor.Red)]
    private float _gravityNeg = 0f;
    [Foldout("Debug"), SerializeField]
    Resistance _resistance;
    [Foldout("Debug"), SerializeField, ProgressBar("resistance", 10, EColor.Red)]
    private float _resistanceTotal = 0;
    [Foldout("Debug"), SerializeField, ProgressBar("acceleration", 10, EColor.Yellow)]
    private float _acceleration = 0;
    [Foldout("Debug"), SerializeField, ProgressBar("speed", 100, EColor.Blue)]
    private float speed = 0;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private float slope = 0;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private float turnResistance;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private bool isWhistling;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private float engineForce;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private float fuel = 0;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private float pressure = 0;
    [Foldout("Debug"), SerializeField, ReadOnly, Range(0, 1)]
    private float overheat = 0;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private bool exploded;

    [Foldout("Debug"), SerializeField]
    private DebugAnimationCurve pressureToForceDEBUG;
    [Foldout("Debug"), SerializeField]
    private DebugAnimationCurve forceEffectivenessOverSpeedDEBUG;
    [Foldout("Debug"), SerializeField]
    private DebugAnimationCurve RollingResistanceOverSpeedMultiplierDEBUG;
    [Foldout("Debug"), SerializeField]
    private DebugAnimationCurve OverheatPerSecondOverPressureDEBUG;


    [Button]
    public void Shovel() => AddFuel(fuelAddedPerShovel);

    [Header("References")]
    [Foldout("References"), SerializeField] private TrainEnvironmentFeedbackController environmentFeedbackController;
    [Foldout("References"), SerializeField] MoveAlongTrack mover;
    [Foldout("References"), SerializeField] TrainSegmentsMover trainRoot;
    [Foldout("References"), SerializeField] TrainGear[] availableGears;
    [Foldout("Control"), SerializeField, Dropdown("availableGears")] TrainGear activeGear;

    //debug
    private float[] acceleration;
    private float[] resistance;
    private float airResistance;

    private void Start()
    {
        acceleration = new float[trainRoot.Segments.Length];
        resistance = new float[trainRoot.Segments.Length];

        pressureToForceDEBUG = new DebugAnimationCurve() { Curve = pressureToForceCurve };
        RollingResistanceOverSpeedMultiplierDEBUG = new DebugAnimationCurve() { Curve = rollingResistanceOverSpeedMultiplierCurve };
        OverheatPerSecondOverPressureDEBUG = new DebugAnimationCurve() { Curve = overheatPerSecondOverPressure };

        ShovelEventBase.Trigger += OnShovelTrigger;
    }
    public void SwitchGear(TrainGear gear)
    {
        activeGear = gear;
    }

    private void OnDestroy()
    {
        ShovelEventBase.Trigger -= OnShovelTrigger;
    }

    private void Update()
    {
        float totalMass = (rootMass + trainRoot.Segments.Length * segmentMass);

        UpdateSlope();

        pressure = UpdatePressure();

        UpdateForce();

        _force = engineForce / totalMass;
        _gravity = CalculateGravity();
        _resistance = CalculateResistance(totalMass, speed);
        _resistanceTotal = (_resistance.GetTotal(speed));

        _gravityPos = Mathf.Max(0, _gravity);
        _gravityNeg = Mathf.Min(0, _gravity) * -1;

        _acceleration = (_force + _gravity);

        float speed2 = speed + _acceleration * Time.deltaTime;
        //breaking to stop
        if (Mathf.Abs(speed2) < Mathf.Abs(_resistanceTotal * Time.deltaTime))
        {
            _acceleration = speed / Time.deltaTime;
            speed = 0;
        }
        else
        {
            _acceleration -= _resistanceTotal;
            speed += _acceleration * Time.deltaTime;
        }

        mover.SetSpeed(speed);

        updateEnvironment();

        updateCheats();
    }

    private void UpdateSlope()
    {
        slope = GetSlopeFromTangent(mover.CurveSample.Tangent);

        for (int i = 0; i < trainRoot.Segments.Length; i++)
            slope += GetSlopeFromTangent(mover.CurveSample.Tangent);

        slope /= (trainRoot.Segments.Length + 1);
    }

    private void updateCheats()
    {
        if (bCheatAlwaysMaxPressure)
            pressure = maxPressure;
    }

    private void updateEnvironment()
    {
        environmentFeedbackController.PressurePercent = pressure / maxPressure;
        environmentFeedbackController.FuelPercent = fuel / maxFuel;
        environmentFeedbackController.Speed = speed;
        environmentFeedbackController.BeatsPerUnit = activeGear.beatsPerUnit;
        environmentFeedbackController.Slope = slope;
        environmentFeedbackController.Tunnel = mover.CurveSample.IsTunnel;
        environmentFeedbackController.Overheat = overheat;
    }

    private float UpdatePressure()
    {
        float dt = Time.deltaTime;

        float pressureBefore = this.pressure;

        fuel = Mathf.Clamp(fuel - fuelConsumptionOverAmount.Evaluate(fuel) * dt, 0, maxFuel);

        float pressureGeneration = pressureBuildupOverFuelAmount.Evaluate(fuel);

        //engine uses steam to throttle up/down
        float pressureRelease = pressureReleaseOverPressure.Evaluate(pressureBefore);
        float pressureConsumption = throttle * pressureRelease * pressureReleaseOverSpeed.Evaluate(speed) * pressureReleaseOverOverheat.Evaluate(overheat);

        if (isWhistling)
            pressureConsumption += pressureRelease * pressureReleaseByWhistle;

        //dont allow pressure buildup if engine is not hot enough
        float maxPressure = maxPressureOverFuelAmount.Evaluate(fuel);
        if (pressureBefore > maxPressure)
            pressureGeneration = 0;

        float pressure = Mathf.Max(0, pressureBefore + (pressureGeneration - pressureConsumption) * dt);
        pressureToForceDEBUG.Value = pressure;
        OverheatPerSecondOverPressureDEBUG.Value = pressure;

        overheat = Mathf.Clamp(overheat + overheatPerSecondOverPressure.Evaluate(pressure) * dt, 0f, 1f);

        if (!exploded && overheat >= 1f)
        {
            exploded = true;
            DeathManager.Die?.Invoke();
            environmentFeedbackController.Explode();
        }

        return pressure;
    }

    private void UpdateForce()
    {
        _forceMultipliers = new Force();

        //engine acceleration
        _forceMultipliers.Pressure = pressure;
        _forceMultipliers.Trottle = throttle;
        _forceMultipliers.PressureToForce = pressureToForceCurve.Evaluate(pressure);
        _forceMultipliers.SpeedToForce = activeGear.forceOverSpeedCurve.Evaluate(speed);
        _forceMultipliers.OverheatToForce = overheatToForceCurve.Evaluate(overheat);
        _forceMultipliers.Direction = activeGear.directionMultiplier;
        //_forceMultipliers.SpeedToForce = forceEffectivenessOverSpeed.Evaluate(speed);
        float force = throttle * _forceMultipliers.PressureToForce * _forceMultipliers.SpeedToForce * _forceMultipliers.Direction * pressureToForceCurveMultiplyer * _forceMultipliers.OverheatToForce;

        forceEffectivenessOverSpeedDEBUG.Curve = activeGear.forceOverSpeedCurve;
        forceEffectivenessOverSpeedDEBUG.Value = speed;

        engineForce = force;
    }

    private float CalculateGravity()
    {
        float gravity = CalculateForceOnWagon(mover.CurveSample.Tangent, rootMass) / rootMass;

        for (int i = 0; i < trainRoot.Segments.Length; i++)
        {
            var segment = trainRoot.Segments[i];
            float newForce = CalculateForceOnWagon(segment.transform.forward, segmentMass);

            var acc = newForce / segmentMass;
            acceleration[i] = acc;
            gravity += acc;
        }

        gravity *= activeGear.GravityScale;
        return gravity;
    }

    private float GetSlopeFromTangent(Vector3 tangent)
    {
        return tangent.normalized.y * 100f;
    }

    private Resistance CalculateResistance(float totalMass, float speed)
    {
        Resistance res = new Resistance();

        res.Air = CalculateAirResistance() / totalMass;

        res.Turn = CalculateTurnResistance(totalMass) / totalMass;

        res.Rolling = CalculateRollingResistanceOnWagon(rootMass, speed) / rootMass;

        for (int i = 0; i < trainRoot.Segments.Length; i++)
        {
            float newForce = CalculateRollingResistanceOnWagon(segmentMass, speed);

            var dec = newForce / segmentMass;
            resistance[i] = dec;
            res.Rolling += dec;
        }

        res.Breaks = breaks * breakingForce / totalMass;

        return res;
    }

    private float CalculateForceOnWagon(Vector3 tangent, float mass)
    {
        float gravityForce = mass * Physics.gravity.y * Vector3.Dot(Vector3.up, tangent);
        return gravityForce;
    }

    private float CalculateAirResistance()
    {
        //coefficient* density *area * v * v * 0.5f
        float airResistance = airDragCoefficient * airDensity * wagonCrossSectionArea * speed * speed * 0.5f;

        return airResistance;
    }

    private float CalculateTurnResistance(float totalMass)
    {
        /*
   R=WF(D+L)/2r
      where:
      R = Curve resistance
      W = vehicle weight
      F = Coefficient of Friction,
      D = track gauge
      L = Rigid wheelbase
      r = curve radius (r = H*0.5 + (Wi*Wi)/(8*H))
      Wi = the length of the chord defining the base of the arc
      H = the height measured at the midpoint of the arc's base. 
   */

        Vector3 front = transform.position;
        var segments = trainRoot.Segments;
        Vector3 midHigh = segments[(segments.Length - 1) / 2].transform.position;
        Vector3 back = segments[segments.Length - 1].transform.position;

        Vector3 mid = Vector3.Lerp(front, back, 0.5f);

        Vector3 fb = back - front;

        Vector3 hAxis = Vector3.Cross(fb.normalized, Vector3.up);

        float h = Vector3.Dot(hAxis, midHigh - mid);
        float wiSquared = fb.sqrMagnitude;

        float r = Mathf.Abs(h * 0.5f + wiSquared / (8 * h));

        Debug.DrawLine(front, back, Color.white);
        Debug.DrawLine(mid, midHigh, Color.white);

        Debug.DrawLine(mid, mid + hAxis, Color.red);

        float w = totalMass * -Physics.gravity.y;
        float f = turnResistanceCoefficient;

        float resistance = (w * f) / (2 * r);

        return resistance;
    }

    private float CalculateRollingResistanceOnWagon(float mass, float speed)
    {
        float rollingResistance = rollingResistanceCoefficient * mass * -Physics.gravity.y;

        RollingResistanceOverSpeedMultiplierDEBUG.Value = speed;

        return rollingResistanceOverSpeedMultiplierCurve.Evaluate(speed) * rollingResistance;
    }

    public void AddFuel(float _amount)
    {
        fuel += _amount;
    }
    public void ReduceOverheat()
    {
        overheat = Mathf.Clamp(0.1f, 0, 1);
    }

    private void OnShovelTrigger(ShovelEventType _type)
    {
        if (_type == ShovelEventType.ThrowCoal)
            AddFuel(fuelAddedPerShovel);
    }

    public void EnableWhistling()
    {
        isWhistling = true;
        environmentFeedbackController.UseWhistle();
    }

    public void DisableWhistling()
    {
        isWhistling = false;
    }

    public void SetThrottleAndBreak(float _percent)
    {
        throttle = Mathf.Clamp01((_percent - 0.5f) * 2);
        breaks = Mathf.Clamp01((_percent - 0.5f) * -2);
    }

    #region EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < trainRoot.Segments.Length; i++)
            {
                Transform trans = trainRoot.Segments[i].transform;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(trans.position, trans.position + Vector3.up * acceleration[i] * 5);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(trans.position + trans.forward * 0.1f, trans.position + Vector3.up * resistance[i] * 5);

            }

            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * airResistance);
            Gizmos.DrawLine(transform.position + transform.right * 0.3f, transform.position + Vector3.up * turnResistance);
        }
    }

    #endregion
}

[System.Serializable]
public class Force
{
    [ProgressBar("Pressure", 1000, EColor.Green)]
    public float Pressure;
    [ProgressBar("Trottle", 1, EColor.Green)]
    public float Trottle;
    [ProgressBar("Pressure To Force", 1, EColor.Green)]
    public float PressureToForce;
    [ProgressBar("Speed To Force", 1, EColor.Green)]
    public float SpeedToForce;
    [ProgressBar("Overheat To Force", 2, EColor.Green)]
    public float OverheatToForce;
    [SerializeField, ReadOnly] public int Direction;
}

[System.Serializable]
public class Resistance
{
    [ProgressBar("Air", 10, EColor.Red)]
    public float Air;
    [ProgressBar("Turn", 10, EColor.Red)]
    public float Turn;
    [ProgressBar("Rolling", 10, EColor.Red)]
    public float Rolling;
    [ProgressBar("Breaks", 10, EColor.Red)]
    public float Breaks;

    internal float GetTotal(float speed)
    {
        float total = Air + Turn + Rolling + Breaks;

        return total * Mathf.Sign(speed);
    }
}
