using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Stable public/menu entry point kept for existing builder callers.
    /// Reuses already generated/validated production prefabs first. The atomic V2 repair path
    /// is only allowed to mutate/import assets when one of those validated prefabs is actually missing.
    /// </summary>
    public static class Production05OperatorArtSetup
    {
        private const string PrefabRoot = "Assets/Deadreach/Art/Production/Prefabs";
        private const string SamPrefabPath = PrefabRoot + "/Survivor_Quaternius_Sam.prefab";
        private const string ScoutPrefabPath = PrefabRoot + "/Survivor_Quaternius_Shaun.prefab";
        private const string WardenPrefabPath = PrefabRoot + "/Survivor_Quaternius_Matt.prefab";

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

            if (sam != null && scout != null && warden != null)
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
                Debug.Log("DEADREACH 0.5 operator art READY: reusing validated Sam / Shaun / Matt production prefabs; destructive repair/import skipped.");
                return true;
            }

            Debug.LogWarning("DEADREACH 0.5 validated operator prefabs are incomplete; entering atomic repair/import path.");
            return Production05OperatorArtSetupV2.EnsureOperatorAssetsReady();
        }
    }
}
