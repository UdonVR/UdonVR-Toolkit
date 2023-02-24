using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System;

using UdonVR.EditorUtility;

public class RadialProgressBar_Editor : ShaderGUI
{
    Material targetMat;
    MaterialEditor materialEditor;
    MaterialProperty[] properties;

    bool showShaderProperties = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.targetMat = materialEditor.target as Material;
        this.materialEditor = materialEditor;
        this.properties = properties;

        EditorGUI.BeginChangeCheck();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float percent = EditorGUILayout.Slider(new GUIContent("Percent", "Completion percentage of the progress bar."), targetMat.GetFloat("_Percent"), 0, 1);
        bool flipProgressBar = UdonVR_GUI.ToggleButton(new GUIContent("Flip Progress Bar", "Flips the progress bar horizontally."), System.Convert.ToBoolean(targetMat.GetFloat("_FlipProgressBar")));
        bool isBillboard = UdonVR_GUI.ToggleButton(new GUIContent("Billboard", "Does the progress bar face towards the camera."), System.Convert.ToBoolean(targetMat.GetFloat("_IsBillboard")));
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        bool isGradient = UdonVR_GUI.ToggleButton(new GUIContent("Use Gradient", "Do we use a different color for the start and end."), System.Convert.ToBoolean(targetMat.GetFloat("_IsGradient")));
        Vector4 color1 = EditorGUILayout.ColorField(!isGradient ? new GUIContent("Color", "Color of the progress bar.") : new GUIContent("Start Color", "Starting Color."), targetMat.GetColor("_Color1"), true, true, false);
        Vector4 color2 = targetMat.GetColor("_Color2");
        if (isGradient) color2 = EditorGUILayout.ColorField(new GUIContent("End Color", "Ending Color."), color2, true, true, false);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        bool isHollow = UdonVR_GUI.ToggleButton(new GUIContent("Is Hollow", "Makes the progress bar hollow."), System.Convert.ToBoolean(targetMat.GetFloat("_IsHollow")));
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float thickness = EditorGUILayout.Slider(new GUIContent("Thickness", "Thickness of the progress bar."), targetMat.GetFloat("_Thickness"), 0.005f, 0.15f);
        float radius = EditorGUILayout.Slider(new GUIContent("Radius", "Distance from the center."), targetMat.GetFloat("_Radius"), 0.05f, 0.475f);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float rotation = EditorGUILayout.Slider(new GUIContent("Rotation", "Rotation offset of the progress bar."), targetMat.GetFloat("_Rotation"), 0f, 360f);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float startAngle = EditorGUILayout.Slider(new GUIContent("Start Angle", "Starting angle of the progress bar."), targetMat.GetFloat("_StartAngle"), 0f, 179.9f);
        float endAngle = EditorGUILayout.Slider(new GUIContent("End Angle", "Ending angle of the progress bar."), targetMat.GetFloat("_EndAngle"), 180f, 360f);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float seperation = EditorGUILayout.Slider(new GUIContent("Seperation", "Amount of seperation to add between the start and end of the progress bar."), targetMat.GetFloat("_Seperation"), 0f, 90f);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (UdonVR_GUI.BeginButtonFoldout(new GUIContent("Show Shader Properties", ""), ref showShaderProperties, "showShaderProperties", null, EditorStyles.helpBox, 0, 12))
        {
            EditorGUILayout.LabelField(new GUIContent("_Percent", "The completion amount of the progress bar."));
            EditorGUILayout.LabelField(new GUIContent("_FlipProgressBar", "Flips the progress bar horizontally."));
            EditorGUILayout.LabelField(new GUIContent("_IsBillboard", "Does the progress bar face the camera. (transform.forward away from camera.)"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("_IsGradient", "Is there a gradient being used."));
            EditorGUILayout.LabelField(new GUIContent("_Color1", "The start / default color of the progress bar."));
            EditorGUILayout.LabelField(new GUIContent("_Color2", "The ending color of the progress bar."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("_IsHollow", "Is the progress bar hollow?"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("_Thickness", "The thickness of the progress bar."));
            EditorGUILayout.LabelField(new GUIContent("_Radius", "The distance the progress bar is from the center."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("_Rotation", "The rotation of the progress bar."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("_StartAngle", "Starting angle of the progress bar."));
            EditorGUILayout.LabelField(new GUIContent("_EndAngle", "Ending angle of the progress bar."));
            EditorGUILayout.LabelField(new GUIContent("_Seperation", "Amount of seperation to add between the start and end of the progress bar."));
        }
        UdonVR_GUI.EndButtonFoldout("showShaderProperties");
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            targetMat.SetFloat("_Percent", percent);
            targetMat.SetFloat("_FlipProgressBar", System.Convert.ToSingle(flipProgressBar));
            targetMat.SetFloat("_IsBillboard", System.Convert.ToSingle(isBillboard));

            targetMat.SetFloat("_IsGradient", System.Convert.ToSingle(isGradient));
            targetMat.SetColor("_Color1", color1);
            targetMat.SetColor("_Color2", color2);

            targetMat.SetFloat("_IsHollow", System.Convert.ToSingle(isHollow));

            targetMat.SetFloat("_Thickness", thickness);
            targetMat.SetFloat("_Radius", radius);

            targetMat.SetFloat("_Rotation", rotation);

            targetMat.SetFloat("_StartAngle", startAngle);
            targetMat.SetFloat("_EndAngle", endAngle);
            targetMat.SetFloat("_Seperation", seperation);
        }
        base.OnGUI(materialEditor, properties);
    }
}
