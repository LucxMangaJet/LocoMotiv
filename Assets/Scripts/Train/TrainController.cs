using NaughtyAttributes;
using UnityEngine;
using Train.Feedback;

public class TrainController : Singleton<TrainController>
{
    [SerializeField] MoveAlongTrack mover;
    [SerializeField] TrainSegmentsMover trainRoot;

    [Header("Control")]
    [Range(0, 1)]
    [SerializeField] float throttle = 1;

    [Range(0, 1)]
    [SerializeField] float breaks = 0;

    [SerializeField] bool bCheatAlwaysMaxPressure;

    [Header("Balance")]

    [SerializeField] float fuelAddedPerShovel = 10;
    [SerializeField] float maxFuel = 100;
    [SerializeField] float maxPressure = 1000;
    [SerializeField] float breakingForce = 100000;

    [SerializeField] AnimationCurve fuelConsumptionOverAmount;

    [SerializeField] AnimationCurve maxPressureOverFuelAmount;

    [SerializeField] AnimationCurve pressureBuildupOverFuelAmount;

    [SerializeField] AnimationCurve pressureToForceCurve;

    [SerializeField] float pressureToForceCurveMultiplyer;

    [SerializeField] AnimationCurve forceEffectivenessOverSpeed;

    [SerializeField] float pressureReleaseByWhistle;

    [SerializeField] AnimationCurve pressureReleaseOverSpeed;

    [SerializeField] AnimationCurve pressureReleaseOverPressure;


    [Header("Settings")]

    [SerializeField] float segmentMass;
    [SerializeField] float rootMass;

    [SerializeField] float rollingResistanceCoefficient = 1;
    [SerializeField] float airDragCoefficient = 1;
    [SerializeField] float turnResistanceCoefficient = 1;

    [SerializeField] float wagonCrossSectionArea = 1;
    [SerializeField] float airDensity = 1;

    [SerializeField, ReadOnly]
    private float speed = 0;
    [SerializeField, ReadOnly]
    private float fuel = 0;
    [SerializeField, ReadOnly]
    private float pressure = 0;
    [SerializeField, ReadOnly]
    private bool isWhistling;

    [SerializeField, ReadOnly]
    private float engineForce;

    [Header("References")]
    [SerializeField] private TrainEnvironmentFeedbackController environmentFeedbackController;

    //debug
    private float[] acceleration;
    private float[] resistance;
    private float airResistance;
    private float turnResistance;

    private void Start()
    {
        acceleration = new float[trainRoot.Segments.Length];
        resistance = new float[trainRoot.Segments.Length];

        ShovelEventBase.Trigger += OnShovelTrigger;
    }

    private void OnDestroy()
    {
        ShovelEventBase.Trigger -= OnShovelTrigger;
    }

    private void Update()
    {
        float totalMass = (rootMass + trainRoot.Segments.Length * segmentMass);

        updateEngine();

        updateAcceleration(totalMass);
        updateDecelleration(totalMass);

        mover.SetSpeed(speed);

        updateEnvironment();

        updateCheats();
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
    }

    private void updateEngine()
    {
        float dt = Time.deltaTime;

        fuel = Mathf.Clamp(fuel - fuelConsumptionOverAmount.Evaluate(fuel) * dt, 0, maxFuel);

        float pressureGeneration = pressureBuildupOverFuelAmount.Evaluate(fuel);

        //engine uses steam to throttle up/down
        float pressureRelease = pressureReleaseOverPressure.Evaluate(pressure);
        float pressureConsumption = throttle * pressureRelease * pressureReleaseOverSpeed.Evaluate(speed);

        if (isWhistling)
            pressureConsumption += pressureRelease * pressureReleaseByWhistle;

        //dont allow pressure buildup if engine is not hot enough
        float maxPressure = maxPressureOverFuelAmount.Evaluate(fuel);
        if (pressure > maxPressure)
            pressureGeneration = 0;

        pressure = Mathf.Max(0, pressure + (pressureGeneration - pressureConsumption) * dt);

        //engine acceleration
        float force = throttle * pressureToForceCurve.Evaluate(pressure) * pressureToForceCurveMultiplyer;

        force *= forceEffectivenessOverSpeed.Evaluate(speed);

        engineForce = force;
    }

    private void updateAcceleration(float totalMass)
    {
        float tempAcceleration = 0;

        tempAcceleration += engineForce / totalMass;

        //gravity
        tempAcceleration += CalculateForceOnWagon(mover.CurveSample.Forward, rootMass) / rootMass;

        for (int i = 0; i < trainRoot.Segments.Length; i++)
        {
            var segment = trainRoot.Segments[i];
            float newForce = CalculateForceOnWagon(segment.transform.forward, segmentMass);

            var acc = newForce / segmentMass;
            acceleration[i] = acc;
            tempAcceleration += acc;
        }

        speed += tempAcceleration * Time.deltaTime;
    }

    private void updateDecelleration(float totalMass)
    {
        // resitances
        float tempDecelleration = 0;

        airResistance = CalculateAirResistance() / totalMass;
        tempDecelleration += airResistance;

        turnResistance = CalculateTurnResistance(totalMass) / totalMass;
        tempDecelleration += turnResistance;

        tempDecelleration += CalculateResistanceOnWagon(rootMass) / rootMass;

        for (int i = 0; i < trainRoot.Segments.Length; i++)
        {
            float newForce = CalculateResistanceOnWagon(segmentMass);

            var dec = newForce / segmentMass;
            resistance[i] = dec;
            tempDecelleration += dec;
        }

        //breaks
        tempDecelleration += breaks * breakingForce / totalMass;

        float decelAmount = tempDecelleration * Time.deltaTime;

        if (Mathf.Abs(speed) < decelAmount)
            speed = 0;
        else
            speed -= decelAmount * Mathf.Sign(speed);
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

    private float CalculateResistanceOnWagon(float mass)
    {
        float rollingResistance = rollingResistanceCoefficient * mass * -Physics.gravity.y;

        return rollingResistance;
    }


    public void AddFuel(float _amount)
    {
        fuel += _amount;
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
