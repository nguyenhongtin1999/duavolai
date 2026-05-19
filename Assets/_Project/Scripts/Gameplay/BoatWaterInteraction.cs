using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatWaterInteraction : MonoBehaviour
    {
        public ParticleSystem wakeTrail;
        public ParticleSystem splashBurst;
        public ParticleSystem speedSpray;
        public ParticleSystem driftSideSpray;
        public ParticleSystem rippleBurst;

        [Header("Thresholds")]
        public float wakeStartSpeed = 4f;
        public float speedSprayStartSpeed = 12f;
        public float hitSplashVelocity = 6f;

        private BoatController _boat;

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
        }

        private void Update()
        {
            if (_boat == null) return;
            float speed = _boat.CurrentSpeed;

            SetEmissionActive(wakeTrail, speed > wakeStartSpeed);
            SetEmissionRate(wakeTrail, Mathf.InverseLerp(wakeStartSpeed, _boat.maxSpeed, speed) * 40f);
            SetEmissionActive(speedSpray, speed > speedSprayStartSpeed || _boat.IsBoosting);
            SetEmissionRate(speedSpray, Mathf.InverseLerp(speedSprayStartSpeed, _boat.maxSpeed, speed) * 55f);
            SetEmissionActive(driftSideSpray, _boat.IsDrifting && speed > wakeStartSpeed);
            SetEmissionRate(driftSideSpray, _boat.IsDrifting ? 45f : 0f);
        }

        private static void SetEmissionRate(ParticleSystem ps, float rate)
        {
            if (ps == null) return;
            var emission = ps.emission;
            emission.rateOverTime = Mathf.Max(0f, rate);
        }

        private static void SetEmissionActive(ParticleSystem ps, bool isActive)
        {
            if (ps == null) return;
            var emission = ps.emission;
            emission.enabled = isActive;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude >= hitSplashVelocity)
            {
                if (splashBurst != null) splashBurst.Play();
                if (rippleBurst != null) rippleBurst.Play();
            }
        }
    }
}
