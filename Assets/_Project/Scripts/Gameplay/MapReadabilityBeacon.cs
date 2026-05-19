using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class MapReadabilityBeacon : MonoBehaviour
    {
        public Renderer beaconRenderer;
        public Color baseColor = new Color(1f, 0.85f, 0.2f, 1f);
        public Color boostColor = new Color(1f, 0.45f, 0.1f, 1f);
        public float pulseSpeed = 3f;
        public float pulseIntensity = 1.5f;

        private Material _materialInstance;

        private void Awake()
        {
            if (beaconRenderer == null) beaconRenderer = GetComponentInChildren<Renderer>();
            if (beaconRenderer != null)
            {
                _materialInstance = beaconRenderer.material;
            }
        }

        private void Update()
        {
            if (_materialInstance == null || !_materialInstance.HasProperty("_EmissionColor")) return;
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseIntensity;
            Color c = Color.Lerp(baseColor, boostColor, pulse);
            _materialInstance.SetColor("_EmissionColor", c);
        }
    }
}
