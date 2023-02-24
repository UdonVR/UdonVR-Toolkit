using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UdonVR.EditorUtility;
using UdonVR.EditorUtility.ShaderGUI;

public class BowlingCarpet_Editor : ShaderGUI
{
    Material targetMat;
    MaterialEditor materialEditor;
    MaterialProperty[] properties;

    bool showShapeColors = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.targetMat = materialEditor.target as Material;
        this.materialEditor = materialEditor;
        this.properties = properties;


        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        UdonVR_GUI.Header(new GUIContent("Works with 'PCFeatureEnable' Prefab", "This shader has parts dissabled for quest users they can be enabled for PC by adding the material into this script."));
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        UdonVR_ShaderGUI.CullBlendSpecularData cullBlendSpecularData = UdonVR_ShaderGUI.ShowCullBlendSpecular(targetMat);

        EditorGUILayout.Space();

        UdonVR_ShaderGUI.GlossMetalAlphaData glossMetalAlphaData = UdonVR_ShaderGUI.ShowGlossMetalAlpha(targetMat);

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        Texture mainTex = (Texture)EditorGUILayout.ObjectField(new GUIContent("Carpet Texture", "Texture to add to the carpet."), targetMat.GetTexture("_MainTex"), typeof(Texture), false);
        bool greyscaleMainTex = UdonVR_GUI.ToggleButton(new GUIContent("Greyscale Carpet Texture", "Greyscales the carpet texture removing color."), System.Convert.ToBoolean(targetMat.GetFloat("_GreyscaleMainTex")));
        Color color = EditorGUILayout.ColorField(new GUIContent("Color", "Color of the carpet."), targetMat.GetColor("_Color"), true, false, false);
        float emissionBrightness = EditorGUILayout.Slider(new GUIContent("Emission Brightness", "Brightness of the emission."), targetMat.GetFloat("_EmissionBrightness"), 0, 1);
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        float shapeBrightnessMultiplier = EditorGUILayout.Slider(new GUIContent("Shape Brightness Multiplier", "Brightness multiplier of the shapes. (Increases base brightness and thus also emission brightness.)"), targetMat.GetFloat("_ShapeBrightnessMultiplier"), 1, 10);

        bool isFrame = System.Convert.ToBoolean(targetMat.GetFloat("_IsFrame"));
        bool isFrameRandom = System.Convert.ToBoolean(targetMat.GetFloat("_IsFrameRandom"));
        UdonVR_GUI.FieldArea(
            new GUIContent[] {
                new GUIContent("Frames", "Make the shapes into frames."),
                new GUIContent("Random Frames", "Randomly makes the shapes into frames."),
            },
            new UdonVR_GUI.FieldAreaValues[]
            {
                UdonVR_GUI.FieldAreaValues.SetValueToggle(isFrame, true),
                UdonVR_GUI.FieldAreaValues.SetValueToggle(isFrameRandom, isFrame)
            },
            new System.Action<UdonVR_GUI.FieldAreaValues>[]
            {
                (areaValues) =>
                {
                    isFrame = areaValues.boolValue.Value;
                },
                (areaValues) =>
                {
                    isFrameRandom = areaValues.boolValue.Value;
                }
            }
        );

        bool removeRandomShapes = System.Convert.ToBoolean(targetMat.GetFloat("_RemoveRandomShapes"));
        removeRandomShapes = UdonVR_GUI.ToggleButton(new GUIContent("Remove Random Shapes", "Randomly remove shapes."), removeRandomShapes);

        bool isTopOnly = System.Convert.ToBoolean(targetMat.GetFloat("_IsTopOnly"));
        isTopOnly = UdonVR_GUI.ToggleButton(new GUIContent("Top Only", "Do we only draw on the top side of the object."), isTopOnly);

        EditorGUILayout.Space();

