using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System;

[CustomEditor(typeof(TrackRoute))]
[CanEditMultipleObjects]

public class TrackRouteEditor : Editor
{
    TrackMeshCreator meshCreator;
    TrackRoute route;

    TrackSection selectedSection;
    TrackSection previousSection;
    TrackSection nextSection;

    bool selectedPointIsStart;

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
        if (previousSection != null) previousSection.UpdateLengthAndSamplesAndMesh();
        if (selectedSection != null) selectedSection.UpdateLengthAndSamplesAndMesh();
        if (nextSection != null) nextSection.UpdateLengthAndSamplesAndMesh();
    }

    protected virtual void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        foreach (TrackSection section in route.Sections)
        {
            if (section != selectedSection && section != nextSection && section != previousSection)
            {
                float startSize = HandleUtility.GetHandleSize(section.StartTransform.position) / 2f;
                float endSize = HandleUtility.GetHandleSize(section.EndTransform.position) / 2f;
                Handles.color = Color.red;
                if (Handles.Button(section.StartTransform.position, Quaternion.identity, startSize, startSize, Handles.SphereHandleCap)
                    || Handles.Button(section.EndTransform.position, Quaternion.identity, endSize, endSize, Handles.SphereHandleCap))
                    SelectSection(section);
            }
        }

        if (selectedSection != null)
        {
            CreateHandle(selectedSection.StartTransform, (previousSection != null ? previousSection.EndTransform : null), "Start");
            CreateHandle(selectedSection.EndTransform, (nextSection != null ? nextSection.StartTransform : null), "End");
        }
    }

    private void SelectSection(TrackSection section)
    {
        selectedSection = section;
        previousSection = route.GetPrevious(section);
        nextSection = route.GetNext(section);
    }

    private void CreateHandle(Transform transform, Transform otherTransform, string name)
    {
        UnityEngine.Object[] recordedObjects = (otherTransform == null) ? new UnityEngine.Object[] { transform } : new UnityEngine.Object[] { transform, otherTransform };

        Handles.color = Color.yellow;
        Quaternion newRot = Handles.Disc(transform.rotation, transform.position, Vector3.up, 5, false, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordedObjects, "Change " + name + " Rotation");
            transform.rotation = newRot;
            if (otherTransform != null) otherTransform.rotation = newRot;
            UpdateSections();
        }

        Handles.color = Color.red;
        Quaternion newSlope = Handles.Disc(transform.rotation, transform.position, transform.right, 5, false, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordedObjects, "Change " + name + " Rotation");
            transform.rotation = newSlope;
            if (otherTransform != null) otherTransform.rotation = newSlope;
            UpdateSections();
        }

        Vector3 newPos = Handles.PositionHandle(transform.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordedObjects, "Change " + name + " Position");
            transform.position = newPos;
            if (otherTransform != null) otherTransform.position = newPos;
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
