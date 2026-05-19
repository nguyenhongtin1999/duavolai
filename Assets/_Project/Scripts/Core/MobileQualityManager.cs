using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MienTayDaiChien.Core
{
    public class MobileQualityManager : MonoBehaviour
    {
        public enum QualityLevel { Low, Mid, High }

        public void ApplyPreset(QualityLevel level)
        {
            var urpAsset = UniversalRenderPipeline.asset;
            if (urpAsset == null) return;

            switch (level)
            {
                case QualityLevel.Low:
                    urpAsset.shadowDistance = 20f;
                    urpAsset.renderScale = 0.8f;
                    Shader.DisableKeyword("_USE_DEPTH_FADE"); // Disable water depth for perf
                    Application.targetFrameRate = 30;
                    break;
                case QualityLevel.Mid:
                    urpAsset.shadowDistance = 40f;
                    urpAsset.renderScale = 1.0f;
                    Shader.EnableKeyword("_USE_DEPTH_FADE");
                    Application.targetFrameRate = 60;
                    break;
                case QualityLevel.High:
                    urpAsset.shadowDistance = 60f;
                    urpAsset.renderScale = 1.0f;
                    Shader.EnableKeyword("_USE_DEPTH_FADE");
                    Application.targetFrameRate = 60;
                    break;
            }
        }

        private void Start()
        {
            // Default to Mid for most Android devices
            ApplyPreset(QualityLevel.Mid);
        }
    }
}
