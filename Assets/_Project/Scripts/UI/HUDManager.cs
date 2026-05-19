using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MienTayDaiChien.Gameplay;
using System.Collections.Generic;

namespace MienTayDaiChien.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Race Info")]
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI lapText;
        public TextMeshProUGUI rankingText;
        public TextMeshProUGUI timerText;
        public Image boostMeter;
        public RectTransform checkpointArrow;

        [Header("References")]
        public BoatController playerBoat;
        public RaceProgress playerProgress;
        public MinimapManager minimap;

        private float _raceStartTime;

        private void Start()
        {
            _raceStartTime = Time.time;
        }

        private void Update()
        {
            if (playerBoat == null)
            {
                // Try to find the local player boat if not assigned
                var racers = Object.FindObjectsByType<RaceProgress>(FindObjectsSortMode.None);
                foreach (var r in racers)
                {
                    if (r.IsLocalPlayer)
                    {
                        playerProgress = r;
                        playerBoat = r.GetComponent<BoatController>();
                        break;
                    }
                }
                return;
            }

            // Speed
            float speedKph = playerBoat.CurrentSpeed * 3.6f; // rough conversion
            if (speedText != null) speedText.text = $"{Mathf.RoundToInt(speedKph)} <size=50%>KM/H</size>";

            // Lap
            if (lapText != null)
            {
                int lap = Mathf.Min(playerProgress.Lap + 1, RaceManager.Instance.totalLaps);
                lapText.text = $"LAP {lap}/{RaceManager.Instance.totalLaps}";
            }

            // Ranking
            if (rankingText != null)
            {
                var rankings = RaceManager.Instance.Rankings;
                int rank = rankings.IndexOf(playerProgress) + 1;
                rankingText.text = GetRankSuffix(rank);
            }

            // Timer
            if (timerText != null && !playerProgress.isFinished.Value)
            {
                float elapsed = Time.time - _raceStartTime;
                timerText.text = string.Format("{0:00}:{1:00}.{2:00}", Mathf.Floor(elapsed / 60), elapsed % 60, (elapsed * 100) % 100);
            }

            // Boost
            if (boostMeter != null) boostMeter.fillAmount = playerBoat.BoostProgress;

            // Checkpoint Arrow
            UpdateCheckpointArrow();
        }

        private void UpdateCheckpointArrow()
        {
            if (checkpointArrow == null || RaceManager.Instance == null || playerProgress == null || RaceManager.Instance.riverSpline == null) return;

            // Simple: Point towards the next point on the spline
            float lookAhead = 0.05f;
            float t = (playerProgress.distanceOnSpline.Value + lookAhead) % 1.0f;
            Vector3 targetPoint = RaceManager.Instance.riverSpline.Container.EvaluatePosition(t);
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPoint);
            bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

            checkpointArrow.gameObject.SetActive(isOffScreen);

            if (isOffScreen)
            {
                // Logic to rotate arrow towards target on screen edges
                Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 dir = ((Vector2)screenPos - center).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                checkpointArrow.rotation = Quaternion.Euler(0, 0, angle - 90);
                
                // Keep it at the edge of the safe area
                checkpointArrow.anchoredPosition = dir * 400f; // Roughly 400px from center
            }
        }

        private string GetRankSuffix(int rank)
        {
            if (rank == 1) return "1ST";
            if (rank == 2) return "2ND";
            if (rank == 3) return "3RD";
            return $"{rank}TH";
        }
    }
}
