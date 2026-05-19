using UnityEngine;
using UnityEngine.UI;
using MienTayDaiChien.Gameplay;
using System.Collections.Generic;

namespace MienTayDaiChien.UI
{
    public class MinimapManager : MonoBehaviour
    {
        public static MinimapManager Instance { get; private set; }

        [Header("Settings")]
        public RenderTexture minimapRT;
        public RawImage minimapDisplay;
        public Camera minimapCamera;
        public float zoomLevel = 50f;
        public bool followPlayer = true;

        [Header("Prefabs")]
        public GameObject playerIconPrefab;
        public GameObject opponentIconPrefab;
        public GameObject checkpointIconPrefab;
        public GameObject directionArrowPrefab;

        private Transform _playerTransform;
        private List<MinimapIcon> _icons = new List<MinimapIcon>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (minimapCamera != null && minimapRT != null)
            {
                minimapCamera.targetTexture = minimapRT;
                if (minimapDisplay != null) minimapDisplay.texture = minimapRT;
            }
        }

        public void SetPlayer(Transform player)
        {
            _playerTransform = player;
        }

        private void LateUpdate()
        {
            if (followPlayer && _playerTransform == null)
            {
                // Auto-detect local player if not set
                var players = Object.FindObjectsByType<RaceProgress>(FindObjectsSortMode.None);
                foreach (var p in players)
                {
                    if (p.IsLocalPlayer)
                    {
                        SetPlayer(p.transform);
                        break;
                    }
                }
            }

            if (followPlayer && _playerTransform != null && minimapCamera != null)
            {
                Vector3 targetPos = _playerTransform.position;
                targetPos.y = minimapCamera.transform.position.y;
                minimapCamera.transform.position = targetPos;
                
                // Rotate camera to match player rotation (optional, but good for racing)
                // minimapCamera.transform.rotation = Quaternion.Euler(90, _playerTransform.eulerAngles.y, 0);
            }
        }

        public void RegisterIcon(MinimapIcon icon)
        {
            if (!_icons.Contains(icon)) _icons.Add(icon);
        }

        public void UnregisterIcon(MinimapIcon icon)
        {
            if (_icons.Contains(icon)) _icons.Remove(icon);
        }
    }
}
