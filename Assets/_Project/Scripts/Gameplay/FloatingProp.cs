using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class FloatingProp : MonoBehaviour
    {
        public float floatingStrength = 5f;
        public float waterHeight = 0f;
        public float bobSpeed = 1f;
        public float bobAmplitude = 0.1f;
        
        private Rigidbody _rb;
        private float _offset;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _offset = Random.value * Mathf.PI * 2f;
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;

            float hoverHeight = waterHeight + Mathf.Sin(Time.time * bobSpeed + _offset) * bobAmplitude;
            float force = (hoverHeight - transform.position.y) * floatingStrength;
            
            _rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
        }
    }
}
