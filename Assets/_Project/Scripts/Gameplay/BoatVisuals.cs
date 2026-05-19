using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatVisuals : MonoBehaviour
    {
        public ParticleSystem[] boostVFX;
        public GameObject boatModel;
        public float modelTiltFactor = 10f;
        
        private BoatController _controller;
        private Rigidbody _rb;

        private void Awake()
        {
            _controller = GetComponent<BoatController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Toggle Boost VFX
            bool boosting = _controller.IsBoosting;
            foreach (var ps in boostVFX)
            {
                if (boosting && !ps.isPlaying) ps.Play();
                else if (!boosting && ps.isPlaying) ps.Stop();
            }

            // Visual tilt of the model only (independent of physics body)
            if (boatModel != null)
            {
                float steerTilt = -Vector3.Dot(_rb.angularVelocity, transform.up) * modelTiltFactor;
                
                // Drift banking bonus
                if (_controller.IsDrifting) steerTilt *= 2.5f;

                boatModel.transform.localRotation = Quaternion.Euler(0, 0, steerTilt);
            }
}

        private void OnCollisionEnter(Collision collision)
        {
            // Trigger Camera Shake hook here if needed
            if (collision.relativeVelocity.magnitude > 5f)
            {
                Debug.Log("Big collision - Trigger Shake");
            }
        }
    }
}
