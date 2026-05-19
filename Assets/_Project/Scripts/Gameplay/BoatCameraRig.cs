using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class BoatCameraRig : MonoBehaviour
    {
        public Transform target;
        public Vector3 followOffset = new Vector3(0, 5, -10);
        public float followLag = 6f;
        public float rotationLag = 6f;
        public float collisionProbeRadius = 0.35f;
        public LayerMask collisionMask = ~0;

        [Header("Feel")]
        public float baseFov = 60f;
        public float speedFovScale = 0.45f;
        public float boostFovBonus = 12f;
        public float driftTilt = 12f;
        public float shakeFrequency = 10f;
        public float shakeAmplitude = 0.08f;

        private Camera _cam;
        private BoatController _boat;
        private float _shakeTimer;
        private float _shakeIntensity;

        public void TriggerShake(float intensity, float duration)
        {
            _shakeIntensity = Mathf.Max(_shakeIntensity, intensity);
            _shakeTimer = Mathf.Max(_shakeTimer, duration);
        }

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (target == null) return;
            if (_boat == null) _boat = target.GetComponent<BoatController>();

            Vector3 desired = target.TransformPoint(followOffset);
            Vector3 lookAt = target.position + target.forward * 5f;

            // collision avoidance
            Vector3 dir = desired - lookAt;
            float dist = dir.magnitude;
            if (dist > 0.01f && Physics.SphereCast(lookAt, collisionProbeRadius, dir.normalized, out RaycastHit hit, dist, collisionMask, QueryTriggerInteraction.Ignore))
            {
                desired = hit.point - dir.normalized * 0.2f;
            }

            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;
                Vector3 shake = new Vector3(
                    Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f,
                    Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f,
                    0f) * (shakeAmplitude * _shakeIntensity);
                desired += transform.TransformDirection(shake);
                if (_shakeTimer <= 0) _shakeIntensity = 0;
            }

            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followLag);

            Quaternion targetRot = Quaternion.LookRotation(lookAt - transform.position);
            float tilt = 0f;
            if (_boat != null && target.TryGetComponent<Rigidbody>(out var rb))
            {
                tilt = -Vector3.Dot(rb.angularVelocity, target.up) * driftTilt;
            }
            targetRot *= Quaternion.Euler(0, 0, tilt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLag);

            if (_boat != null)
            {
                float fov = baseFov + _boat.CurrentSpeed * speedFovScale + (_boat.IsBoosting ? boostFovBonus : 0f);
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, fov, Time.deltaTime * 5f);
            }
        }
    }
}
