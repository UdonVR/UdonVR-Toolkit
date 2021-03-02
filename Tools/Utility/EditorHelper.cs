#if UNITY_EDITOR
using UnityEngine;
using UdonVR.Tools.Utility;
using UnityEditor.SceneManagement;
using UdonSharpEditor;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace UdonVR.EditorUtility
{
    public static class EditorHelper 
    {
        public struct PopupData
        {
            public GUIContent[] names;
            public int[] ints;
        }

        public static PopupData MakeTransformPopup(Transform[] transforms = null)
        { 
            PopupData data = new PopupData();
            List<GUIContent> transformNames;
            List<int> transformInts;

            if (transforms.IsNullOrEmpty())
            {
                data.ints = new int[] { -1 };
                data.names = new GUIContent[] { new GUIContent("No Teleport") };
                return data;
            }
            transformInts = new List<int>();
            transformNames = new List<GUIContent>();
            transformInts.Add(-1);
            transformNames.Add(new GUIContent("No Teleport"));
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null)
                {
                    transformInts.Add(i);
                    transformNames.Add(new GUIContent(transforms[i].name));
                }
            }
            data.ints = transformInts.ToArray();
            data.names = transformNames.ToArray();
            return data;
        }

        public static PlayerTransforms GetPlayerTransforms()
        {
            GameObject[] rootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var go in rootGameObjects)
            {
                //Debug.Log(go);

                try
                {
                    var temp = go.GetUdonSharpComponentsInChildren<PlayerTransforms>();

                    if (temp != null && temp.Length > 0)
                    {
                        return temp[0];
                    }
                }
                catch (System.Exception)
                {
                    //Debug.LogError(go, go);

                    //Debug.LogException(e, this);
                }
            }
            return null;
        }

        public static void ShowPositionHandles(Transform transform, Space space = Space.World)
        {
            Quaternion baseRot = (space == Space.World) ? Quaternion.identity : transform.rotation * Quaternion.LookRotation(Vector3.forward);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(transform.position, baseRot);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Change Transform Position");
                transform.position = newTargetPosition;
            }
        }

        public static void ShowRotationHandles(Transform transform, Space space = Space.World)
        {
            Quaternion baseRot = (space == Space.World) ? transform.rotation : transform.localRotation;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            EditorGUI.BeginChangeCheck();
            Quaternion newRot = Handles.RotationHandle(baseRot, transform.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Change Transform Rotation");
                transform.rotation = newRot;
            }
        }
        
        public static void ShowTransform(Transform transform, Transform drawLineFrom = null, bool _Handel = true)
        {
            if (drawLineFrom != null)
                ShowTransform(transform, true, drawLineFrom.position, _Handel);
            else
                ShowTransform(transform,false, new Vector3(), _Handel);
        }

        public static void ShowTransform(Transform transform, Vector3 drawLineFrom, bool _Handel = true)
        {
            ShowTransform(transform, true, drawLineFrom, _Handel);               
        }

        private static void ShowTransform(Transform transform,bool drawLine = false, Vector3 drawLineFrom = new Vector3(),bool _Handel = true)
        {
            float size = 0.5f;
            Color color = new Color(0.14f, 0, 0.5f, 0.9f);
            if (drawLine)
            {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Color lineColor = new Color(0.5f, 0.5f, 0.5f, 1);
                Handles.color = lineColor;
                Handles.DrawLine(drawLineFrom, transform.position);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
                lineColor.a = 0.7f;
                Handles.color = lineColor;
                Handles.DrawDottedLine(drawLineFrom, transform.position, size * 10);
            }
            if (_Handel == true)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Handles.lighting = false;
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                    //Handles.color = Handles.zAxisColor;
                    //Handles.color = new Color(1, 0, 1, 1);
                    Handles.color = color;
                    Handles.ArrowHandleCap(
                        0,
                        transform.position + transform.forward * 0.1f,
                        transform.rotation * Quaternion.LookRotation(Vector3.forward),
                        size * 0.5f,
                        EventType.Repaint
                    );
                    Handles.SphereHandleCap(
                        0,
                        transform.position,
                        transform.rotation * Quaternion.LookRotation(Vector3.forward),
                        size * 0.5f,
                        EventType.Repaint
                    );
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
                    Handles.color = new Color(1, 0, 1, 0.2f);
                    color.a *= 0.5f;
                    Handles.color = color;

                    Handles.ArrowHandleCap(
                        0,
                        transform.position + transform.forward * 0.1f,
                        transform.rotation * Quaternion.LookRotation(Vector3.forward),
                        size * 0.5f,
                        EventType.Repaint
                    );
                    Handles.SphereHandleCap(
                        0,
                        transform.position,
                        transform.rotation * Quaternion.LookRotation(Vector3.forward),
                        size * 0.5f,
                        EventType.Repaint
                    );
                }
            }
        }

        public static void TranformPosRotFields(Transform transform, Space space)
        {
           
            if (space == Space.World)
            {
                EditorGUI.BeginChangeCheck();
               Vector3 newPos = EditorGUILayout.Vector3Field("Target Position", transform.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Change Target Position");
                    transform.position = newPos;
                }
                EditorGUI.BeginChangeCheck();
                Vector3 newRot = EditorGUILayout.Vector3Field("Target Rotation", WrapAngles(transform.eulerAngles));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Change Target Rotation");
                    transform.eulerAngles = UnwrapAngles(newRot);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = EditorGUILayout.Vector3Field("Target Position", transform.localPosition);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Change Target Position");
                    transform.localPosition = newPos;
                }
                Vector3 newRot = TransformUtils.GetInspectorRotation(transform);
                EditorGUI.BeginChangeCheck();
                newRot = EditorGUILayout.Vector3Field("Target Rotation", newRot);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Change Target Rotation");
                    TransformUtils.SetInspectorRotation(transform, newRot);
                }
            }
        }

        public static void LookAt(Transform transform, float newSize = 1.5f)
        {
            LookAt(transform.position, newSize);
        }

        public static void LookAt(Vector3 position, float newSize = 1.5f)
        {
            SceneView.lastActiveSceneView.LookAt(position);
            SceneView.lastActiveSceneView.size = newSize;
        }

        public static void LookAt(Vector3 position, Quaternion rotation, float newSize = 1.5f)
        {
            SceneView.lastActiveSceneView.LookAt(position,rotation,newSize);
        }
        /// <summary>Editor Only Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static string GetfileDirectory([System.Runtime.CompilerServices.CallerFilePath] string filepath = "")
        {
            return System.IO.Path.GetDirectoryName(filepath);
        }

        private static Vector3 WrapAngles(Vector3 angles)
        {

            angles.x = WrapAngle(angles.x);
            angles.y = WrapAngle(angles.y);
            angles.z = WrapAngle(angles.z);
            return angles;
        }
        private static Vector3 UnwrapAngles(Vector3 angles)
        {

            angles.x = UnwrapAngle(angles.x);
            angles.y = UnwrapAngle(angles.y);
            angles.z = UnwrapAngle(angles.z);
            return angles;
        }

        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private static float UnwrapAngle(float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }
    }
    

}
#endif
