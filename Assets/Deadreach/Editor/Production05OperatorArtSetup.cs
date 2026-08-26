using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Stable public/menu entry point kept for existing builder callers.
    /// Reuses generated production prefabs only when they still resolve to real body meshes.
    /// If a recovered wrapper exists but its original glTF source was not restored, the wrapper can
    /// load as a GameObject while every MeshFilter/SkinnedMeshRenderer resolves sharedMesh to null.
    /// That state must enter the atomic V2 repair path rather than being accepted as "validated".
    /// </summary>
    public static class Production05OperatorArtSetup
    {
        private const string PrefabRoot = "Assets/Deadreach/Art/Production/Prefabs";
        private const string SamPrefabPath = PrefabRoot + "/Survivor_Quaternius_Sam.prefab";
        private const string ScoutPrefabPath = PrefabRoot + "/Survivor_Quaternius_Shaun.prefab";
        private const string WardenPrefabPath = PrefabRoot + "/Survivor_Quaternius_Matt.prefab";

        private static readonly string[] WeaponTokens =
        {
            "rifle", "smg", "pistol", "shotgun", "gun", "firearm",
            "axe", "knife", "guitar", "spear", "woodenbat", "baseballbat"
        };

        [MenuItem("DEADREACH/Production/Repair 0.5 Operator Art", priority = 24)]
        public static void RepairMenu()
        {
            EnsureOperatorAssetsReady();
        }

        public static bool EnsureOperatorAssetsReady()
        {
            var sam = AssetDatabase.LoadAssetAtPath<GameObject>(SamPrefabPath);
            var scout = AssetDatabase.LoadAssetAtPath<GameObject>(ScoutPrefabPath);
            var warden = AssetDatabase.LoadAssetAtPath<GameObject>(WardenPrefabPath);

            var samReady = HasUsableBodyMesh(sam);
            var scoutReady = HasUsableBodyMesh(scout);
            var wardenReady = HasUsableBodyMesh(warden);

            if (samReady && scoutReady && wardenReady)
            {
                var catalog = ProductionArtBootstrap.EnsureCatalog();
                if (catalog == null)
                {
                    Debug.LogError("DEADREACH 0.5 operator reuse failed: ProductionAssetCatalog unavailable.");
                    return false;
                }

                catalog.ConfigureSurvivorOperators(sam, scout, warden);
                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();
                Debug.Log("DEADREACH 0.5 operator art READY: Sam / Shaun / Matt prefabs contain real body meshes; destructive repair/import skipped.");
                return true;
            }

            Debug.LogWarning(
                $"DEADREACH 0.5 operator art requires atomic repair: " +
                $"SAM={(samReady ? "OK" : "BROKEN")}, " +
                $"RAVEN/Shaun={(scoutReady ? "OK" : "BROKEN")}, " +
                $"BRIGGS/Matt={(wardenReady ? "OK" : "BROKEN")}. " +
                "A wrapper file without a resolvable body mesh is not considered valid.");

            return Production05OperatorArtSetupV2.EnsureOperatorAssetsReady();
        }

        private static bool HasUsableBodyMesh(GameObject prefab)
        {
            if (prefab == null)
                return false;

            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || IsWeaponRenderer(renderer.transform, prefab.transform))
                    continue;

                if (renderer is SkinnedMeshRenderer skinned)
                {
                    if (skinned.sharedMesh != null && skinned.sharedMesh.vertexCount > 0)
                        return true;
                    continue;
                }

                var filter = renderer.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null && filter.sharedMesh.vertexCount > 0)
                    return true;
            }

            return false;
        }

        private static bool IsWeaponRenderer(Transform rendererTransform, Transform visualRoot)
        {
            var current = rendererTransform;
            var depth = 0;
            while (current != null && depth < 8)
            {
                var normalized = NormalizeName(current.name);
                foreach (var token in WeaponTokens)
                {
                    if (normalized.Contains(token))
                        return true;
                }

                if (current == visualRoot)
                    break;

                current = current.parent;
                depth++;
            }

            return false;
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var chars = value.ToCharArray();
            var buffer = new char[chars.Length];
            var length = 0;
            foreach (var character in chars)
            {
                if (!char.IsLetterOrDigit(character))
                    continue;

                buffer[length++] = char.ToLowerInvariant(character);
            }

            return new string(buffer, 0, length);
        }
    }
}
