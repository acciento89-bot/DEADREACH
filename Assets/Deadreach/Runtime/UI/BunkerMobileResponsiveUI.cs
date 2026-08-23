using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// First production mobile-landscape pass for the generated Bunker Command Center.
    /// Applies Screen.safeArea and aspect-ratio breakpoints without changing the accepted desktop layout source.
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
                scaler.matchWidthOrHeight = aspect >= 2.05f ? 0.72f : aspect <= 1.72f ? 0.42f : 0.55f;
            }

            var backdrop = FindNamedRect("Backdrop");
            if (backdrop == null)
                return;

            var safeMin = Screen.safeArea.position;
            var safeMax = Screen.safeArea.position + Screen.safeArea.size;
            safeMin.x /= Screen.width;
            safeMin.y /= Screen.height;
            safeMax.x /= Screen.width;
            safeMax.y /= Screen.height;
            backdrop.anchorMin = safeMin;
            backdrop.anchorMax = safeMax;
            backdrop.offsetMin = Vector2.zero;
            backdrop.offsetMax = Vector2.zero;

            var navigation = FindNamedRect("Navigation");
            var content = FindNamedRect("ContentFrame");
            var deploy = FindNamedRect("DeployBar");
            var header = FindNamedRect("Header");

            if (navigation == null || content == null || deploy == null || header == null)
                return;

            if (aspect >= 2.05f)
            {
                // Notched / ultrawide phones: reduce nav width and give the content more horizontal room.
                SetAnchors(navigation, 0.010f, 0.095f, 0.175f, 0.845f);
                SetAnchors(content, 0.185f, 0.095f, 0.990f, 0.845f);
                SetAnchors(header, 0.010f, 0.858f, 0.990f, 0.989f);
                SetAnchors(deploy, 0.010f, 0.014f, 0.990f, 0.078f);
            }
            else if (aspect <= 1.72f)
            {
                // Compact landscape / tablet: slightly wider navigation and taller deploy bar.
                SetAnchors(navigation, 0.014f, 0.105f, 0.225f, 0.842f);
                SetAnchors(content, 0.235f, 0.105f, 0.986f, 0.842f);
                SetAnchors(header, 0.014f, 0.855f, 0.986f, 0.988f);
                SetAnchors(deploy, 0.014f, 0.014f, 0.986f, 0.088f);
            }
            else
            {
                SetAnchors(navigation, 0.012f, 0.09f, 0.205f, 0.85f);
                SetAnchors(content, 0.215f, 0.09f, 0.988f, 0.85f);
                SetAnchors(header, 0.0f, 0.865f, 1.0f, 0.993f);
                SetAnchors(deploy, 0.012f, 0.012f, 0.988f, 0.075f);
            }

            EnforceTouchTargets(canvas.transform);
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
