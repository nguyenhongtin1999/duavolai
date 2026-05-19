using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [CreateAssetMenu(menuName = "MienTayDaiChien/Gameplay/River Atmosphere Profile", fileName = "RiverAtmosphereProfile")]
    public class RiverAtmosphereProfile : ScriptableObject
    {
        [Header("Volumes")]
        [Range(0f, 1f)] public float riverBaseVolume = 0.45f;
        [Range(0f, 1f)] public float windBaseVolume = 0.15f;
        [Range(0f, 1f)] public float birdsBaseVolume = 0.2f;

        [Header("Speed Response")]
        public float windSpeedScale = 0.018f;
        public float distantBoatSpeedScale = 0.012f;

        [Header("Transition")]
        public float blendSpeed = 2.5f;
    }
}
