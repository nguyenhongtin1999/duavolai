using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoostPickup : MonoBehaviour
    {
        public float boostAmount = 0.5f;
        public float respawnTime = 10f;
        public GameObject visualRoot;
        public ParticleSystem collectVFX;
        public AudioClip collectSFX;

        private bool _isAvailable = true;
        private float _respawnTimer;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1.0f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAvailable) return;

            if (other.TryGetComponent<BoatController>(out var boat))
            {
                Collect(boat);
            }
        }

        private void Collect(BoatController boat)
        {
            boat.AddBoost(boostAmount);
            
            _isAvailable = false;
            _respawnTimer = respawnTime;
            
            if (visualRoot != null) visualRoot.SetActive(false);
            if (collectVFX != null) collectVFX.Play();
            if (collectSFX != null && _audioSource != null) _audioSource.PlayOneShot(collectSFX);
            
            Debug.Log($"[BoostPickup] Collected by {boat.name}");
        }

        private void Update()
        {
            if (!_isAvailable)
            {
                _respawnTimer -= Time.deltaTime;
                if (_respawnTimer <= 0)
                {
                    Respawn();
                }
            }
        }

        private void Respawn()
        {
            _isAvailable = true;
            if (visualRoot != null) visualRoot.SetActive(true);
            Debug.Log("[BoostPickup] Respawned");
        }
    }
}
