using Kamilunavo.Deadreach.Combat;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    public enum ProductionVisualRole
    {
        Survivor = 0,
        Infected = 1
    }

    [DisallowMultipleComponent]
    public sealed class ProductionVisualBinder : MonoBehaviour
    {
        [SerializeField] private ProductionAssetCatalog catalog;
        [SerializeField] private ProductionVisualRole role;
        [SerializeField] private int variantIndex;
        [SerializeField] private Transform visualAnchor;
        [SerializeField] private bool hidePrototypeRenderers = true;

        private GameObject _instance;
        private GameObject _weaponInstance;

        public bool HasProductionVisual => _instance != null;
        public GameObject VisualInstance => _instance;

        private void Start()
        {
            BindNow();
        }

        public void Configure(ProductionVisualRole newRole, int newVariantIndex = 0, ProductionAssetCatalog newCatalog = null)
        {
            role = newRole;
            variantIndex = newVariantIndex;
            if (newCatalog != null)
                catalog = newCatalog;
        }

        public bool BindNow()
        {
            if (_instance != null)
                return true;

            catalog ??= Resources.Load<ProductionAssetCatalog>("Deadreach/ProductionAssetCatalog");
            if (catalog == null)
                return false;

            var prefab = role == ProductionVisualRole.Survivor
                ? catalog.SurvivorPrefab
                : catalog.GetInfectedPrefab(variantIndex);

            if (prefab == null)
                return false;

            if (hidePrototypeRenderers)
                DisablePrototypeRenderers();

            var anchor = visualAnchor != null ? visualAnchor : transform;
            _instance = Instantiate(prefab, anchor, false);
            _instance.name = $"ProductionVisual_{role}";

            if (role == ProductionVisualRole.Survivor)
            {
                _instance.transform.localPosition = catalog.SurvivorLocalPosition;
                _instance.transform.localRotation = Quaternion.Euler(catalog.SurvivorLocalEuler);
                _instance.transform.localScale = Vector3.one * catalog.SurvivorScale;
            }
            else
            {
                _instance.transform.localPosition = catalog.InfectedLocalPosition;
                _instance.transform.localRotation = Quaternion.Euler(catalog.InfectedLocalEuler);
                _instance.transform.localScale = Vector3.one * catalog.InfectedScale;
            }

            RebindAnimationAndWeaponSockets();
            return true;
        }

        private void DisablePrototypeRenderers()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
                renderer.enabled = false;
        }

        private void RebindAnimationAndWeaponSockets()
        {
            var animator = _instance.GetComponentInChildren<Animator>(true);

            if (role == ProductionVisualRole.Survivor)
            {
                GetComponent<PlayerAnimationDriver>()?.SetAnimator(animator);

                Transform muzzle = null;
                var weaponSocket = FindNamedTransform(_instance.transform, "WeaponSocket")
                                   ?? FindNamedTransform(_instance.transform, "RightHandWeaponSocket");

                if (catalog.PrimaryWeaponPrefab != null && weaponSocket != null)
                {
                    _weaponInstance = Instantiate(catalog.PrimaryWeaponPrefab, weaponSocket, false);
                    _weaponInstance.name = "ProductionPrimaryWeapon";

                    ApplyQuaterniusRifleMountFix(_weaponInstance);

                    muzzle = FindNamedTransform(_weaponInstance.transform, "MuzzleSocket")
                             ?? FindNamedTransform(_weaponInstance.transform, "Muzzle");
                }

                muzzle ??= FindNamedTransform(_instance.transform, "MuzzleSocket")
                           ?? FindNamedTransform(_instance.transform, "Muzzle");

                if (muzzle != null)
                    GetComponent<HitscanWeapon>()?.SetMuzzle(muzzle);
            }
            else
            {
                GetComponent<InfectedAnimationDriver>()?.SetAnimator(animator);
            }
        }

        private static void ApplyQuaterniusRifleMountFix(GameObject weaponInstance)
        {
            // IMPORTANT: The successful Unity Inspector correction was made on the generated
            // rifle MODEL child, not on the weapon root attached to the hand. The wrapper already
            // calculated the correct grip offset on Model (about Y -0.02084 / Z -0.00481).
            // Preserve that position exactly and apply only the validated 180° local Z roll.
            // Do not add a LateUpdate/world rotation here; that was the source of the previous
            // regressions and made Inspector edits appear to have no effect.
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = Vector3.one;

            var model = FindNamedTransform(weaponInstance.transform, "Model");
            if (model != null)
            {
                var preservedPosition = model.localPosition;
                model.localRotation = Quaternion.Euler(0f, 0f, 180f);
                model.localPosition = preservedPosition;
            }

            // MuzzleSocket was generated before the visual 180° roll. Mirror its grip-plane X/Y
            // offset around the weapon root so it follows the same roll while retaining barrel Z.
            var muzzle = FindNamedTransform(weaponInstance.transform, "MuzzleSocket")
                         ?? FindNamedTransform(weaponInstance.transform, "Muzzle");
            if (muzzle != null && muzzle.parent == weaponInstance.transform)
            {
                var p = muzzle.localPosition;
                muzzle.localPosition = new Vector3(-p.x, -p.y, p.z);
            }
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
