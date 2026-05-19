using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    public class BoatPresentationRefs : MonoBehaviour
    {
        public BoatCameraRig cameraRig;
        public GameObject speedLinesVfx;

        private void Awake()
        {
            if (cameraRig == null)
            {
                cameraRig = Object.FindFirstObjectByType<BoatCameraRig>();
            }
        }
    }
}
