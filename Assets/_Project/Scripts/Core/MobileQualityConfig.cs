using UnityEngine;

namespace MienTayDaiChien.Core
{
    [CreateAssetMenu(menuName = "MienTayDaiChien/Core/Mobile Quality Config", fileName = "MobileQualityConfig")]
    public class MobileQualityConfig : ScriptableObject
    {
        [System.Serializable]
        public struct Preset
        {
            public float shadowDistance;
            public float renderScale;
            public bool enableWaterDepthFade;
            public int targetFrameRate;
        }

        public Preset low = new Preset { shadowDistance = 20f, renderScale = 0.8f, enableWaterDepthFade = false, targetFrameRate = 30 };
        public Preset mid = new Preset { shadowDistance = 40f, renderScale = 1f, enableWaterDepthFade = true, targetFrameRate = 60 };
        public Preset high = new Preset { shadowDistance = 60f, renderScale = 1f, enableWaterDepthFade = true, targetFrameRate = 60 };
    }
}
