using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Presentation;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BunkerOperatorPreviewUI : MonoBehaviour
    {
        private const int PreviewLayer = 30;

        private RenderTexture _renderTexture;
        private Camera _previewCamera;
        private RawImage _previewImage;
        private GameObject _previewRoot;
        private GameObject _operatorVisual;
        private string _lastOperatorId;
        private float _nextProbe;

        private void Start()
        {
            BuildCanvas();
            BuildStage();
            RefreshOperator();
            SetActive(false);
        }

        private void Update()
        {
            if (Time.unscaledTime >= _nextProbe)
            {
                _nextProbe = Time.unscaledTime + 0.2f;
                var open = GameObject.Find("OperatorList") != null;
                SetActive(open);

                if (open)
                {
                    var id = SaveService.Data.selectedCharacterId ?? "ranger";
                    if (id != _lastOperatorId)
                        RefreshOperator();
                }
            }

            if (_previewRoot != null && _previewRoot.activeSelf)
                _previewRoot.transform.Rotate(Vector3.up, 10f * Time.unscaledDeltaTime, Space.World);
        }

        private void BuildCanvas()
        {
            var canvasObject = new GameObject("Operator_3D_Inspector_Canvas");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 47;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var frameObject = new GameObject("Operator_Inspector_Frame", typeof(RectTransform), typeof(Image));
            frameObject.transform.SetParent(canvasObject.transform, false);
            var frame = frameObject.GetComponent<RectTransform>();
            frame.anchorMin = new Vector2(0.635f, 0.285f);
            frame.anchorMax = new Vector2(0.955f, 0.735f);
            frame.offsetMin = Vector2.zero;
            frame.offsetMax = Vector2.zero;
            frameObject.GetComponent<Image>().color = new Color(0.012f, 0.015f, 0.014f, 0.90f);

            var stripeObject = new GameObject("OperatorPreview_Stripe", typeof(RectTransform), typeof(Image));
            stripeObject.transform.SetParent(frameObject.transform, false);
            var stripe = stripeObject.GetComponent<RectTransform>();
            stripe.anchorMin = new Vector2(0f, 0.985f);
            stripe.anchorMax = Vector2.one;
            stripe.offsetMin = Vector2.zero;
            stripe.offsetMax = Vector2.zero;
            stripeObject.GetComponent<Image>().color = new Color(0.18f, 0.78f, 0.45f, 1f);

            var rawObject = new GameObject("Operator_Render", typeof(RectTransform), typeof(RawImage));
            rawObject.transform.SetParent(frameObject.transform, false);
            var rawRect = rawObject.GetComponent<RectTransform>();
            rawRect.anchorMin = new Vector2(0.03f, 0.025f);
            rawRect.anchorMax = new Vector2(0.97f, 0.96f);
            rawRect.offsetMin = Vector2.zero;
            rawRect.offsetMax = Vector2.zero;
            _previewImage = rawObject.GetComponent<RawImage>();
            _previewImage.color = Color.white;
            _previewImage.raycastTarget = false;

            _renderTexture = new RenderTexture(640, 640, 16, RenderTextureFormat.ARGB32)
            {
                name = "Runtime_BunkerOperatorPreview",
                antiAliasing = 2,
                filterMode = FilterMode.Bilinear
            };
            _renderTexture.Create();
            _previewImage.texture = _renderTexture;
        }

        private void BuildStage()
        {
            var cameraObject = new GameObject("Bunker_OperatorPreview_Camera");
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.position = new Vector3(0f, 1.15f, -4.6f);
            cameraObject.transform.LookAt(new Vector3(0f, 1.05f, 0f));
            _previewCamera = cameraObject.AddComponent<Camera>();
            _previewCamera.clearFlags = CameraClearFlags.SolidColor;
            _previewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _previewCamera.cullingMask = 1 << PreviewLayer;
            _previewCamera.targetTexture = _renderTexture;
            _previewCamera.fieldOfView = 28f;
            _previewCamera.nearClipPlane = 0.05f;
            _previewCamera.farClipPlane = 20f;
            _previewCamera.allowHDR = true;

            var keyObject = new GameObject("OperatorPreview_Key");
            keyObject.transform.SetParent(cameraObject.transform, false);
            keyObject.transform.localPosition = new Vector3(-1.5f, 1.7f, 1.0f);
            var key = keyObject.AddComponent<Light>();
            key.type = LightType.Point;
            key.color = new Color(1f, 0.46f, 0.18f);
            key.intensity = 4.6f;
            key.range = 8f;
            key.cullingMask = 1 << PreviewLayer;

            var fillObject = new GameObject("OperatorPreview_Fill");
            fillObject.transform.SetParent(cameraObject.transform, false);
            fillObject.transform.localPosition = new Vector3(1.4f, 1.1f, 0.8f);
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.color = new Color(0.14f, 0.50f, 0.72f);
            fill.intensity = 2.7f;
            fill.range = 7f;
            fill.cullingMask = 1 << PreviewLayer;
        }

        private void RefreshOperator()
        {
            if (_operatorVisual != null)
                Destroy(_operatorVisual);
            if (_previewRoot != null)
                Destroy(_previewRoot);

            _lastOperatorId = SaveService.Data.selectedCharacterId ?? "ranger";
            var definition = OperatorCatalog.Get(_lastOperatorId);

            _previewRoot = new GameObject("OperatorPreview_Root");
            _previewRoot.transform.SetParent(transform, false);
            _previewRoot.transform.position = Vector3.zero;
            SetLayerRecursive(_previewRoot, PreviewLayer);

            var catalog = Resources.Load<ProductionAssetCatalog>("Deadreach/ProductionAssetCatalog");
            var source = catalog != null ? catalog.SurvivorPrefab : null;
            if (source != null)
            {
                _operatorVisual = Instantiate(source, _previewRoot.transform, false);
                _operatorVisual.name = $"Preview_{definition.Name}";
                SetLayerRecursive(_operatorVisual, PreviewLayer);
                ApplyTint(_operatorVisual, definition.Accent);
                NormalizeCharacter(_operatorVisual);
            }
            else
            {
                _operatorVisual = BuildFallbackCharacter(_previewRoot.transform, definition.Accent);
                SetLayerRecursive(_operatorVisual, PreviewLayer);
            }
        }

        private static void NormalizeCharacter(GameObject visual)
        {
            var renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var height = Mathf.Max(0.01f, bounds.size.y);
            visual.transform.localScale *= 2.35f / height;

            renderers = visual.GetComponentsInChildren<Renderer>(true);
            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            visual.transform.position -= new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            visual.transform.rotation = Quaternion.Euler(0f, 18f, 0f);
        }

        private static void ApplyTint(GameObject visual, Color accent)
        {
            foreach (var renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || IsWeaponRenderer(renderer.transform))
                    continue;

                var materials = renderer.materials;
                foreach (var material in materials)
                {
                    if (material == null)
                        continue;
                    var tint = Color.Lerp(Color.white, accent, 0.25f);
                    if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", tint);
                    if (material.HasProperty("_Color")) material.SetColor("_Color", tint);
                }
            }
        }

        private static bool IsWeaponRenderer(Transform transform)
        {
            var current = transform;
            for (var depth = 0; current != null && depth < 5; depth++, current = current.parent)
            {
                var value = current.name.ToLowerInvariant();
                if (value.Contains("weapon") || value.Contains("pistol") || value.Contains("rifle") || value.Contains("gun"))
                    return true;
            }
            return false;
        }

        private static GameObject BuildFallbackCharacter(Transform parent, Color accent)
        {
            var root = new GameObject("Fallback_Operator");
            root.transform.SetParent(parent, false);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(root.transform, false);
            body.transform.position = new Vector3(0f, 1.1f, 0f);
            body.transform.localScale = new Vector3(0.55f, 1.0f, 0.55f);
            Destroy(body.GetComponent<Collider>());
            var renderer = body.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = accent;
            return root;
        }

        private void SetActive(bool active)
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
