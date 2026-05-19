using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class RiverAtmosphereHooks : MonoBehaviour
    {
        public RiverAtmosphereProfile profile;
        public AudioSource riverAmbience;
        public AudioSource windAmbience;
        public AudioSource birdsAmbience;
        public AudioSource distantBoatAmbience;
        public BoatController playerBoat;
        public float updateInterval = 0.1f;

        private float _nextUpdate;

        private void Update()
        {
            if (playerBoat == null || Time.time < _nextUpdate) return;
            _nextUpdate = Time.time + updateInterval;
            float speed = playerBoat.CurrentSpeed;
            float blend = profile != null ? profile.blendSpeed * Time.deltaTime : 2f * Time.deltaTime;
            float riverTarget = profile != null ? profile.riverBaseVolume : 0.5f;
            float windTarget = profile != null ? profile.windBaseVolume + speed * profile.windSpeedScale : speed * 0.015f;
            float birdsTarget = profile != null ? profile.birdsBaseVolume * (1f - Mathf.Clamp01(speed / 30f)) : 0.15f;
            float distantTarget = profile != null ? Mathf.Clamp01(speed * profile.distantBoatSpeedScale) : 0.1f;

            if (riverAmbience != null) riverAmbience.volume = Mathf.Lerp(riverAmbience.volume, riverTarget, blend);
            if (windAmbience != null) windAmbience.volume = Mathf.Lerp(windAmbience.volume, Mathf.Clamp01(windTarget), blend);
            if (birdsAmbience != null) birdsAmbience.volume = Mathf.Lerp(birdsAmbience.volume, Mathf.Clamp01(birdsTarget), blend);
            if (distantBoatAmbience != null) distantBoatAmbience.volume = Mathf.Lerp(distantBoatAmbience.volume, distantTarget, blend);
        }
    }
}
