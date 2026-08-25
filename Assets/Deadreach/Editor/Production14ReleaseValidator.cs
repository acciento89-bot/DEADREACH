using System.Collections.Generic;
using System.Linq;
using Kamilunavo.Deadreach.Core;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production14ReleaseValidator
    {
        private const string BunkerScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        private const string ExpeditionScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";

        [MenuItem("DEADREACH/Validate Production 0.14 Release Readiness", priority = 9)]
        public static void ValidateReleaseReadiness()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            ValidateSceneAsset(BunkerScenePath, SceneFlowService.BunkerSceneName, errors);
            ValidateSceneAsset(ExpeditionScenePath, SceneFlowService.ExpeditionSceneName, errors);
            ValidateBuildSettings(SceneFlowService.BunkerSceneName, errors);
            ValidateBuildSettings(SceneFlowService.ExpeditionSceneName, errors);

            ValidateResource<GameObject>("Production14/Quaternius/Door_Frame_A", "Quaternius Door_Frame_A", errors);
            ValidateResource<GameObject>("Production14/Quaternius/Door_DarkMetal", "Quaternius Door_DarkMetal", errors);

            var wenrexaReady = Production14ExternalUISetup.EnsureReady(false);
            if (!wenrexaReady)
                warnings.Add("Wenrexa UI pack could not be refreshed; the runtime fallback skin remains available.");

            ValidateProjectSettings(errors, warnings);

            foreach (var warning in warnings)
                Debug.LogWarning($"DEADREACH 0.14 RELEASE CHECK WARNING: {warning}");

            if (errors.Count == 0)
            {
                Debug.Log(
                    "DEADREACH 0.14 RELEASE STATIC CHECK: PASS\n" +
                    "- Bunker + expedition scenes present and enabled\n" +
                    "- Production 0.14 Quaternius command-center resources present\n" +
                    "- Wenrexa UI preparation checked\n" +
                    "- Landscape/mobile project settings checked\n" +
                    "Continue with Play Mode deep-action + expedition regression.");
                return;
            }

            Debug.LogError(
                "DEADREACH 0.14 RELEASE STATIC CHECK: FAIL\n- " +
                string.Join("\n- ", errors));
        }

        private static void ValidateSceneAsset(string assetPath, string expectedName, ICollection<string> errors)
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            if (scene == null)
                errors.Add($"Missing scene asset: {assetPath}");
            else if (scene.name != expectedName)
                errors.Add($"Scene name mismatch at {assetPath}: expected {expectedName}, found {scene.name}");
        }

        private static void ValidateBuildSettings(string sceneName, ICollection<string> errors)
        {
            var match = EditorBuildSettings.scenes.FirstOrDefault(scene =>
                scene != null && scene.enabled && System.IO.Path.GetFileNameWithoutExtension(scene.path) == sceneName);
            if (match == null)
                errors.Add($"Build Settings does not contain enabled scene '{sceneName}'.");
        }

        private static void ValidateResource<T>(string resourcePath, string label, ICollection<string> errors) where T : Object
        {
            if (Resources.Load<T>(resourcePath) == null)
                errors.Add($"Missing required resource: {label} ({resourcePath})");
        }

        private static void ValidateProjectSettings(ICollection<string> errors, ICollection<string> warnings)
        {
            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeLeft &&
                PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeRight &&
                PlayerSettings.defaultInterfaceOrientation != UIOrientation.AutoRotation)
            {
                errors.Add("Default interface orientation is not landscape-compatible.");
            }

            if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
            {
                if (PlayerSettings.allowedAutorotateToPortrait || PlayerSettings.allowedAutorotateToPortraitUpsideDown)
                    errors.Add("Portrait autorotation is enabled; DEADREACH must remain landscape-only.");

                if (!PlayerSettings.allowedAutorotateToLandscapeLeft && !PlayerSettings.allowedAutorotateToLandscapeRight)
                    errors.Add("No landscape autorotation direction is enabled.");
            }

            if (PlayerSettings.runInBackground)
                warnings.Add("Run In Background is enabled. Mobile save hardening handles focus loss, but disabling it is preferable for release.");
        }
    }
}
