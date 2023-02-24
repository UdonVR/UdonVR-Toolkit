//Copyright (c) 2021 UdonVR LLC
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
using System.Collections.Generic;
using System.Linq;
using UdonVR.EditorUtility;
#endif

public class Whitelist_GameObject_Toggle : UdonSharpBehaviour
{
    public string[] Players;
    [Tooltip("These are OFF by default\nWhen a player's name matches the list, these will TURN ON.")]
    public GameObject[] TargetsDefaultOff;
    [Tooltip("These are ON by default\nWhen a player's name matches the list, these will TURN OFF.")]
    public GameObject[] TargetsDefaultOn;

    private bool isMatched = false;
    private void Start()
    {
        foreach(string _str in Players)
        {
            if (Networking.LocalPlayer.displayName == _str)
            {
                isMatched = true;
            }
        }

        foreach(GameObject _obj in TargetsDefaultOn)
        {
            _obj.SetActive(!isMatched);
        }
        foreach (GameObject _obj in TargetsDefaultOff)
        {
            _obj.SetActive(isMatched);
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(Whitelist_GameObject_Toggle))]
    public class EasyLocks_Button_Editor : Editor
    {
        private SerializedProperty TargetsDefaultOff;
        private SerializedProperty TargetsDefaultOn;
        private Transform baseTransform;

        private void OnEnable()
        {
            TargetsDefaultOff = serializedObject.FindProperty("TargetsDefaultOff");
            TargetsDefaultOn = serializedObject.FindProperty("TargetsDefaultOn");
            baseTransform = ((Whitelist_GameObject_Toggle)target).transform;
        }
        public void OnSceneGUI()
        {
            for (int i = 0; i < TargetsDefaultOff.arraySize; i++)
            {
                GameObject _target = (GameObject)TargetsDefaultOff.GetArrayElementAtIndex(i).objectReferenceValue;
                if (_target != null)
                    EditorHelper.ShowTransform(_target.transform, baseTransform, false);
            }
            for (int i = 0; i < TargetsDefaultOn.arraySize; i++)
            {
                GameObject _target = (GameObject)TargetsDefaultOn.GetArrayElementAtIndex(i).objectReferenceValue;
                if (_target != null)
                    EditorHelper.ShowTransform(_target.transform, baseTransform, false);
            }
        }
    }
#endif
}
