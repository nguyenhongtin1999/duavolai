using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using MienTayDaiChien.Gameplay;

namespace MienTayDaiChien.UI
{
    public class DebugPanel : MonoBehaviour
    {
        public GameObject panelRoot;
        public TextMeshProUGUI debugText;
        
        private BoatController _player;
        private float _fps;

        private void Start()
        {
            panelRoot.SetActive(false);
        }

        private void Update()
        {
            // Backquote ` key in new input system
            if (Keyboard.current.backquoteKey.wasPressedThisFrame) panelRoot.SetActive(!panelRoot.activeSelf);

            if (!panelRoot.activeInHierarchy) return;

            if (_player == null)
            {
                var progress = Object.FindObjectsByType<RaceProgress>(FindObjectsSortMode.None);
                foreach (var r in progress) if (r.IsLocalPlayer) _player = r.GetComponent<BoatController>();
            }

            _fps = 1.0f / Time.deltaTime;
            
            string info = $"FPS: {Mathf.RoundToInt(_fps)}\n";
            if (_player != null)
            {
                info += $"Speed: {_player.CurrentSpeed:F1} m/s\n";
                info += $"Pos: {_player.transform.position}\n";
                if (_player.TryGetComponent<RaceProgress>(out var p))
                {
                    info += $"Checkpoint: {p.lastCheckpointIndex.Value}\n";
                    info += $"Spline T: {p.distanceOnSpline.Value:F3}\n";
                }
            }

            debugText.text = info;
        }

        public void ResetBoat() => _player?.Respawn(Vector3.up * 5, Quaternion.identity);
        public void ToggleSlowMo() => Time.timeScale = Time.timeScale < 1 ? 1 : 0.2f;
    }

    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea != _lastSafeArea)
            {
                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect r)
        {
            _lastSafeArea = r;

            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
