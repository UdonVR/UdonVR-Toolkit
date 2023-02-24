using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UdonVR.EditorUtility;

[CustomEditor(typeof(UdonVR.PlayerLogger))]
public class ExampleEditor : Editor
{
    // code here
    private SerializedProperty enableSounds;
    private SerializedProperty bell;
    private SerializedProperty join;
    private SerializedProperty leave;
    private SerializedProperty joinEnable;
    private SerializedProperty leaveEnable;
    private SerializedProperty joinToggle;
    private SerializedProperty leaveToggle;
    private SerializedProperty logger;
    private SerializedProperty playerLogger;
    private SerializedProperty timeStamps;
    private SerializedProperty joinPrefix;
    private SerializedProperty leavePrefix;
    private SerializedProperty header;

    protected virtual void OnEnable()
    {
        // code here
        enableSounds = serializedObject.FindProperty("enableSounds");
        bell = serializedObject.FindProperty("bell");
        join = serializedObject.FindProperty("join");
        leave = serializedObject.FindProperty("leave");
        joinEnable = serializedObject.FindProperty("joinEnable");
        leaveEnable = serializedObject.FindProperty("leaveEnable");
        joinToggle = serializedObject.FindProperty("joinToggle");
        leaveToggle = serializedObject.FindProperty("leaveToggle");
        logger = serializedObject.FindProperty("logger");
        playerLogger = serializedObject.FindProperty("playerLogger");
        timeStamps = serializedObject.FindProperty("timeStamps");
        joinPrefix = serializedObject.FindProperty("joinPrefix");
        leavePrefix = serializedObject.FindProperty("leavePrefix");
        header = serializedObject.FindProperty("header");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //EditorGUILayout.Space();
        serializedObject.Update();
        DrawGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawGUI()
    {
        // code here
        if (UdonSharpEditor.UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        UdonVR_GUI.BeginButtonFoldout(enableSounds, null, "foldout_enableSounds", EditorStyles.helpBox, null, 0, 13);
        if (enableSounds.boolValue)
        {
            EditorGUILayout.PropertyField(bell);
            EditorGUILayout.PropertyField(join);
            EditorGUILayout.PropertyField(leave);
            EditorGUILayout.PropertyField(joinEnable);
            EditorGUILayout.PropertyField(leaveEnable);
            EditorGUILayout.PropertyField(joinToggle);
            EditorGUILayout.PropertyField(leaveToggle);
        }
        UdonVR_GUI.EndButtonFoldout("foldout_enableSounds");

        UdonVR_GUI.BeginButtonFoldout(logger, null, "foldout_logger", EditorStyles.helpBox, null, 0, 13);
        if (logger.boolValue)
        {
            EditorGUILayout.PropertyField(playerLogger);
            EditorGUILayout.PropertyField(timeStamps);
            EditorGUILayout.PropertyField(joinPrefix);
            EditorGUILayout.PropertyField(leavePrefix);
            EditorGUILayout.PropertyField(header);
        }
        UdonVR_GUI.EndButtonFoldout("foldout_logger");

        serializedObject.ApplyModifiedProperties();
    }
}