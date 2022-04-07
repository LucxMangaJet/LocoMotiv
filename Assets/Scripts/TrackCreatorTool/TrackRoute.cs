using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackRoute : MonoBehaviour
{
    [SerializeField, ReadOnly] List<TrackSection> sections = new List<TrackSection>();
    public List<TrackSection> Sections => sections;

    public float Length => sections.Sum((x) => x.Length);

    public TrackSection GetPrevious(TrackSection section)
    {
        CheckUpdateSections();

        for (int i = 1; i < sections.Count; i++)
        {
            if (sections[i] == section)
                return sections[i - 1];
        }

        return null;
    }

    internal TrackPoint SampleTrackpoint(float currentDistanceOnRoute)
    {
        float distanceBefore = 0;
        float distance = 0;
        TrackSection section = null;

        foreach (TrackSection s in sections)
        {
            distance += s.Length;
            if (currentDistanceOnRoute < distance)
            {
                section = s;
                break;
            }
            else
            {
                distanceBefore = distance;
            }
        }

        if (section == null) return null;

        return section.CalculateTrackPointAtT((currentDistanceOnRoute - distanceBefore) / section.Length);
    }

    public void CheckUpdateSections()
    {
        if (sections.Count == transform.childCount) return;
        sections = new List<TrackSection>(GetComponentsInChildren<TrackSection>());
    }

    public TrackSection GetNext(TrackSection section)
    {
        CheckUpdateSections();

        for (int i = 0; i < sections.Count - 1; i++)
        {
            if (sections[i] == section)
                return sections[i + 1];
        }

        return null;
    }
}
