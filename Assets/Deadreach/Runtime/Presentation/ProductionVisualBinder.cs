using System;
using System.Linq;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Persistence;
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

        private static readonly string[] EmbeddedFirearmTokens =
        {
            "rifle",
            "smg",
            "pistol",
            "shotgun",
            "gun",
            "firearm"
        };

        private GameObject _instance;

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
                ? catalog.GetSurvivorPrefab(SaveService.Data.selectedCharacterId)
                : catalog.GetInfectedPrefab(variantIndex);

            if (prefab == null)
                return false;

            if (hidePrototypeRenderers)
                DisablePrototypeRenderers();

            var anchor = visualAnchor != null ? visualAnchor : transform;
            _instance = Instantiate(prefab, anchor, false);
            _instance.name = role == ProductionVisualRole.Survivor
                ? $"ProductionVisual_{role}_{SaveService.Data.selectedCharacterId}"
                : $"ProductionVisual_{role}";

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

            RebindPresentation();
            return true;
        }

        private void DisablePrototypeRenderers()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
                renderer.enabled = false;
        }

        private void RebindPresentation()
        {
            var animator = _instance.GetComponentInChildren<Animator>(true);

            if (role == ProductionVisualRole.Survivor)
            {
                GetComponent<PlayerAnimationDriver>()?.SetAnimator(animator);

                // Production operators keep a weapon authored on the character rig. Shaun uses his
                // SingleWeapon SMG; Matt's full export is filtered by the Editor wrapper to one Rifle.
                // Never mount a second external weapon onto an imported hand basis.
                var muzzle = BindEmbeddedSingleWeaponMuzzle();
                if (muzzle != null)
                {
                    GetComponent<HitscanWeapon>()?.SetMuzzle(muzzle);
                }
                else
                {
                    Debug.LogWarning(
                        "DEADREACH could not find an artist-rigged embedded firearm renderer in the selected operator. " +
                        "No external rifle was mounted; HitscanWeapon will keep its safe fallback origin.");
                }
            }
            else
            {
                GetComponent<InfectedAnimationDriver>()?.SetAnimator(animator);
            }
        }

        private Transform BindEmbeddedSingleWeaponMuzzle()
        {
            var firearmRenderer = FindEmbeddedFirearmRenderer(_instance);
            if (firearmRenderer == null)
                return null;

            // Old Sam prefabs may have the accepted embedded pistol disabled by their historical
            // wrapper. FindEmbeddedFirearmRenderer only falls back to disabled renderers when there
            // is no enabled firearm at all, so intentionally hidden Matt weapons stay hidden.
            firearmRenderer.enabled = true;

            var existingMuzzle = FindNamedTransform(firearmRenderer.transform, "MuzzleSocket_Embedded");
            if (existingMuzzle != null)
                return existingMuzzle;

            if (!TryGetRendererMeshBounds(firearmRenderer, out var localBounds))
            {
                Debug.LogWarning(
                    $"DEADREACH found embedded weapon renderer '{firearmRenderer.name}' but could not resolve its mesh bounds.");
                return null;
            }

            var muzzle = new GameObject("MuzzleSocket_Embedded").transform;
            muzzle.SetParent(firearmRenderer.transform, false);

            var center = localBounds.center;
            var size = localBounds.size;

            var axis = 0;
            if (size.y > size.x && size.y >= size.z)
                axis = 1;
            else if (size.z > size.x && size.z > size.y)
                axis = 2;

            var negative = center;
            var positive = center;
            SetAxis(ref negative, axis, GetAxis(localBounds.min, axis));
            SetAxis(ref positive, axis, GetAxis(localBounds.max, axis));

            var negativeWorld = firearmRenderer.transform.TransformPoint(negative);
            var positiveWorld = firearmRenderer.transform.TransformPoint(positive);
            var bodyCenter = GetSurvivorBodyCenter();

            var tipLocal = Vector3.SqrMagnitude(positiveWorld - bodyCenter) >= Vector3.SqrMagnitude(negativeWorld - bodyCenter)
                ? positive
                : negative;

            var outwardLocal = tipLocal - center;
            if (outwardLocal.sqrMagnitude > 0.000001f)
                tipLocal += outwardLocal.normalized * 0.015f;

            muzzle.localPosition = tipLocal;
            muzzle.localRotation = Quaternion.identity;
            muzzle.localScale = Vector3.one;

            Debug.Log(
                $"DEADREACH using artist-rigged embedded firearm '{firearmRenderer.name}' on operator '{SaveService.Data.selectedCharacterId}'. " +
                $"External hand-mounted Rifle disabled; muzzle={muzzle.position}.");

            return muzzle;
        }

        private Vector3 GetSurvivorBodyCenter()
        {
            var bodyRenderers = _instance.GetComponentsInChildren<Renderer>(true)
                .Where(renderer => renderer != null && renderer.enabled)
                .ToArray();

            if (bodyRenderers.Length == 0)
                return _instance.transform.position;

            var bounds = bodyRenderers[0].bounds;
            for (var i = 1; i < bodyRenderers.Length; i++)
                bounds.Encapsulate(bodyRenderers[i].bounds);

            return bounds.center;
        }

        private static Renderer FindEmbeddedFirearmRenderer(GameObject root)
        {
            // First pass: honor the wrapper's explicit weapon choice. This is critical for Matt's
            // full character export where Pistol/Rifle/Shotgun/SMG are all present but only Rifle is
            // intentionally enabled.
            var enabled = FindBestFirearmRenderer(root, enabledOnly: true);
            if (enabled != null)
                return enabled;

            // Compatibility fallback for the previously validated Sam prefab where the historical
            // setup disabled the embedded pistol and runtime re-enables it.
            return FindBestFirearmRenderer(root, enabledOnly: false);
        }

        private static Renderer FindBestFirearmRenderer(GameObject root, bool enabledOnly)
        {
            Renderer best = null;
            var bestScore = int.MinValue;

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || (enabledOnly && !renderer.enabled))
                    continue;

                var score = ScoreFirearmRenderer(renderer);
                if (score <= bestScore)
                    continue;

                best = renderer;
                bestScore = score;
            }

            return bestScore > 0 ? best : null;
        }

        private static int ScoreFirearmRenderer(Renderer renderer)
        {
            var score = 0;
            var current = renderer.transform;
            var depth = 0;

            while (current != null && depth < 5)
            {
                var normalized = NormalizeName(current.name);
                foreach (var token in EmbeddedFirearmTokens)
                {
                    if (normalized == token)
                        score = Mathf.Max(score, 200 - depth * 10);
                    else if (normalized.Contains(token))
                        score = Mathf.Max(score, 140 - depth * 10);
                }

                current = current.parent;
                depth++;
            }

            var meshName = GetRendererMeshName(renderer);
            if (!string.IsNullOrEmpty(meshName))
            {
                var normalizedMesh = NormalizeName(meshName);
                foreach (var token in EmbeddedFirearmTokens)
                {
                    if (normalizedMesh == token)
                        score = Mathf.Max(score, 190);
                    else if (normalizedMesh.Contains(token))
                        score = Mathf.Max(score, 130);
                }
            }

            return score;
        }

        private static string GetRendererMeshName(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer skinned && skinned.sharedMesh != null)
                return skinned.sharedMesh.name;

            var filter = renderer.GetComponent<MeshFilter>();
            return filter != null && filter.sharedMesh != null ? filter.sharedMesh.name : null;
        }

        private static bool TryGetRendererMeshBounds(Renderer renderer, out Bounds bounds)
        {
            if (renderer is SkinnedMeshRenderer skinned && skinned.sharedMesh != null)
            {
                bounds = skinned.sharedMesh.bounds;
                return true;
            }

            var filter = renderer.GetComponent<MeshFilter>();
            if (filter != null && filter.sharedMesh != null)
            {
                bounds = filter.sharedMesh.bounds;
                return true;
            }

            bounds = default;
            return false;
        }

        private static float GetAxis(Vector3 value, int axis)
        {
            return axis switch
            {
                0 => value.x,
                1 => value.y,
                _ => value.z
            };
        }

        private static void SetAxis(ref Vector3 value, int axis, float component)
        {
            switch (axis)
            {
                case 0:
                    value.x = component;
                    break;
                case 1:
                    value.y = component;
                    break;
                default:
                    value.z = component;
                    break;
            }
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return new string(value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
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
