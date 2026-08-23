using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Presentation;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// RenderTexture-backed weapon inspector docked inside the Arsenal's right-hand inspector column.
    /// Production 0.7 keeps the validated DR-7 orientation while correcting imported family-specific axes.
    /// </summary>
    public sealed class BunkerWeaponPreviewUI : MonoBehaviour
    {
        private const int PreviewLayer = 31;

        private static readonly Quaternion[] OrientationCandidates =
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(0f, 0f, -90f),
            Quaternion.Euler(90f, 0f, 0f),
            Quaternion.Euler(-90f, 0f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            Quaternion.Euler(0f, -90f, 0f),
            Quaternion.Euler(180f, 0f, 0f),
            Quaternion.Euler(0f, 180f, 0f),
            Quaternion.Euler(0f, 0f, 180f)
        };

        private RenderTexture _renderTexture;
        private Camera _previewCamera;
        private RawImage _previewImage;
        private Text _finishLabel;
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
                _previewRoot.transform.Rotate(Vector3.up, 12f * Time.unscaledDeltaTime, Space.World);
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
            frameObject.GetComponent<Image>().color = new Color(0.005f, 0.018f, 0.017f, 0.96f);

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
            rawRect.anchorMin = new Vector2(0.025f, 0.15f);
            rawRect.anchorMax = new Vector2(0.975f, 0.955f);
            rawRect.offsetMin = Vector2.zero;
            rawRect.offsetMax = Vector2.zero;
            _previewImage = rawObject.GetComponent<RawImage>();
            _previewImage.color = Color.white;
            _previewImage.raycastTarget = false;

            var labelObject = new GameObject("Weapon_Finish_Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(frameObject.transform, false);
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.04f, 0.025f);
            labelRect.anchorMax = new Vector2(0.96f, 0.135f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            _finishLabel = labelObject.GetComponent<Text>();
            _finishLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _finishLabel.fontSize = 16;
            _finishLabel.fontStyle = FontStyle.Bold;
            _finishLabel.alignment = TextAnchor.MiddleCenter;
            _finishLabel.raycastTarget = false;

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
            _previewCamera = cameraObject.AddComponent<Camera>();
            _previewCamera.clearFlags = CameraClearFlags.SolidColor;
            _previewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _previewCamera.cullingMask = 1 << PreviewLayer;
            _previewCamera.targetTexture = _renderTexture;
            _previewCamera.fieldOfView = 30f;
            _previewCamera.nearClipPlane = 0.05f;
            _previewCamera.farClipPlane = 30f;
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

            var equippedWeapon = SaveService.GetEquippedPrimaryWeapon();
            var family = equippedWeapon != null ? equippedWeapon.family : WeaponFamily.Rifle;
            var catalog = Resources.Load<ProductionAssetCatalog>("Deadreach/ProductionAssetCatalog");
            var source = catalog != null ? catalog.GetWeaponPrefab(family) : null;

            _weaponVisual = source != null
                ? Instantiate(source, _previewRoot.transform, false)
                : BuildFallbackWeapon(_previewRoot.transform);
            _weaponVisual.name = source != null ? $"Preview_{family}" : "Preview_FallbackWeapon";

            SetLayerRecursive(_weaponVisual, PreviewLayer);
            NormalizeWeaponForPreview(_weaponVisual, family);
            FramePreviewCamera(_weaponVisual);
            WeaponVisualStyle.Apply(_weaponVisual, equippedWeapon);

            if (_finishLabel != null)
            {
                var finishId = WeaponVisualStyle.ResolveFinishId(equippedWeapon);
                _finishLabel.text = $"{family.ToString().ToUpperInvariant()} // FINISH // {WeaponVisualStyle.GetDisplayName(finishId)}";
                _finishLabel.color = WeaponVisualStyle.ResolveColor(equippedWeapon);
            }
        }

        private static void NormalizeWeaponForPreview(GameObject visual, WeaponFamily family)
        {
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            if (!TryGetCombinedBounds(visual, out var bounds))
                return;

            var longest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (longest > 0.001f)
                visual.transform.localScale *= 2.35f / longest;

            var bestRotation = Quaternion.identity;
            var bestScore = float.NegativeInfinity;

            foreach (var candidate in OrientationCandidates)
            {
                visual.transform.localRotation = candidate;
                if (!TryGetCombinedBounds(visual, out var candidateBounds))
                    continue;

                // Prefer a long horizontal silhouette with the least possible vertical footprint.
                var score = candidateBounds.size.x * 2.5f
                            - candidateBounds.size.y * 1.25f
                            - candidateBounds.size.z * 0.10f;
                if (score <= bestScore)
                    continue;

                bestScore = score;
                bestRotation = candidate;
            }

            // The artist-rigged DR-7 baseline needs the historical X flip. The imported Quaternius
            // families are already right-side-up after horizontal normalization; applying the same
            // X flip turns their magazine / grip upward in the inspector.
            var familyCorrection = family == WeaponFamily.Rifle
                ? Quaternion.Euler(180f, 0f, 0f)
                : Quaternion.identity;

            var presentationYaw = family switch
            {
                WeaponFamily.Pistol => -7f,
                WeaponFamily.Smg => -9f,
                WeaponFamily.Shotgun => -8f,
                _ => -11f
            };

            visual.transform.localRotation = Quaternion.Euler(4f, presentationYaw, 0f)
                                             * familyCorrection
                                             * bestRotation;

            if (!TryGetCombinedBounds(visual, out bounds))
                return;

            visual.transform.position -= bounds.center;
            visual.transform.position += Vector3.up * 0.02f;
        }

        private void FramePreviewCamera(GameObject visual)
        {
            if (_previewCamera == null || !TryGetCombinedBounds(visual, out var bounds))
                return;

            var aspect = _renderTexture != null && _renderTexture.height > 0
                ? (float)_renderTexture.width / _renderTexture.height
                : 4f / 3f;
            var halfHeight = Mathf.Max(bounds.extents.y, bounds.extents.x / Mathf.Max(0.1f, aspect));
            var radians = _previewCamera.fieldOfView * Mathf.Deg2Rad * 0.5f;
            var distance = halfHeight * 1.2f / Mathf.Max(0.05f, Mathf.Tan(radians));
            distance += bounds.extents.z + 0.3f;
            distance = Mathf.Clamp(distance, 3.1f, 6.5f);

            var target = bounds.center + Vector3.up * 0.02f;
            _previewCamera.transform.position = target + new Vector3(0f, 0.12f, -distance);
            _previewCamera.transform.LookAt(target);
        }

        private static bool TryGetCombinedBounds(GameObject visual, out Bounds bounds)
        {
            var renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return true;
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
