using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MienTayDaiChien.Gameplay;
using TMPro;

namespace MienTayDaiChien.UI
{
    public class RaceHUD : MonoBehaviour
    {
        [Header("Race Info")]
        public TextMeshProUGUI rankingText;
        public TextMeshProUGUI lapText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI wrongWayIndicator;

        [Header("Navigation")]
        public RectTransform checkpointArrow;
        public GameObject finishPanel;

        private RaceProgress _localPlayer;
        private float _nextLookupTime;

        private void Start()
        {
            // Detection of local player happens automatically in Update if null
        }

        private void Update()
        {
            if (RaceManager.Instance == null) return;

            // Find local player if not set
            if (_localPlayer == null && Time.time >= _nextLookupTime)
            {
                _nextLookupTime = Time.time + 1f;
                var racers = Object.FindObjectsByType<RaceProgress>(FindObjectsSortMode.None);
                foreach (var r in racers)
                {
                    if (r.IsOwner) // For Netcode, IsOwner indicates the local player's boat
                    {
                        _localPlayer = r;
                        break;
                    }
                }
            }

            UpdateCountdown();
            UpdateLeaderboard();
            UpdateTimer();

            if (_localPlayer != null)
            {
                UpdatePlayerHUD();
                UpdateNavigation();
            }
        }

        private void UpdateCountdown()
        {
            if (countdownText == null) return;
            var state = RaceManager.Instance.currentState.Value;
            if (state == RaceState.Countdown)
            {
                float time = RaceManager.Instance.countdownTimer.Value;
                countdownText.text = time > 1.0f ? Mathf.CeilToInt(time).ToString() : "GO!";
                countdownText.gameObject.SetActive(true);
            }
            else
            {
                countdownText.gameObject.SetActive(false);
            }
        }

        private void UpdateLeaderboard()
        {
            if (rankingText == null) return;
            var rankings = RaceManager.Instance.Rankings;
            string rankStr = "RANKINGS\n";
            for (int i = 0; i < rankings.Count; i++)
            {
                rankStr += $"{i + 1}. {rankings[i].name}\n";
            }
            rankingText.text = rankStr;
        }

        private void UpdateTimer()
        {
            if (timerText == null) return;
            float time = RaceManager.Instance.raceTimer.Value;
            timerText.text = string.Format("{0:00}:{1:00}.{2:00}", Mathf.Floor(time / 60), time % 60, (time * 100) % 100);
        }

        private void UpdatePlayerHUD()
        {
            if (lapText != null) 
                lapText.text = $"LAP {Mathf.Min(_localPlayer.Lap + 1, RaceManager.Instance.totalLaps)} / {RaceManager.Instance.totalLaps}";
            
            if (wrongWayIndicator != null) 
                wrongWayIndicator.gameObject.SetActive(_localPlayer.isWrongWay.Value);
            
            if (finishPanel != null) 
                finishPanel.SetActive(_localPlayer.isFinished.Value);
        }

        private void UpdateNavigation()
        {
            if (checkpointArrow == null || RaceManager.Instance.riverSpline == null) return;

            // Point to next checkpoint
            int nextCPIndex = _localPlayer.LastCheckpointIndex + 1;
            if (nextCPIndex >= RaceManager.Instance.GetCheckpointCount()) nextCPIndex = 0;

            RaceCheckpoint nextCP = RaceManager.Instance.GetCheckpoint(nextCPIndex);
            if (nextCP != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(nextCP.transform.position);
                bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;
                
                checkpointArrow.gameObject.SetActive(isOffScreen);

                if (isOffScreen)
                {
                    Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
                    Vector2 dir = ((Vector2)screenPos - center).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    checkpointArrow.rotation = Quaternion.Euler(0, 0, angle - 90);
                    checkpointArrow.anchoredPosition = dir * 300f; // Edge distance
                }
            }
        }
    }
}
