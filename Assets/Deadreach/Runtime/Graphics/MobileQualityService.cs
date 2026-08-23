using UnityEngine;

namespace Kamilunavo.Deadreach.Graphics
{
    public enum GraphicsPreset
    {
        Performance = 0,
        Balanced = 1,
        Ultra = 2
    }

    public static class MobileQualityService
    {
        private const string PresetKey = "DEADREACH.GraphicsPreset";

        public static GraphicsPreset Current { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var preset = PlayerPrefs.HasKey(PresetKey)
                ? (GraphicsPreset)Mathf.Clamp(PlayerPrefs.GetInt(PresetKey), 0, 2)
                : DetectRecommendedPreset();

            Apply(preset, false);
        }

        public static GraphicsPreset DetectRecommendedPreset()
        {
            var memoryMb = SystemInfo.systemMemorySize;
            var graphicsMemoryMb = SystemInfo.graphicsMemorySize;

            if (memoryMb > 0 && memoryMb <= 4096)
                return GraphicsPreset.Performance;

            if ((memoryMb >= 8000 || memoryMb <= 0) && (graphicsMemoryMb >= 3000 || graphicsMemoryMb <= 0))
                return GraphicsPreset.Ultra;

            return GraphicsPreset.Balanced;
        }

        public static void Apply(GraphicsPreset preset, bool persist = true)
        {
            Current = preset;
            QualitySettings.vSyncCount = 0;

            switch (preset)
            {
                case GraphicsPreset.Performance:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadowDistance = 24f;
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    QualitySettings.lodBias = 0.85f;
                    QualitySettings.antiAliasing = 0;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;

                case GraphicsPreset.Ultra:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadowDistance = 55f;
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    QualitySettings.lodBias = 1.6f;
                    QualitySettings.antiAliasing = 4;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;

                default:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadowDistance = 38f;
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    QualitySettings.lodBias = 1.15f;
                    QualitySettings.antiAliasing = 2;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
            }

            if (persist)
            {
                PlayerPrefs.SetInt(PresetKey, (int)preset);
                PlayerPrefs.Save();
            }
        }
    }
}
