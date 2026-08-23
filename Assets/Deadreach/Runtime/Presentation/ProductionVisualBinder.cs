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
        private Transform _weaponSocket;

        public bool HasProductionVisual => _instance != null;
        public GameObject VisualInstance => _instance;

        private void Start()
        {
            BindNow();
        }

        private void LateUpdate()
        {
            if (role == ProductionVisualRole.Survivor && _weaponInstance != null && _weaponSocket != null)
                AlignMountedWeapon();
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
                _weaponSocket = FindNamedTransform(_instance.transform, "WeaponSocket")
                                ?? FindNamedTransform(_instance.transform, "RightHandWeaponSocket");

                if (catalog.PrimaryWeaponPrefab != null && _weaponSocket != null)
                {
                    _weaponInstance = Instantiate(catalog.PrimaryWeaponPrefab, _weaponSocket, false);
                    _weaponInstance.name = "ProductionPrimaryWeapon";

                    // Keep the grip at the animated right hand while DEADREACH owns the visual
                    // weapon basis. Quaternius' imported rifle is authored with the opposite roll
                    // convention to the gameplay basis, so it needs a 180-degree roll around its
                    // own forward/barrel axis. This correction is applied here (and in LateUpdate)
                    // instead of in the prefab, because runtime alignment intentionally overwrites
                    // the prefab root rotation every frame.
                    AlignMountedWeapon();

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

        private void AlignMountedWeapon()
        {
            if (_weaponInstance == null || _weaponSocket == null)
                return;

            // Position stays locked to the animated hand/grip socket.
            _weaponInstance.transform.position = _weaponSocket.position;

            // DEADREACH combat is planar. Quaternius' rifle forward axis is already normalized
            // to local +Z by the production wrapper, but its roll is inverted relative to Unity's
            // world-up basis. Using world DOWN as the LookRotation up-vector is exactly a 180°
            // roll around the barrel axis: same aim direction and hand position, rifle right-side-up.
            var forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f && _instance != null)
            {
                forward = _instance.transform.forward;
                forward.y = 0f;
            }

            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;

            _weaponInstance.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.down);
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
