using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production landscape layout for the generated Bunker Command Center.
    /// Keeps header, navigation/content and deploy bar in non-overlapping safe-area zones.
    /// </summary>
    public sealed class BunkerMobileResponsiveUI : MonoBehaviour
    {
        private Rect _lastSafeArea;
        private Vector2Int _lastSize;

        private void Start()
        {
            Apply(force: true);
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea || _lastSize.x != Screen.width || _lastSize.y != Screen.height)
                Apply(force: true);
        }

        private void Apply(bool force)
        {
            if (Screen.width <= 0 || Screen.height <= 0)
                return;

            _lastSafeArea = Screen.safeArea;
            _lastSize = new Vector2Int(Screen.width, Screen.height);

            var canvas = FindNamedComponent<Canvas>("Bunker_CommandCenter_Canvas");
            if (canvas == null)
                return;

            var scaler = canvas.GetComponent<CanvasScaler>();
            var aspect = Screen.safeArea.width / Mathf.Max(1f, Screen.safeArea.height);
            if (scaler != null)
            {
                scaler.referenceResolution = new Vector2(1600f, 900f);
                scaler.matchWidthOrHeight = aspect >= 2.05f
                    ? 0.68f
                    : aspect <= 1.45f
                        ? 0.34f
                        : aspect <= 1.72f
                            ? 0.40f
                            : 0.53f;
            }

            var backdrop = FindNamedRect("Backdrop");
            if (backdrop == null)
                return;

            ApplySafeArea(backdrop);

            var navigation = FindNamedRect("Navigation");
            var content = FindNamedRect("ContentFrame");
            var deploy = FindNamedRect("DeployBar");
            var header = FindNamedRect("Header");

            if (navigation == null || content == null || deploy == null || header == null)
                return;

            if (aspect >= 2.05f)
            {
                // Ultrawide / notched phones: compact navigation, generous horizontal content,
                // and explicit vertical gutters between header/content/deploy.
                SetAnchors(header, 0.012f, 0.875f, 0.988f, 0.985f);
                SetAnchors(navigation, 0.012f, 0.115f, 0.172f, 0.850f);
                SetAnchors(content, 0.187f, 0.115f, 0.988f, 0.850f);
                SetAnchors(deploy, 0.012f, 0.018f, 0.988f, 0.085f);
            }
            else if (aspect <= 1.45f)
            {
                // 4:3-ish compact landscape: reclaim horizontal room from the navigation rail.
                // The Operators/Arsenal split panels need width more than the nav needs it.
                SetAnchors(header, 0.014f, 0.858f, 0.986f, 0.985f);
                SetAnchors(navigation, 0.014f, 0.132f, 0.195f, 0.832f);
                SetAnchors(content, 0.208f, 0.132f, 0.986f, 0.832f);
                SetAnchors(deploy, 0.014f, 0.020f, 0.986f, 0.106f);
            }
            else if (aspect <= 1.72f)
            {
                // 16:10 / compact landscape: keep comfortable touch nav but give content more room
                // than the first 0.7 pass did.
                SetAnchors(header, 0.015f, 0.860f, 0.985f, 0.985f);
                SetAnchors(navigation, 0.015f, 0.135f, 0.205f, 0.830f);
                SetAnchors(content, 0.218f, 0.135f, 0.985f, 0.830f);
                SetAnchors(deploy, 0.015f, 0.020f, 0.985f, 0.105f);
            }
            else
            {
                // Desktop / 16:9 production baseline.
                SetAnchors(header, 0.014f, 0.872f, 0.986f, 0.985f);
                SetAnchors(navigation, 0.014f, 0.118f, 0.205f, 0.848f);
                SetAnchors(content, 0.220f, 0.118f, 0.986f, 0.848f);
                SetAnchors(deploy, 0.014f, 0.020f, 0.986f, 0.090f);
            }

            EnforceTouchTargets(canvas.transform);
        }

        private static void ApplySafeArea(RectTransform rect)
        {
            var safeMin = Screen.safeArea.position;
            var safeMax = Screen.safeArea.position + Screen.safeArea.size;
            safeMin.x /= Mathf.Max(1f, Screen.width);
            safeMin.y /= Mathf.Max(1f, Screen.height);
            safeMax.x /= Mathf.Max(1f, Screen.width);
            safeMax.y /= Mathf.Max(1f, Screen.height);
            rect.anchorMin = safeMin;
            rect.anchorMax = safeMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnforceTouchTargets(Transform root)
        {
            var buttons = root.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button == null || button.transform is not RectTransform rect)
                    continue;

                var layout = button.GetComponent<LayoutElement>();
                if (layout == null)
                    layout = button.gameObject.AddComponent<LayoutElement>();

                layout.minHeight = Mathf.Max(layout.minHeight, 46f);
                layout.minWidth = Mathf.Max(layout.minWidth, 92f);
            }
        }

        private T FindNamedComponent<T>(string objectName) where T : Component
        {
            foreach (var component in GetComponentsInChildren<T>(true))
            {
                if (component != null && component.gameObject.name == objectName)
                    return component;
            }
            return null;
        }

        private RectTransform FindNamedRect(string objectName)
        {
            foreach (var rect in GetComponentsInChildren<RectTransform>(true))
            {
                if (rect != null && rect.gameObject.name == objectName)
                    return rect;
            }
            return null;
        }

        private static void SetAnchors(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
