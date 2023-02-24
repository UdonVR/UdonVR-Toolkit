using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UdonVR.EditorUtility.EditorHelper;
using System.Text;
using System;

using UdonVR.EditorUtility;

public class UdonVRInfo : EditorWindow
{
    private StreamReader VersionFile;

    public string newPath = " ";
    private GUILayoutOption[] space = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16) };
    private GUILayoutOption[] noExpandWidth = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16), GUILayout.ExpandWidth(false) };
    private GUIStyle style;
    public GUIStyle logoStyle;
    private string _Version = "null";

    [MenuItem("UdonVR/About UdonVR", false, 5000)]
    public static void ShowWindow()
    {
        UdonVRInfo window = GetWindow<UdonVRInfo>("About UdonVR");
        window.minSize = new Vector2(385, 250);
    }
    private void GUIWarning(string text)
    {

        EditorGUILayout.LabelField(new GUIContent(text, EditorGUIUtility.FindTexture("d_console.warnicon")), style);

    }
    private void GUIError(string text)
    {
        EditorGUILayout.LabelField(new GUIContent(text, EditorGUIUtility.FindTexture("d_console.erroricon")), style);

    }
    private void InitGuiStyles()
    {
        style = new GUIStyle(EditorStyles.helpBox);
        style.richText = true;
        style.fontSize = 15;

        logoStyle = new GUIStyle("flow node hex 0")
        {
            fontSize = 15,
            richText = true,
            alignment = TextAnchor.MiddleCenter,
            hover = ((GUIStyle)"flow node hex 1").normal,
            active = ((GUIStyle)"flow node hex 1 on").normal
        };

        logoStyle.padding.top = 17;
        logoStyle.normal.textColor = Color.cyan;
    }
    private void OnGUI()
    {
        if (style == null)
            InitGuiStyles();
        if (_Version == "null")
            CheckUpdate();
        EditorGUILayout.LabelField("UdonVR Patreon Tools.\nThese are a collection of tools that are trying to streamline the creation of VRChat worlds.", style);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Version: " + _Version, style);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("If you didnt get these tools through our Patreon please consider supporting us.", style);

        UdonVR_GUI.ShowUdonVRLinks(32, 32, true);
    }
    private void CheckUpdate()
    {
        VersionFile = new StreamReader(GetfileDirectory() + "/Version.txt", Encoding.Default);
        _Version = VersionFile.ReadToEnd();
        VersionFile.Close();
    }
}
