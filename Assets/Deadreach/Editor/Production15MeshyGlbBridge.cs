using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    public static class Production15MeshyGlbBridge
    {
        private const string AssetRoot = "Assets/Deadreach/Art/Production15/Meshy/Bunker";
        private const string PreparedRoot = AssetRoot + "/Prepared";
        private const string SessionKey = "DEADREACH_0_15_MESHY_GLB_BRIDGE_V1";

        private static readonly string[] Required =
        {
            "Wall_Standard", "Wall_Technical", "Wall_Utility", "Wall_Corner", "Door_Blast",
            "Floor_A", "Pillar_A", "CommandConsole", "HologramTable", "ArsenalRack",
            "WorkshopBench", "SupplyStation", "OperatorBay", "Generator", "Crates",
            "VentUnit", "PowerBox", "LightFixture"
        };

        static Production15MeshyGlbBridge()
        {
            EditorApplication.delayCall += TryAutoPrepare;
        }

        [MenuItem("DEADREACH/Production 0.15/Repair Meshy GLB Imports", priority = 0)]
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
                    Debug.LogError($"DEADREACH 0.15 Meshy asset folder not found: {AssetRoot}");
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

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (failed.Count == 0)
            {
                Debug.Log($"DEADREACH 0.15 Meshy GLB bridge PASSED: {ready}/18 source models prepared for the bunker builder.");
            }
            else if (logResult)
            {
                Debug.LogError(
                    $"DEADREACH 0.15 Meshy GLB bridge could not prepare {failed.Count} model(s): " +
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

            var all = AssetDatabase.LoadAllAssetsAtPath(sourcePath);
            foreach (var asset in all)
            {
                if (asset is GameObject gameObject)
                    return gameObject;
            }

            return null;
        }

        private static string ResolveSourcePath(string stem)
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
            if (AssetDatabase.IsValidFolder(PreparedRoot))
                return;

            AssetDatabase.CreateFolder(AssetRoot, "Prepared");
        }
    }
}
