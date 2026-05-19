using UnityEngine;
using System.Collections.Generic;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Base class for arcade racing triggers (Boost, Drift zones).
    /// </summary>
    public abstract class RacingTrigger : MonoBehaviour
    {
        public float multiplier = 1.5f;
        public float duration = 2.0f;
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            // Logic for player/AI interaction
            Debug.Log($"{gameObject.name} triggered by {other.name}");
        }
    }

    public class BoostZone : RacingTrigger
    {
        public Color boostColor = Color.cyan;
        
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            // Apply boost to the vehicle controller
        }
    }

    public class DriftBonusZone : RacingTrigger
    {
        // Increases drift score or handling while inside
    }

    /// <summary>
    /// For barrels, crates, etc. that react to boat collisions.
    /// </summary>
    public class DestructibleProp : MonoBehaviour
    {
        public float health = 10f;
        public GameObject destructionVFX;
        
        public void TakeDamage(float amount)
        {
            health -= amount;
            if (health <= 0) DestroyProp();
        }

        private void DestroyProp()
        {
            if (destructionVFX != null) Instantiate(destructionVFX, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Dynamic signs that can point in different directions or change based on race state.
    /// </summary>
    public class RacingSign : MonoBehaviour
    {
        public enum SignType { Left, Right, Forward, Shortcut }
        public SignType currentType;
        
        public void SetSign(SignType type)
        {
            currentType = type;
            // Adjust mesh or texture UVs if needed
        }
    }

    /// <summary>
    /// Handles crowd animations/reactions based on player proximity or events.
    /// </summary>
    public class CrowdReaction : MonoBehaviour
    {
        public Animator animator;
        public float reactionRadius = 15f;

        private void Update()
        {
            // Detect nearby racers to trigger cheer animations
        }
    }
}
