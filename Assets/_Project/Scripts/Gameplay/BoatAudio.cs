using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatAudio : MonoBehaviour
    {
        [Header("Sources")]
        public AudioSource engineSource;
        public AudioSource boostSource;
        public AudioSource collisionSource;
        public AudioSource riverAmbienceSource;

        [Header("Engine Pitching")]
        public float minPitch = 0.5f;
        public float maxPitch = 2.0f;
        public float maxSpeedForPitch = 40f;

        [Header("Clips")]
        public AudioClip boostSFX;
        public AudioClip collisionSFX;

        private BoatController _boat;
        private Rigidbody _rb;
        private bool _wasBoosting;
        private float _nextAudioTick;

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Time.time < _nextAudioTick) return;
            _nextAudioTick = Time.time + 0.033f;

            HandleEngineAudio();
            HandleBoostAudio();
            HandleAmbienceAudio();
        }

        private void HandleEngineAudio()
        {
            if (engineSource == null || _boat == null) return;

            float speed = _boat.CurrentSpeed;
            float pitchFactor = Mathf.Clamp01(speed / maxSpeedForPitch);
            engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, pitchFactor);
            
            // Adjust volume slightly with speed
            engineSource.volume = Mathf.Lerp(0.3f, 0.8f, pitchFactor);
        }

        private void HandleAmbienceAudio()
        {
            if (riverAmbienceSource == null || _boat == null) return;
            float speed01 = Mathf.Clamp01(_boat.CurrentSpeed / maxSpeedForPitch);
            riverAmbienceSource.volume = Mathf.Lerp(0.25f, 0.55f, speed01);
            riverAmbienceSource.pitch = Mathf.Lerp(0.95f, 1.1f, speed01);
        }

        private void HandleBoostAudio()
        {
            if (boostSource == null || _boat == null) return;

            bool isBoosting = _boat.IsBoosting;
            if (isBoosting && !_wasBoosting)
            {
                if (boostSFX != null) boostSource.PlayOneShot(boostSFX);
            }
            _wasBoosting = isBoosting;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collisionSource == null || collisionSFX == null) return;

            // Only play if relative velocity is high enough
            if (collision.relativeVelocity.magnitude > 5f)
            {
                collisionSource.PlayOneShot(collisionSFX);
            }
        }
    }
}
