using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, -10);
        public float positionLag = 5f;
        public float rotationLag = 5f;
        
        [Header("Cinematic Effects")]
        public float turnTilt = 15f;
        public float speedFOVMultiplier = 0.5f;
        public float baseFOV = 60f;
        public float boostFOVBonus = 15f;

        [Header("Camera Shake")]
        public float shakeFrequency = 10f;
        public float shakeAmplitude = 0.1f;
        private float _shakeTimer;
        private float _shakeIntensity;

        private Camera _cam;
        private BoatController _boatController;

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeTimer = duration;
        }

        private void Start()
        {
            _cam = GetComponent<Camera>();
            if (target != null)
                _boatController = target.GetComponent<BoatController>();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Smooth position follow
            Vector3 targetPos = target.TransformPoint(offset);
            
            // Apply Camera Shake
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;
                Vector3 shakeOffset = new Vector3(
                    Mathf.PerlinNoise(Time.time * shakeFrequency, 0) - 0.5f,
                    Mathf.PerlinNoise(0, Time.time * shakeFrequency) - 0.5f,
                    0
                ) * shakeAmplitude * _shakeIntensity;
                targetPos += transform.TransformDirection(shakeOffset);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionLag);

            // Smooth rotation follow
            Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position + target.forward * 5f);
            
            // Add Tilt while turning
            float tilt = 0;
            if (_boatController != null)
            {
                // We estimate steering from angular velocity or input
                tilt = -Vector3.Dot(target.GetComponent<Rigidbody>().angularVelocity, target.up) * turnTilt;
            }
            targetRot *= Quaternion.Euler(0, 0, tilt);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLag);

            // FOV Effect
            if (_boatController != null)
            {
                float targetFOV = baseFOV + (_boatController.CurrentSpeed * speedFOVMultiplier);
                if (_boatController.IsBoosting) targetFOV += boostFOVBonus;
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, Time.deltaTime * 5f);
            }
        }
    }
}
