using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MienTayDaiChien.Core
{
    public class MobileQualityManager : MonoBehaviour
    {
        public enum QualityLevel { Low, Mid, High }

        public MobileQualityConfig qualityConfig;

        public void ApplyPreset(QualityLevel level)
        {
            var urpAsset = UniversalRenderPipeline.asset;
            if (urpAsset == null) return;

            MobileQualityConfig.Preset preset = level switch
            {
                QualityLevel.Low => qualityConfig != null ? qualityConfig.low : new MobileQualityConfig.Preset { shadowDistance = 20f, renderScale = 0.8f, enableWaterDepthFade = false, targetFrameRate = 30 },
                QualityLevel.Mid => qualityConfig != null ? qualityConfig.mid : new MobileQualityConfig.Preset { shadowDistance = 40f, renderScale = 1f, enableWaterDepthFade = true, targetFrameRate = 60 },
                _ => qualityConfig != null ? qualityConfig.high : new MobileQualityConfig.Preset { shadowDistance = 60f, renderScale = 1f, enableWaterDepthFade = true, targetFrameRate = 60 }
            };

            urpAsset.shadowDistance = preset.shadowDistance;
            urpAsset.renderScale = preset.renderScale;
            if (preset.enableWaterDepthFade) Shader.EnableKeyword("_USE_DEPTH_FADE");
            else Shader.DisableKeyword("_USE_DEPTH_FADE");
            Application.targetFrameRate = preset.targetFrameRate;
        }

        private void Start()
        {
            // Default to Mid for most Android devices
            ApplyPreset(QualityLevel.Mid);
        }
    }
}
