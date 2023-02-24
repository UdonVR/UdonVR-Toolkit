using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Sparkle_Editor : ShaderGUI
{
    Material targetMat;
    MaterialEditor materialEditor;
    MaterialProperty[] properties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.targetMat = materialEditor.target as Material;
        this.materialEditor = materialEditor;
        this.properties = properties;

        EditorGUI.BeginChangeCheck();
        // code here

        /*
        _MainTex ("Noise Tex", 2D) = "white" {}
        _NoiseRotationSpeed("Noise Rotation Speed", Range(1, 100)) = 25
        _SparkleOffset("Sparkle Offset", Range(0, 1)) = 0.5
        _Color("Color", Color) = (0.6941177, 1, 0.9647059, 1)
        _NoiseScale("Noise Scale", Vector) = (1, 1, 0, 0)

        // fresnel
        [MaterialToggle] _EnableFresnel("Enable Fresnel", Float) = 0
        _FresnelBias("Fresnel Bias", Float) = -0.35
        _FresnelScale("Fresnel Scale", Float) = 1
        _FresnelPower("Fresnel Power", Float) = 0.85
        */

        GUILayout.BeginVertical(EditorStyles.helpBox);
        Texture mainTex = (Texture)EditorGUILayout.ObjectField(new GUIContent("Noise Tex"), targetMat.GetTexture("_MainTex"), typeof(Texture), false);
        float noiseRotationSpeed = EditorGUILayout.Slider(new GUIContent("Noise Rotation Speed"), targetMat.GetFloat("_NoiseRotationSpeed"), 0, 100);
        float sparkleOffset = EditorGUILayout.Slider(new GUIContent("Sparkle Offset"), targetMat.GetFloat("_SparkleOffset"), 0, 1);
        Color color = EditorGUILayout.ColorField(new GUIContent("Color"), targetMat.GetColor("_Color"), true, false, false);
        Vector2 noiseScale = EditorGUILayout.Vector2Field(new GUIContent("Noise Scale"), targetMat.GetVector("_NoiseScale"));
        GUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            // code here
            targetMat.SetTexture("_MainTex", mainTex);
            targetMat.SetFloat("_NoiseRotationSpeed", noiseRotationSpeed);
            targetMat.SetFloat("_SparkleOffset", sparkleOffset);
            targetMat.SetColor("_Color", color);
            targetMat.SetVector("_NoiseScale", noiseScale);
        }
        base.OnGUI(materialEditor, properties);
    }
}