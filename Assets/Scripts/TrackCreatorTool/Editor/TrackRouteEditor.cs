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
    TrackMeshCreator meshCreator;
    TrackRoute route;

    TrackSection selectedSection;
    TrackSection previousSection;
    TrackSection nextSection;

    Transform[] selectedTransforms;

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

        TrackSection last = route.Sections.Last();

        foreach (TrackSection section in route.Sections)
        {
            float startSize = HandleUtility.GetHandleSize(section.StartTransform.position) / 2f;
            float endSize = HandleUtility.GetHandleSize(section.EndTransform.position) / 2f;
            Handles.color = Color.red;

            if (selectedTransforms == null || !selectedTransforms.Contains(section.StartTransform))
            {
                if (Handles.Button(section.StartTransform.position, Quaternion.identity, startSize, startSize, Handles.SphereHandleCap))
                    SelectSection(section, selectStart: true);
            }

            if (section == last)
            {
                if (selectedTransforms == null || !selectedTransforms.Contains(section.EndTransform))
                {
                    if (Handles.Button(section.EndTransform.position, Quaternion.identity, endSize, endSize, Handles.SphereHandleCap))
                        SelectSection(section, selectStart: false);
                }
            }
        }

        if (selectedSection != null && selectedTransforms.Length > 0)
        {
            CreateHandle(selectedTransforms[0], (nextSection != null ? nextSection.StartTransform : null), "Point");
        }
    }

    private void SelectSection(TrackSection section, bool selectStart)
    {
        selectedSection = section;
        previousSection = route.GetPrevious(section);
        nextSection = route.GetNext(section);

        if (selectStart)
        {
            if (previousSection != null)
                selectedTransforms = new Transform[] { section.StartTransform, previousSection.EndTransform };
            else
                selectedTransforms = new Transform[] { section.StartTransform };
        } else
        {
            if (nextSection != null)
                selectedTransforms = new Transform[] { section.EndTransform, nextSection.StartTransform };
            else
                selectedTransforms = new Transform[] { section.EndTransform };
        }
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
