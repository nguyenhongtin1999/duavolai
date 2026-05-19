using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatAudio : MonoBehaviour
    {
        [Header("Sources")]
        public AudioSource engineSource;
        public AudioSource boostSource;
        public AudioSource collisionSource;

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

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            HandleEngineAudio();
            HandleBoostAudio();
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
