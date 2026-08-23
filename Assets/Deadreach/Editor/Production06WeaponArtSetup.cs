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

                EnsureDownloaded(PistolSource, PistolUrl, 40000);
                EnsureDownloaded(SmgSource, SmgUrl, 80000);
                EnsureDownloaded(ShotgunSource, ShotgunUrl, 40000);

                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                var pistolSource = AssetDatabase.LoadAssetAtPath<GameObject>(PistolSource);
                var smgSource = AssetDatabase.LoadAssetAtPath<GameObject>(SmgSource);
                var shotgunSource = AssetDatabase.LoadAssetAtPath<GameObject>(ShotgunSource);
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

        private static void EnsureDownloaded(string assetPath, string url, long minimumBytes)
        {
            var absolute = ToAbsolute(assetPath);
            if (File.Exists(absolute) && new FileInfo(absolute).Length >= minimumBytes)
                return;

            using var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "DEADREACH-Unity-Editor");
            client.DownloadFile(url, absolute);

            var file = new FileInfo(absolute);
            if (!file.Exists || file.Length < minimumBytes)
                throw new IOException($"Downloaded weapon glTF is unexpectedly small: {assetPath}");
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
