using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using static UdonVR.EditorUtility.EditorHelper;
using System.Text;
using System;

public class SortChild : EditorWindow
{

    public string newPath = " ";
    private GUILayoutOption[] space = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16) };
    private GUILayoutOption[] noExpandWidth = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16), GUILayout.ExpandWidth(false) };
    private GUIStyle style;
    public GUIStyle logoStyle;

    Transform parentTransform;
    

    [MenuItem("UdonVR/Sort Children")]
    public static void ShowWindow()
    {
        SortChild window = GetWindow<SortChild>("Sort Children");
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
    public static string PadNumbers(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
    }
    private void OnGUI()
    {
        if (style == null)
            InitGuiStyles();
        
        EditorGUILayout.LabelField("Sort", style);
        EditorGUILayout.Space();

       parentTransform =  (Transform)EditorGUILayout.ObjectField("Parent", parentTransform, typeof(Transform), true);

        if (GUILayout.Button("Sort"))
        {
            if (parentTransform != null && !UnityEditor.EditorUtility.IsPersistent(parentTransform))
            {
                
                List<Transform> children = new List<Transform>(); 
                    foreach (Transform child in parentTransform)
                {
                    children.Add(child);
                }

                var sorted = children.OrderBy(child => PadNumbers(child.name)).ToList();

                for (int i = 0; i < sorted.Count; i++)
                {
                    sorted[i].SetSiblingIndex(i);
                }
            }
        }
    }

}
