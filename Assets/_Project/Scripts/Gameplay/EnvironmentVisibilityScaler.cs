using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class EnvironmentVisibilityScaler : MonoBehaviour
    {
        public Transform player;
        public float disableDistance = 140f;
        public float checkInterval = 0.5f;
        public bool disableRenderers = true;
        public bool disableColliders = true;

        private Renderer[] _renderers;
        private Collider[] _colliders;
        private float _nextCheck;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _colliders = GetComponentsInChildren<Collider>(true);
        }

        private void Update()
        {
            if (player == null || Time.time < _nextCheck) return;
            _nextCheck = Time.time + checkInterval;

            bool active = Vector3.SqrMagnitude(player.position - transform.position) <= disableDistance * disableDistance;

            if (disableRenderers)
            {
                for (int i = 0; i < _renderers.Length; i++) _renderers[i].enabled = active;
            }

            if (disableColliders)
            {
                for (int i = 0; i < _colliders.Length; i++) _colliders[i].enabled = active;
            }
        }
    }
}
