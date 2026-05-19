using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MienTayDaiChien.Gameplay;
using TMPro;

namespace MienTayDaiChien.UI
{
    public class RaceHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI rankingText;
        public TextMeshProUGUI lapText;
        public TextMeshProUGUI wrongWayIndicator;
        public GameObject finishPanel;

        private RaceProgress _localPlayer;

        private void Start()
        {
            // Find local player (In NGO, we look for IsLocalPlayer)
            // For now, we'll find the first one for the demo
        }

        private void Update()
        {
            if (RaceManager.Instance == null) return;

            var rankings = RaceManager.Instance.Rankings;
            
            // Update Leaderboard
            string rankStr = "RANKINGS\n";
            for (int i = 0; i < rankings.Count; i++)
            {
                rankStr += $"{i + 1}. {rankings[i].name} (Lap {rankings[i].Lap})\n";
                
                // Track local player reference
                if (rankings[i].IsLocalPlayer) _localPlayer = rankings[i];
            }
            if (rankingText != null) rankingText.text = rankStr;

            // Update Player Specific HUD
            if (_localPlayer != null)
            {
                if (lapText != null) lapText.text = $"LAP {Mathf.Min(_localPlayer.Lap + 1, RaceManager.Instance.totalLaps)} / {RaceManager.Instance.totalLaps}";
                if (wrongWayIndicator != null) wrongWayIndicator.gameObject.SetActive(_localPlayer.isWrongWay.Value);
                if (finishPanel != null) finishPanel.SetActive(_localPlayer.isFinished.Value);
}
        }
    }
}
