using System.Collections;
using System.Collections.Generic;
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
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Update Length And Samples"))
        {
            section.UpdateLengthAndSamples();
        }

        section.UpdateLengthAndSamples();
        meshCreator.CreateMesh();
    }
}

