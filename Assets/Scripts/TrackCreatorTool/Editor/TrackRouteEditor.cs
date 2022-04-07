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
        EditorGUI.BeginChangeCheck();

        TrackSection first = route.Sections.First();
        TrackSection last = route.Sections.Last();

        UpdateHoldingControlKey();
        DrawStartEndExtentionHandles(first, last);

        foreach (TrackSection section in route.Sections)
        {
            DrawInbetweenExtentionHandles();
            DrawSelectionHandle(TrackSectionEnd.Start, section);
            if (section == last) DrawSelectionHandle(TrackSectionEnd.End, section);
        }

        if (selectedSection != null && selectedTransforms.Length > 0)
        {
            DrawEditHandle(selectedTransforms[0], (nextSection != null ? nextSection.StartTransform : null), "Point");
        }
    }

    private void UpdateHoldingControlKey()
    {
        var e = Event.current;
        if (e.keyCode == KeyCode.LeftControl)
        {
            if (e.type == EventType.KeyUp)
                holdingControl = false;

            else if (e.type == EventType.KeyDown)
                holdingControl = true;
        }
    }

    private void DrawSelectionHandle(TrackSectionEnd end, TrackSection section)
    {
        Handles.color = Color.red;
        Transform transform = (end == TrackSectionEnd.Start) ? section.StartTransform : section.EndTransform;
        if (selectedTransforms == null || !selectedTransforms.Contains(transform))
        {
            float size = HandleUtility.GetHandleSize(transform.position) / 2f;
            if (Handles.Button(transform.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                if (holdingControl)
                    DeleteSection(section, end);
                else
                    SelectSection(section, end);
            }
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
                previous.EndTransform.position = next.StartTransform.position;
                previous.EndTransform.rotation = next.StartTransform.rotation;
            }
            else
            {
                next.StartTransform.position = previous.EndTransform.position;
                next.StartTransform.rotation = previous.EndTransform.rotation;
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
    private void DrawInbetweenExtentionHandles()
    {
        if (selectedSection != null)
            DrawInbetweenExtentionHandle(selectedSection);

        if (previousSection != null)
            DrawInbetweenExtentionHandle(previousSection);
    }

    private void DrawInbetweenExtentionHandle(TrackSection section)
    {
        TrackPoint point = section.CalculateTrackPointAtT(0.5f);
        Handles.color = Color.green;
        float size = HandleUtility.GetHandleSize(point.Position) / 3f;
        if (Handles.Button(point.Position, point.Rotation, size, size, Handles.CubeHandleCap))
        {
            ExtendRouteInbetween(section, point);
        }
    }

    private void ExtendRouteInbetween(TrackSection section, TrackPoint point)
    {
        TrackSection newSection = Instantiate(section, route.transform);
        newSection.name = "TrackSection";
        newSection.transform.SetSiblingIndex(section.transform.GetSiblingIndex() +1);

        section.EndTransform.position = point.Position;
        section.EndTransform.rotation = point.Rotation;

        newSection.StartTransform.position = point.Position;
        newSection.StartTransform.rotation = point.Rotation;

        SelectSection(newSection, TrackSectionEnd.Start);
        UpdateSections();
    }

    private void DrawStartEndExtentionHandles(TrackSection first, TrackSection last)
    {
        Handles.color = Color.green;
        if (DrawExtentionHandle(first.StartTransform, first.StartTangent))
        {
            ExtendRoute(TrackSectionEnd.Start, first, first.StartTransform.position, InvertedTangent(first.StartTransform, first.StartTangent));
        }
        else if (DrawExtentionHandle(last.EndTransform, last.EndTangent))
        {
            ExtendRoute(TrackSectionEnd.End, last, last.EndTransform.position, InvertedTangent(last.EndTransform, last.EndTangent));
        }
    }
    private static bool DrawExtentionHandle(Transform transform, Vector3 tangent)
    {
        float size = HandleUtility.GetHandleSize(transform.position) / 3f;
        Vector3 startLocalTangent = size * 1.5f * (tangent - transform.position).normalized;
        return (Handles.Button(transform.position - startLocalTangent, Quaternion.LookRotation(-startLocalTangent, Vector3.up), size, size, Handles.ConeHandleCap));
    }

    private Vector3 InvertedTangent(Transform transform, Vector3 tangent)
    {
        Vector3 local = transform.InverseTransformPoint(tangent);
        return transform.TransformPoint(-local);
    }

    private void ExtendRoute(TrackSectionEnd start, TrackSection section, Vector3 pos1, Vector3 pos2)
    {
        TrackSection newSection = Instantiate(section, route.transform);
        newSection.name = "TrackSection";

        if (start == TrackSectionEnd.Start)
            newSection.transform.SetSiblingIndex(0);

        bool isStart = start == TrackSectionEnd.Start;

        newSection.StartTransform.position = isStart ? pos2 : pos1;
        newSection.EndTransform.position = isStart ? pos1 : pos2;

        SelectSection(newSection, start);
        UpdateSections();
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

    private void DrawEditHandle(Transform transform, Transform otherTransform, string name)
    {

        float size = HandleUtility.GetHandleSize(transform.position) / 3f;

        Handles.color = Color.yellow;
        Quaternion newRot = Handles.Disc(transform.rotation, transform.position, Vector3.up, size, false, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(selectedTransforms, "Change " + name + " Rotation");
            foreach (Transform t in selectedTransforms)
                t.rotation = newRot;

            UpdateSections();
        }

        Handles.color = Color.red;
        Quaternion newSlope = Handles.Disc(transform.rotation, transform.position, transform.right, size, false, 0);
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
