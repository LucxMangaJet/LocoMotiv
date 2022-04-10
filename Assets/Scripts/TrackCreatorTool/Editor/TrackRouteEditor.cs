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
            DrawEditHandle(selectedTransforms[0], "Point");

            Vector2 botRight = new Vector2(Screen.width, Screen.height) * 0.905f - new Vector2(20, 65);
            Vector2 size = new Vector2(250, 150);
            Handles.BeginGUI();

            GUIStyle box = new GUIStyle("box");

            GUILayout.BeginArea(new Rect(botRight.x - size.x, botRight.y - size.y, size.x, size.y), box);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorGUIUtility.FindTexture("d_tab_prev")))
            {

            }

            if (GUILayout.Button(EditorGUIUtility.FindTexture("d_tab_next")))
            {

            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();


            float height = selectedTransforms[0].position.y;
            int iconSize = 40;
            GUIStyle bold = new GUIStyle();
            bold.normal.textColor = Color.white;
            bold.fontStyle = FontStyle.Bold;

            if (previousSection != null)
            {

                float difference = height - previousSection.StartTransform.position.y;
                float distance = Vector2.Distance(new Vector2(selectedTransforms[0].position.x, selectedTransforms[0].position.z), new Vector2(previousSection.StartTransform.position.x, previousSection.StartTransform.position.z));

                float slope = difference / distance * 100f;

                GUILayout.BeginVertical();

                GUILayout.Label("previous height");

                GUILayout.BeginHorizontal();

                GUILayout.Label((Texture)AssetDatabase.LoadAssetAtPath("Assets/Textures/EditorIcons/icon_editor_track_height-difference-to-previous.png", typeof(Texture)), GUILayout.Height(iconSize), GUILayout.Width(iconSize));

                GUILayout.BeginVertical();

                GUILayout.Label($"{difference.ToString("N1")}m");
                GUILayout.Label($"{slope.ToString("N1")}%", bold);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            if (nextSection != null)
            {

                float difference = nextSection.StartTransform.position.y - height;
                float distance = Vector2.Distance(new Vector2(selectedTransforms[0].position.x, selectedTransforms[0].position.z), new Vector2(nextSection.StartTransform.position.x, nextSection.StartTransform.position.z));

                float slope = difference / distance * 100f;

                GUILayout.BeginVertical();

                GUILayout.Label("next height");

                GUILayout.BeginHorizontal();

                GUILayout.Label((Texture)AssetDatabase.LoadAssetAtPath("Assets/Textures/EditorIcons/icon_editor_track_height-difference-to-next.png", typeof(Texture)), GUILayout.Height(iconSize), GUILayout.Width(iconSize));

                GUILayout.BeginVertical();

                GUILayout.Label($"{difference.ToString("N1")}m");
                GUILayout.Label($"{slope.ToString("N1")}%", bold);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (previousSection != null && GUILayout.Button("adapt height"))
            {
                AdaptHeightTo(previousSection.StartTransform);
            }

            if (nextSection != null && GUILayout.Button("adapt height"))
            {
                AdaptHeightTo(nextSection.StartTransform);
            }

            GUILayout.EndHorizontal();

            if (previousSection != null && nextSection != null)
            {
                if (GUILayout.Button("Auto Adapt Slope"))
                {
                    Vector3 prev = previousSection.StartTransform.position;
                    Vector3 next = nextSection.StartTransform.position;

                    float difference = next.y - prev.y;
                    float distance = Vector3.Distance(prev, next);

                    Undo.RecordObjects(selectedTransforms, "Auto Adapt Slope");
                    foreach (Transform t in selectedTransforms)
                        t.rotation = Quaternion.Euler(-Mathf.Asin(difference / distance) * Mathf.Rad2Deg, t.rotation.eulerAngles.y, t.rotation.eulerAngles.z);

                    UpdateSections();
                }
            }


            if (GUILayout.Button("Set slope to 0"))
            {
                Undo.RecordObjects(selectedTransforms, "Reset " + name + " Slope");
                foreach (Transform t in selectedTransforms)
                    t.rotation = Quaternion.Euler(0, t.rotation.eulerAngles.y, t.rotation.eulerAngles.z);

                UpdateSections();
            }

            GUILayout.EndArea();
            Handles.EndGUI();
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
        newSection.transform.SetSiblingIndex(section.transform.GetSiblingIndex() + 1);

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

    private void DrawEditHandle(Transform transform, string name)
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
        if (EditorGUI.EndChangeCheck() && newPos != transform.position)
        {
            Undo.RecordObjects(selectedTransforms, "Change " + name + " Position");
            foreach (Transform t in selectedTransforms)
                t.position = newPos;

            UpdateSections();
        }

        Handles.color = Color.white;

        if (previousSection != null)
        {
            Vector3 cur = transform.position;
            Vector3 prev = previousSection.StartTransform.position;
            DrawSlopeHelper(cur, prev);
        }

        if (nextSection != null)
        {
            Vector3 cur = transform.position;
            Vector3 nex = nextSection.StartTransform.position;
            DrawSlopeHelper(cur, nex);
        }
    }

    private static void DrawSlopeHelper(Vector3 cur, Vector3 prev)
    {
        Vector3 remapped = new Vector3(cur.x, prev.y, cur.z);

        float slope = (cur.y - prev.y) / Vector3.Distance(prev, remapped) * 100f;

        Handles.DrawLine(prev, remapped, 1);
        Handles.Label(remapped, slope.ToString("N1") + "%");
        Handles.DrawLine(remapped, cur, 1);
    }

    private void AdaptHeightTo(Transform startTransform)
    {
        Vector3 ownPos = selectedTransforms[0].position;
        Vector3 otherPos = startTransform.position;
        Vector3 newPos = new Vector3(ownPos.x, otherPos.y, ownPos.z);

        Undo.RecordObjects(selectedTransforms, "Adapt Point Height");
        foreach (Transform t in selectedTransforms)
            t.position = newPos;

        UpdateSections();
    }


    private bool DrawButton(string name, float size, Vector3 pos, string text)
    {
        Texture2D debugTex = new Texture2D(1, 1);
        debugTex.SetPixel(0, 0, Color.black);
        debugTex.Apply();

        GUIStyle style = new GUIStyle();
        style.normal.background = debugTex;
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        Handles.color = Color.white;
        Handles.Label(pos, text, style);

        return (Handles.Button(pos, Quaternion.LookRotation(pos - SceneView.GetAllSceneCameras()[0].transform.position, Vector3.up), size, size, Handles.CircleHandleCap));
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
