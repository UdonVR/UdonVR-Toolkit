using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Editor;

namespace UdonVR.Tools.Utility
{
    public class UdonVRProjectMenu
    {
        // Start is called before the first frame update
        [MenuItem("Assets/Create/VRChat Scene", false, 201)]
        public static void CreateVRCScene()
        {
            Debug.Log("[UdonVR] Creating New VRChat Scene");
            string folderPath = "Assets/";
            if (Selection.activeObject != null)
            {
                folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Selection.activeObject.GetType() != typeof(UnityEditor.DefaultAsset))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
            }
            else if (Selection.assetGUIDs.Length > 0)
            {
                folderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            }
            folderPath = folderPath.Replace('\\', '/');


            string filePath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/VRChatScene.unity");
            Debug.Log("[UdonVR] Creating: " + folderPath);
            string chosenFilePath = UnityEditor.EditorUtility.SaveFilePanelInProject("Save New Scene", Path.GetFileName(filePath), "unity", "Save New Scene", folderPath);
            AssetDatabase.CopyAsset("Assets/_UdonVR/Tools/Utility/Scenes/VRChatScene.unity", chosenFilePath);
            AssetDatabase.Refresh();
        }
    }
}