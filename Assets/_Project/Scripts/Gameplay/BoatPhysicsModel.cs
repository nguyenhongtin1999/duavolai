using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [System.Serializable]
    public class BoatPhysicsModel
    {
        [Header("Feel")]
        public AnimationCurve accelerationBySpeed = AnimationCurve.EaseInOut(0, 1.2f, 1, 0.6f);
        public AnimationCurve steeringBySpeed = AnimationCurve.EaseInOut(0, 1.1f, 1, 0.7f);
        public float weightResponse = 0.15f;
        public float waterResistance = 0.08f;

        public float EvaluateAccelMultiplier(float speed01) => accelerationBySpeed.Evaluate(Mathf.Clamp01(speed01));
        public float EvaluateSteeringMultiplier(float speed01) => steeringBySpeed.Evaluate(Mathf.Clamp01(speed01));
    }
}
