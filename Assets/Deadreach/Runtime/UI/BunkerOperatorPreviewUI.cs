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
        private RectTransform _frameRect;
        private GameObject _previewRoot;
        private GameObject _operatorVisual;
        private string _lastOperatorId;
        private float _nextProbe;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

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

                var inspectorObject = GameObject.Find("OperatorInspector");
                var open = inspectorObject != null && GameObject.Find("OperatorList") != null;
                var landscape = Screen.width >= Screen.height;
                SetActive(open && landscape);

                if (open && landscape)
                {
                    if (inspectorObject.transform is RectTransform inspectorRect)
                        ApplyResponsiveFrame(inspectorRect);

                    var id = SaveService.Data.selectedCharacterId ?? "ranger";
                    if (id != _lastOperatorId)
                        RefreshOperator();
                }
            }

            if (_previewRoot != null && _previewRoot.activeSelf)
                _previewRoot.transform.Rotate(Vector3.up, 7f * Time.unscaledDeltaTime, Space.World);
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
            _frameRect = frameObject.GetComponent<RectTransform>();
            _frameRect.anchorMin = new Vector2(0.635f, 0.285f);
            _frameRect.anchorMax = new Vector2(0.955f, 0.735f);
            _frameRect.offsetMin = Vector2.zero;
            _frameRect.offsetMax = Vector2.zero;
            frameObject.GetComponent<Image>().color = new Color(0.012f, 0.015f, 0.014f, 0.90f);

            var stripeObject = new GameObject("OperatorPreview_Stripe", typeof(RectTransform), typeof(Image));
            stripeObject.transform.SetParent(frameObject.transform, false);
            var stripe = stripeObject.GetComponent<RectTransform>();
            stripe.anchorMin = new Vector2(0f, 0.985f);
            stripe.anchorMax = Vector2.one;
            stripe.offsetMin = Vector2.zero;
            stripe.offsetMax = Vector2.zero;
            stripeObject.GetComponent<Image>().color = new Color(0.18f, 0.78f, 0.45f, 1f);

            var rawObject = new GameObject("Operator_Render", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter));
            rawObject.transform.SetParent(frameObject.transform, false);
            var rawRect = rawObject.GetComponent<RectTransform>();
            rawRect.anchorMin = new Vector2(0.03f, 0.025f);
            rawRect.anchorMax = new Vector2(0.97f, 0.96f);
            rawRect.offsetMin = Vector2.zero;
            rawRect.offsetMax = Vector2.zero;

            var aspectFitter = rawObject.GetComponent<AspectRatioFitter>();
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspectFitter.aspectRatio = 1f;

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

        private void ApplyResponsiveFrame(RectTransform inspector)
        {
            if (_frameRect == null || Screen.width <= 0 || Screen.height <= 0)
                return;

            if (_lastSafeArea == Screen.safeArea && _lastScreenSize.x == Screen.width && _lastScreenSize.y == Screen.height)
            {
                // The host can still move when a tab is rebuilt, so intentionally continue and re-anchor.
            }

            _lastSafeArea = Screen.safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            var corners = new Vector3[4];
            inspector.GetWorldCorners(corners);
            var bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            var topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            var width = Mathf.Max(1f, topRight.x - bottomLeft.x);
            var height = Mathf.Max(1f, topRight.y - bottomLeft.y);
            var screenAspect = Screen.width / Mathf.Max(1f, (float)Screen.height);

            var xInset = screenAspect <= 1.45f ? 0.045f : 0.055f;
            var yMin = screenAspect <= 1.45f ? 0.34f : screenAspect <= 1.72f ? 0.32f : 0.30f;
            const float yMax = 0.86f;

            var min = new Vector2(
                bottomLeft.x + width * xInset,
                bottomLeft.y + height * yMin);
            var max = new Vector2(
                topRight.x - width * xInset,
                bottomLeft.y + height * yMax);

            _frameRect.anchorMin = new Vector2(
                Mathf.Clamp01(min.x / Screen.width),
                Mathf.Clamp01(min.y / Screen.height));
            _frameRect.anchorMax = new Vector2(
                Mathf.Clamp01(max.x / Screen.width),
                Mathf.Clamp01(max.y / Screen.height));
            _frameRect.offsetMin = Vector2.zero;
            _frameRect.offsetMax = Vector2.zero;
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
            var source = catalog != null ? catalog.GetSurvivorPrefab(_lastOperatorId) : null;
            if (source != null)
            {
                _operatorVisual = Instantiate(source, _previewRoot.transform, false);
                _operatorVisual.name = $"Preview_{definition.Name}";
                SetLayerRecursive(_operatorVisual, PreviewLayer);
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
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            var renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var height = Mathf.Max(0.01f, bounds.size.y);
            visual.transform.localScale *= 2.35f / height;

            // Quaternius survivors face away from the preview camera at identity in this setup.
            // Turn the authored model around, then add a slight showroom angle.
            visual.transform.rotation = Quaternion.Euler(0f, 198f, 0f);

            renderers = visual.GetComponentsInChildren<Renderer>(true);
            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            visual.transform.position -= new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
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
