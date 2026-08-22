using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    internal static class DeadreachUrpBootstrap
    {
        private const string SettingsFolder = "Assets/Deadreach/Settings";
        private const string PipelinePath = SettingsFolder + "/Deadreach_URP.asset";

        static DeadreachUrpBootstrap()
        {
            EditorApplication.delayCall += EnsureConfigured;
        }

        [MenuItem("DEADREACH/Project/Ensure URP Pipeline")]
        public static void EnsureConfigured()
        {
            EnsureSettingsFolder();

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                pipeline.name = "Deadreach_URP";
                AssetDatabase.CreateAsset(pipeline, PipelinePath);

                var rendererData = pipeline.LoadBuiltinRendererData(RendererType.UniversalRenderer);
                if (rendererData != null && !AssetDatabase.Contains(rendererData))
                {
                    rendererData.name = "Deadreach_UniversalRenderer";
                    AssetDatabase.AddObjectToAsset(rendererData, pipeline);
                }

                pipeline.EnsureGlobalSettings();
                EditorUtility.SetDirty(pipeline);
                AssetDatabase.SaveAssets();
            }

            if (GraphicsSettings.defaultRenderPipeline != pipeline)
                GraphicsSettings.defaultRenderPipeline = pipeline;

            if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline != pipeline)
                QualitySettings.renderPipeline = null;

            EditorUtility.SetDirty(pipeline);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureSettingsFolder()
        {
            if (!AssetDatabase.IsValidFolder(SettingsFolder))
                AssetDatabase.CreateFolder("Assets/Deadreach", "Settings");
        }
    }
}
