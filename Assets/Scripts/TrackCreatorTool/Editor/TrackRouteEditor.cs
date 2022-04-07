using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(TrackRoute))]
[CanEditMultipleObjects]

public class TrackRouteEditor : Editor
{
    TrackRoute route;

    TrackSection selectedSection;
    TrackSection previousSection;
    TrackSection nextSection;

    Transform[] selectedTransforms;

    bool holdingControl = false;

    void OnEnable()
    {
        route = target as TrackRoute;
        HideTool(true);
    }

    private void OnDisable()
    {
        HideTool(false);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void UpdateSections()
    {
        Debug.Log($"UpdateSections \n { previousSection != null } \n { selectedSection != null } \n { nextSection != null }");

        if (previousSection != null) previousSection.UpdateLengthAndSamplesAndMesh();
        if (selectedSection != null) selectedSection.UpdateLengthAndSamplesAndMesh();
        if (nextSection != null) nextSection.UpdateLengthAndSamplesAndMesh();
    }

    protected virtual void OnSceneGUI()
    {
        Debug.Log($"OnSceneGUI!");

        EditorGUI.BeginChangeCheck();

        TrackSection first = route.Sections.First();
        TrackSection last = route.Sections.Last();

        CreateExtentionHandles(first, last);

        foreach (TrackSection section in route.Sections)
        {
            float startSize = HandleUtility.GetHandleSize(section.StartTransform.position) / 2f;
            float endSize = HandleUtility.GetHandleSize(section.EndTransform.position) / 2f;
            Handles.color = Color.red;

            var e = Event.current;

            if (e.keyCode == KeyCode.LeftControl)
            {
                if (e.type == EventType.KeyUp)
                    holdingControl = false;

                else if (e.type == EventType.KeyDown)
                    holdingControl = true;
            }

            Debug.Log($"isHoldingControl = { holdingControl }");

            if (selectedTransforms == null || !selectedTransforms.Contains(section.StartTransform))
            {
                if (Handles.Button(section.StartTransform.position, Quaternion.identity, startSize, startSize, Handles.SphereHandleCap))
                {
                    if (holdingControl)
                        DeleteSection(section, TrackSectionEnd.Start);
                    else
                        SelectSection(section, TrackSectionEnd.Start);
                }
            }

            if (section == last)
            {
                if (selectedTransforms == null || !selectedTransforms.Contains(section.EndTransform))
                {
                    if (Handles.Button(section.EndTransform.position, Quaternion.identity, endSize, endSize, Handles.SphereHandleCap))
                    {
                        if (holdingControl)
                            DeleteSection(section, TrackSectionEnd.End);
                        else
                            SelectSection(section, TrackSectionEnd.End);
                    }
                }
            }
        }

        if (selectedSection != null && selectedTransforms.Length > 0)
        {
            CreateHandle(selectedTransforms[0], (nextSection != null ? nextSection.StartTransform : null), "Point");
        }
    }

    private void DeleteSection(TrackSection section, TrackSectionEnd start)
    {
        bool isStart = start == TrackSectionEnd.Start;

        TrackSection next = route.GetNext(section);
        TrackSection previous = route.GetPrevious(section);

        Debug.LogWarning($"previousSection = { previous }, nextSection = { next }");

        if (previous != null && next != null)
        {
            if (isStart)
            {
                next.StartTransform.position = previous.EndTransform.position;
                next.StartTransform.rotation = previous.EndTransform.rotation;
            }
            else
            {
                previous.EndTransform.position = next.StartTransform.position;
                previous.EndTransform.rotation = next.StartTransform.rotation;
            }
        }

        DestroyImmediate(section.gameObject);
        route.CheckUpdateSections();

        if (isStart && previous != null)
            SelectSection(previous, TrackSectionEnd.End);
        else if (next != null)
            SelectSection(next, TrackSectionEnd.Start);
        else
            Deselect();
    }

    private void CreateExtentionHandles(TrackSection first, TrackSection last)
    {
        if (DrawExtentionHandle(first.StartTransform, first.StartTangent))
        {
            ExtendRoute(TrackSectionEnd.Start, first, first.StartTransform.position, InvertedTangent(first.StartTransform, first.StartTangent));
        }
        else if (DrawExtentionHandle(last.EndTransform, last.EndTangent))
        {
            ExtendRoute(TrackSectionEnd.End, last, last.EndTransform.position, InvertedTangent(last.EndTransform, last.EndTangent));
        }
    }

    private Vector3 InvertedTangent(Transform transform, Vector3 tangent)
    {
        Vector3 local = transform.InverseTransformPoint(tangent);
        return transform.TransformPoint(-local);
    }

    private void ExtendRoute(TrackSectionEnd start, TrackSection section, Vector3 pos1, Vector3 pos2)
    {
        TrackSection newSection = Instantiate(section, route.transform);

        if (start == TrackSectionEnd.Start)
            newSection.transform.SetSiblingIndex(0);

        bool isStart = start == TrackSectionEnd.Start;

        newSection.StartTransform.position = isStart ? pos2 : pos1;
        newSection.EndTransform.position = isStart ? pos1 : pos2;

        SelectSection(newSection, start);
        UpdateSections();
    }

    private static bool DrawExtentionHandle(Transform transform, Vector3 tangent)
    {
        float startSize = HandleUtility.GetHandleSize(transform.position);
        Vector3 startLocalTangent = startSize * 1.5f * (tangent - transform.position).normalized;
        return (Handles.Button(transform.position - startLocalTangent, Quaternion.LookRotation(-startLocalTangent, Vector3.up), startSize, startSize, Handles.ConeHandleCap));
    }

    private void SelectSection(TrackSection section, TrackSectionEnd selectStart)
    {
        selectedSection = section;
        previousSection = route.GetPrevious(section);
        nextSection = route.GetNext(section);

        if (selectStart == TrackSectionEnd.Start)
        {
            if (previousSection != null)
                selectedTransforms = new Transform[] { section.StartTransform, previousSection.EndTransform };
            else
                selectedTransforms = new Transform[] { section.StartTransform };
        }
        else
        {
            if (nextSection != null)
                selectedTransforms = new Transform[] { section.EndTransform, nextSection.StartTransform };
            else
                selectedTransforms = new Transform[] { section.EndTransform };
        }
    }

    private void Deselect()
    {
        selectedSection = null;
        previousSection = null;
        nextSection = null;
        selectedTransforms = null;
    }

    private void CreateHandle(Transform transform, Transform otherTransform, string name)
    {
        Handles.color = Color.yellow;
        Quaternion newRot = Handles.Disc(transform.rotation, transform.position, Vector3.up, 5, false, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(selectedTransforms, "Change " + name + " Rotation");
            foreach (Transform t in selectedTransforms)
                t.rotation = newRot;

            UpdateSections();
        }

        Handles.color = Color.red;
        Quaternion newSlope = Handles.Disc(transform.rotation, transform.position, transform.right, 5, false, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(selectedTransforms, "Change " + name + " Rotation");
            foreach (Transform t in selectedTransforms)
                t.rotation = newSlope;

            UpdateSections();
        }

        Vector3 newPos = Handles.PositionHandle(transform.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(selectedTransforms, "Change " + name + " Position");
            foreach (Transform t in selectedTransforms)
                t.position = newPos;

            UpdateSections();
        }
    }

    public static void HideTool(bool hidden)
    {
        Type type = typeof(Tools);
        FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, hidden);
    }
}

public enum TrackSectionEnd
{
    Start,
    End,
}
