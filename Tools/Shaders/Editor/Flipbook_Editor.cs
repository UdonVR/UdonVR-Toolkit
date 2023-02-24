using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UdonVR.EditorUtility;
using UdonVR.EditorUtility.ShaderGUI;

public class Flipbook_Editor : ShaderGUI
{
    Material targetMat;
    MaterialEditor materialEditor;
    MaterialProperty[] properties;

    enum WaveType
    {
        Sawtooth, Triangle
    };


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.targetMat = materialEditor.target as Material;
        this.materialEditor = materialEditor;
        this.properties = properties;

        EditorGUI.BeginChangeCheck();

        UdonVR_ShaderGUI.CullBlendSpecularData cullBlendSpecularData = UdonVR_ShaderGUI.ShowCullBlendSpecular(targetMat);

        EditorGUILayout.Space();

        UdonVR_ShaderGUI.GlossMetalAlphaData glossMetalAlphaData = UdonVR_ShaderGUI.ShowGlossMetalAlpha(targetMat);

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        Texture mainTex = (Texture)EditorGUILayout.ObjectField(new GUIContent("Main Texture"), targetMat.GetTexture("_MainTex"), typeof(Texture), false);
        bool enableEmission = UdonVR_GUI.ToggleButton(new GUIContent("Enable Emission"), System.Convert.ToBoolean(targetMat.GetFloat("_EnableEmission")));
        Texture emissionMap = targetMat.GetTexture("_EmissionMap");
        bool emissionMapIsFlipbook = System.Convert.ToBoolean(targetMat.GetFloat("_EmissionMapIsFlipbook"));
        if (enableEmission)
        {
            emissionMap = (Texture)EditorGUILayout.ObjectField(new GUIContent("Emission Map"), emissionMap, typeof(Texture), false);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            emissionMapIsFlipbook = UdonVR_GUI.ToggleButton(new GUIContent("Emission Map Is Flipbook", "Is the emission map a flipbook and does it follow the flipbook."), emissionMapIsFlipbook);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        Color color = EditorGUILayout.ColorField(new GUIContent("Color"), targetMat.GetColor("_Color"), true, false, false);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        bool enableLoop = System.Convert.ToBoolean(targetMat.GetFloat("_EnableLoop"));
        float percent = targetMat.GetFloat("_Percent");
        int index = targetMat.GetInt("_Index");
        WaveType waveType = (WaveType)targetMat.GetInt("_WaveType");
        float speed = targetMat.GetFloat("_Speed");
        int columns = targetMat.GetInt("_Columns");
        int rows = targetMat.GetInt("_Rows");

        enableLoop = UdonVR_GUI.ToggleButton(new GUIContent("Enable Loop"), enableLoop);

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical();

        if (!enableLoop)
        {
            UdonVR_GUI.FieldArea(
                new GUIContent[]
                {
                    new GUIContent("Percent", "Completion percentage."),
                    new GUIContent("Index", "Desired index.")
                },
                new UdonVR_GUI.FieldAreaValues[]{
                    UdonVR_GUI.FieldAreaValues.SetValueSlider(percent, true, 0, 1),
                    UdonVR_GUI.FieldAreaValues.SetValueSlider(index, true, 0, (columns * rows) - 1)
                },
                new System.Action<UdonVR_GUI.FieldAreaValues>[]
                {
                    (value) => {
                        percent = value.floatValue.Value;
                    },
                    (value) => {
                        index = (int)value.intValue.Value;
                    }
                },
                EditorStyles.helpBox
            );
        }
        else
        {
            waveType = (WaveType)EditorGUILayout.EnumPopup(new GUIContent("Wave Type"), waveType);
            speed = EditorGUILayout.Slider(new GUIContent("Speed"), speed, 0.005f, 5f);
        }

        EditorGUILayout.Space(1);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        columns = EditorGUILayout.IntSlider(new GUIContent("Columns"), columns, 0, 32);
        rows = EditorGUILayout.IntSlider(new GUIContent("Rows"), rows, 0, 32);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            UdonVR_ShaderGUI.SaveCullBlendSpecular(targetMat, cullBlendSpecularData);
            UdonVR_ShaderGUI.SaveGlossMetalAlpha(targetMat, glossMetalAlphaData);

            targetMat.SetTexture("_MainTex", mainTex);
            targetMat.SetFloat("_EnableEmission", System.Convert.ToSingle(enableEmission));
            targetMat.SetTexture("_EmissionMap", emissionMap);
            targetMat.SetFloat("_EmissionMapIsFlipbook", System.Convert.ToSingle(emissionMapIsFlipbook));
            targetMat.SetColor("_Color", color);

            targetMat.SetFloat("_EnableLoop", System.Convert.ToSingle(enableLoop));
            targetMat.SetFloat("_Percent", percent);
            targetMat.SetInt("_Index", index);
            targetMat.SetInt("_WaveType", UdonVR_EnumHelpers.IntFromEnum(waveType));
            targetMat.SetFloat("_Speed", speed);
            targetMat.SetInt("_Columns", columns);
            targetMat.SetInt("_Rows", rows);
        }
        base.OnGUI(materialEditor, properties);
    }
}