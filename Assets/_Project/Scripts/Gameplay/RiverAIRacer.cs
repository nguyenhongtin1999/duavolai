using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Netcode;

namespace MienTayDaiChien.Gameplay
{
    [System.Serializable]
    public enum AIPersonality { Beginner, Balanced, Aggressive, Risky }

    [System.Serializable]
    public struct AIProfile
    {
        public float speedMultiplier;
        public float steeringSensitivity;
        public float avoidanceDistance;
        public float boostProbability;
        public float shortcutRisk;
    }

    [RequireComponent(typeof(BoatController))]
    public class RiverAIRacer : NetworkBehaviour
    {
        [Header("AI Configuration")]
        public AIPersonality personality;
        public AIProfile profile;
        public LayerMask obstacleLayer;

        [Header("Rubber Banding")]
        public float catchUpStrength = 2f;
        public float fallBackStrength = 1f;

        private BoatController _boat;
        private RiverSpline _river;
        private RaceProgress _progress;
        private float _currentSplinePos = 0f;
        private float _targetLaneOffset = 0f;
        private float _avoidanceOffset = 0f;

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
            _progress = GetComponent<RaceProgress>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false; // Only run AI logic on the server
                return;
            }

            _river = RaceManager.Instance.riverSpline;
            ApplyPersonality();
        }

        private void ApplyPersonality()
        {
            switch (personality)
            {
                case AIPersonality.Beginner:
                    profile = new AIProfile { speedMultiplier = 0.8f, steeringSensitivity = 0.5f, avoidanceDistance = 15f, boostProbability = 0.1f, shortcutRisk = 0.1f };
                    break;
                case AIPersonality.Aggressive:
                    profile = new AIProfile { speedMultiplier = 1.1f, steeringSensitivity = 1.2f, avoidanceDistance = 5f, boostProbability = 0.8f, shortcutRisk = 0.5f };
                    break;
                case AIPersonality.Risky:
                    profile = new AIProfile { speedMultiplier = 1.0f, steeringSensitivity = 1.0f, avoidanceDistance = 10f, boostProbability = 0.5f, shortcutRisk = 0.9f };
                    break;
                default: // Balanced
                    profile = new AIProfile { speedMultiplier = 1.0f, steeringSensitivity = 1.0f, avoidanceDistance = 10f, boostProbability = 0.4f, shortcutRisk = 0.3f };
                    break;
            }
        }

        private void Update()
        {
            if (!IsServer || _river == null) return;

            HandlePathFollowing();
            HandleObstacleAvoidance();
            HandleRubberBanding();
            HandleBoostUsage();
        }

        private void HandlePathFollowing()
        {
            _currentSplinePos = _progress.distanceOnSpline.Value;

            float lookAhead = 0.05f;
            float targetT = (_currentSplinePos + lookAhead) % 1.0f;

            float3 worldPos, tangent, up;
            _river.Container.Spline.Evaluate(targetT, out worldPos, out tangent, out up);

            Vector3 worldRight = Vector3.Cross((Vector3)up, (Vector3)tangent).normalized;
            Vector3 targetPoint = (Vector3)worldPos + (worldRight * (_targetLaneOffset + _avoidanceOffset));

            Vector3 localTarget = transform.InverseTransformPoint(targetPoint);
            float steerInput = Mathf.Clamp(localTarget.x * profile.steeringSensitivity, -1f, 1f);

            _boat.SetInput(steerInput, 1.0f, 0f, false);
            }

        private void HandleObstacleAvoidance()
        {
            RaycastHit hit;
            _avoidanceOffset = Mathf.Lerp(_avoidanceOffset, 0f, Time.deltaTime * 2f);

            if (Physics.Raycast(transform.position, transform.forward + transform.right * 0.5f, out hit, profile.avoidanceDistance, obstacleLayer))
            {
                _avoidanceOffset = -6f;
            }
            else if (Physics.Raycast(transform.position, transform.forward - transform.right * 0.5f, out hit, profile.avoidanceDistance, obstacleLayer))
            {
                _avoidanceOffset = 6f;
            }
        }

        private void HandleRubberBanding()
        {
            if (RaceManager.Instance == null) return;

            var rankings = RaceManager.Instance.Rankings;
            int myRank = rankings.IndexOf(_progress);
            if (myRank == -1) return;

            float speedAdjustment = 1.0f;
            
            // Logic: back of the pack gets a speed boost, leader gets a slight nerf
            if (myRank > rankings.Count / 2) 
                speedAdjustment += catchUpStrength * 0.1f;
            else if (myRank == 0 && rankings.Count > 1) 
                speedAdjustment -= fallBackStrength * 0.05f;

            _boat.speedMultiplier = profile.speedMultiplier * speedAdjustment;

            // Drift behavior: Reduce lateral drag when making sharp turns
            float angularVel = Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y);
            if (angularVel > 1.5f) // Sharp turn threshold
                _boat.currentLateralDragOverride = 0.5f; // Slipperier
            else
                _boat.currentLateralDragOverride = -1f; // Use default
        }

        private void HandleBoostUsage()
        {
            if (UnityEngine.Random.value < profile.boostProbability * Time.deltaTime)
            {
                _boat.TryBoost();
            }
        }
    }
}
