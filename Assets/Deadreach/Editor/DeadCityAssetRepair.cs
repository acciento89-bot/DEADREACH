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
        // Version the key whenever repair semantics change so an old failed attempt can never
        // suppress a newer repair pass in the same Unity editor session.
        private const string SessionKey = "DEADREACH_0_4_ASSET_REPAIR_ATTEMPTED_V2";
        private const string EnvironmentRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Environment/glTF";
        private const string VehicleRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Vehicles/glTF";

        private static readonly string[] RequiredEnvironmentAssets =
        {
            "Street_Straight.gltf",
            "Street_Crack1.gltf",
            "Street_Crack2.gltf",
            "Street_4Way.gltf",
            "TrafficBarrier_1.gltf",
            "PlasticBarrier.gltf",
            "StreetLights.gltf",
            "TrafficLight_1.gltf",
            "Container_Green.gltf",
            "Container_Red.gltf",
            "Barrel.gltf",
            "Pallet_Broken.gltf",
            "Pipes.gltf",
            "TrashBag_1.gltf",
            "TrashBag_2.gltf",
            "Blood_1.gltf",
            "Blood_2.gltf",
            "WaterTower.gltf",
            "Wheels_Stack.gltf"
        };

        private static readonly string[] RequiredVehicleAssets =
        {
            "Vehicle_Pickup.gltf",
            "Vehicle_Sports.gltf",
            "Vehicle_Truck.gltf"
        };

        static DeadCityAssetRepair()
        {
            EditorApplication.delayCall += TryAutomaticRepair;
        }

        [MenuItem("DEADREACH/Production/Repair Dead City 0.4 Imports", priority = 41)]
        private static void RepairFromMenu()
        {
            SessionState.EraseBool(SessionKey);
            RepairFailedImports(true);
            ValidateRequiredAssets(true);
        }

        /// <summary>
        /// Hard gate used by the Production Slice 0.4 generator. The scene must never be
        /// generated with silently missing containers/vehicles/landmarks.
        /// </summary>
        public static bool EnsureRequiredAssetsReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogError("DEADREACH 0.4 assets cannot be validated while Unity is compiling/importing. Wait for the Editor to become idle and run Build Production Slice 0.4 again.");
                return false;
            }

            RepairFailedImports(false);
            return ValidateRequiredAssets(true);
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
            ValidateRequiredAssets(false);
        }

        private static void RepairFailedImports(bool forceAll)
        {
            SessionState.SetBool(SessionKey, true);

            var candidates = new List<string>();
            CollectGltfPaths(EnvironmentRoot, candidates);
            CollectGltfPaths(VehicleRoot, candidates);

            if (candidates.Count == 0)
            {
                Debug.LogWarning("DEADREACH 0.4 asset repair found no local glTF files. Run tools/install-quaternius-deadcity-set.ps1 first.");
                return;
            }

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
                Debug.LogWarning($"DEADREACH 0.4 asset repair still has {remainingFailed} failed glTF imports. The 0.4 generator will refuse to build until the required set is healthy.");
            }
        }

        private static bool ValidateRequiredAssets(bool logSuccess)
        {
            var missing = new List<string>();

            ValidateGroup(EnvironmentRoot, RequiredEnvironmentAssets, missing);
            ValidateGroup(VehicleRoot, RequiredVehicleAssets, missing);

            if (missing.Count == 0)
            {
                if (logSuccess)
                    Debug.Log("DEADREACH 0.4 required environment gate PASSED: streets, containers, props and all three vehicles are imported and available.");
                return true;
            }

            Debug.LogError(
                "DEADREACH 0.4 required environment gate FAILED. Build aborted instead of silently omitting production art. Missing/failed imports:\n- " +
                string.Join("\n- ", missing) +
                "\nUse DEADREACH > Production > Repair Dead City 0.4 Imports, then inspect the first remaining glTF importer error if this list does not clear.");
            return false;
        }

        private static void ValidateGroup(string root, IEnumerable<string> names, ICollection<string> missing)
        {
            foreach (var name in names)
            {
                var path = $"{root}/{name}";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
                    missing.Add(path);
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
