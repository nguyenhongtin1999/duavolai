using UnityEngine;
using UnityEngine.InputSystem;

namespace MienTayDaiChien.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float acceleration = 2500f;
        public float maxSpeed = 30f;
        public float reverseSpeed = 10f;
        public float steeringTorque = 1500f;
        public float waterDrag = 1f;
        public float angularDrag = 3f;
        public float speedMultiplier = 1.0f;
        
        [Header("Drift & Slippery Feel")]
        [Range(0, 1)] public float lateralDrag = 0.95f; // Lower = more slippery
        public float driftLateralDrag = 0.6f;
        public float currentLateralDragOverride = -1f;

        [Header("Boost System")]
        public float boostMultiplier = 2f;
        public float boostDuration = 3f;
        public float boostCooldown = 5f;
        private float _currentBoostTime;
        private float _boostCooldownTimer;
        private bool _isBoosting;

        [Header("Physics & Floating")]
        public float floatStrength = 15f;
        public float waterHeight = 0f;
        public float targetSpeedStabilization = 0.1f;

        private Rigidbody _rb;
        public float mass => _rb != null ? _rb.mass : 1000f;
        private float _steerInput;
private float _accelInput;
        private float _brakeInput;
        private bool _isDrifting;

        public bool IsBoosting => _isBoosting;
        public bool IsDrifting => _isDrifting;
        public float CurrentSpeed => _rb != null ? _rb.linearVelocity.magnitude : 0;
        public float BoostProgress => _isBoosting ? _currentBoostTime / boostDuration : (boostCooldown - _boostCooldownTimer) / boostCooldown;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.linearDamping = waterDrag;
            _rb.angularDamping = angularDrag;
        }

        public void SetInput(float steer, float accel, float brake, bool drift)
        {
            if (Mathf.Abs(steer) > 0.01f || Mathf.Abs(accel) > 0.01f || Mathf.Abs(brake) > 0.01f)
            {
                Debug.Log($"[BoatController] Input received: Steer={steer}, Accel={accel}, Brake={brake}, Drift={drift}");
            }
            _steerInput = steer;
            _accelInput = accel;
            _brakeInput = brake;
            _isDrifting = drift;
        }

        public void TryBoost()
        {
            Debug.Log("[BoatController] Boost Requested");
            if (!_isBoosting && _boostCooldownTimer <= 0)
            {
                _isBoosting = true;
                _currentBoostTime = boostDuration;
            }
        }

        public void Respawn(Vector3 pos, Quaternion rot)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = pos;
            transform.rotation = rot;
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;
            
            ApplyBuoyancy();
            HandleMovement();
            HandleSteering();
            ApplyLateralDrag();
            HandleBoost();

            if (_rb.linearVelocity.magnitude > 0.1f)
            {
                Debug.Log($"[BoatController] Velocity: {_rb.linearVelocity}, Speed: {_rb.linearVelocity.magnitude}");
            }
        }

        private void ApplyBuoyancy()
        {
            float depth = waterHeight - transform.position.y;
            if (depth > 0)
            {
                float force = depth * floatStrength;
                _rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
                _rb.AddForce(-_rb.linearVelocity.y * Vector3.up * 2f, ForceMode.Acceleration);
            }
        }

        private void HandleMovement()
        {
            float combinedInput = _accelInput - _brakeInput;
            float targetAccel = combinedInput > 0 ? acceleration : reverseSpeed;
            targetAccel *= speedMultiplier;
            if (_isBoosting) targetAccel *= boostMultiplier;

            if (Mathf.Abs(combinedInput) > 0.05f)
            {
                Vector3 moveForce = transform.forward * combinedInput * targetAccel;
                _rb.AddForce(moveForce, ForceMode.Force);
            }

            float currentMax = _isBoosting ? maxSpeed * boostMultiplier : maxSpeed;
            if (_rb.linearVelocity.magnitude > currentMax)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * currentMax;
            }
            
            if (Mathf.Abs(combinedInput) < 0.05f)
            {
                _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, targetSpeedStabilization * Time.fixedDeltaTime);
            }
        }

        private void HandleSteering()
        {
            // Arcade Steering: Direct angular velocity for instant response
            float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / maxSpeed);
            float steeringPower = Mathf.Max(0.5f, speedFactor); // Minimum power at low speed
            
            float targetAngularVel = _steerInput * (steeringTorque / mass) * steeringPower;
            if (_isDrifting) targetAngularVel *= 1.5f;

            // Smoothly interpolate to target angular velocity
            _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, transform.up * targetAngularVel, Time.fixedDeltaTime * 10f);

            // Arcade Trick: Rotate the velocity vector to match the boat's turn
            // This ensures the boat "grips" the water and turns its momentum
            if (!_isDrifting && _rb.linearVelocity.magnitude > 1f)
            {
                float turnAngle = _rb.angularVelocity.y * Time.fixedDeltaTime;
                _rb.linearVelocity = Quaternion.Euler(0, turnAngle * Mathf.Rad2Deg * 0.5f, 0) * _rb.linearVelocity;
            }

            if (Mathf.Abs(_steerInput) > 0.05f && Time.frameCount % 60 == 0)
            {
                Debug.Log($"[Steering] Input: {_steerInput:F2}, TargetAngVel: {targetAngularVel:F2}, Current: {_rb.angularVelocity.y:F2}");
            }
        }

        private void ApplyLateralDrag()
        {
            Vector3 lateralVelocity = transform.right * Vector3.Dot(_rb.linearVelocity, transform.right);
            float dragToApply = _isDrifting ? driftLateralDrag : lateralDrag;
            if (currentLateralDragOverride >= 0) dragToApply = currentLateralDragOverride;
            if (_isBoosting) dragToApply = Mathf.Min(1.0f, dragToApply * 1.5f);
            _rb.AddForce(-lateralVelocity * dragToApply, ForceMode.VelocityChange);
        }

        private void HandleBoost()
        {
            if (_isBoosting)
            {
                _currentBoostTime -= Time.fixedDeltaTime;
                if (_currentBoostTime <= 0)
                {
                    _isBoosting = false;
                    _boostCooldownTimer = boostCooldown;
                }
            }
            else if (_boostCooldownTimer > 0)
            {
                _boostCooldownTimer -= Time.fixedDeltaTime;
            }
        }
    }
}
