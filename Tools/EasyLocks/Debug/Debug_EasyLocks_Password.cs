
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace UdonVR.Childofthebeast.EasyLocks.Password
{
    public class Debug_EasyLocks_Password : UdonSharpBehaviour
    {
        public EasyLocks_Password Script;
        public Text DebugOut;
        void Update()
        {
            DebugOut.text = "Synced: " + Script._isSyncUnlocked + "| Local: " + Script._isLocked + "| Password: " + Script.Code;
        }
    }
}
