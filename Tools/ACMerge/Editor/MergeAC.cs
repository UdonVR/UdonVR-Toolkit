using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using UdonVR.EditorUtility;

namespace UdonVR.Tools
{
    //using System;
    public class MergeAC : EditorWindow
    {
        public AnimatorController baseController;
        public AnimatorController addController;
        public bool saveAsNewController = true;

        public string newPath = " ";
        private GUILayoutOption[] space = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16) };
        private GUILayoutOption[] noExpandWidth = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.MinHeight(16), GUILayout.MaxHeight(16), GUILayout.ExpandWidth(false) };
        private GUIStyle style;
        public GUIStyle logoStyle;
        private bool[] canAddLayer;
        private string[] layerNames;
        private bool ChangeCheck;
        private bool showLayer = true;
        private string currLayer;
        private int currLayerNum;
        private int canLayerNum;
        private AnimatorController tempAddController;
        public AnimatorController tempController;

        private bool disableMerge;
        private string warning = "None";
        private Vector2 scrollPos;

        [MenuItem("UdonVR/Merge Controllers")]
        public static void ShowWindow()
        {
            MergeAC window = GetWindow<MergeAC>("Merge Controllers");
            window.minSize = new Vector2(385, 250);
        }

        private void GUIWarning(string text)
        {
            EditorGUILayout.LabelField(new GUIContent(text, EditorGUIUtility.FindTexture("d_console.warnicon")), style);
        }

        private void GUIError(string text)
        {
            disableMerge = true;
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

            EditorGUI.BeginChangeCheck();
            baseController = EditorGUILayout.ObjectField("BaseController", baseController, typeof(AnimatorController), false) as AnimatorController;
            bool ChangeBaseCheck = EditorGUI.EndChangeCheck();
            if (baseController != null)
            {
                EditorGUI.BeginChangeCheck();
                addController = EditorGUILayout.ObjectField("AddController", addController, typeof(AnimatorController), false) as AnimatorController;
                ChangeCheck = EditorGUI.EndChangeCheck();
            }
            else
            {
                EditorGUILayout.BeginVertical(space);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
            if (addController != null)
            {
                if (canAddLayer == null || ChangeCheck)
                {
                    canAddLayer = Enumerable.Repeat(true, addController.layers.Length).ToArray();
                    layerNames = new string[addController.layers.Length];
                }

                EditorGUILayout.BeginHorizontal();
                Rect foldRect = EditorGUILayout.GetControlRect(true);
                if (GUILayout.Button("All", noExpandWidth))
                {
                    canAddLayer = Enumerable.Repeat(true, addController.layers.Length).ToArray();
                }
                if (GUILayout.Button("None", noExpandWidth))
                {
                    canAddLayer = Enumerable.Repeat(false, addController.layers.Length).ToArray();
                }
                EditorGUILayout.EndHorizontal();
                showLayer = EditorGUI.Foldout(foldRect, showLayer, "Layers", true);
                if (showLayer)
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));
                    for (int i = 0; i < addController.layers.Length; i++)
                    {
                        string addLayerName = addController.layers[i].name;

                        if (layerNames[i] == null)
                        {
                            layerNames[i] = baseController.MakeUniqueLayerName(addLayerName);
                        }
                        EditorGUILayout.BeginHorizontal();

                        canAddLayer[i] = EditorGUILayout.ToggleLeft(addLayerName, canAddLayer[i], noExpandWidth);

                        layerNames[i] = EditorGUILayout.TextField(layerNames[i]);

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            else
            {
                canAddLayer = null;
            }
            EditorGUI.BeginDisabledGroup(baseController == null);
            Rect rect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            saveAsNewController = EditorGUILayout.Toggle("Save As New Controller", saveAsNewController, noExpandWidth);
            if (saveAsNewController)
            {
                newPath = EditorGUILayout.TextField(newPath);

                if (GUILayout.Button(EditorGUIUtility.IconContent("Folder Icon"), noExpandWidth))
                {
                    string tempNewPath = UnityEditor.EditorUtility.SaveFilePanelInProject("Save New BaseController", "New" + baseController.name + ".controller", "controller", "Please enter a file name to save the New Controller to", Path.GetDirectoryName(AssetDatabase.GetAssetPath(baseController)));
                    if (tempNewPath != "")
                        newPath = tempNewPath;
                }
            }
            bool pathChangeCheck = EditorGUI.EndChangeCheck();
            if (ChangeBaseCheck || baseController != null && newPath.Trim() == "" && !EditorGUIUtility.editingTextField)
            {
                //Debug.Log("makePath!");
                newPath = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(baseController)) + "/New " + baseController.name + ".controller");
                pathChangeCheck = true;
            }
            if (pathChangeCheck)
            {
                if (!saveAsNewController)
                {
                    disableMerge = false;
                    warning = "SaveToBase";
                }
                else
                {
                    if (newPath.StartsWith("Assets/", true, null))
                    {
                        if (newPath.EndsWith(".controller", true, null))
                        {
                            string mvcheck = AssetDatabase.ValidateMoveAsset(AssetDatabase.GetAssetPath(baseController), newPath);
                            if (mvcheck == "")
                            {
                                disableMerge = false;
                                warning = "None";
                            }
                            else if (mvcheck.EndsWith("Destination path name does already exist"))
                            {
                                disableMerge = false;
                                warning = "Overwrite";
                            }
                            else if (mvcheck.StartsWith("Trying to move asset as a sub directory of a directory that does not exist"))
                            {
                                disableMerge = true;
                                warning = "NoFolder";
                            }
                            else if (mvcheck.StartsWith("Trying to move asset to location it came from"))
                            {
                                warning = "SaveOverBase";
                                disableMerge = true;
                            }
                        }
                        else
                        {
                            disableMerge = true;
                            warning = "Extension";
                        }
                    }
                    else
                    {
                        disableMerge = true;
                        warning = "AssetsFolder";
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(disableMerge || baseController == null || addController == null);
            if (GUILayout.Button("Merge!"))
            {
                Merge();
            }
            EditorGUI.EndDisabledGroup();
            if (baseController == null)
                EditorGUILayout.LabelField("Insert a Base <b>AnimatorController</b> to add to.", style);
            else if (addController == null)
                EditorGUILayout.LabelField("Now add an <b>AnimatorController</b> to put on the Base", style);
            else
                EditorGUILayout.LabelField("Select Layers you want and click <b>Merge!</b>", style);

            switch (warning)
            {
                case "AssetsFolder":
                    GUIError("Can not Save Controller outside of Assets Folder!");
                    break;

                case "Extension":
                    GUIError("Save file extension is not \".controller\"!");
                    break;

                case "NoFolder":
                    GUIError("Can not save to this location, Folder does not exist!");
                    break;

                case "Overwrite":
                    GUIWarning("Overwrite Warning! This will <b>delete</b> the controller at this location and save a new controller at the same location. This will unlink the controller from every thing!");
                    break;

                case "SaveOverBase":
                    GUIError("Overwrite Error! You can not Save a new controller over the Base Controller, Uncheck <b>Save As New Controller</b> if you want to save to the Base Controller.");
                    break;

                case "SaveToBase":
                    GUIWarning("This will save to the Base Controller.");
                    break;

                case "None":

                    break;

                default:
                    GUIError("Unknown Error");
                    break;
            }

            UdonVR_GUI.ShowUdonVRLinks(32, 32, true);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void Merge()
        {
            try
            {
                DoMerge();
            }
            finally
            {
            }
        }

        public void DoMerge()
        {
            string path = AssetDatabase.GetAssetPath(baseController);
            string pathAdd = AssetDatabase.GetAssetPath(addController);
            string tempPath = "Assets/TempController.controller";
            string tempPathAdd = "Assets/TempAddController.controller";

            canLayerNum = canAddLayer.Count(b => b == true);

            if (saveAsNewController)
                AssetDatabase.CopyAsset(path, tempPath);
            AssetDatabase.CopyAsset(pathAdd, tempPathAdd);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (saveAsNewController)
                tempController = AssetDatabase.LoadAssetAtPath<AnimatorController>(tempPath);
            else
                tempController = baseController;

            tempAddController = AssetDatabase.LoadAssetAtPath<AnimatorController>(tempPathAdd);
            if (tempController == null)
            {
                Debug.LogError("TempController not found!");
                return;
            }

            Debug.Log("Merging Controllers");
            AnimatorControllerParameter[] baseParameters = baseController.parameters;
            AnimatorControllerParameter[] addParameters = tempAddController.parameters;
            ParameterComparer parameterComparer = new ParameterComparer();
            if (addParameters.Length > 0)
            {
                //Debug.Log("Merging Stage: Parameters");

                for (int iPar = 0; iPar < addParameters.Length; iPar++)
                {
                    AnimatorControllerParameter addPar = addParameters[iPar];
                    if (!baseParameters.Contains(addPar, parameterComparer))
                    {
                        tempController.AddParameter(addPar);
                    }
                }
            }

            AnimatorControllerLayer[] addLayers = tempAddController.layers;
            if (addLayers.Length > 0)
            {
                //Debug.Log("Merging Stage: Layers");
                currLayerNum = 0;

                List<AnimatorControllerLayer> baseLayers = baseController.layers.ToList();
                for (int i = 0; i < addLayers.Length; i++)
                {
                    if (canAddLayer[i])
                    {
                        currLayer = layerNames[i];
                        currLayerNum++;

                        float p = (float)i / addLayers.Length;
                        UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers Build Layer: " + currLayer + " " + p, "Building Layer: " + layerNames[i], p);
                        AnimatorControllerLayer addLayer = addLayers[i];
                        if (i == 0)
                            addLayer.defaultWeight = 1;

                        addLayer.name = baseController.MakeUniqueLayerName(layerNames[i]);
                        AnimatorStateMachine sm = addLayer.stateMachine;
                        MoveStateMachine(sm, tempController);
                        baseLayers.Add(addLayer);
                    }
                }
                tempController.layers = baseLayers.ToArray();
            }
            UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers", "Done", 1);
            AssetDatabase.SaveAssets();
            addController = null;
            if (saveAsNewController)
            {
                AssetDatabase.CopyAsset(tempPath, newPath);
                AssetDatabase.DeleteAsset(tempPath);
            }
            AssetDatabase.DeleteAsset(tempPathAdd);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityEditor.EditorUtility.ClearProgressBar();
        }

        private string ln;

        private void MoveStateMachine(AnimatorStateMachine sm, AnimatorController tempController)
        {
            DoMove(sm, tempController);

            ln = " " + currLayerNum + "/" + canLayerNum;

            if (sm.states.Length != 0)
            {
                int sl = sm.states.Length;

                for (int i = 0; i < sl; i++)
                {
                    var s = sm.states[i];

                    float p = (i + 1) / (float)sl;

                    UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers Build Layer: " + currLayer + ln + " > States " + p, "Building State: " + s.state.name, p);
                    DoMove(s.state, tempController);

                    if (s.state.motion != null)
                    {
                        if (s.state.motion.GetType() == typeof(BlendTree))
                        {
                            BlendTree blendTree = (BlendTree)s.state.motion;
                            MoveBlendTree(blendTree, tempController);
                        }
                    }

                    //Debug.Log("transition: " + s.state.name + " > "+ s.state.transitions.Length);
                    if (s.state.transitions.Length != 0)
                    {
                        int l = s.state.transitions.Length;
                        //Debug.Log("transition: "+ l);
                        for (int iTrans = 0; iTrans < s.state.transitions.Length; iTrans++)
                        {
                            var t = s.state.transitions[iTrans];
                            //Debug.Log("Building State: " + s.state.name + " > transition: " + iTrans + "/" + l);
                            UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers Build Layer: " + currLayer + ln + " > States " + p, "Building State: " + s.state.name + " > transition: " + iTrans + "/" + l, p);
                            DoMove(t, tempController);
                        }
                    }
                }
            }

            //Debug.Log("transition: AnyState > " + sm.anyStateTransitions.Length);
            if (sm.anyStateTransitions.Length != 0)
            {
                MoveTransitions(sm.anyStateTransitions, tempController, "AnyState");
            }

            //Debug.Log("transition: Entry > " + sm.entryTransitions.Length);
            if (sm.entryTransitions.Length != 0)
            {
                MoveTransitions(sm.entryTransitions, tempController, "Entry");
            }

            if (sm.stateMachines.Length != 0)
            {
                foreach (var csm in sm.stateMachines)
                {
                    AnimatorTransition[] animatorTransitions = sm.GetStateMachineTransitions(csm.stateMachine);
                    //Debug.Log("transition stateMachine: "+csm.stateMachine.name+" > " + animatorTransitions.Length);
                    if (animatorTransitions.Length != 0)
                        MoveTransitions(animatorTransitions, tempController, csm.stateMachine.name);
                    MoveStateMachine(csm.stateMachine, tempController);
                }
            }
        }

        private void DoMove(Object assObj, AnimatorController tempController)
        {
            if (AssetDatabase.GetAssetPath(assObj) == AssetDatabase.GetAssetPath(tempAddController))
            {
                AssetDatabase.RemoveObjectFromAsset(assObj);
                AssetDatabase.SaveAssets();

                AssetDatabase.AddObjectToAsset(assObj, tempController);
                assObj.hideFlags = HideFlags.HideInHierarchy;
                //Debug.Log("Adding");
            }
        }

        private void MoveTransitions(AnimatorStateTransition[] transitions, AnimatorController tempController, string stateName)
        {
            //Debug.Log("transition: "+stateName+" > " + transitions.Length);
            if (transitions.Length != 0)
            {
                int l = transitions.Length;

                for (int iTrans = 0; iTrans < l; iTrans++)
                {
                    float p = (iTrans + 1) / (float)l;
                    var t = transitions[iTrans];
                    //Debug.Log("Building State: " + stateName + " > transition: " + iTrans + "/" + l);
                    UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers Build Layer: " + currLayer + ln + " > States ", "Building State: " + stateName + " > transition: " + iTrans + "/" + l, p);
                    DoMove(t, tempController);
                }
            }
        }

        private void MoveTransitions(AnimatorTransition[] transitions, AnimatorController tempController, string stateName)
        {
            //Debug.Log("transition: " + stateName + " > " + transitions.Length);
            if (transitions.Length != 0)
            {
                int l = transitions.Length;

                for (int iTrans = 0; iTrans < l; iTrans++)
                {
                    float p = (iTrans + 1) / (float)l;
                    var t = transitions[iTrans];
                    //Debug.Log("Building State: " + stateName + " > transition: " + iTrans + "/" + l);
                    UnityEditor.EditorUtility.DisplayProgressBar("Merging Stage: Layers Build Layer: " + currLayer + ln + " > States ", "Building State: " + stateName + " > transition: " + iTrans + "/" + l, p);
                    DoMove(t, tempController);
                }
            }
        }

        private void MoveBlendTree(BlendTree bT, AnimatorController tempController)
        {
            DoMove(bT, tempController);

            if (bT.children.Length != 0)
            {
                foreach (var m in bT.children)
                {
                    if (m.motion != null)
                    {
                        if (m.motion.GetType() == typeof(BlendTree))
                        {
                            MoveBlendTree((BlendTree)m.motion, tempController);
                        }
                    }
                }
            }
        }
    }

    internal class ParameterComparer : IEqualityComparer<AnimatorControllerParameter>
    {
        // AnimatorControllerParameters are equal if their names are equal.
        public bool Equals(AnimatorControllerParameter x, AnimatorControllerParameter y)
        {
            //Check whether the compared objects reference the same data.
            if (UnityEngine.Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (UnityEngine.Object.ReferenceEquals(x, null) || UnityEngine.Object.ReferenceEquals(y, null))
                return false;

            //Check whether the properties are equal.
            return x.name == y.name;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(AnimatorControllerParameter parameter)
        {
            //Check whether the object is null
            if (UnityEngine.Object.ReferenceEquals(parameter, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashAnimatorControllerParameterName = parameter.nameHash;

            return hashAnimatorControllerParameterName;
        }
    }
}