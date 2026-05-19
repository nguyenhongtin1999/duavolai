using UnityEngine;
using UnityEngine.Rendering;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Manages a global Wake Map for the river water shader.
    /// Optimized for mobile by using a low-res RenderTexture.
    /// </summary>
    public class RiverWakeManager : MonoBehaviour
    {
        public static RiverWakeManager Instance { get; private set; }

        [Header("Settings")]
        public int resolution = 512;
        public float worldSize = 150f; // Increased for high speed racing
        public LayerMask wakeLayer;
        
        private RenderTexture _wakeRT;
        private Camera _wakeCamera;
        
        private static readonly int WakeMapId = Shader.PropertyToID("_WakeMap");
        private static readonly int WakeDataId = Shader.PropertyToID("_WakeData");

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SetupWakeSystem();
        }

        private void SetupWakeSystem()
        {
            _wakeRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.R8);
            _wakeRT.name = "RiverWakeMap";
            _wakeRT.filterMode = FilterMode.Bilinear;
            _wakeRT.wrapMode = TextureWrapMode.Clamp;
            
            GameObject camObj = new GameObject("WakeCamera");
            _wakeCamera = camObj.AddComponent<Camera>();
            _wakeCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            _wakeCamera.orthographic = true;
            _wakeCamera.orthographicSize = worldSize * 0.5f;
            _wakeCamera.clearFlags = CameraClearFlags.SolidColor;
            _wakeCamera.backgroundColor = Color.black;
            _wakeCamera.targetTexture = _wakeRT;
            _wakeCamera.cullingMask = wakeLayer;
            _wakeCamera.farClipPlane = 100f;
            _wakeCamera.nearClipPlane = 0.1f;
            
            Shader.SetGlobalTexture(WakeMapId, _wakeRT);
        }

        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 pos = Camera.main.transform.position;
                pos.y = 50f; 
                _wakeCamera.transform.position = pos;
                
                Vector4 wakeData = new Vector4(pos.x, pos.z, worldSize, 0);
                Shader.SetGlobalVector(WakeDataId, wakeData);
            }
        }
    }

    /// <summary>
    /// Attached to boats to emit wake/foam into the RiverWakeMap.
    /// </summary>
    public class BoatWakeEmitter : MonoBehaviour
    {
        public float baseIntensity = 1f;
        public float boostMultiplier = 2f;
        public float driftMultiplier = 1.5f;
        
        private float _currentIntensity = 1f;
        private TrailRenderer _trail;

        private void Start()
        {
            _trail = GetComponent<TrailRenderer>();
            if (_trail != null)
            {
                _trail.gameObject.layer = LayerMask.NameToLayer("Wake"); 
            }
        }

        public void SetState(bool isBoosting, bool isDrifting)
        {
            float target = baseIntensity;
            if (isBoosting) target *= boostMultiplier;
            if (isDrifting) target *= driftMultiplier;
            
            _currentIntensity = Mathf.Lerp(_currentIntensity, target, Time.deltaTime * 5f);
            if (_trail != null) _trail.widthMultiplier = _currentIntensity;
        }
    }

    /// <summary>
    /// Handles water collision splashes and effects.
    /// </summary>
    public class WaterCollisionHandler : MonoBehaviour
    {
        public GameObject splashPrefab;
        public float minCollisionForce = 2f;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > minCollisionForce)
            {
                ContactPoint contact = collision.contacts[0];
                if (splashPrefab != null)
                {
                    Instantiate(splashPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                }
                
                // Also trigger a "ripple" in the wake map if possible
                // (Handled visually by the emitter usually)
            }
        }
    }
}
