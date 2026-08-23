using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    public static class DeadCityAssetRepair
    {
        private const string SessionKey = "DEADREACH_0_4_ASSET_REPAIR_ATTEMPTED";
        private const string EnvironmentRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Environment/glTF";
        private const string VehicleRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Vehicles/glTF";

        static DeadCityAssetRepair()
        {
            EditorApplication.delayCall += TryAutomaticRepair;
        }

        [MenuItem("DEADREACH/Production/Repair Dead City 0.4 Imports", priority = 41)]
        private static void RepairFromMenu()
        {
            SessionState.EraseBool(SessionKey);
            RepairFailedImports(true);
        }

        private static void TryAutomaticRepair()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryAutomaticRepair;
                return;
            }

            RepairFailedImports(false);
        }

        private static void RepairFailedImports(bool forceAll)
        {
            SessionState.SetBool(SessionKey, true);

            var candidates = new List<string>();
            CollectGltfPaths(EnvironmentRoot, candidates);
            CollectGltfPaths(VehicleRoot, candidates);

            if (candidates.Count == 0)
                return;

            var repaired = 0;
            var alreadyHealthy = 0;

            foreach (var assetPath in candidates)
            {
                var healthy = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null;
                if (healthy && !forceAll)
                {
                    alreadyHealthy++;
                    continue;
                }

                AssetDatabase.ImportAsset(
                    assetPath,
                    ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null)
                    repaired++;
            }

            AssetDatabase.Refresh();

            var remainingFailed = 0;
            foreach (var assetPath in candidates)
            {
                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
                    remainingFailed++;
            }

            if (remainingFailed == 0)
            {
                Debug.Log($"DEADREACH 0.4 asset repair PASSED: {repaired} glTF imports repaired, {alreadyHealthy} already healthy.");
            }
            else
            {
                Debug.LogWarning($"DEADREACH 0.4 asset repair completed with {remainingFailed} failed glTF imports remaining. Inspect the first glTF importer error after scripts compile; do not run the scene generator until these are resolved.");
            }
        }

        private static void CollectGltfPaths(string assetRoot, List<string> output)
        {
            var absoluteRoot = Path.Combine(
                Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty,
                assetRoot.Replace('/', Path.DirectorySeparatorChar));

            if (!Directory.Exists(absoluteRoot))
                return;

            foreach (var file in Directory.GetFiles(absoluteRoot, "*.gltf", SearchOption.TopDirectoryOnly))
            {
                var normalized = file.Replace('\\', '/');
                var assetsIndex = normalized.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
                if (assetsIndex < 0)
                    continue;

                output.Add(normalized.Substring(assetsIndex + 1));
            }
        }
    }
}
