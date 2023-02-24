using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public static class UdonVR_Handles
    {
        // created by Hamster9090901

        /// <summary>
        /// Draws a sphere using handles. Must be in OnSceneGUI()
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(position, new Vector3(1, 0, 0), radius); // x
            Handles.DrawWireDisc(position, new Vector3(0, 1, 0), radius); // y
            Handles.DrawWireDisc(position, new Vector3(0, 0, 1), radius); // z
        }
        /// <summary>
        /// Draws a sphere using handles. Must be in OnSceneGUI()
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        public static void DrawWireSphere(Vector3 position, float radius)
        {
            DrawWireSphere(position, radius, Color.white);
        }
    }
}