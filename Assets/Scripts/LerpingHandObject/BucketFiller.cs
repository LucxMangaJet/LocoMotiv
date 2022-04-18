using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketFiller : MonoBehaviour
{
    public float fillValue;
    Bucket current;

    public void StartFilling()
    {
        current = FindObjectOfType<Bucket>();
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
}
