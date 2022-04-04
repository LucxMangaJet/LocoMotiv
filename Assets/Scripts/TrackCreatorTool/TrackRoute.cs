using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRoute : MonoBehaviour
{
    [SerializeField, ReadOnly] List<TrackSection> sections = new List<TrackSection>();
}
