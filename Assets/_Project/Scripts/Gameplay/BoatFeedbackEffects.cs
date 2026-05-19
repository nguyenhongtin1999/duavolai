using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MienTayDaiChien.Gameplay
{
    public class BoatFeedbackEffects : MonoBehaviour
    {
        [Header("References")]
        private BoatController _boat;
        private Rigidbody _rb;
        public ParticleSystem waterSplashL;
        public ParticleSystem waterSplashR;
        public ParticleSystem driftSparks;
        public GameObject speedLinesUI;
        public Renderer boatModelRenderer;
        
        [Header("Settings")]
        public float splashMinSpeed = 5f;
        public float splashMaxEmission = 50f;
        public Color boostGlowColor = new Color(0, 1, 1, 1);
        private Color _baseGlowColor;
        private Material _boatMat;

        [Header("Haptics")]
        public float driftHapticIntensity = 0.1f;
        public float boostHapticIntensity = 0.5f;

        private BoatCameraRig _mainCam;

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
            _rb = GetComponent<Rigidbody>();
            if (boatModelRenderer != null)
            {
                _boatMat = boatModelRenderer.material;
                if (_boatMat.HasProperty("_EmissionColor"))
                    _baseGlowColor = _boatMat.GetColor("_EmissionColor");
            }
            _mainCam = Camera.main?.GetComponent<BoatCameraRig>();
            
            if (speedLinesUI == null)
            {
                speedLinesUI = GameObject.Find("SpeedLines_VFX");
            }
        }

        private void Update()
        {
            HandleWaterSplashes();
            HandleBoostEffects();
            HandleDriftFeedback();
            HandleSpeedLines();
        }

        private void HandleWaterSplashes()
        {
            float speed = _boat.CurrentSpeed;
            float emissionScale = Mathf.InverseLerp(splashMinSpeed, _boat.maxSpeed, speed);
            
            // Boost amplification
            if (_boat.IsBoosting) emissionScale *= 1.5f;

            var emissionL = waterSplashL.emission;
            var emissionR = waterSplashR.emission;
            
            bool isActive = speed > splashMinSpeed;
            emissionL.enabled = isActive;
            emissionR.enabled = isActive;

            if (isActive)
            {
                float rate = emissionScale * splashMaxEmission;
                emissionL.rateOverTime = rate;
                emissionR.rateOverTime = rate;
                
                // Scale particle size with speed/boost
                var mainL = waterSplashL.main;
                var mainR = waterSplashR.main;
                float baseSize = 1.5f;
                mainL.startSize = baseSize * (1f + emissionScale * 0.5f);
                mainR.startSize = baseSize * (1f + emissionScale * 0.5f);
            }
        }

        private void HandleBoostEffects()
        {
            bool isBoosting = _boat.IsBoosting;
            
            if (isBoosting)
            {
                if (_boatMat != null && _boatMat.HasProperty("_EmissionColor"))
                {
                    _boatMat.SetColor("_EmissionColor", boostGlowColor * 2f);
                }
                
                if (_mainCam != null)
                {
                    _mainCam.TriggerShake(1.5f, 0.1f);
                }
                
                TriggerHaptics(boostHapticIntensity, boostHapticIntensity);
            }
            else
            {
                if (_boatMat != null && _boatMat.HasProperty("_EmissionColor"))
                {
                    _boatMat.SetColor("_EmissionColor", _baseGlowColor);
                }
            }
        }

        private void HandleDriftFeedback()
        {
            bool isDrifting = _boat.IsDrifting;
            int driftLevel = _boat.DriftLevel;
            
            if (isDrifting && _boat.CurrentSpeed > 5f)
            {
                if (!driftSparks.isPlaying) driftSparks.Play();
                
                // Color sparks based on level
                var main = driftSparks.main;
                if (driftLevel == 1) main.startColor = Color.blue;
                else if (driftLevel == 2) main.startColor = new Color(1, 0.5f, 0); // Orange
                else if (driftLevel == 3) main.startColor = new Color(0.6f, 0, 1); // Purple
                else main.startColor = Color.white;

                TriggerHaptics(driftHapticIntensity * (1 + driftLevel * 0.2f), 0);
                
                if (_mainCam != null)
                    _mainCam.TriggerShake(0.5f + driftLevel * 0.1f, 0.05f);
            }
            else
            {
                if (driftSparks.isPlaying) driftSparks.Stop();
            }
        }

        private void HandleSpeedLines()
        {
            if (speedLinesUI == null) return;
            
            float speedFactor = _boat.CurrentSpeed / _boat.maxSpeed;
            bool showLines = speedFactor > 0.8f || _boat.IsBoosting;
            
            if (speedLinesUI.activeSelf != showLines)
                speedLinesUI.SetActive(showLines);
        }

        private void TriggerHaptics(float left, float right)
        {
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(left, right);
            }
        }

        private void OnDisable()
        {
            if (Gamepad.current != null) Gamepad.current.ResetHaptics();
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > 8f)
            {
                if (_mainCam != null) _mainCam.TriggerShake(2.0f, 0.2f);
                TriggerHaptics(0.8f, 0.8f);
                // Hook for sound: PlayCollisionSFX()
            }
        }
    }
}
