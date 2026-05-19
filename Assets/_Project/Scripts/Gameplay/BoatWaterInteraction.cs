using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatWaterInteraction : MonoBehaviour
    {
        public ParticleSystem wakeTrail;
        public ParticleSystem splashBurst;
        public ParticleSystem speedSpray;

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
            SetEmissionActive(speedSpray, speed > speedSprayStartSpeed || _boat.IsBoosting);
        }

        private static void SetEmissionActive(ParticleSystem ps, bool isActive)
        {
            if (ps == null) return;
            var emission = ps.emission;
            emission.enabled = isActive;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (splashBurst != null && collision.relativeVelocity.magnitude >= hitSplashVelocity)
            {
                splashBurst.Play();
            }
        }
    }
}
