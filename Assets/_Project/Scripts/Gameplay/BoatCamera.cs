using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, -12);
        public float positionSmoothTime = 0.15f;
        public float rotationSmoothTime = 0.1f;
        
        [Header("Cinematic Effects")]
        public float turnTilt = 12f;
        public float speedFOVMultiplier = 0.35f;
        public float baseFOV = 65f;
        public float boostFOVBonus = 15f;
        public float speedElasticity = 0.12f;

        [Header("Camera Shake")]
        public float shakeFrequency = 15f;
        public float shakeAmplitude = 0.08f;
        private float _shakeTimer;
        private float _shakeIntensity;

        private Camera _cam;
        private BoatController _boatController;
        private Rigidbody _targetRb;
        private Vector3 _currentVelocity;
        private Vector3 _currentRotationVelocity;

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeTimer = duration;
        }

        private void Start()
        {
            _cam = GetComponent<Camera>();
            if (target != null)
            {
                _boatController = target.GetComponent<BoatController>();
                _targetRb = target.GetComponent<Rigidbody>();
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            float speed = _boatController != null ? _boatController.CurrentSpeed : 0;
            float speedFactor = Mathf.Clamp01(speed / 45f);

            // 1. Dynamic Position with Elasticity
            Vector3 dynamicOffset = offset;
            dynamicOffset.z -= speed * speedElasticity; // Pull back at high speed
            dynamicOffset.y += speed * speedElasticity * 0.2f; // Lift slightly

            Vector3 targetWorldPos = target.TransformPoint(dynamicOffset);
            
            // Apply Camera Shake
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;
                Vector3 shakeOffset = new Vector3(
                    Mathf.PerlinNoise(Time.time * shakeFrequency, 0) - 0.5f,
                    Mathf.PerlinNoise(0, Time.time * shakeFrequency) - 0.5f,
                    0
                ) * shakeAmplitude * _shakeIntensity;
                targetWorldPos += transform.TransformDirection(shakeOffset);
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref _currentVelocity, positionSmoothTime);

            // 2. Dynamic Look-Ahead
            // Look at a point slightly ahead of the boat based on its movement
            Vector3 lookTarget = target.position + target.forward * 8f;
            if (_targetRb != null)
            {
                lookTarget += _targetRb.linearVelocity * 0.15f;
            }

            Quaternion targetRot = Quaternion.LookRotation(lookTarget - transform.position);
            
            // 3. Turn Banking Tilt
            float tilt = 0;
            if (_targetRb != null)
            {
                // Smoothly estimate tilt from angular velocity
                tilt = -Vector3.Dot(_targetRb.angularVelocity, target.up) * turnTilt;
            }
            targetRot *= Quaternion.Euler(0, 0, tilt);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime / rotationSmoothTime);

            // 4. Pro FOV Kick
            if (_cam != null)
            {
                float targetFOV = baseFOV + (speed * speedFOVMultiplier);
                if (_boatController != null && _boatController.IsBoosting) targetFOV += boostFOVBonus;
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, Time.deltaTime * 4f);
            }
        }
    }
}
