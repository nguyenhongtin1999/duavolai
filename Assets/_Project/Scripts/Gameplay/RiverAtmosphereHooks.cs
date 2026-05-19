using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class RiverAtmosphereHooks : MonoBehaviour
    {
        public AudioSource riverAmbience;
        public AudioSource windAmbience;
        public BoatController playerBoat;

        [Range(0f,1f)] public float baseRiverVolume = 0.5f;
        public float speedWindVolumeScale = 0.015f;

        private void Update()
        {
            if (playerBoat == null) return;
            float speed = playerBoat.CurrentSpeed;
            if (riverAmbience != null) riverAmbience.volume = baseRiverVolume;
            if (windAmbience != null) windAmbience.volume = Mathf.Clamp01(speed * speedWindVolumeScale);
        }
    }
}
