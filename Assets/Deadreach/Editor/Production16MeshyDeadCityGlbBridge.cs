using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    public static class Production16MeshyDeadCityGlbBridge
    {
        public const string AssetRoot = "Assets/Deadreach/Art/Production16/Meshy/DeadCity";
        public const string PreparedRoot = AssetRoot + "/Prepared";
        private const string SessionKey = "DEADREACH_0_16_MESHY_DEADCITY_GLB_BRIDGE_V1";

        public static readonly string[] Required =
        {
            "RuinedBuilding_A",
            "RuinedBuilding_B",
            "CornerBuilding",
            "CollapsedStorefront",
            "IndustrialBuilding",
            "RubbleLarge",
            "MilitaryCheckpoint"
        };

        public const string OptionalVehicle = "VehicleVan";

        static Production16MeshyDeadCityGlbBridge()
        {
            EditorApplication.delayCall += TryAutoPrepare;
        }

        [MenuItem("DEADREACH/Production 0.16/Repair Meshy Dead City GLB Imports", priority = 10)]
        public static void RepairFromMenu()
        {
            SessionState.EraseBool(SessionKey);
            Prepare(true);
        }

        private static void TryAutoPrepare()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryAutoPrepare;
                return;
            }

            Prepare(false);
        }

        private static void Prepare(bool logResult)
        {
            SessionState.SetBool(SessionKey, true);

            if (!AssetDatabase.IsValidFolder(AssetRoot))
            {
                if (logResult)
                    Debug.LogError($"DEADREACH 0.16 Meshy Dead City asset folder not found: {AssetRoot}");
                return;
            }

            EnsurePreparedFolder();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var ready = 0;
            var failed = new List<string>();
            foreach (var stem in Required)
            {
                if (PrepareOne(stem))
                    ready++;
                else
                    failed.Add(stem);
            }

            var optionalPrepared = false;
            if (!string.IsNullOrEmpty(ResolveSourcePath(OptionalVehicle)))
                optionalPrepared = PrepareOne(OptionalVehicle);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (failed.Count == 0)
            {
                Debug.Log(
                    $"DEADREACH 0.16 Meshy Dead City GLB bridge PASSED: {ready}/{Required.Length} required models prepared" +
                    (optionalPrepared ? " + optional VehicleVan." : "."));
            }
            else if (logResult)
            {
                Debug.LogError(
                    $"DEADREACH 0.16 Meshy Dead City GLB bridge could not prepare {failed.Count} required model(s): " +
                    string.Join(", ", failed) +
                    ". Inspect the first GLB importer error in the Console.");
            }
        }

        private static bool PrepareOne(string stem)
        {
            var preparedPath = $"{PreparedRoot}/{stem}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(preparedPath) != null)
                return true;

            var sourcePath = ResolveSourcePath(stem);
            if (string.IsNullOrEmpty(sourcePath))
                return false;

            var source = LoadSourceGameObject(sourcePath);
            if (source == null)
                return false;

            GameObject instance = null;
            try
            {
                instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
                if (instance == null)
                    instance = UnityEngine.Object.Instantiate(source);
                if (instance == null)
                    return false;

                instance.name = stem;
                var prefab = PrefabUtility.SaveAsPrefabAsset(instance, preparedPath, out var success);
                return success && prefab != null;
            }
            finally
            {
                if (instance != null)
                    UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static GameObject LoadSourceGameObject(string sourcePath)
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source != null)
                return source;

            AssetDatabase.ImportAsset(
                sourcePath,
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source != null)
                return source;

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(sourcePath))
            {
                if (asset is GameObject gameObject)
                    return gameObject;
            }

            return null;
        }

        public static string ResolveSourcePath(string stem)
        {
            var extensions = new[] { ".glb", ".gltf", ".fbx" };
            foreach (var extension in extensions)
            {
                var path = $"{AssetRoot}/{stem}{extension}";
                if (File.Exists(ToAbsoluteProjectPath(path)))
                    return path;
            }
            return null;
        }

        private static string ToAbsoluteProjectPath(string assetPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static void EnsurePreparedFolder()
        {
            if (!AssetDatabase.IsValidFolder(PreparedRoot))
                AssetDatabase.CreateFolder(AssetRoot, "Prepared");
        }
    }
}
