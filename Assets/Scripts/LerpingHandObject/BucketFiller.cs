using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketFiller : MonoBehaviour
{
    public float fillValue;
    Bucket current;

    public void StartFilling()
    {
        current = Bucket.Instance;
    }

    public void StopFilling()
    {
        current = null;
    }

    private void Update()
    {
        if (current == null) return;

        current.SetFillValue(fillValue);
    }

    public void SetFillValue(float value)
    {
        Bucket.Instance.SetFillValue(value);
    }
}
