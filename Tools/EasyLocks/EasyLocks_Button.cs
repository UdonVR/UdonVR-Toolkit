using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;


#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
using System.Collections.Generic;
using System.Linq;
using UdonVR.EditorUtility;
#endif

namespace UdonVR.Childofthebeast.EasyLocks.Button
{
    [AddComponentMenu("UdonVR/Tools/EasyLocks/Button")]
    public class EasyLocks_Button : UdonSharpBehaviour
    {
        [Header("Easy Locks - Button")]

        [Space]

        [Tooltip("If 'is Global' is True then this toggles for everyone")]
        public bool isGlobal = true;

        [Tooltip("If 'is Master Only' is True then the lock can only be used by the room master. Use Username ignores this.")]
        public bool isMasterOnly = true;

        [Tooltip("If 'Use Username' is True then Usernames are the first thing the lock checks. This ignores MasterOnly.")]
        public bool useUsername = false;
        [Tooltip("Usernames need to be exactly how they are In-Game. They are Case Sensitive.")]
        public string[] Usernames;

        [Space]

        [Tooltip("This is what get's toggled when the lock is used.\n\nDefault Off should be used for objects that are off when you upload the world.\n\nThese will turn ON the first time the lock is used.")]
        public GameObject[] LockTargetsDefaultOff;
        [Tooltip("This is what get's toggled when the lock is used.\n\nDefault On should be used for objects that are on when you upload the world.\n\nThese will turn OFF the first time the lock is used.")]
        public GameObject[] LockTargetsDefaultOn;

        [HideInInspector]
        [UdonSynced] public bool _isSyncUnlocked = false;
        [HideInInspector]
        public bool _isLocked = false;

        public void Start()
        {
            if (Networking.IsMaster && isMasterOnly)
            {
                _isSyncUnlocked = LockTargetsDefaultOff[1].activeSelf;
            }
        }

        public override void Interact()
        {
            foreach (string _user in Usernames)
            {
                if (_user == Networking.LocalPlayer.displayName)
                {
                    GlobalCheck();
                    break;
                }
            }
            if (Networking.IsMaster && isMasterOnly)
            {
                GlobalCheck();
            }
            else if (isMasterOnly == false)
            {
                GlobalCheck();
            }
        }

        private void GlobalCheck()
        {
            if (isGlobal)
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, "GlobalToggleLock");
            }
            else
            {
                _isLocked = !_isLocked;
                Setlock();
            }
        }

        void Update()
        {
            if (_isSyncUnlocked != _isLocked && isGlobal)
            {
                _isLocked = _isSyncUnlocked;
                Setlock();
            }
        }
        private void Setlock()
        {
            foreach (GameObject _Target in LockTargetsDefaultOff)
            {
                if (_Target != null)
                    _Target.SetActive(_isLocked);
            }
            foreach (GameObject _Target in LockTargetsDefaultOn)
            {
                if (_Target != null)
                    _Target.SetActive(!_isLocked);
            }
        }
        public void GlobalToggleLock()
        {
            _isSyncUnlocked = !_isSyncUnlocked;
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(EasyLocks_Button))]
    public class EasyLocks_Button_Editor : Editor
    {
        private SerializedProperty LockTargetsDefaultOff;
        private SerializedProperty LockTargetsDefaultOn;
        private Transform baseTransform;

        private void OnEnable()
        {
            LockTargetsDefaultOff = serializedObject.FindProperty("LockTargetsDefaultOff");
            LockTargetsDefaultOn = serializedObject.FindProperty("LockTargetsDefaultOn");
            baseTransform = ((EasyLocks_Button)target).transform;
        }
        public void OnSceneGUI()
        {
            for (int i = 0; i < LockTargetsDefaultOff.arraySize; i++)
            {
                GameObject _target = (GameObject)LockTargetsDefaultOff.GetArrayElementAtIndex(i).objectReferenceValue;
                if (_target != null)
                    EditorHelper.ShowTransform(_target.transform, baseTransform, false);
            }
            for (int i = 0; i < LockTargetsDefaultOn.arraySize; i++)
            {
                GameObject _target = (GameObject)LockTargetsDefaultOn.GetArrayElementAtIndex(i).objectReferenceValue;
                if (_target != null)
                    EditorHelper.ShowTransform(_target.transform, baseTransform, false);
            }
        }
    }
#endif
}