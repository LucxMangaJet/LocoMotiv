using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(DebugAnimationCurve))]
public class DebugAnimationCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        AnimationCurve curve = property.FindPropertyRelative("Curve").animationCurveValue;

        if (curve == null || curve.keys.Length == 0) return;

        float tMin = curve.keys[0].time;
        float tMax = curve.keys[curve.length - 1].time;

        float value = property.FindPropertyRelative("Value").floatValue;

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.CurveField(position, curve);
        EditorGUI.Slider(position, value, tMin, tMax);
        EditorGUI.LabelField(position, label);
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 3f;
    }
}
