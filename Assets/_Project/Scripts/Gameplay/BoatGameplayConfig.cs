using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [CreateAssetMenu(menuName = "MienTayDaiChien/Gameplay/Boat Gameplay Config", fileName = "BoatGameplayConfig")]
    public class BoatGameplayConfig : ScriptableObject
    {
        [Header("Movement")]
        public float acceleration = 2500f;
        public float maxSpeed = 30f;
        public float reverseSpeed = 10f;
        public float steeringTorque = 1500f;
        public float waterDrag = 1f;
        public float angularDrag = 3f;
        public float speedMultiplier = 1f;

        [Header("Drift")]
        [Range(0, 1)] public float lateralDrag = 0.95f;
        public float driftLateralDrag = 0.6f;
        public float driftChargeRate = 0.5f;
        public float driftBoostLevel1 = 0.4f;
        public float driftBoostLevel2 = 0.8f;
        public float driftBoostLevel3 = 1.2f;
        public float driftBoostMultiplier = 1.4f;
        public float driftBoostDurationBase = 1f;

        [Header("Boost")]
        public float boostMultiplier = 2f;
        public float boostDuration = 3f;
        public float boostCooldown = 5f;
        public float boostCapacity = 1f;
        public float boostRefillRate = 0.05f;

        [Header("Floating")]
        public float floatStrength = 15f;
        public float waterHeight = 0f;
        public float targetSpeedStabilization = 0.1f;
    }
}
