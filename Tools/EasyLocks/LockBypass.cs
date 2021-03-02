
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonVR.Tools.EasyLocks.Button;
using UdonVR.Tools.EasyLocks.Password;

namespace UdonVR.Tools.EasyLocks.Bypass
{
    public class LockBypass : UdonSharpBehaviour
    {
        public EasyLocks_Password PasswordLock;
        public EasyLocks_Button ButtonLock;

        [Tooltip("if this is enabled, the bypass button will ignore all checks on the lock and just unlock it.\nThis ignores checking to see if the button is Global.\n\nLeave this false in most cases.")]
        public bool BypassButtonChecks = false;

        [Tooltip("'This Button' refers to what object you want this script to reference when refurring to the bypass button.\n\nThis object is whatever the script is attached to by default.\nTypically you want this to be either the Canvas or the button when using UI, or whatever the script is attached to when using a physical button.")]
        public GameObject ThisButton;

        public bool isMasterOnly = true;

        public bool useUsername = false;
        public string[] Usernames;

        private string _localplayer;

        void Start()
        {
            if (ThisButton == null) ThisButton = gameObject;
            _localplayer = Networking.LocalPlayer.displayName;
            ThisButton.SetActive(false);
            ThisButton.SetActive(CheckEnable2());
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            ThisButton.SetActive(false);
            ThisButton.SetActive(CheckEnable2());
        }
        private void CheckEnable()
        {
            if (useUsername == false && isMasterOnly == false)
            {
                ThisButton.SetActive(true);
            }
            else
            {
                ThisButton.SetActive(false);
                if (useUsername == true)
                {
                    foreach (string i in Usernames)
                    {
                        if (i == _localplayer) ThisButton.SetActive(true);
                    }
                }
                if (isMasterOnly == true)
                {
                    if (Networking.IsMaster) ThisButton.SetActive(true);
                }
            }
        }
        private bool CheckEnable2()
        {
            if (useUsername == false && isMasterOnly == false)
            {
                return true;
            }
            else
            {
                if (useUsername == true)
                {
                    foreach (string i in Usernames)
                    {
                        if (i == _localplayer) return true;
                    }
                }
                if (isMasterOnly == true)
                {
                    if (Networking.IsMaster) return true;
                }
            }
            return false;
        }
        public override void Interact()
        {
            if (CheckEnable2() == false) return;
            if (BypassButtonChecks)
            {
                if (PasswordLock != null)
                {
                    PasswordLock.GlobalCheck();
                }
                if (ButtonLock != null)
                {
                    ButtonLock.GlobalCheck();
                }
            } else
            {
                if (PasswordLock != null)
                {
                    PasswordLock.GlobalCheck();
                }
                if (ButtonLock != null)
                {
                    ButtonLock.GlobalCheck();
                }
            }

        }
    }
}
