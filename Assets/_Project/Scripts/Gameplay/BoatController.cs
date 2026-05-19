using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public BoatGameplayConfig gameplayConfig;
        public BoatPhysicsModel feelModel = new BoatPhysicsModel();

        public float acceleration = 2500f;
        public float maxSpeed = 30f;
        public float reverseSpeed = 10f;
        public float steeringTorque = 1500f;
        public float waterDrag = 1f;
        public float angularDrag = 3f;
        public float speedMultiplier = 1.0f;
        
        [Header("Drift & Slippery Feel")]
        [Range(0, 1)] public float lateralDrag = 0.95f; 
        public float driftLateralDrag = 0.6f;
        public float currentLateralDragOverride = -1f;

        [Header("Drift Boost System")]
        public float driftChargeRate = 0.5f;
        public float driftBoostLevel1 = 0.4f; // Blue sparks
        public float driftBoostLevel2 = 0.8f; // Orange sparks
        public float driftBoostLevel3 = 1.2f; // Purple sparks
        public float driftBoostMultiplier = 1.4f;
        public float driftBoostDurationBase = 1.0f;

        private float _currentDriftCharge;
        private int _driftLevel; // 0, 1, 2, 3
        private int _driftDirection; // -1 for Left, 1 for Right, 0 for none

        [Header("Boost System")]
        public float boostMultiplier = 2f;
        public float boostDuration = 3f;
        public float boostCooldown = 5f;
        public float boostCapacity = 1.0f;
        public float boostRefillRate = 0.05f;
        
        private float _currentBoostAmount;
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
        public int DriftLevel => _driftLevel;
        public float CurrentSpeed => _rb != null ? _rb.linearVelocity.magnitude : 0;
        public float BoostProgress => _isBoosting ? _currentBoostTime / boostDuration : _currentBoostAmount / boostCapacity;
        public float BoostMeter => _currentBoostAmount;

        private void Awake()
        {
            ApplyConfig();
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.linearDamping = waterDrag;
            _rb.angularDamping = angularDrag;
            _currentBoostAmount = boostCapacity;
        }

        public void SetInput(float steer, float accel, float brake, bool drift)
        {
            // Detect Drift Entry
            if (drift && !_isDrifting && Mathf.Abs(steer) > 0.1f && CurrentSpeed > 5f)
            {
                StartDrift(steer);
            }
            else if (!drift && _isDrifting)
            {
                EndDrift();
            }

            if (Mathf.Abs(steer) > 0.01f || Mathf.Abs(accel) > 0.01f || Mathf.Abs(brake) > 0.01f)
            {
                // Debug.Log($"[BoatController] Input received: Steer={steer}, Accel={accel}, Brake={brake}, Drift={drift}");
            }
            _steerInput = steer;
            _accelInput = accel;
            _brakeInput = brake;
            // Drift state is managed by Start/End methods
        }

        private void StartDrift(float steer)
        {
            _isDrifting = true;
            _driftDirection = steer > 0 ? 1 : -1;
            _currentDriftCharge = 0f;
            _driftLevel = 0;
            
            // Arcade "Hop" on entry
            _rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
            
        }

        private void EndDrift()
        {
            if (_driftLevel > 0)
            {
                TriggerDriftBoost();
            }

            _isDrifting = false;
            _driftDirection = 0;
            _currentDriftCharge = 0f;
            _driftLevel = 0;
        }

        private void TriggerDriftBoost()
        {
            float duration = driftBoostDurationBase * _driftLevel;
            // Temporary boost override or just use the existing boost logic
            // For now, let's inject it into the boost system but with a shorter duration
            _isBoosting = true;
            _currentBoostTime = duration;
        }

        public void TryBoost()
        {
            if (!_isBoosting && _currentBoostAmount > 0.2f && _boostCooldownTimer <= 0)
            {
                _isBoosting = true;
                _currentBoostTime = boostDuration;
            }
        }

        public void AddBoost(float amount)
        {
            _currentBoostAmount = Mathf.Clamp(_currentBoostAmount + amount, 0, boostCapacity);
        }

        public void Respawn(Vector3 pos, Quaternion rot)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = pos;
            transform.rotation = rot;
        }


        private void ApplyConfig()
        {
            if (gameplayConfig == null) return;

            acceleration = gameplayConfig.acceleration;
            maxSpeed = gameplayConfig.maxSpeed;
            reverseSpeed = gameplayConfig.reverseSpeed;
            steeringTorque = gameplayConfig.steeringTorque;
            waterDrag = gameplayConfig.waterDrag;
            angularDrag = gameplayConfig.angularDrag;
            speedMultiplier = gameplayConfig.speedMultiplier;
            lateralDrag = gameplayConfig.lateralDrag;
            driftLateralDrag = gameplayConfig.driftLateralDrag;
            driftChargeRate = gameplayConfig.driftChargeRate;
            driftBoostLevel1 = gameplayConfig.driftBoostLevel1;
            driftBoostLevel2 = gameplayConfig.driftBoostLevel2;
            driftBoostLevel3 = gameplayConfig.driftBoostLevel3;
            driftBoostMultiplier = gameplayConfig.driftBoostMultiplier;
            driftBoostDurationBase = gameplayConfig.driftBoostDurationBase;
            boostMultiplier = gameplayConfig.boostMultiplier;
            boostDuration = gameplayConfig.boostDuration;
            boostCooldown = gameplayConfig.boostCooldown;
            boostCapacity = gameplayConfig.boostCapacity;
            boostRefillRate = gameplayConfig.boostRefillRate;
            floatStrength = gameplayConfig.floatStrength;
            waterHeight = gameplayConfig.waterHeight;
            targetSpeedStabilization = gameplayConfig.targetSpeedStabilization;
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;
            
            ApplyBuoyancy();
            HandleMovement();
            HandleSteering();
            ApplyLateralDrag();
            HandleBoost();

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
            float speed01 = maxSpeed > 0.01f ? _rb.linearVelocity.magnitude / maxSpeed : 0f;
            float accelFeel = feelModel != null ? feelModel.EvaluateAccelMultiplier(speed01) : 1f;
            float targetAccel = (combinedInput > 0 ? acceleration : reverseSpeed) * speedMultiplier * accelFeel;
            if (_isBoosting) targetAccel *= boostMultiplier;

            if (Mathf.Abs(combinedInput) > 0.05f)
            {
                Vector3 moveForce = transform.forward * combinedInput * targetAccel;
                _rb.AddForce(moveForce, ForceMode.Force);
            }

            float currentMax = _isBoosting ? maxSpeed * boostMultiplier : maxSpeed;
            if (_rb.linearVelocity.magnitude > currentMax)
            {
                float weightResponse = feelModel != null ? feelModel.weightResponse : 0.15f;
                _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, _rb.linearVelocity.normalized * currentMax, weightResponse);
            }
            
            if (Mathf.Abs(combinedInput) < 0.05f)
            {
                float resistance = feelModel != null ? feelModel.waterResistance : 0.08f;
                _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, (targetSpeedStabilization + resistance) * Time.fixedDeltaTime);
            }
        }

        private void HandleSteering()
        {
            float steerVal = _steerInput;
            
            if (_isDrifting)
            {
                // Drift Steering: steer direction is locked to _driftDirection, 
                // but input affects the tightness of the slide
                float steerInfluence = _steerInput * _driftDirection; // 1 if matching, -1 if counter-steering
                
                // Rotation Y influence
                float torque = _driftDirection * steeringTorque * 1.5f;
                if (steerInfluence > 0) torque *= 1.2f; // Tighten slide
                else torque *= 0.5f; // Counter-steer / widen slide

                _rb.AddTorque(transform.up * torque, ForceMode.Force);

                // Charge drift meter based on speed and steer influence
                if (steerInfluence > 0) // Charging only while steering into it or sliding
                {
                    _currentDriftCharge += Time.fixedDeltaTime * driftChargeRate;
                    UpdateDriftLevel();
                }
            }
            else if (Mathf.Abs(steerVal) > 0.05f)
            {
                // Normal Speed-sensitive Steering
                float speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / maxSpeed);
                float steeringFeel = feelModel != null ? feelModel.EvaluateSteeringMultiplier(speedFactor) : 1f;
                float steeringPower = Mathf.Max(0.5f, speedFactor) * steeringFeel; 
                float torque = steerVal * steeringTorque * steeringPower;
                _rb.AddTorque(transform.up * torque, ForceMode.Force);
                
                // Arcade Trick: Rotate velocity vector to match turn
                if (_rb.linearVelocity.magnitude > 1f)
                {
                    float turnAngle = _rb.angularVelocity.y * Time.fixedDeltaTime;
                    _rb.linearVelocity = Quaternion.Euler(0, turnAngle * Mathf.Rad2Deg * 0.5f, 0) * _rb.linearVelocity;
                }
            }
        }

        private void UpdateDriftLevel()
        {
            if (_currentDriftCharge >= driftBoostLevel3) _driftLevel = 3;
            else if (_currentDriftCharge >= driftBoostLevel2) _driftLevel = 2;
            else if (_currentDriftCharge >= driftBoostLevel1) _driftLevel = 1;
            else _driftLevel = 0;
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
                _currentBoostAmount -= (boostCapacity / boostDuration) * Time.fixedDeltaTime;
                
                if (_currentBoostTime <= 0 || _currentBoostAmount <= 0)
                {
                    _isBoosting = false;
                    _boostCooldownTimer = boostCooldown;
                    _currentBoostAmount = Mathf.Max(0, _currentBoostAmount);
                }
            }
            else
            {
                if (_boostCooldownTimer > 0)
                {
                    _boostCooldownTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    // Slow refill when not on cooldown
                    AddBoost(boostRefillRate * Time.fixedDeltaTime);
                }
            }
        }
    }
}
