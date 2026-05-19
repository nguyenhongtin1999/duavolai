using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class AtmosphereZone : MonoBehaviour
    {
        [Range(0f, 1f)] public float riverVolumeOverride = 0.55f;
        [Range(0f, 1f)] public float birdsVolumeOverride = 0.25f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<BoatController>(out var boat)) return;
            var hooks = Object.FindFirstObjectByType<RiverAtmosphereHooks>();
            if (hooks == null || hooks.profile == null) return;

            hooks.profile.riverBaseVolume = riverVolumeOverride;
            hooks.profile.birdsBaseVolume = birdsVolumeOverride;
        }
    }
}
