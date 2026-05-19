using UnityEngine;
using MienTayDaiChien.Gameplay;

namespace MienTayDaiChien.UI
{
    public class MinimapIcon : MonoBehaviour
    {
        public enum IconType { Player, Opponent, AI, Checkpoint, DirectionArrow }
        public IconType type;
        public SpriteRenderer iconRenderer;
        
        [Header("Colors")]
        public Color playerColor = Color.green;
        public Color opponentColor = Color.red;
        public Color aiColor = Color.orange;
        public Color checkpointColor = Color.blue;
        public Color arrowColor = Color.white;

        private void Start()
        {
            if (MinimapManager.Instance != null)
                MinimapManager.Instance.RegisterIcon(this);
            
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (MinimapManager.Instance != null)
                MinimapManager.Instance.UnregisterIcon(this);
        }

        public void UpdateVisuals()
        {
            if (iconRenderer == null) return;

            switch (type)
            {
                case IconType.Player:
                    iconRenderer.color = playerColor;
                    break;
                case IconType.Opponent:
                    iconRenderer.color = opponentColor;
                    break;
                case IconType.AI:
                    iconRenderer.color = aiColor;
                    break;
                case IconType.Checkpoint:
                    iconRenderer.color = checkpointColor;
                    break;
                case IconType.DirectionArrow:
                    iconRenderer.color = arrowColor;
                    break;
            }
        }

        private void LateUpdate()
        {
            // Keep icons flat and at the right rotation for a top-down minimap
            // Only lock X and Z rotation to keep the sprite facing "up" relative to the map camera
            Vector3 euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(90f, euler.y, 0f);
        }
    }
}
