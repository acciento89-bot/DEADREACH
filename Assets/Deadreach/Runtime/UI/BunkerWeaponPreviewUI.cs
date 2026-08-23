using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// RenderTexture-backed weapon inspector docked inside the Arsenal's right-hand inspector column.
    /// It renders only while the Arsenal tab exists.
    /// </summary>
    public sealed class BunkerWeaponPreviewUI : MonoBehaviour
    {
        private const int PreviewLayer = 31;

        private RenderTexture _renderTexture;
        private Camera _previewCamera;
        private RawImage _previewImage;
        private GameObject _previewRoot;
        private GameObject _weaponVisual;
        private string _lastEquippedId;
        private float _nextUiProbe;

        private void Start()
        {
            BuildPreviewCanvas();
            BuildPreviewStage();
            RefreshWeapon();
            SetPreviewActive(false);
        }

        private void Update()
        {
            if (Time.unscaledTime >= _nextUiProbe)
            {
                _nextUiProbe = Time.unscaledTime + 0.2f;
                var arsenalOpen = GameObject.Find("ArsenalScroll") != null;
                SetPreviewActive(arsenalOpen);

                if (arsenalOpen)
                {
                    var currentId = SaveService.Data.equippedPrimaryWeaponId ?? string.Empty;
                    if (currentId != _lastEquippedId)
                        RefreshWeapon();
                }
            }

            if (_previewRoot != null && _previewRoot.activeSelf)
                _previewRoot.transform.Rotate(Vector3.up, 22f * Time.unscaledDeltaTime, Space.World);
        }

        private void BuildPreviewCanvas()
        {
            var canvasObject = new GameObject("Arsenal_3D_Inspector_Canvas");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 48;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var frameObject = new GameObject("Arsenal_WeaponInspector_Frame", typeof(RectTransform), typeof(Image));
            frameObject.transform.SetParent(canvasObject.transform, false);
            var frame = frameObject.GetComponent<RectTransform>();
            frame.anchorMin = new Vector2(0.735f, 0.285f);
            frame.anchorMax = new Vector2(0.968f, 0.695f);
            frame.offsetMin = Vector2.zero;
            frame.offsetMax = Vector2.zero;
            frameObject.GetComponent<Image>().color = new Color(0.005f, 0.018f, 0.017f, 0.92f);

            var stripeObject = new GameObject("Inspector_Stripe", typeof(RectTransform), typeof(Image));
            stripeObject.transform.SetParent(frameObject.transform, false);
            var stripe = stripeObject.GetComponent<RectTransform>();
            stripe.anchorMin = new Vector2(0f, 0.985f);
            stripe.anchorMax = Vector2.one;
            stripe.offsetMin = Vector2.zero;
            stripe.offsetMax = Vector2.zero;
            stripeObject.GetComponent<Image>().color = new Color(0.78f, 0.27f, 0.07f, 1f);

            var rawObject = new GameObject("Weapon_Render", typeof(RectTransform), typeof(RawImage));
            rawObject.transform.SetParent(frameObject.transform, false);
            var rawRect = rawObject.GetComponent<RectTransform>();
            rawRect.anchorMin = new Vector2(0.025f, 0.035f);
            rawRect.anchorMax = new Vector2(0.975f, 0.955f);
            rawRect.offsetMin = Vector2.zero;
            rawRect.offsetMax = Vector2.zero;
            _previewImage = rawObject.GetComponent<RawImage>();
            _previewImage.color = Color.white;
            _previewImage.raycastTarget = false;

            _renderTexture = new RenderTexture(640, 480, 16, RenderTextureFormat.ARGB32)
            {
                name = "Runtime_BunkerWeaponPreview",
                antiAliasing = 2,
                filterMode = FilterMode.Bilinear
            };
            _renderTexture.Create();
            _previewImage.texture = _renderTexture;
        }

        private void BuildPreviewStage()
        {
            var cameraObject = new GameObject("Bunker_WeaponPreview_Camera");
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.position = new Vector3(0f, 0.25f, -4.2f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.15f, 0f));
            _previewCamera = cameraObject.AddComponent<Camera>();
            _previewCamera.clearFlags = CameraClearFlags.SolidColor;
            _previewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _previewCamera.cullingMask = 1 << PreviewLayer;
            _previewCamera.targetTexture = _renderTexture;
            _previewCamera.fieldOfView = 31f;
            _previewCamera.nearClipPlane = 0.05f;
            _previewCamera.farClipPlane = 20f;
            _previewCamera.allowHDR = true;

            var keyObject = new GameObject("Preview_KeyLight");
            keyObject.transform.SetParent(cameraObject.transform, false);
            keyObject.transform.localPosition = new Vector3(-1.2f, 1.4f, 0.8f);
            var key = keyObject.AddComponent<Light>();
            key.type = LightType.Point;
            key.color = new Color(1f, 0.48f, 0.18f);
            key.intensity = 4.8f;
            key.range = 8f;
            key.cullingMask = 1 << PreviewLayer;

            var fillObject = new GameObject("Preview_FillLight");
            fillObject.transform.SetParent(cameraObject.transform, false);
            fillObject.transform.localPosition = new Vector3(1.4f, 0.6f, 1.1f);
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.color = new Color(0.18f, 0.52f, 0.74f);
            fill.intensity = 2.4f;
            fill.range = 7f;
            fill.cullingMask = 1 << PreviewLayer;
        }

        private void RefreshWeapon()
        {
            if (_weaponVisual != null)
                Destroy(_weaponVisual);
            if (_previewRoot != null)
                Destroy(_previewRoot);

            _lastEquippedId = SaveService.Data.equippedPrimaryWeaponId ?? string.Empty;
            _previewRoot = new GameObject("Arsenal_WeaponPreview_Root");
            _previewRoot.transform.SetParent(transform, false);
            _previewRoot.transform.position = Vector3.zero;
            SetLayerRecursive(_previewRoot, PreviewLayer);

            var catalog = Resources.Load<ProductionAssetCatalog>("Deadreach/ProductionAssetCatalog");
            var source = catalog != null ? catalog.PrimaryWeaponPrefab : null;
            _weaponVisual = source != null
                ? Instantiate(source, _previewRoot.transform, false)
                : BuildFallbackWeapon(_previewRoot.transform);
            _weaponVisual.name = source != null ? "Preview_ProductionWeapon" : "Preview_FallbackWeapon";

            SetLayerRecursive(_weaponVisual, PreviewLayer);
            NormalizeWeapon(_weaponVisual);
        }

        private static void NormalizeWeapon(GameObject visual)
        {
            var renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var longest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (longest > 0.001f)
                visual.transform.localScale *= 2.35f / longest;

            renderers = visual.GetComponentsInChildren<Renderer>(true);
            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            visual.transform.position -= bounds.center;
            visual.transform.position += Vector3.up * 0.10f;
            visual.transform.rotation = Quaternion.Euler(8f, -28f, -4f);
        }

        private static GameObject BuildFallbackWeapon(Transform parent)
        {
            var root = new GameObject("Fallback_FieldWeapon");
            root.transform.SetParent(parent, false);

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(1.35f, 0.16f, 0.22f);
            Destroy(body.GetComponent<Collider>());

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.transform.SetParent(root.transform, false);
            barrel.transform.localPosition = new Vector3(0.98f, 0.03f, 0f);
            barrel.transform.localScale = new Vector3(0.62f, 0.07f, 0.08f);
            Destroy(barrel.GetComponent<Collider>());
            return root;
        }

        private void SetPreviewActive(bool active)
        {
            if (_previewImage != null && _previewImage.transform.parent != null)
                _previewImage.transform.parent.gameObject.SetActive(active);
            if (_previewCamera != null)
                _previewCamera.enabled = active;
            if (_previewRoot != null)
                _previewRoot.SetActive(active);
        }

        private static void SetLayerRecursive(GameObject root, int layer)
        {
            root.layer = layer;
            foreach (Transform child in root.transform)
                SetLayerRecursive(child.gameObject, layer);
        }

        private void OnDestroy()
        {
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
            }
        }
    }
}
