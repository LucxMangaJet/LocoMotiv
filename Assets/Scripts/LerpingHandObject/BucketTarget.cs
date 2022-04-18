using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketTarget : HandObjectTarget
{
    public static System.Action<BucketTarget> ChangeOverrideBucket;
    internal static void SetOverride(BucketTarget target)
    {
        ChangeOverrideBucket?.Invoke(target);
    }
}
