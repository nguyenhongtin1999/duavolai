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
        public float splashMaxEmission = 80f;
        public Color boostGlowColor = new Color(0, 1, 1, 1);
        private Color _baseGlowColor;
        private Material _boatMat;

        [Header("Post-Processing Juice")]
        private Volume _globalVolume;
        private ChromaticAberration _ca;
        private LensDistortion _lens;

        [Header("Haptics")]
        public float driftHapticIntensity = 0.15f;
        public float boostHapticIntensity = 0.6f;

        private BoatCamera _mainCam;

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
            _mainCam = Camera.main?.GetComponent<BoatCamera>();
            
            if (speedLinesUI == null)
                speedLinesUI = GameObject.Find("SpeedLines_VFX");

            // Setup PP hooks
            _globalVolume = Object.FindAnyObjectByType<Volume>();
            if (_globalVolume != null && _globalVolume.sharedProfile != null)
            {
                _globalVolume.sharedProfile.TryGet(out _ca);
                _globalVolume.sharedProfile.TryGet(out _lens);
            }
        }

        private void Update()
        {
            HandleWaterSplashes();
            HandleBoostEffects();
            HandleDriftFeedback();
            HandleSpeedLines();
            HandlePostProcessingJuice();
        }

        private void HandleWaterSplashes()
        {
            float speed = _boat.CurrentSpeed;
            float emissionScale = Mathf.InverseLerp(splashMinSpeed, _boat.maxSpeed, speed);
            
            if (_boat.IsBoosting) emissionScale *= 1.8f;

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
                
                var mainL = waterSplashL.main;
                var mainR = waterSplashR.main;
                float baseSize = 1.2f;
                mainL.startSize = baseSize * (1f + emissionScale * 0.4f);
                mainR.startSize = baseSize * (1f + emissionScale * 0.4f);
            }
        }

        private void HandleBoostEffects()
        {
            bool isBoosting = _boat.IsBoosting;
            
            if (isBoosting)
            {
                if (_boatMat != null && _boatMat.HasProperty("_EmissionColor"))
                {
                    _boatMat.SetColor("_EmissionColor", boostGlowColor * 3f);
                }
                
                if (_mainCam != null)
                {
                    _mainCam.TriggerShake(1.2f, 0.1f);
                }
                
                TriggerHaptics(boostHapticIntensity, boostHapticIntensity);
            }
            else
            {
                if (_boatMat != null && _boatMat.HasProperty("_EmissionColor"))
                {
                    _boatMat.SetColor("_EmissionColor", Color.Lerp(_boatMat.GetColor("_EmissionColor"), _baseGlowColor, Time.deltaTime * 5f));
                }
            }
        }

        private void HandlePostProcessingJuice()
        {
            if (_ca == null || _lens == null) return;

            float boostFactor = _boat.IsBoosting ? 1f : 0f;
            float speedFactor = Mathf.Clamp01(_boat.CurrentSpeed / 40f);

            // Pulse CA and Lens during boost
            _ca.intensity.Override(Mathf.Lerp(_ca.intensity.value, 0.1f + (boostFactor * 0.4f), Time.deltaTime * 5f));
            _lens.intensity.Override(Mathf.Lerp(_lens.intensity.value, -0.1f * boostFactor, Time.deltaTime * 5f));
        }

        private void HandleDriftFeedback()
        {
            bool isDrifting = _boat.IsDrifting;
            int driftLevel = _boat.DriftLevel;
            
            if (isDrifting && _boat.CurrentSpeed > 5f)
            {
                if (!driftSparks.isPlaying) driftSparks.Play();
                
                var main = driftSparks.main;
                if (driftLevel == 1) main.startColor = Color.blue;
                else if (driftLevel == 2) main.startColor = new Color(1, 0.5f, 0);
                else if (driftLevel == 3) main.startColor = new Color(0.6f, 0, 1);
                else main.startColor = Color.white;

                TriggerHaptics(driftHapticIntensity * (1 + driftLevel * 0.3f), 0);
                
                if (_mainCam != null)
                    _mainCam.TriggerShake(0.4f + driftLevel * 0.15f, 0.05f);
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
            bool showLines = speedFactor > 0.75f || _boat.IsBoosting;
            
            if (speedLinesUI.activeSelf != showLines)
                speedLinesUI.SetActive(showLines);

            if (showLines)
            {
                // Pulsate speed lines UI slightly
                float scale = 1f + Mathf.PingPong(Time.time * 10f, 0.05f);
                speedLinesUI.transform.localScale = new Vector3(scale, scale, 1f);
            }
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
            if (collision.relativeVelocity.magnitude > 6f)
            {
                if (_mainCam != null) _mainCam.TriggerShake(1.5f, 0.25f);
                TriggerHaptics(0.9f, 0.9f);
            }
        }
    }
}
