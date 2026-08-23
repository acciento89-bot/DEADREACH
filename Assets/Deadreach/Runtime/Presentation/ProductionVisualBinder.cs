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

                    muzzle = ApplyQuaterniusRifleMountFix(_weaponInstance);
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

        private static Transform ApplyQuaterniusRifleMountFix(GameObject weaponInstance)
        {
            // The generated weapon root belongs to the hand socket and must stay untouched.
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = Vector3.one;

            // The Unity Inspector screenshot that produced the correct visual orientation was
            // the actual MeshFilter object (Cube.010), not the wrapper root / Model container.
            // Set exactly that transform to X=0, Y=0, Z=180 while preserving its generated grip offset.
            MeshFilter meshFilter = null;
            var meshFilters = weaponInstance.GetComponentsInChildren<MeshFilter>(true);
            foreach (var candidate in meshFilters)
            {
                if (candidate != null && candidate.sharedMesh != null)
                {
                    meshFilter = candidate;
                    break;
                }
            }

            if (meshFilter == null)
            {
                Debug.LogError("DEADREACH could not find the Quaternius Rifle MeshFilter at runtime; weapon mount was not modified.");
                return FindNamedTransform(weaponInstance.transform, "MuzzleSocket")
                       ?? FindNamedTransform(weaponInstance.transform, "Muzzle");
            }

            var meshTransform = meshFilter.transform;
            var preservedLocalPosition = meshTransform.localPosition;
            var preservedLocalScale = meshTransform.localScale;

            meshTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            meshTransform.localPosition = preservedLocalPosition;
            meshTransform.localScale = preservedLocalScale;

            // Tie the muzzle directly to the same mesh transform. Quaternius Rifle geometry is
            // authored along local +Z (glTF mesh bounds: Z is the long/barrel axis), so max.z is
            // the barrel tip. This removes the old root-space muzzle mismatch completely.
            var muzzle = FindNamedTransform(weaponInstance.transform, "MuzzleSocket")
                         ?? FindNamedTransform(weaponInstance.transform, "Muzzle");

            if (muzzle == null)
            {
                muzzle = new GameObject("MuzzleSocket").transform;
            }

            muzzle.SetParent(meshTransform, false);
            var bounds = meshFilter.sharedMesh.bounds;
            muzzle.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
            muzzle.localRotation = Quaternion.identity;
            muzzle.localScale = Vector3.one;

            Debug.Log(
                $"DEADREACH Rifle mount fixed on MeshFilter '{meshTransform.name}': " +
                $"localPos={meshTransform.localPosition}, localRot={meshTransform.localEulerAngles}; " +
                $"muzzleLocal={muzzle.localPosition}.");

            return muzzle;
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
