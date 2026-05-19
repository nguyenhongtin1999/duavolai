using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [CreateAssetMenu(menuName = "MienTayDaiChien/Gameplay/Boat Camera Profile", fileName = "BoatCameraProfile")]
    public class BoatCameraProfile : ScriptableObject
    {
        public Vector3 followOffset = new Vector3(0, 5, -10);
        public float followLag = 6f;
        public float rotationLag = 6f;
        public float baseFov = 60f;
        public float speedFovScale = 0.45f;
        public float boostFovBonus = 12f;
        public float driftTilt = 12f;
        public float shakeAmplitude = 0.08f;
        public float shakeFrequency = 10f;
    }
}
