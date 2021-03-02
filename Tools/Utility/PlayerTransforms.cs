using UnityEngine;
using UdonSharp;
using System;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
using UdonVR.EditorUtility;
#endif

namespace UdonVR.Tools.Utility
{
    [AddComponentMenu("UdonVR/Tools/Player Teleports")]
    /// <summary>
    /// Holds Transforms for scripts
    /// WIP
    /// </summary>
    public class PlayerTransforms : UdonSharpBehaviour 
    {
        public Transform[] transforms;
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR

    [Flags]
    enum EditMode
    {
        Position = 1 << 0,
        Rotation = 1 << 1
    }

    [CustomEditor(typeof(PlayerTransforms))]
    public class PlayerTransformsEditor : Editor
    {
        private SerializedProperty transforms;

        private GUIContent[] teleportSpotsNames;
        //private Transform editTransform;
        private static int editInt;
        private static EditMode editMode = EditMode.Position;
        //private string[] toolbarButtons = new string[] {"Pos","Rot"};
        GUIStyle[] buttons;




        private int[] teleportSpotsInts;
        private bool foldout=true;
        private static Rect transformRect;

        // private string[] notProps = new string[] { "Base", "m_Script", "size", "data" };
        //private PlayerTransforms pTransforms;

        private void OnEnable()
        {
            transforms = serializedObject.FindProperty("transforms");
            editInt = -1;
            //color1 = serializedObject.FindProperty("color1");

            //Debug.Log("Enable");
            buttons = new GUIStyle[4];
            buttons[0] = new GUIStyle(EditorStyles.miniButtonLeft);
            buttons[1] = new GUIStyle(buttons[0]);
            buttons[1].normal = buttons[1].active;
            buttons[2] = new GUIStyle(EditorStyles.miniButtonRight);
            buttons[3] = new GUIStyle(buttons[2]);
            buttons[3].normal = buttons[3].active;

        }

        
        public override void OnInspectorGUI()
        {
            // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            if(buttons == null)
            {
                buttons = new GUIStyle[4];
                buttons[0] = new GUIStyle(EditorStyles.miniButtonLeft);
                buttons[1] = new GUIStyle(buttons[0]);
                buttons[1].normal = buttons[1].active;
                buttons[2] = new GUIStyle(EditorStyles.miniButtonRight);
                buttons[3] = new GUIStyle(buttons[2]);
                buttons[3].normal = buttons[3].active;
            }
            //PlayerTransforms PlayerTransforms = (PlayerTransforms)target;
            //Debug.Log("GUI");
            //Debug.Log($"OnInspectorGUI {editInt} < {transforms.arraySize} + {GetInstanceID()}");
            transformRect = EditorGUILayout.BeginHorizontal();
            //GUI.Box(transformRect,"",EditorStyles.helpBox);
            foldout = EditorGUILayout.Foldout(foldout, "Transforms",true);
            //GUILayout.FlexibleSpace();
            if(editInt != -1)
            {
                if (editMode.HasFlag(EditMode.Position))
                {
                    if (GUILayout.Button("Position", buttons[1]))
                    {
                        editMode &= ~EditMode.Position;
                        //if (editMode == 0)
                        //    editMode = EditMode.Rotation;
                    }
                }
                else
                {
                    if (GUILayout.Button("Position", buttons[0]))
                    {
                        editMode |= EditMode.Position;

                    }
                }
                if (editMode.HasFlag(EditMode.Rotation))
                {
                    if (GUILayout.Button("Rotation", buttons[3]))
                    {
                        editMode &= ~EditMode.Rotation;
                        //if (editMode == 0)
                        //    editMode = EditMode.Position;
                    }
                }
                else
                {
                    if (GUILayout.Button("Rotation", buttons[2]))
                    {
                        editMode |= EditMode.Rotation;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (foldout)
            {

                transforms.arraySize = EditorGUILayout.DelayedIntField("Size", transforms.arraySize);
 
                for (int i = 0; i < transforms.arraySize; i++)
                {
                    var transform = transforms.GetArrayElementAtIndex(i);
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    //EditorGUILayout.PropertyField(transforms.GetArrayElementAtIndex(i));
                    EditorGUILayout.PrefixLabel(transform.displayName);
                    if (GUILayout.Button("Find", buttons[0], GUILayout.ExpandWidth(false)))
                    {
                        //editInt = -1;
                        var t = (Transform)transform.objectReferenceValue;
                        if (t != null)
                        {

                            EditorHelper.LookAt(t);
                            editInt = i;
                            
                        }
                        //Debug.Log(editMode);
                        //SceneView.RepaintAll();
                    }
                    if (editInt == i)
                    {

                        if (GUILayout.Button("Edit", buttons[3], GUILayout.ExpandWidth(false)))
                        {
                            editInt = -1;
                            Debug.Log(editMode);
                            //SceneView.RepaintAll();
                        }
                    }
                    else {
                        if (GUILayout.Button("Edit",buttons[2], GUILayout.ExpandWidth(false))) 
                        {
                            editInt = i;
                            //SceneView.RepaintAll();
                        } 
                    }
                    EditorGUILayout.PropertyField(transform, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
            }
            //EditorGUILayout.PropertyField(transforms,true);

            //EditorGUILayout.PropertyField(color1);
            //transforms. 
           
                
            if (Event.current.type == EventType.DragUpdated)
            {
                if (transformRect.Contains(Event.current.mousePosition))
                {
                    //Debug.Log(Event.current.type+ " Contained " + Event.current.mousePosition);
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Debug.Log("Drag Updated!");
                    Event.current.Use();
                }
                else
                {
                    //Debug.Log(Event.current.type + " Not Contained " + Event.current.mousePosition);
                }
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (transformRect.Contains(Event.current.mousePosition))
                {
                    Debug.Log(Event.current.type + " Contained " + Event.current.mousePosition);
                    Debug.Log("Drag Perform!");
                    Debug.Log(DragAndDrop.objectReferences.Length);
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        GameObject obj = DragAndDrop.objectReferences[i] as GameObject;
                        if (obj != null)
                        {

                            int j = transforms.arraySize;
                            transforms.InsertArrayElementAtIndex(j);
                            transforms.GetArrayElementAtIndex(j).objectReferenceValue = obj.transform;
                            Debug.Log(obj.transform);
                        }
                    }
                    Event.current.Use();
                }
                
            }


            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            
            //float size = 0.5f;
            //Color color1 = new Color(0.14f, 0, 0.5f, 0.9f); //PRTT.color1;


            if (editInt > -1 && editInt < transforms.arraySize)
            {
                //Debug.Log("SceneGUI Edit");
                Transform transform = (Transform)transforms.GetArrayElementAtIndex(editInt).objectReferenceValue;

                if (transform != null)
                {

                    if (editMode.HasFlag(EditMode.Position))
                        EditorHelper.ShowPositionHandles(transform);

                    if (editMode.HasFlag(EditMode.Rotation))
                        EditorHelper.ShowRotationHandles(transform);
                 
                    EditorHelper.ShowTransform(transform);

                }
            }
        }

        private void MakePopup(Transform[] teleportToSpots = null)
        {
            if (teleportToSpots == null)
            {
                teleportSpotsInts = new int[] { -1 };
                teleportSpotsNames = new GUIContent[] { new GUIContent("No Teleport") };
                return;
            }
            teleportSpotsInts = new int[teleportToSpots.Length + 1];
            teleportSpotsNames = new GUIContent[teleportToSpots.Length + 1];
            teleportSpotsInts[0] = -1;
            teleportSpotsNames[0] = new GUIContent("No Teleport");
            for (int i = 0; i < teleportToSpots.Length; i++)
            {
                teleportSpotsInts[i + 1] = i;
                teleportSpotsNames[i + 1] = new GUIContent(teleportToSpots[i].name);
            }
        }
    }

#endif



}