        int colorCount = targetMat.GetInt("_ColorCount");
        Color shapeColor1 = targetMat.GetColor("_ShapeColor1");
        Color shapeColor2 = targetMat.GetColor("_ShapeColor2");
        Color shapeColor3 = targetMat.GetColor("_ShapeColor3");
        Color shapeColor4 = targetMat.GetColor("_ShapeColor4");
        Color shapeColor5 = targetMat.GetColor("_ShapeColor5");
        Color shapeColor6 = targetMat.GetColor("_ShapeColor6");
        Color shapeColor7 = targetMat.GetColor("_ShapeColor7");
        Color shapeColor8 = targetMat.GetColor("_ShapeColor8");

        if (UdonVR_GUI.BeginButtonFoldout(new GUIContent("Show Shape Colors"), ref showShapeColors, "showShapeColors", EditorStyles.helpBox, EditorStyles.helpBox, 0, 12))
        {
            colorCount = EditorGUILayout.IntSlider(new GUIContent("Number of Colors", "The number of colors to use."), colorCount, 1, 8);
            shapeColor1 = EditorGUILayout.ColorField(new GUIContent("Shape Color 1"), shapeColor1, true, false, false);
            shapeColor2 = EditorGUILayout.ColorField(new GUIContent("Shape Color 2"), shapeColor2, true, false, false);
            shapeColor3 = EditorGUILayout.ColorField(new GUIContent("Shape Color 3"), shapeColor3, true, false, false);
            shapeColor4 = EditorGUILayout.ColorField(new GUIContent("Shape Color 4"), shapeColor4, true, false, false);
            shapeColor5 = EditorGUILayout.ColorField(new GUIContent("Shape Color 5"), shapeColor5, true, false, false);
            shapeColor6 = EditorGUILayout.ColorField(new GUIContent("Shape Color 6"), shapeColor6, true, false, false);
            shapeColor7 = EditorGUILayout.ColorField(new GUIContent("Shape Color 7"), shapeColor7, true, false, false);
            shapeColor8 = EditorGUILayout.ColorField(new GUIContent("Shape Color 8"), shapeColor8, true, false, false);
        }
        UdonVR_GUI.EndButtonFoldout("showShapeColors");
        GUILayout.EndVertical();

        EditorGUILayout.Space();


        if (EditorGUI.EndChangeCheck())
        {
            UdonVR_ShaderGUI.SaveCullBlendSpecular(targetMat, cullBlendSpecularData);
            UdonVR_ShaderGUI.SaveGlossMetalAlpha(targetMat, glossMetalAlphaData);

            targetMat.SetTexture("_MainTex", mainTex);
            targetMat.SetFloat("_GreyscaleMainTex", System.Convert.ToSingle(greyscaleMainTex));
            targetMat.SetColor("_Color", color);
            targetMat.SetFloat("_EmissionBrightness", UdonVR_MathHelpers.Clamp(emissionBrightness, 0, 1));

            targetMat.SetFloat("_ShapeBrightnessMultiplier", shapeBrightnessMultiplier);
            targetMat.SetFloat("_IsFrame", System.Convert.ToSingle(isFrame));
            targetMat.SetFloat("_IsFrameRandom", System.Convert.ToSingle(isFrameRandom));
            targetMat.SetFloat("_RemoveRandomShapes", System.Convert.ToSingle(removeRandomShapes));
            targetMat.SetFloat("_IsTopOnly", System.Convert.ToSingle(isTopOnly));

            targetMat.SetInt("_ColorCount", colorCount);
            targetMat.SetColor("_ShapeColor1", shapeColor1);
            targetMat.SetColor("_ShapeColor2", shapeColor2);
            targetMat.SetColor("_ShapeColor3", shapeColor3);
            targetMat.SetColor("_ShapeColor4", shapeColor4);
            targetMat.SetColor("_ShapeColor5", shapeColor5);
            targetMat.SetColor("_ShapeColor6", shapeColor6);
            targetMat.SetColor("_ShapeColor7", shapeColor7);
            targetMat.SetColor("_ShapeColor8", shapeColor8);
        }
        base.OnGUI(materialEditor, properties);
    }
}
