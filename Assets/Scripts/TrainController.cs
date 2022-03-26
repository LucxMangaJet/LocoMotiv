using UnityEngine;


public class TrainController : MonoBehaviour
{
    [SerializeField] MoveAlongTrack mover;
    [SerializeField] TrainRoot trainRoot;

    [Header("Control")]
    [Range(0, 1)]
    [SerializeField] float throttle = 1;

    [Header("Settings")]

    [SerializeField] float segmentMass;
    [SerializeField] float rootMass;

    [SerializeField] float rollingResistanceCoefficient = 1;
    [SerializeField] float airDragCoefficient = 1;
    [SerializeField] float turnResistanceCoefficient = 1;

    [SerializeField] float wagonCrossSectionArea = 1;
    [SerializeField] float airDensity = 1;

    [SerializeField] float engineMaxForce = 1;

    [SerializeField]
    private float speed = 0;

    private float[] acceleration;
    private float[] resistance;

    private float airResistance;
    private float turnResistance;

    private void Start()
    {
        acceleration = new float[trainRoot.Segments.Length];
        resistance = new float[trainRoot.Segments.Length];
    }

    private void Update()
    {
        float totalMass = (rootMass + trainRoot.Segments.Length * segmentMass);

        float tempAcceleration = 0;

        tempAcceleration += throttle * engineMaxForce / totalMass;

        //gravity
        tempAcceleration += CalculateForceOnWagon(mover.CurveSample.tangent, rootMass) / rootMass;

        for (int i = 0; i < trainRoot.SegmentsSamples.Length; i++)
        {
            SplineMesh.CurveSample item = trainRoot.SegmentsSamples[i];
            float newForce = CalculateForceOnWagon(item.tangent, segmentMass);

            var acc = newForce / segmentMass;
            acceleration[i] = acc;
            tempAcceleration += acc;
        }

        speed += tempAcceleration * Time.deltaTime;

        // resitances
        float tempDecelleration = 0;

        airResistance = CalculateAirResistance() / totalMass;
        tempDecelleration += airResistance;

        turnResistance = CalculateTurnResistance(totalMass) / totalMass;
        tempDecelleration += turnResistance;

        tempDecelleration += CalculateResistanceOnWagon(rootMass) / rootMass;

        for (int i = 0; i < trainRoot.SegmentsSamples.Length; i++)
        {
            SplineMesh.CurveSample item = trainRoot.SegmentsSamples[i];
            float newForce = CalculateResistanceOnWagon(segmentMass);

            var dec = newForce / segmentMass;
            resistance[i] = dec;
            tempDecelleration += dec;
        }

        //breaks
        //TODO

        float decelAmount = tempDecelleration * Time.deltaTime;

        if (Mathf.Abs(speed) < decelAmount)
            speed = 0;
        else
            speed -= decelAmount * Mathf.Sign(speed);

        mover.SetSpeed(speed);
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
        Vector3 midHigh = segments[(segments.Length - 1) / 2].position;
        Vector3 back = segments[segments.Length - 1].position;

        Vector3 mid = Vector3.Lerp(front, back, 0.5f);

        Vector3 fb = back - front;

        Vector3 hAxis = Vector3.Cross(fb.normalized, Vector3.up);

        float h = Vector3.Dot(hAxis, midHigh - mid);
        float wiSquared = fb.sqrMagnitude;

        float r = Mathf.Abs(h * 0.5f + wiSquared / (8 * h));

        Debug.DrawLine(front, back, Color.white);
        Debug.DrawLine(mid, midHigh, Color.white);

        Debug.DrawLine(mid, mid + hAxis, Color.red);
        Debug.Log(r);

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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < trainRoot.Segments.Length; i++)
            {
                Transform item = trainRoot.Segments[i];
                Gizmos.color = Color.green;
                Gizmos.DrawLine(item.position, item.position + Vector3.up * acceleration[i] * 5);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(item.position + item.forward * 0.1f, item.position + Vector3.up * resistance[i] * 5);

            }

            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * airResistance);
            Gizmos.DrawLine(transform.position + transform.right * 0.3f, transform.position + Vector3.up * turnResistance);
        }
    }

}
