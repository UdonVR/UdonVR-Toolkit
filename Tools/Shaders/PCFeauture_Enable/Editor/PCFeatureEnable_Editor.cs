using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UdonVR.EditorUtility;


[CustomEditor(typeof(PCFeatureEnable)), CanEditMultipleObjects]
public class PCFeatureEnable_Editor : Editor
{
    // code here

    protected virtual void OnEnable()
    {
        // code here
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //EditorGUILayout.Space();
        serializedObject.Update();
        DrawGUI();
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    protected virtual void DrawGUI()
    {
        // code here
        UdonVR_GUI.Header(new GUIContent("Used to enable the parts of shaders disabled for quest users."));
    }
}