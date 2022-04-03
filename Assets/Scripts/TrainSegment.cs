using UnityEngine;

public class TrainSegment : MonoBehaviour
{
    [SerializeField] private float frontPointOffset, backPointOffset;

    protected float length;

    public float Length => length;

    private void Awake()
    {
        length = frontPointOffset - backPointOffset;
    }

    public void SetPosition(Vector3 frontPoint, Vector3 backPoint)
    {
        Vector3 midpoint = Vector3.Lerp(frontPoint, backPoint, frontPointOffset / length);

        transform.position = midpoint;
        transform.rotation = Quaternion.LookRotation(frontPoint - backPoint, Vector3.up);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * frontPointOffset, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * backPointOffset, 0.5f);
    }
}
