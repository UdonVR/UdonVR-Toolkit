using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System;

using UdonVR.EditorUtility;

public class Hamster9090901_TestingWindow : EditorWindow
{
    private static Hamster9090901_TestingWindow window;

    private bool show_rgbToColor = false;
    private Color rgbToColor = Color.white;

    private bool testingButton = false;
    private bool foldoutState = false;

    private Vector2 scrollview = Vector2.zero;

    [MenuItem("UdonVR/Dev/Hamster9090901_TestingWindow")]
    private static void Init()
    {
        window = EditorWindow.GetWindow<Hamster9090901_TestingWindow>();
        //window.maxSize = new Vector2(700, 850);
        window.minSize = new Vector2(350, 300);
        //window.titleContent = windowTitleContent;
        window.Show();
    }

    private void OnGUI()
    {
        if (window == null) window = EditorWindow.GetWindow<Hamster9090901_TestingWindow>(); // get window if it was null

        #region Color Convert
        if (UdonVR_GUI.BeginButtonFoldout(new GUIContent("Color Convert"), ref show_rgbToColor, "show_rgbToColor", EditorStyles.helpBox))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            UdonVR_GUI.Header(new GUIContent("Show Input Color with a (0 - 1) range."));
            rgbToColor = EditorGUILayout.ColorField(GUIContent.none, rgbToColor);
            EditorGUILayout.SelectableLabel(String.Format("({0},{1},{2},{3})", rgbToColor.r, rgbToColor.g, rgbToColor.b, rgbToColor.a), UdonVR_Style.SetTextSettings(GUIStyle.none, UdonVR_Predefined.Color.Style_DefaultTextColor, TextAnchor.MiddleCenter));
            EditorGUILayout.EndVertical();
        }
        UdonVR_GUI.EndButtonFoldout("show_rgbToColor");
        #endregion

        #region FieldArea
        MeshRenderer testing = null;
        UdonVR_GUI.FieldArea(
            new GUIContent[]
            {
                new GUIContent("bool"),
                new GUIContent("string"),
                new GUIContent("int"),
                new GUIContent("float"),
                new GUIContent("double"),
                new GUIContent("vector2"),
                new GUIContent("vector3"),
                new GUIContent("vector4"),
                new GUIContent("color"),
                new GUIContent("object"),
            },
            new UdonVR_GUI.FieldAreaValues[]
            {
                UdonVR_GUI.FieldAreaValues.SetValueToggle(testingButton, true),
                UdonVR_GUI.FieldAreaValues.SetValue("hi"),
                UdonVR_GUI.FieldAreaValues.SetValueSlider(5, true, 0, 100),
                UdonVR_GUI.FieldAreaValues.SetValue(7.5f, true),
                UdonVR_GUI.FieldAreaValues.SetValueSlider(0.6, true, 0, 1),
                UdonVR_GUI.FieldAreaValues.SetValue(new Vector2(8, 3)),
                UdonVR_GUI.FieldAreaValues.SetValue(new Vector3(8, 3, 7)),
                UdonVR_GUI.FieldAreaValues.SetValue(new Vector4(8, 3, 7, 2)),
                UdonVR_GUI.FieldAreaValues.SetValueColor(Color.cyan),
                UdonVR_GUI.FieldAreaValues.SetValue(testing)
            },
            new Action<UdonVR_GUI.FieldAreaValues>[]
            {
                (areaValues) =>
                {
                    testingButton = areaValues.boolValue.Value;
                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                },
                (areaValues) =>
                {

                }
            }
        );
        #endregion

        #region Foldout
        foldoutState = UdonVR_GUI.BeginButtonFoldout(new GUIContent("Foldout"), foldoutState, "foldout");
        if (foldoutState)
        {
            GUILayout.Button("HI");
        }
        UdonVR_GUI.EndButtonFoldout("foldout");
        #endregion

        scrollview = GUI.BeginScrollView(new Rect(100, 100, 800, 400), scrollview, new Rect(0, 0, 5000, 5000));
        UdonVR_GUI_DragAndDrop.Controller.Update();
        GUI.EndScrollView();
    }
}