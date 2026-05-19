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

        [Header("Profiles")]
        public BoatCameraProfile lowSpeedProfile;
        public BoatCameraProfile highSpeedProfile;

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
        private float _speedLagVelocity;
        private float _followDistanceVelocity;

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

            float speed01 = _boat != null && _boat.maxSpeed > 0.01f ? Mathf.Clamp01(_boat.CurrentSpeed / _boat.maxSpeed) : 0f;
            ApplyProfileBlend(speed01);

            Vector3 dynamicOffset = followOffset;
            dynamicOffset.z = Mathf.SmoothDamp(dynamicOffset.z, followOffset.z - speed01 * 2.5f, ref _followDistanceVelocity, 0.2f);
            Vector3 desired = target.TransformPoint(dynamicOffset);
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

            float dynamicFollowLag = Mathf.Lerp(followLag * 0.85f, followLag * 1.15f, speed01);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * dynamicFollowLag);

            Quaternion targetRot = Quaternion.LookRotation(lookAt - transform.position);
            float tilt = 0f;
            float terrainTilt = 0f;
            if (_boat != null && target.TryGetComponent<Rigidbody>(out var rb))
            {
                tilt = -Vector3.Dot(rb.angularVelocity, target.up) * driftTilt;
            }

            if (Physics.Raycast(target.position + Vector3.up * 1f, Vector3.down, out RaycastHit groundHit, 5f, collisionMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 flatForward = Vector3.ProjectOnPlane(target.forward, groundHit.normal).normalized;
                if (flatForward.sqrMagnitude > 0.001f)
                {
                    terrainTilt = Vector3.SignedAngle(target.forward, flatForward, target.right) * 0.35f;
                }
            }
            targetRot *= Quaternion.Euler(0, 0, tilt + terrainTilt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLag);

            if (_boat != null)
            {
                float fov = baseFov + _boat.CurrentSpeed * speedFovScale + (_boat.IsBoosting ? boostFovBonus : 0f);
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, fov, Time.deltaTime * 5f);
            }
        }

        private void ApplyProfileBlend(float speed01)
        {
            if (lowSpeedProfile == null || highSpeedProfile == null) return;

            followOffset = Vector3.Lerp(lowSpeedProfile.followOffset, highSpeedProfile.followOffset, speed01);
            followLag = Mathf.Lerp(lowSpeedProfile.followLag, highSpeedProfile.followLag, speed01);
            rotationLag = Mathf.Lerp(lowSpeedProfile.rotationLag, highSpeedProfile.rotationLag, speed01);
            baseFov = Mathf.Lerp(lowSpeedProfile.baseFov, highSpeedProfile.baseFov, speed01);
            speedFovScale = Mathf.Lerp(lowSpeedProfile.speedFovScale, highSpeedProfile.speedFovScale, speed01);
            boostFovBonus = Mathf.Lerp(lowSpeedProfile.boostFovBonus, highSpeedProfile.boostFovBonus, speed01);
            driftTilt = Mathf.Lerp(lowSpeedProfile.driftTilt, highSpeedProfile.driftTilt, speed01);
            shakeAmplitude = Mathf.Lerp(lowSpeedProfile.shakeAmplitude, highSpeedProfile.shakeAmplitude, speed01);
            shakeFrequency = Mathf.Lerp(lowSpeedProfile.shakeFrequency, highSpeedProfile.shakeFrequency, speed01);
        }

    }
}
