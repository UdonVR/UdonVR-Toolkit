using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using System.Collections.Generic;
using System.Linq;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds

using UnityEditor;
using UdonSharpEditor;
using UdonVR.EditorUtility;

#endif

namespace UdonVR.Takato.PlayerTools
{
    /// <summary>
    ///
    /// </summary>
    [AddComponentMenu("UdonVR/Tools/EasyDoors")]
    public class PlayerInteractTeleport : UdonSharpBehaviour
    {
        [Tooltip("This is the GameObject that the player will teleport to.")]
        public Transform targetLocation;
        public VRC_SceneDescriptor.SpawnOrientation teleportOrientation = VRC_SceneDescriptor.SpawnOrientation.AlignPlayerWithSpawnPoint;
        [Tooltip("If Lerp is true, other players will see the teleporting player ''Move'' to the Target Location instead of instantly teleporting there.")]
        public bool lerpOnRemote = false;
        public int interactType = 1 << 0;
        const int typeINTERACT = 1 << 0;
        const int typeENTER = 1 << 1;
        const int typeEXIT = 1 << 2;
        public override void Interact()
        {
            if(canType(typeINTERACT))
                Teleport();
        }
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (canType(typeENTER))
                Teleport();
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (canType(typeEXIT))
                Teleport();
        }

        private void Teleport()
        {
            Networking.LocalPlayer.TeleportTo(targetLocation.position, targetLocation.rotation, teleportOrientation, lerpOnRemote);
        }
        private bool canType(int type)
        {
            return (type & interactType) == type;
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR

    [CustomEditor(typeof(PlayerInteractTeleport))]
    public class PlayerInteractTeleportEditor : Editor
    {
        private SerializedProperty targetLocation;
        private SerializedProperty teleportOrientation;
        private SerializedProperty lerpOnRemote;
        private SerializedProperty interactType;
        private InteractTypes iTypes;
        private int transformInt = -1;
        private EditorHelper.PopupData transformData;
        private bool hasTransforms = false;
        private static Space space = Space.World;
        private GUIContent[] toolbarbuttons = new GUIContent[]
        { 
            new GUIContent("Global", "Global Position of the Target Location. This is it's absolute position."), 
            new GUIContent("Local","Local Position of the Target Location. This is it's relative position based on it's parent.") 
        };

        enum InteractTypes
        {
            Nothing = 0,
            OnInteract = 1 << 0,
            OnEnter = 1 << 1,
            OnExit = 1 << 2,
            Everything = ~0,
        }


        // private string[] notProps = new string[] { "Base", "m_Script", "size", "data" };
        private Utility.PlayerTransforms pTransforms;

        private void OnEnable()
        {
            targetLocation = serializedObject.FindProperty("targetLocation");
            teleportOrientation = serializedObject.FindProperty("teleportOrientation");
            lerpOnRemote = serializedObject.FindProperty("lerpOnRemote");
            interactType = serializedObject.FindProperty("interactType");

            iTypes = (InteractTypes)interactType.intValue;
            pTransforms = EditorHelper.GetPlayerTransforms();

            if (pTransforms != null)
            {
                hasTransforms = true;
                Transform targetTransform = (Transform)targetLocation.objectReferenceValue;
                if (targetTransform != null)
                {
                    int numLenght = pTransforms.transforms.IsNullOrEmpty() ? 0 : pTransforms.transforms.Length;
                    if (numLenght > 0)
                        transformInt = System.Array.IndexOf(pTransforms.transforms, targetTransform);

                    if (transformInt == -1)
                    {
                        
                        Transform[] newTransforms = new Transform[numLenght + 1];
                        if(numLenght != 0)
                            pTransforms.transforms.CopyTo(newTransforms, 0);
                        newTransforms[numLenght] = targetTransform;
                        pTransforms.transforms = newTransforms;
                        transformInt = numLenght;
                        pTransforms.ApplyProxyModifications();
                    }
                }
                transformData = EditorHelper.MakeTransformPopup(pTransforms.transforms);
            }
        }

        public override void OnInspectorGUI()
        {
            // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            //Debug.Log("GUI");
            Rect rect = EditorGUILayout.GetControlRect(true);
            if (hasTransforms)
            {
                GUIContent lable = EditorGUI.BeginProperty(rect, null, targetLocation);
                EditorGUI.BeginChangeCheck();
                transformInt = EditorGUI.IntPopup(rect, lable, transformInt, transformData.names, transformData.ints);
                EditorGUI.EndProperty();
                if (EditorGUI.EndChangeCheck())
                {
                    if (transformInt < pTransforms.transforms.Length)
                    {
                        if (transformInt < 0)
                            targetLocation.objectReferenceValue = null;
                        else
                            targetLocation.objectReferenceValue = pTransforms.transforms[transformInt];
                    }
                    transformData = EditorHelper.MakeTransformPopup(pTransforms.transforms);
                }
            }
            else
            {
                EditorGUI.PropertyField(rect, targetLocation);
            }
            if ((Transform)targetLocation.objectReferenceValue != null)
            {
                space = (Space)GUILayout.Toolbar((int)space, toolbarbuttons);
                EditorHelper.TranformPosRotFields((Transform)targetLocation.objectReferenceValue, space);
            }

            EditorGUILayout.PropertyField(teleportOrientation);
            EditorGUI.BeginChangeCheck();
            iTypes = (InteractTypes)EditorGUILayout.EnumFlagsField("Interact Type", iTypes);
            if (EditorGUI.EndChangeCheck())
            {
                interactType.intValue = (int)iTypes;
            }
            EditorGUILayout.PropertyField(lerpOnRemote);

            if (hasTransforms)
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log(Event.current.type+ " Contained " + Event.current.mousePosition);
                        GameObject obj = DragAndDrop.objectReferences[0] as GameObject;
                        if (obj != null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            //Debug.Log("Drag Updated!");
                            Event.current.Use();
                        }
                    }
                    else
                    {
                        //Debug.Log(Event.current.type + " Not Contained " + Event.current.mousePosition);
                    }
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        Debug.Log(Event.current.type + " Contained " + Event.current.mousePosition);
                        //Debug.Log("Drag Perform!");
                        Debug.Log(DragAndDrop.objectReferences.Length);
                        List<Transform> transforms = pTransforms.transforms.ToList();
                        bool added = false;
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            GameObject obj = DragAndDrop.objectReferences[i] as GameObject;
                            if (obj != null)
                            {
                                if (!transforms.Contains(obj.transform))
                                {
                                    transforms.Add(obj.transform);
                                    added = true;
                                }
                            }
                            else
                                break;
                        }
                        if (added)
                        {
                            pTransforms.transforms = transforms.ToArray();
                            pTransforms.ApplyProxyModifications();
                            transformData = EditorHelper.MakeTransformPopup(pTransforms.transforms);
                            transformInt = pTransforms.transforms.Length - 1;
                        }
                        Event.current.Use();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            Transform targetTransform = (Transform)targetLocation.objectReferenceValue;
            if (targetTransform != null)
            {
                Transform baseTransform = ((PlayerInteractTeleport)target).transform;
                EditorHelper.ShowTransform(targetTransform, baseTransform);

                EditorHelper.ShowPositionHandles(targetTransform,space);
                EditorHelper.ShowRotationHandles(targetTransform,space);
            }
        }
    }

#endif
}