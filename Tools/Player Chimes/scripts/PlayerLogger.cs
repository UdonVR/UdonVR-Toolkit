using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

namespace UdonVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerLogger : UdonSharpBehaviour
    {
        [Header("Join/Leave Sounds")]
        public bool enableSounds = true;

        public AudioSource bell;
        public AudioClip join;
        public AudioClip leave;
        public bool joinEnable = true;
        public bool leaveEnable = true;
        public GameObject joinToggle;
        public GameObject leaveToggle;
        private int _numPlayers;
        private int _numJoins;

        [Header("Player Logger")]
        public bool logger = true;

        public TextMeshProUGUI playerLogger;
        public bool timeStamps = true;
        public string joinPrefix = "[<color=green>Joined</color>]";
        public string leavePrefix = "[<color=red>Left</color>]";
        private string _log = "";

        private bool hasHeader = false;
        public TextMeshProUGUI header;

        private void Start()
        {
            _log = "PlayerChimes made by UdonVR\n=========================================\n";
            _numPlayers = VRCPlayerApi.GetPlayerCount();
            if (enableSounds)
            {
                if (joinToggle != null) joinToggle.SetActive(joinEnable);
                if (leaveToggle != null) leaveToggle.SetActive(leaveEnable);
            }

            if (header != null) hasHeader = true;
        }

        private void Update()
        {
            if (!hasHeader) return;
            header.text = "<size=2>----------------------------------------------------------------------------------------------------------\n</size>[" + DateTime.Now.ToString("hh:mm") + "] Players: " + VRCPlayerApi.GetPlayerCount();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (logger) LogPlayer(true, player);
            if (enableSounds)
            {
                _numJoins = _numJoins + 1;
                if (join != null && joinEnable && _numJoins > _numPlayers)
                {
                    bell.clip = join;
                    bell.Play();
                }
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (playerLogger != null) LogPlayer(false, player);
            if (enableSounds)
            {
                if (leave != null && leaveEnable)
                {
                    bell.clip = leave;
                    bell.Play();
                }
            }
        }

        public void JoinToggle()
        {
            joinEnable = !joinEnable;
            if (joinToggle != null) joinToggle.SetActive(joinEnable);
        }

        public void LeaveToggle()
        {
            leaveEnable = !leaveEnable;
            if (leaveToggle != null) leaveToggle.SetActive(leaveEnable);
        }

        private void LogPlayer(bool _isJoin, VRCPlayerApi _player)
        {
            if (playerLogger == null)
            {
                Debug.LogError("[UdonVR] Player Logger has no logger attached");
                return;
            }
            if (timeStamps)
            {
                _log = _log + DateTime.Now.ToString("hh:mm");
            }
            if (_isJoin)
            {
                _log = _log + joinPrefix + _player.displayName;
            }
            else
            {
                _log = _log + leavePrefix + _player.displayName;
            }
            if (_log.Length > 2000) _log = _log.Remove(0, (_log.Length - 2000));
            _log = _log + "\n";
            playerLogger.text = _log;
        }
    }
}