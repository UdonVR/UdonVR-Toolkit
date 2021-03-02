
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace UdonVR.Tools.EasyLocks.Button
{
    public class Debug_EasyLocks_Button : UdonSharpBehaviour
    {
        public EasyLocks_Button Script;
        public Text DebugOut;
        void Update()
        {
            DebugOut.text = "Synced: " + Script._isSyncUnlocked + "| Local: " + Script._isLocked;
        }
    }
}
