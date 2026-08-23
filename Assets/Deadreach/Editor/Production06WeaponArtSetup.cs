using System;
using System.IO;
using System.Net;
using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production06WeaponArtSetup
    {
        private const string SourceRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF";
        private const string PrefabRoot = "Assets/Deadreach/Art/Production/Prefabs";
        private const string AtlasMaterialPath = "Assets/Deadreach/Art/Production/Materials/Quaternius_ZombieAtlas.mat";

        private const string RiflePrefabPath = PrefabRoot + "/Weapon_Quaternius_Rifle.prefab";
        private const string PistolSource = SourceRoot + "/Weapon_Pistol_06.gltf";
        private const string SmgSource = SourceRoot + "/Weapon_SMG_06.gltf";
        private const string ShotgunSource = SourceRoot + "/Weapon_Shotgun_06.gltf";

        private const string PistolUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Weapons/glTF/Pistol.gltf";
        private const string SmgUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Weapons/glTF/SMG.gltf";
        private const string ShotgunUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Weapons/glTF/Shotgun.gltf";

        [MenuItem("DEADREACH/Production/Setup Weapon Families 0.6", priority = 24)]
        public static void SetupMenu()
        {
            EnsureReady();
        }

        public static bool EnsureReady()
        {
            try
            {
                Directory.CreateDirectory(ToAbsolute(SourceRoot));
                Directory.CreateDirectory(ToAbsolute(PrefabRoot));

                // Import every newly downloaded ScriptedImporter asset explicitly. A global
                // AssetDatabase.Refresh here used to make glTFast discover three files that
                // had just been written directly into Assets, which was vulnerable to an
                // asset-refresh/import race on Unity 6. Stage downloads outside Assets,
                // validate them first, then replace the finished source and import only that
                // exact asset synchronously.
                var pistolSource = EnsureImportedSource(PistolSource, PistolUrl, 40000);
                var smgSource = EnsureImportedSource(SmgSource, SmgUrl, 80000);
                var shotgunSource = EnsureImportedSource(ShotgunSource, ShotgunUrl, 40000);
                var riflePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RiflePrefabPath);
                var atlasMaterial = AssetDatabase.LoadAssetAtPath<Material>(AtlasMaterialPath);

                if (pistolSource == null || smgSource == null || shotgunSource == null || riflePrefab == null || atlasMaterial == null)
                {
                    Debug.LogError("DEADREACH 0.6 weapon-family setup failed: one or more Quaternius weapon sources, the validated Rifle prefab, or atlas material are unavailable.");
                    return false;
                }

                var pistol = BuildPreviewPrefab(pistolSource, "Weapon_Quaternius_Pistol", atlasMaterial);
                var smg = BuildPreviewPrefab(smgSource, "Weapon_Quaternius_SMG", atlasMaterial);
                var shotgun = BuildPreviewPrefab(shotgunSource, "Weapon_Quaternius_Shotgun", atlasMaterial);

                if (pistol == null || smg == null || shotgun == null)
                {
                    Debug.LogError("DEADREACH 0.6 weapon-family setup failed: one or more production weapon preview prefabs could not be generated. Catalog was not modified.");
                    return false;
                }

                var catalog = ProductionArtBootstrap.EnsureCatalog();
                if (catalog == null)
                {
                    Debug.LogError("DEADREACH 0.6 weapon-family setup failed: ProductionAssetCatalog could not be created or loaded.");
                    return false;
                }

                catalog.ConfigureWeaponFamilies(riflePrefab, smg, pistol, shotgun);
                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();

                Debug.Log("DEADREACH 0.6 weapon family art READY: Rifle / SMG / Pistol / Shotgun standalone Arsenal models configured.");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"DEADREACH 0.6 weapon-family setup failed: {exception}");
                return false;
            }
        }

        private static GameObject EnsureImportedSource(string assetPath, string url, long minimumBytes)
        {
            var alreadyImported = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (alreadyImported != null && IsValidSelfContainedGltf(ToAbsolute(assetPath), minimumBytes))
                return alreadyImported;

            // If a previous 0.6 attempt left a source behind after glTFast failed to import
            // it, do not immediately re-import that same on-disk payload. Fetch a fresh,
            // fully completed copy into Library first and only then expose it to Assets.
            DownloadValidatedSource(assetPath, url, minimumBytes);

            AssetDatabase.ImportAsset(
                assetPath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            var imported = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (imported == null)
            {
                throw new InvalidOperationException(
                    $"glTFast did not produce a GameObject for '{assetPath}' after a validated staged download and explicit synchronous import. " +
                    "Select the glTF asset in Unity and inspect the importer details for the underlying glTFast error.");
            }

            return imported;
        }

        private static void DownloadValidatedSource(string assetPath, string url, long minimumBytes)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var stagingRoot = Path.Combine(projectRoot, "Library", "Deadreach", "Production06Downloads");
            Directory.CreateDirectory(stagingRoot);

            var staged = Path.Combine(stagingRoot, Path.GetFileName(assetPath) + ".download");
            if (File.Exists(staged))
                File.Delete(staged);

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.UserAgent, "DEADREACH-Unity-Editor");
                    client.DownloadFile(url, staged);
                }

                if (!IsValidSelfContainedGltf(staged, minimumBytes))
                {
                    var size = File.Exists(staged) ? new FileInfo(staged).Length : 0;
                    throw new IOException(
                        $"Downloaded weapon glTF failed validation: {assetPath} ({size} bytes). " +
                        "Expected a self-contained glTF 2.0 payload with an embedded binary buffer.");
                }

                var absolute = ToAbsolute(assetPath);
                Directory.CreateDirectory(Path.GetDirectoryName(absolute) ?? throw new InvalidOperationException("Weapon source directory could not be resolved."));

                // Keep Unity's automatic watcher away from the destination while the complete
                // staged file replaces it. The matching Allow call is guaranteed by finally.
                AssetDatabase.DisallowAutoRefresh();
                try
                {
                    File.Copy(staged, absolute, true);
                }
                finally
                {
                    AssetDatabase.AllowAutoRefresh();
                }
            }
            finally
            {
                if (File.Exists(staged))
                    File.Delete(staged);
            }
        }

        private static bool IsValidSelfContainedGltf(string absolutePath, long minimumBytes)
        {
            if (!File.Exists(absolutePath))
                return false;

            var file = new FileInfo(absolutePath);
            if (file.Length < minimumBytes)
                return false;

            var text = File.ReadAllText(absolutePath);
            return text.IndexOf("\"asset\"", StringComparison.Ordinal) >= 0
                   && text.IndexOf("\"version\"", StringComparison.Ordinal) >= 0
                   && text.IndexOf("\"2.0\"", StringComparison.Ordinal) >= 0
                   && text.IndexOf("\"buffers\"", StringComparison.Ordinal) >= 0
                   && text.IndexOf("data:application/octet-stream;base64,", StringComparison.Ordinal) >= 0;
        }

        private static GameObject BuildPreviewPrefab(GameObject source, string prefabName, Material material)
        {
            var root = new GameObject(prefabName);
            try
            {
                var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
                if (model == null)
                    return null;

                model.name = "Model";
                model.transform.SetParent(root.transform, false);

                if (PrefabUtility.IsPartOfPrefabInstance(model))
                    PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                    UnityEngine.Object.DestroyImmediate(collider);

                foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    var slots = Mathf.Max(1, renderer.sharedMaterials?.Length ?? 0);
                    var materials = new Material[slots];
                    for (var i = 0; i < slots; i++)
                        materials[i] = material;
                    renderer.sharedMaterials = materials;
                }

                var prefabPath = $"{PrefabRoot}/{prefabName}.prefab";
                return PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static string ToAbsolute(string assetPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
        }
    }
}
