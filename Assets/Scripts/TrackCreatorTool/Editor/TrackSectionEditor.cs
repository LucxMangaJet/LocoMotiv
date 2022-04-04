using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TrackSection))]
[CanEditMultipleObjects]
public class TrackSectionEditor : Editor
{
    TrackMeshCreator meshCreator;
    TrackSection section;

    void OnEnable()
    {
        section = target as TrackSection;
        meshCreator = section.MeshCreator;

        HideTool(true);
    }

    private void OnDisable()
    {
        HideTool(false);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap Start To Previous End"))
        {
            TrackSection previous = section.Route.GetPrevious(section);
            if (previous != null)
            {
                section.StartTransform.position = previous.EndTransform.position;
                section.StartTransform.rotation = previous.EndTransform.rotation;
            }
        }
        if (GUILayout.Button("Snap Previous End To Start"))
        {
            TrackSection previous = section.Route.GetPrevious(section);
            if (previous != null)
            {
                previous.EndTransform.position = section.StartTransform.position;
                previous.EndTransform.rotation = section.StartTransform.rotation;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap End To Next Start"))
        {
            TrackSection next = section.Route.GetNext(section);
            if (next != null)
            {
                section.EndTransform.position = next.StartTransform.position;
                section.EndTransform.rotation = next.StartTransform.rotation;
            }
        }
        if (GUILayout.Button("Snap Next Start To End"))
        {
            TrackSection next = section.Route.GetNext(section);
            if (next != null)
            {
                next.StartTransform.position = section.EndTransform.position;
                next.StartTransform.rotation = section.EndTransform.rotation;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Middle"))
        {
            section.MidTransform.position = Vector3.Lerp(section.StartTransform.position, section.EndTransform.position, 0.5f);
            section.MidTransform.rotation = Quaternion.Lerp(section.StartTransform.rotation, section.EndTransform.rotation, 0.5f);
        }
        EditorGUILayout.EndHorizontal();
        UpdateSection();
    }

    private void UpdateSection()
    {
        section.UpdateLengthAndSamples();
        meshCreator.UpdateMesh();
    }

    protected virtual void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        CreateHandle(section.StartTransform, "Start");
        CreateHandle(section.MidTransform, "Middle");
        CreateHandle(section.EndTransform, "End");
    }

    private void CreateHandle(Transform transform, string name)
    {
        switch (Tools.current)
        {
            case Tool.Rotate:
                Quaternion newRot = Handles.RotationHandle(transform.rotation, transform.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(section, "Change " + name + " Rotation");
                    transform.rotation = newRot;
                    UpdateSection();
                }
                break;

            default:
                Vector3 newPos = Handles.PositionHandle(transform.position, transform.rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(section, "Change " + name + " Position");
                    transform.position = newPos;
                    UpdateSection();
                }
                break;
        }
    }

    public static void HideTool(bool hidden)
    {
        Type type = typeof(Tools);
        FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, hidden);
    }
}

