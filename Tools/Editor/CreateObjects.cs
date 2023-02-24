using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UdonVR.EditorUtility;

namespace UdonVR.Tools.Utility{
    public class CreateObjects
    {
        //49
        private const int Menu = 49;
        private static void CreateUdonVRPrefab(string filename, MenuCommand _cmd, bool _unPack = false)
        {
            //Debug.Log("[UdonVR] Trying to load Prefab from file [Assets/_UdonVR/Tools/Utility/Prefabs/" + filename + "]");
            var loadedObject = AssetDatabase.LoadAssetAtPath("Assets/_UdonVR/Tools/Utility/Prefabs/" + filename,typeof(UnityEngine.Object));
            if (loadedObject == null)
            {
                Debug.LogError("[UdonVR] Failed to find File, did you move the _UdonVR folder? File[" + filename + "]");
                return;
            }
            CreateObj(loadedObject, _cmd, _unPack);
        }
        private static void CreatePrefabFromFile(string filename, MenuCommand _cmd, bool _unPack = false)
        {
            //Debug.Log("[UdonVR] Trying to load Prefab from file [" + filename + "]");
            var loadedObject = AssetDatabase.LoadAssetAtPath(filename, typeof(UnityEngine.Object));
            if (loadedObject == null)
            {
                Debug.LogError("[UdonVR] Failed to find File at ["+filename+"]?");
                return;
            }
            CreateObj(loadedObject, _cmd, _unPack);
        }

        private static void CreateObj(UnityEngine.Object _loadedObject, MenuCommand _cmd, bool _unPack)
        {
            GameObject _obj = (GameObject)PrefabUtility.InstantiatePrefab(_loadedObject);
            GameObject _target = (_cmd.context as GameObject);
            Undo.RegisterCreatedObjectUndo(_obj, "[UdonVR] Created Prefab");
            if (_target != null)
            {
                _obj.transform.SetParent(_target.transform);
                _obj.transform.SetPositionAndRotation(_target.transform.position, _target.transform.rotation);
                _obj.layer = _target.layer;
            }
            _obj.transform.SetAsLastSibling();
            Selection.activeGameObject = _obj;
            if (_unPack)
            {
                PrefabUtility.UnpackPrefabInstance(_obj.gameObject,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            }
        }
        #region VRC
        [MenuItem("GameObject/UdonVR/VRC/SceneDescriptor", false, Menu)]
        static void CreateSceneDescriptor(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Scene Descriptor");
            CreateUdonVRPrefab("UdonVR_SceneDescriptor.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/VRC/AvatarPedestal", false, Menu)]
        static void CreateAvatarPedestal(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Avatar Pedestal");
            CreatePrefabFromFile("Assets/VRChat Examples/Prefabs/AvatarPedestal.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/VRC/Chair", false, Menu)]
        static void CreateChair(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating VRCChair3");
            CreatePrefabFromFile("Assets/VRChat Examples/Prefabs/VRCChair/VRCChair3.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/VRC/Mirror", false, Menu)]
        static void CreateMirror(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating VRCMirror");
            CreatePrefabFromFile("Assets/VRChat Examples/Prefabs/VRCMirror.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/VRC/Mirror - No Collider", false, Menu)]
        static void CreateMirrorNoCollider(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating VRCMirror - No Collider");
            CreateUdonVRPrefab("VRCMirror_noCollider.prefab", _cmd);
        }

        #endregion
        #region Meshes
        [MenuItem("GameObject/UdonVR/3D Object/Quad (box collider)", false, Menu)]
        static void CreateQuad(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Quad (box collider)");
            CreateUdonVRPrefab("Quad (box collider).prefab", _cmd, true);
        }
        [MenuItem("GameObject/3D Object/Quad (box collider)", false, 6)]
        static void CreateQuad2(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Quad (box collider)");
            CreateUdonVRPrefab("Quad (box collider).prefab", _cmd, true);
        }
        #endregion
        #region EasyDoors
        [MenuItem("GameObject/UdonVR/EasyDoors/PlayerTeleports", false, Menu)]//
        static void CreateDoor_PlayerTeleports(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating PlayerTeleports");

            PlayerTransforms _transforms = EditorHelper.GetPlayerTransforms();
            if (_transforms != null)
            {
                Debug.LogWarning("[UdonVR] PlayerTeleports Already Exists");
                Selection.activeObject = _transforms.gameObject;
            } else
            {
                CreatePrefabFromFile("Assets/_UdonVR/Tools/EasyDoors/Prefabs/PlayerTeleports.prefab", _cmd);
            }
        }
        [MenuItem("GameObject/UdonVR/EasyDoors/Spawn", false, Menu)]//
        static void CreateDoor_Interact_Spawn(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Door With Spawn");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/EasyDoors/Prefabs/Door+Spawn.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/EasyDoors/NoSpawn", false, Menu)]//
        static void CreateDoor_Interact(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Door");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/EasyDoors/Prefabs/Door.prefab", _cmd);
        }
        #endregion

        #region Whitelists
        [MenuItem("GameObject/UdonVR/Whitelist", false, Menu)]
        static void CreateWhitelist(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Whitelist");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/Whitelist/Toggle/Whitelist - Toggles.prefab", _cmd);
        }
        #endregion

        #region PlayerChimes
        [MenuItem("GameObject/UdonVR/PlayerChimes/JoinLeaveSounds", false, Menu)]
        static void CreateJoinSounds(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating JoinLeaveSounds");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/Player Chimes/JoinSounds.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/PlayerChimes/JoinLeaveSounds + Logger", false, Menu)]
        static void CreateJoinSoundsLogger(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating JoinLeaveSounds + Logger");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/Player Chimes/PlayerLogger-Sounds.prefab", _cmd);
        }
        [MenuItem("GameObject/UdonVR/PlayerChimes/Logger", false, Menu)]
        static void CreatePlayerLogger(MenuCommand _cmd)
        {
            Debug.Log("[UdonVR] Creating Player Logger");
            CreatePrefabFromFile("Assets/_UdonVR/Tools/Player Chimes/PlayerLogger.prefab", _cmd);
        }
        #endregion
    }
}