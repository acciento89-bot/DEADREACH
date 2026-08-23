using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class DeadreachBuildSettings
    {
        public const string BunkerScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        public const string ExpeditionScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";

        [MenuItem("DEADREACH/Project/Repair Scene Build Settings")]
        public static void Repair()
        {
            var bunkerExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(BunkerScenePath) != null;
            var expeditionExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(ExpeditionScenePath) != null;

            if (!bunkerExists && !expeditionExists)
            {
                EditorBuildSettings.scenes = System.Array.Empty<EditorBuildSettingsScene>();
                Debug.LogError("DEADREACH: Neither generated scene exists. Run DEADREACH > Build Complete Vertical Slice 0.1 first.");
                return;
            }

            if (bunkerExists && expeditionExists)
            {
                ConfigureCompleteSlice();
                return;
            }

            if (bunkerExists)
            {
                EditorBuildSettings.scenes = new[]
                {
                    new EditorBuildSettingsScene(BunkerScenePath, true)
                };
                Debug.LogWarning("DEADREACH: Bunker exists, but Dead City is missing. Build the complete vertical slice before testing Deploy.");
                return;
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ExpeditionScenePath, true)
            };
            Debug.LogWarning("DEADREACH: Dead City exists, but Bunker_Hub is missing. Use Build Complete Vertical Slice 0.1 before testing death/extraction return flow.");
        }

        public static bool ConfigureCompleteSlice()
        {
            var bunkerExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(BunkerScenePath) != null;
            var expeditionExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(ExpeditionScenePath) != null;

            if (!bunkerExists || !expeditionExists)
            {
                Debug.LogError($"DEADREACH: Cannot configure complete slice. Bunker exists: {bunkerExists}, Dead City exists: {expeditionExists}.");
                return false;
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BunkerScenePath, true),
                new EditorBuildSettingsScene(ExpeditionScenePath, true)
            };

            Debug.Log("DEADREACH Build Settings repaired: [0] Bunker_Hub, [1] DeadCity_VerticalSlice.");
            return true;
        }
    }
}
