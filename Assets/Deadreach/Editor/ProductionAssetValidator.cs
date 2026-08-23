using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class ProductionAssetValidator
    {
        [MenuItem("DEADREACH/Production/Validate Asset Catalog")]
        private static void ValidateMenu()
        {
            Validate(ProductionArtBootstrap.EnsureCatalog());
        }

        public static bool Validate(ProductionAssetCatalog catalog)
        {
            if (catalog == null)
            {
                Debug.LogError("DEADREACH production asset catalog is missing.");
                return false;
            }

            var valid = true;

            if (catalog.SurvivorPrefab == null)
            {
                Debug.LogWarning("DEADREACH production survivor prefab is not assigned yet. Prototype survivor will remain visible.");
                valid = false;
            }
            else
            {
                valid &= RequireAnimator(catalog.SurvivorPrefab, "Survivor");
                valid &= RequireAnySocket(catalog.SurvivorPrefab, "Survivor", "WeaponSocket", "RightHandWeaponSocket");
            }

            if (catalog.PrimaryWeaponPrefab == null)
            {
                Debug.LogWarning("DEADREACH production primary weapon prefab is not assigned yet.");
                valid = false;
            }
            else
            {
                valid &= RequireAnySocket(catalog.PrimaryWeaponPrefab, "Primary weapon", "MuzzleSocket", "Muzzle");
            }

            if (catalog.InfectedPrefabCount <= 0)
            {
                Debug.LogWarning("DEADREACH production infected prefabs are not assigned yet. Prototype infected will remain visible.");
                valid = false;
            }
            else
            {
                for (var i = 0; i < catalog.InfectedPrefabCount; i++)
                {
                    var prefab = catalog.GetInfectedPrefab(i);
                    if (prefab == null)
                    {
                        Debug.LogError($"DEADREACH infected variant {i} is null in the production asset catalog.");
                        valid = false;
                        continue;
                    }

                    valid &= RequireAnimator(prefab, $"Infected variant {i}");
                }
            }

            if (valid)
                Debug.Log("DEADREACH production asset catalog validation PASSED.");
            else
                Debug.LogWarning("DEADREACH production asset catalog validation is incomplete. Gameplay can still run with prototype fallback visuals.");

            return valid;
        }

        private static bool RequireAnimator(GameObject prefab, string label)
        {
            if (prefab.GetComponentInChildren<Animator>(true) != null)
                return true;

            Debug.LogError($"DEADREACH {label} prefab '{prefab.name}' has no Animator in its hierarchy.", prefab);
            return false;
        }

        private static bool RequireAnySocket(GameObject prefab, string label, params string[] names)
        {
            foreach (var name in names)
            {
                if (FindNamedTransform(prefab.transform, name) != null)
                    return true;
            }

            Debug.LogError($"DEADREACH {label} prefab '{prefab.name}' is missing required socket: {string.Join(" or ", names)}.", prefab);
            return false;
        }

        private static Transform FindNamedTransform(Transform root, string targetName)
        {
            if (root.name == targetName)
                return root;

            for (var i = 0; i < root.childCount; i++)
            {
                var found = FindNamedTransform(root.GetChild(i), targetName);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
