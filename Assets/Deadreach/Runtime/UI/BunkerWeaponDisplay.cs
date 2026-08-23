using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Presentation;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BunkerWeaponDisplay : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 24f;
        [SerializeField] private float targetSize = 1.75f;

        private GameObject _visual;
        private Transform _spinRoot;

        private void Start()
        {
            RefreshFromProfile();
        }

        private void Update()
        {
            if (_spinRoot != null)
                _spinRoot.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        public void RefreshFromProfile()
        {
            if (_visual != null)
                Destroy(_visual);
            if (_spinRoot != null)
                Destroy(_spinRoot.gameObject);

            var spin = new GameObject("WeaponTurntable");
            spin.transform.SetParent(transform, false);
            _spinRoot = spin.transform;

            var catalog = Resources.Load<ProductionAssetCatalog>("Deadreach/ProductionAssetCatalog");
            var source = catalog != null ? catalog.PrimaryWeaponPrefab : null;
            if (source == null)
            {
                BuildFallbackRifle(_spinRoot);
                return;
            }

            _visual = Instantiate(source, _spinRoot, false);
            _visual.name = "Displayed_EquippedWeapon";
            NormalizeVisual(_visual);

            var weapon = SaveService.GetEquippedPrimaryWeapon();
            gameObject.name = weapon != null
                ? $"Bunker_WeaponDisplay_{weapon.displayNameSnapshot}"
                : "Bunker_WeaponDisplay_FieldIssue";
        }

        private void NormalizeVisual(GameObject visual)
        {
            var renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (maxSize > 0.001f)
                visual.transform.localScale *= targetSize / maxSize;

            // Recalculate after scaling and move the geometry's center onto the turntable origin.
            renderers = visual.GetComponentsInChildren<Renderer>(true);
            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            visual.transform.position += transform.position - bounds.center + Vector3.up * 0.18f;
            visual.transform.rotation = Quaternion.Euler(8f, -32f, 0f);
        }

        private static void BuildFallbackRifle(Transform parent)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "FallbackWeaponBody";
            body.transform.SetParent(parent, false);
            body.transform.localScale = new Vector3(1.1f, 0.16f, 0.22f);
            Destroy(body.GetComponent<Collider>());

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.name = "FallbackWeaponBarrel";
            barrel.transform.SetParent(parent, false);
            barrel.transform.localPosition = new Vector3(0.78f, 0.03f, 0f);
            barrel.transform.localScale = new Vector3(0.55f, 0.07f, 0.08f);
            Destroy(barrel.GetComponent<Collider>());
        }
    }
}
