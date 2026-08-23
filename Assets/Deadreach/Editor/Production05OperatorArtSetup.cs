using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production05OperatorArtSetup
    {
        private const string SourceRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF";
        private const string PrefabRoot = "Assets/Deadreach/Art/Production/Prefabs";
        private const string AtlasMaterialPath = "Assets/Deadreach/Art/Production/Materials/Quaternius_ZombieAtlas.mat";
        private const string SamPrefabPath = PrefabRoot + "/Survivor_Quaternius_Sam.prefab";
        private const string ScoutSourcePath = SourceRoot + "/Survivor_Lis.gltf";
        private const string WardenSourcePath = SourceRoot + "/Survivor_Matt.gltf";
        private const string ScoutPrefabPath = PrefabRoot + "/Survivor_Quaternius_Lis.prefab";
        private const string WardenPrefabPath = PrefabRoot + "/Survivor_Quaternius_Matt.prefab";

        private const string ScoutUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Lis_SingleWeapon.gltf";
        private const string WardenUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Matt_SingleWeapon.gltf";

        [MenuItem("DEADREACH/Production/Repair 0.5 Operator Art", priority = 24)]
        public static void RepairMenu()
        {
            EnsureOperatorAssetsReady();
        }

        public static bool EnsureOperatorAssetsReady()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF"));

            if (!EnsureSource(ScoutSourcePath, ScoutUrl, 100000))
                return false;
            if (!EnsureSource(WardenSourcePath, WardenUrl, 100000))
                return false;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            var samPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SamPrefabPath);
            var atlasMaterial = AssetDatabase.LoadAssetAtPath<Material>(AtlasMaterialPath);
            if (samPrefab == null || atlasMaterial == null)
            {
                Debug.LogError(
                    "DEADREACH 0.5 operator setup requires the validated Sam production prefab and Quaternius atlas material. " +
                    "Run DEADREACH > Production > Setup Quaternius Starter Art once if either baseline asset is missing.");
                return false;
            }

            var sharedController = samPrefab.GetComponentInChildren<Animator>(true)?.runtimeAnimatorController;
            var scoutPrefab = BuildOperatorWrapper(ScoutSourcePath, ScoutPrefabPath, "Survivor_Quaternius_Lis", atlasMaterial, sharedController);
            var wardenPrefab = BuildOperatorWrapper(WardenSourcePath, WardenPrefabPath, "Survivor_Quaternius_Matt", atlasMaterial, sharedController);
            if (scoutPrefab == null || wardenPrefab == null)
                return false;

            var catalog = ProductionArtBootstrap.EnsureCatalog();
            catalog.ConfigureSurvivorOperators(samPrefab, scoutPrefab, wardenPrefab);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("DEADREACH 0.5 operator art READY: SAM=Sam, RAVEN=Lis, BRIGGS=Matt. All use artist-authored SingleWeapon rigs.");
            return true;
        }

        private static bool EnsureSource(string assetPath, string url, long minimumBytes)
        {
            var absolutePath = AssetPathToAbsolute(assetPath);
            if (File.Exists(absolutePath) && new FileInfo(absolutePath).Length >= minimumBytes)
            {
                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null;
            }

            try
            {
                Debug.Log($"DEADREACH downloading missing Quaternius operator art: {Path.GetFileName(assetPath)}");
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "DEADREACH-Unity-Operator-Art-Bootstrap";
                    client.DownloadFile(url, absolutePath);
                }

                var file = new FileInfo(absolutePath);
                if (!file.Exists || file.Length < minimumBytes)
                    throw new IOException($"Downloaded operator glTF is unexpectedly small ({(file.Exists ? file.Length : 0)} bytes).");

                NormalizeAtlasReference(absolutePath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
                    throw new InvalidOperationException("Unity imported the file but no GameObject asset is available yet.");

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"DEADREACH could not prepare operator asset '{assetPath}': {exception.Message}");
                return false;
            }
        }

        private static GameObject BuildOperatorWrapper(
            string sourcePath,
            string prefabPath,
            string prefabName,
            Material atlasMaterial,
            RuntimeAnimatorController sharedController)
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source == null)
            {
                Debug.LogError($"DEADREACH operator source is not imported: {sourcePath}");
                return null;
            }

            var root = new GameObject(prefabName);
            try
            {
                var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
                model.name = "Model";
                model.transform.SetParent(root.transform, false);

                foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                    UnityEngine.Object.DestroyImmediate(collider);

                foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    var slotCount = Mathf.Max(1, renderer.sharedMaterials?.Length ?? 0);
                    var materials = new Material[slotCount];
                    for (var i = 0; i < materials.Length; i++)
                        materials[i] = atlasMaterial;
                    renderer.sharedMaterials = materials;
                }

                var animator = model.GetComponentInChildren<Animator>(true);
                if (animator == null)
                    animator = model.AddComponent<Animator>();
                animator.runtimeAnimatorController = sharedController;
                animator.applyRootMotion = false;

                var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                if (prefab == null)
                    Debug.LogError($"DEADREACH failed to create operator prefab at {prefabPath}.");
                return prefab;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void NormalizeAtlasReference(string absolutePath)
        {
            var text = File.ReadAllText(absolutePath);
            var updated = Regex.Replace(
                text,
                "(\\\"uri\\\"\\s*:\\s*\\\")[^\\\"]*Zombie_Atlas\\.png(\\\")",
                "$1Zombie_Atlas.png$2",
                RegexOptions.IgnoreCase);

            if (updated == text)
                return;

            File.WriteAllText(absolutePath, updated, new UTF8Encoding(false));
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            var relative = assetPath.Replace("Assets/", string.Empty).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relative);
        }
    }
}
