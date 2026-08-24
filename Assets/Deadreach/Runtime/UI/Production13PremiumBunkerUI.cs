using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    [DisallowMultipleComponent]
    public sealed class Production13PremiumBunkerUI : MonoBehaviour
    {
        private static readonly Color Background = new(0.004f, 0.009f, 0.013f, 0.94f);
        private static readonly Color Panel = new(0.018f, 0.031f, 0.038f, 0.965f);
        private static readonly Color PanelRaised = new(0.026f, 0.046f, 0.054f, 0.98f);
        private static readonly Color PanelDeep = new(0.009f, 0.018f, 0.024f, 0.985f);
        private static readonly Color Cyan = new(0.18f, 0.78f, 0.86f, 1f);
        private static readonly Color CyanSoft = new(0.10f, 0.40f, 0.46f, 1f);
        private static readonly Color Amber = new(1f, 0.48f, 0.11f, 1f);
        private static readonly Color Red = new(0.92f, 0.12f, 0.08f, 1f);
        private static readonly Color Green = new(0.16f, 0.86f, 0.48f, 1f);
        private static readonly Color Text = new(0.92f, 0.96f, 0.96f, 1f);
        private static readonly Color Muted = new(0.50f, 0.60f, 0.62f, 1f);
        private static readonly Color NavIdle = new(0.018f, 0.035f, 0.043f, 0.99f);
        private static readonly Color NavActive = new(0.055f, 0.155f, 0.175f, 1f);

        private readonly Dictionary<int, bool> _styled = new();
        private Font _font;
        private float _nextRefresh;
        private Image _deployRail;
        private Text _signalLabel;
        private bool _foundationInstalled;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallAfterSceneLoad()
        {
            var bunker = UnityEngine.Object.FindFirstObjectByType<BunkerCommandCenterUI>();
            if (bunker == null || bunker.GetComponent<Production13PremiumBunkerUI>() != null)
                return;

            bunker.gameObject.AddComponent<Production13PremiumBunkerUI>();
        }

        private void Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            InstallFoundation();
            ApplyStyles();
        }

        private void Update()
        {
            if (!_foundationInstalled)
                InstallFoundation();

            if (Time.unscaledTime >= _nextRefresh)
            {
                _nextRefresh = Time.unscaledTime + 0.20f;
                ApplyStyles();
            }

            var pulse = 0.58f + Mathf.Sin(Time.unscaledTime * 3.2f) * 0.22f;
            if (_deployRail != null)
            {
                var c = Amber;
                c.a = pulse;
                _deployRail.color = c;
            }

            if (_signalLabel != null)
            {
                var bars = Mathf.FloorToInt(Mathf.PingPong(Time.unscaledTime * 1.25f, 3.99f));
                _signalLabel.text = $"TACTICAL LINK  {new string('▮', bars + 1)}{new string('▯', 3 - bars)}  //  NODE 07-A";
            }
        }

        private void InstallFoundation()
        {
            var canvas = FindNamed<Canvas>("Bunker_CommandCenter_Canvas");
            var backdrop = FindNamedRect("Backdrop");
            if (canvas == null || backdrop == null)
                return;

            var backdropImage = backdrop.GetComponent<Image>();
            if (backdropImage != null)
                backdropImage.color = Background;

            if (FindDeepChild(backdrop, "P13_TacticalBackdrop") == null)
            {
                var tactical = new GameObject("P13_TacticalBackdrop", typeof(RectTransform), typeof(Production13TacticalBackdropGraphic));
                tactical.transform.SetParent(backdrop, false);
                tactical.transform.SetAsFirstSibling();
                var rect = tactical.GetComponent<RectTransform>();
                Fill(rect);
                var graphic = tactical.GetComponent<Production13TacticalBackdropGraphic>();
                graphic.raycastTarget = false;
            }

            AddFrame("Header", Cyan, 0.9f);
            AddFrame("Navigation", CyanSoft, 0.85f);
            AddFrame("ContentFrame", CyanSoft, 0.75f);
            AddFrame("DeployBar", Amber, 1f);

            var header = FindNamedRect("Header");
            if (header != null)
            {
                EnsureRail(header, "P13_HeaderRailTop", Cyan, 0f, 0.985f, 1f, 1f, 0.75f);
                EnsureRail(header, "P13_HeaderRailBottom", CyanSoft, 0f, 0f, 1f, 0.012f, 0.42f);
                CreateOverlayLabel(header, "P13_Protocol", "DR // PROTOCOL 013", 10, Amber, TextAnchor.MiddleRight,
                    0.80f, 0.79f, 0.985f, 0.98f);
            }

            var nav = FindNamedRect("Navigation");
            if (nav != null)
            {
                EnsureRail(nav, "P13_NavRail", Cyan, 0f, 0f, 0.010f, 1f, 0.85f);
                _signalLabel ??= CreateOverlayLabel(nav, "P13_Signal", "TACTICAL LINK  ▮▮▮▮  //  NODE 07-A", 9, Cyan,
                    TextAnchor.LowerLeft, 0.07f, 0.165f, 0.93f, 0.215f);
                CreateOverlayLabel(nav, "P13_Clearance", "CLEARANCE // OMEGA", 9, Amber, TextAnchor.LowerLeft,
                    0.07f, 0.118f, 0.93f, 0.158f);
            }

            var content = FindNamedRect("ContentFrame");
            if (content != null)
            {
                EnsureRail(content, "P13_ContentRail", Amber, 0f, 0.992f, 0.19f, 1f, 0.9f);
                EnsureRail(content, "P13_ContentRailCold", Cyan, 0.81f, 0.992f, 1f, 1f, 0.65f);
            }

            var deploy = FindNamedRect("DeployBar");
            if (deploy != null)
            {
                _deployRail = EnsureRail(deploy, "P13_DeployPulse", Amber, 0f, 0f, 1f, 0.045f, 0.8f);
                CreateOverlayLabel(deploy, "P13_DeployCode", "EXPEDITION CONTROL // LIVE", 9, Cyan,
                    TextAnchor.MiddleRight, 0.56f, 0.60f, 0.705f, 0.92f);
            }

            _foundationInstalled = true;
        }

        private void ApplyStyles()
        {
            var canvas = FindNamed<Canvas>("Bunker_CommandCenter_Canvas");
            if (canvas == null)
                return;

            foreach (var image in canvas.GetComponentsInChildren<Image>(true))
                StyleImage(image);

            foreach (var text in canvas.GetComponentsInChildren<Text>(true))
                StyleText(text);

            foreach (var button in canvas.GetComponentsInChildren<Button>(true))
                StyleButton(button);
        }

        private void StyleImage(Image image)
        {
            if (image == null || image.gameObject.name.StartsWith("P13_", StringComparison.Ordinal))
                return;

            var name = image.gameObject.name;
            if (name == "Backdrop")
            {
                image.color = Background;
                return;
            }

            if (name == "Header") image.color = new Color(0.008f, 0.021f, 0.028f, 0.975f);
            else if (name == "Navigation") image.color = PanelDeep;
            else if (name == "ContentFrame") image.color = new Color(0.011f, 0.025f, 0.031f, 0.96f);
            else if (name == "DeployBar") image.color = new Color(0.018f, 0.048f, 0.048f, 0.985f);
            else if (name.Contains("Inspector", StringComparison.OrdinalIgnoreCase)) image.color = new Color(0.012f, 0.040f, 0.049f, 0.97f);
            else if (name.Contains("Summary", StringComparison.OrdinalIgnoreCase)) image.color = new Color(0.018f, 0.052f, 0.045f, 0.97f);
            else if (IsPremiumPanelName(name) && image.GetComponent<Button>() == null && image.color.a > 0.1f)
            {
                var luminance = image.color.r + image.color.g + image.color.b;
                image.color = luminance > 0.22f ? PanelRaised : Panel;
            }

            if (IsFeaturePanel(name))
            {
                EnsureOutline(image, new Color(Cyan.r, Cyan.g, Cyan.b, 0.20f), new Vector2(1f, -1f));
                EnsureFrameGraphic(image.rectTransform, AccentForName(name), 0.58f);
            }
        }

        private void StyleText(Text text)
        {
            if (text == null || text.gameObject.name.StartsWith("P13_", StringComparison.Ordinal))
                return;

            var id = text.GetInstanceID();
            if (!_styled.ContainsKey(id))
            {
                EnsureShadow(text, new Color(0f, 0f, 0f, 0.78f), new Vector2(1.2f, -1.2f));
                _styled[id] = true;
            }

            var name = text.gameObject.name;
            if (name == "Brand")
            {
                text.fontSize = Mathf.Max(text.fontSize, 38);
                text.color = Text;
                EnsureOutline(text, new Color(Cyan.r, Cyan.g, Cyan.b, 0.22f), new Vector2(1f, -1f));
            }
            else if (name == "BrandSub" || name == "NavCallsign")
            {
                text.color = Cyan;
            }
            else if (name == "SectionTitle")
            {
                text.fontSize = Mathf.Max(text.fontSize, 28);
                text.color = Text;
            }
            else if (name == "SectionSubtitle")
            {
                text.color = Muted;
            }
            else if (name == "PanelHeader")
            {
                text.fontSize = Mathf.Max(text.fontSize, 12);
                text.fontStyle = FontStyle.Bold;
            }
            else if (name == "ProfileSummary")
            {
                text.color = Green;
            }
        }

        private void StyleButton(Button button)
        {
            if (button == null || button.image == null)
                return;

            var name = button.gameObject.name;
            var label = FindDirectText(button.transform);
            Color baseColor;

            if (name.StartsWith("Nav_", StringComparison.Ordinal))
            {
                var active = IsNavActive(button.image.color);
                baseColor = active ? NavActive : NavIdle;
                button.image.color = baseColor;
                if (label != null)
                {
                    label.text = PremiumNavLabel(name, label.text);
                    label.alignment = TextAnchor.MiddleLeft;
                    label.rectTransform.offsetMin = new Vector2(18f, label.rectTransform.offsetMin.y);
                    label.color = active ? Text : new Color(0.72f, 0.80f, 0.82f, 1f);
                }
                UpdateButtonRail(button, active ? Cyan : CyanSoft, active ? 0.95f : 0.35f);
            }
            else if (name == "DeployButton")
            {
                baseColor = new Color(0.56f, 0.16f, 0.055f, 1f);
                button.image.color = baseColor;
                if (label != null)
                {
                    label.text = "DEPLOY  //  ENTER";
                    label.fontSize = Mathf.Max(label.fontSize, 14);
                }
                UpdateButtonRail(button, Amber, 1f);
                EnsureOutline(button.image, new Color(Amber.r, Amber.g, Amber.b, 0.55f), new Vector2(1f, -1f));
            }
            else
            {
                baseColor = button.image.color;
                if (baseColor.a > 0.2f)
                {
                    var warm = baseColor.r > baseColor.g * 1.35f;
                    var positive = baseColor.g > baseColor.r * 1.35f;
                    var accent = warm ? Amber : positive ? Green : Cyan;
                    baseColor = Color.Lerp(new Color(0.022f, 0.042f, 0.050f, baseColor.a), accent, button.interactable ? 0.18f : 0.07f);
                    button.image.color = baseColor;
                    UpdateButtonRail(button, accent, button.interactable ? 0.58f : 0.22f);
                }
            }

            var colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            colors.pressedColor = Color.Lerp(baseColor, Color.black, 0.28f);
            colors.disabledColor = new Color(baseColor.r * 0.42f, baseColor.g * 0.42f, baseColor.b * 0.42f, 0.72f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }

        private static bool IsNavActive(Color color)
        {
            var oldActive = color.r > 0.16f && color.g > 0.07f && color.g < 0.18f && color.b < 0.12f;
            var premiumActive = Mathf.Abs(color.r - NavActive.r) < 0.04f && Mathf.Abs(color.g - NavActive.g) < 0.05f;
            return oldActive || premiumActive;
        }

        private static string PremiumNavLabel(string objectName, string fallback)
        {
            return objectName switch
            {
                "Nav_OVERVIEW" => "01   //   OVERVIEW",
                "Nav_ARSENAL" => "02   //   ARSENAL",
                "Nav_OPERATORS" => "03   //   OPERATORS",
                "Nav_CAMPAIGN" => "04   //   CAMPAIGN",
                "Nav_WORKSHOP" => "05   //   WORKSHOP",
                "Nav_STORE" => "06   //   SUPPLY",
                _ => fallback
            };
        }

        private static bool IsPremiumPanelName(string name)
        {
            return name.Contains("Panel", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Frame", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("List", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Inspector", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Summary", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Facility_", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("WorkshopWeapon_", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Product_", StringComparison.OrdinalIgnoreCase)
                   || name == "BunkerSystems"
                   || name == "WeaponCalibration"
                   || name == "SectorNavigation"
                   || name == "SectorInfo";
        }

        private static bool IsFeaturePanel(string name)
        {
            return name == "MissionPanel" || name == "ProgressPanel" || name == "IntelPanel"
                   || name == "ArsenalList" || name == "ArsenalInspector"
                   || name == "OperatorList" || name == "OperatorInspector"
                   || name == "SectorNavigation" || name == "SectorInfo"
                   || name == "StoreIntro" || name.StartsWith("Product_", StringComparison.Ordinal)
                   || name == "WorkshopSummary" || name == "BunkerSystems" || name == "WeaponCalibration"
                   || name.StartsWith("Facility_", StringComparison.Ordinal)
                   || name.StartsWith("WorkshopWeapon_", StringComparison.Ordinal);
        }

        private static Color AccentForName(string name)
        {
            if (name.Contains("Inspector", StringComparison.OrdinalIgnoreCase)) return Cyan;
            if (name.Contains("Workshop", StringComparison.OrdinalIgnoreCase) || name.Contains("Facility", StringComparison.OrdinalIgnoreCase)) return Amber;
            if (name.Contains("Mission", StringComparison.OrdinalIgnoreCase)) return Amber;
            if (name.Contains("Progress", StringComparison.OrdinalIgnoreCase)) return Green;
            if (name.Contains("Product", StringComparison.OrdinalIgnoreCase) || name.Contains("Store", StringComparison.OrdinalIgnoreCase)) return Amber;
            return CyanSoft;
        }

        private void AddFrame(string objectName, Color accent, float alpha)
        {
            var rect = FindNamedRect(objectName);
            if (rect != null)
                EnsureFrameGraphic(rect, accent, alpha);
        }

        private static void EnsureFrameGraphic(RectTransform parent, Color accent, float alpha)
        {
            var existing = FindDeepChild(parent, "P13_Frame")?.GetComponent<Production13FrameGraphic>();
            if (existing != null)
            {
                existing.SetAccent(accent, alpha);
                return;
            }

            var go = new GameObject("P13_Frame", typeof(RectTransform), typeof(Production13FrameGraphic));
            go.transform.SetParent(parent, false);
            go.transform.SetAsLastSibling();
            var rect = go.GetComponent<RectTransform>();
            Fill(rect);
            var frame = go.GetComponent<Production13FrameGraphic>();
            frame.SetAccent(accent, alpha);
            frame.raycastTarget = false;
        }

        private static Image EnsureRail(Transform parent, string name, Color color, float xMin, float yMin, float xMax, float yMax, float alpha)
        {
            var existing = FindDeepChild(parent, name)?.GetComponent<Image>();
            if (existing != null)
                return existing;

            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            var c = color;
            c.a = alpha;
            image.color = c;
            image.raycastTarget = false;
            Place(go.GetComponent<RectTransform>(), xMin, yMin, xMax, yMax);
            return image;
        }

        private static void UpdateButtonRail(Button button, Color accent, float alpha)
        {
            var image = EnsureRail(button.transform, "P13_ButtonRail", accent, 0f, 0f, 1f, 0.045f, alpha);
            var c = accent;
            c.a = alpha;
            image.color = c;
        }

        private Text CreateOverlayLabel(Transform parent, string name, string value, int size, Color color, TextAnchor alignment,
            float xMin, float yMin, float xMax, float yMax)
        {
            var existing = FindDeepChild(parent, name)?.GetComponent<Text>();
            if (existing != null)
                return existing;

            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            Place(text.rectTransform, xMin, yMin, xMax, yMax);
            EnsureShadow(text, new Color(0f, 0f, 0f, 0.85f), new Vector2(1f, -1f));
            return text;
        }

        private static Text FindDirectText(Transform parent)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                var text = parent.GetChild(i).GetComponent<Text>();
                if (text != null)
                    return text;
            }
            return null;
        }

        private T FindNamed<T>(string objectName) where T : Component
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

        private static Transform FindDeepChild(Transform parent, string objectName)
        {
            if (parent == null)
                return null;
            foreach (var child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == objectName)
                    return child;
            }
            return null;
        }

        private static void EnsureShadow(Graphic graphic, Color color, Vector2 distance)
        {
            var shadow = graphic.GetComponent<Shadow>();
            if (shadow == null)
                shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = color;
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private static void EnsureOutline(Graphic graphic, Color color, Vector2 distance)
        {
            var outline = graphic.GetComponent<Outline>();
            if (outline == null)
                outline = graphic.gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = distance;
            outline.useGraphicAlpha = true;
        }

        private static void Fill(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Place(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class Production13TacticalBackdropGraphic : MaskableGraphic
    {
        private float _scan;

        protected override void Awake()
        {
            base.Awake();
            color = Color.white;
            raycastTarget = false;
        }

        private void Update()
        {
            _scan = Mathf.Repeat(Time.unscaledTime * 0.075f, 1f);
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            var min = r.min;
            var max = r.max;

            AddGradientQuad(vh, min, max,
                new Color(0.006f, 0.018f, 0.025f, 0.88f),
                new Color(0.002f, 0.007f, 0.011f, 0.94f));

            var grid = new Color(0.16f, 0.68f, 0.75f, 0.035f);
            var major = new Color(0.16f, 0.68f, 0.75f, 0.070f);
            for (var i = 1; i < 20; i++)
            {
                var x = Mathf.Lerp(min.x, max.x, i / 20f);
                var width = i % 5 == 0 ? 1.5f : 0.65f;
                AddSolidQuad(vh, new Vector2(x - width * 0.5f, min.y), new Vector2(x + width * 0.5f, max.y), i % 5 == 0 ? major : grid);
            }
            for (var i = 1; i < 12; i++)
            {
                var y = Mathf.Lerp(min.y, max.y, i / 12f);
                var width = i % 4 == 0 ? 1.5f : 0.65f;
                AddSolidQuad(vh, new Vector2(min.x, y - width * 0.5f), new Vector2(max.x, y + width * 0.5f), i % 4 == 0 ? major : grid);
            }

            var side = new Color(0.05f, 0.38f, 0.46f, 0.10f);
            AddGradientQuad(vh, new Vector2(min.x, min.y), new Vector2(min.x + r.width * 0.08f, max.y), side, Color.clear);
            AddGradientQuadHorizontal(vh, new Vector2(max.x - r.width * 0.08f, min.y), new Vector2(max.x, max.y), Color.clear, side);

            var scanY = Mathf.Lerp(min.y, max.y, _scan);
            AddSolidQuad(vh, new Vector2(min.x, scanY - 1.2f), new Vector2(max.x, scanY + 1.2f), new Color(0.20f, 0.80f, 0.88f, 0.055f));
            AddSolidQuad(vh, new Vector2(min.x, scanY + 2f), new Vector2(max.x, scanY + 2.5f), new Color(1f, 0.42f, 0.08f, 0.035f));

            var vignette = new Color(0f, 0f, 0f, 0.34f);
            AddGradientQuad(vh, new Vector2(min.x, max.y - r.height * 0.10f), max, Color.clear, vignette);
            AddGradientQuad(vh, min, new Vector2(max.x, min.y + r.height * 0.12f), vignette, Color.clear);
        }

        private static void AddSolidQuad(VertexHelper vh, Vector2 min, Vector2 max, Color color)
        {
            AddQuad(vh, min, max, color, color, color, color);
        }

        private static void AddGradientQuad(VertexHelper vh, Vector2 min, Vector2 max, Color bottom, Color top)
        {
            AddQuad(vh, min, max, bottom, bottom, top, top);
        }

        private static void AddGradientQuadHorizontal(VertexHelper vh, Vector2 min, Vector2 max, Color left, Color right)
        {
            AddQuad(vh, min, max, left, right, right, left);
        }

        private static void AddQuad(VertexHelper vh, Vector2 min, Vector2 max, Color bl, Color br, Color tr, Color tl)
        {
            var start = vh.currentVertCount;
            var v = UIVertex.simpleVert;
            v.position = new Vector3(min.x, min.y); v.color = bl; vh.AddVert(v);
            v.position = new Vector3(max.x, min.y); v.color = br; vh.AddVert(v);
            v.position = new Vector3(max.x, max.y); v.color = tr; vh.AddVert(v);
            v.position = new Vector3(min.x, max.y); v.color = tl; vh.AddVert(v);
            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start, start + 2, start + 3);
        }
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class Production13FrameGraphic : MaskableGraphic
    {
        private Color _accent = new(0.18f, 0.78f, 0.86f, 1f);
        private float _alpha = 0.7f;

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
        }

        public void SetAccent(Color accent, float alpha)
        {
            _accent = accent;
            _alpha = alpha;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            var min = r.min;
            var max = r.max;
            var c = _accent;
            c.a *= _alpha;
            var soft = c;
            soft.a *= 0.20f;
            var thickness = Mathf.Clamp(Mathf.Min(r.width, r.height) * 0.004f, 1f, 3f);
            var corner = Mathf.Clamp(Mathf.Min(r.width, r.height) * 0.10f, 12f, 48f);

            AddRect(vh, new Vector2(min.x, max.y - thickness), new Vector2(min.x + corner, max.y), c);
            AddRect(vh, new Vector2(max.x - corner, max.y - thickness), new Vector2(max.x, max.y), c);
            AddRect(vh, new Vector2(min.x, min.y), new Vector2(min.x + corner, min.y + thickness), c);
            AddRect(vh, new Vector2(max.x - corner, min.y), new Vector2(max.x, min.y + thickness), c);
            AddRect(vh, new Vector2(min.x, max.y - corner), new Vector2(min.x + thickness, max.y), c);
            AddRect(vh, new Vector2(max.x - thickness, max.y - corner), new Vector2(max.x, max.y), c);
            AddRect(vh, new Vector2(min.x, min.y), new Vector2(min.x + thickness, min.y + corner), c);
            AddRect(vh, new Vector2(max.x - thickness, min.y), new Vector2(max.x, min.y + corner), c);

            AddRect(vh, new Vector2(min.x + corner, max.y - 1f), new Vector2(max.x - corner, max.y), soft);
        }

        private static void AddRect(VertexHelper vh, Vector2 min, Vector2 max, Color color)
        {
            var start = vh.currentVertCount;
            var v = UIVertex.simpleVert;
            v.color = color;
            v.position = new Vector3(min.x, min.y); vh.AddVert(v);
            v.position = new Vector3(max.x, min.y); vh.AddVert(v);
            v.position = new Vector3(max.x, max.y); vh.AddVert(v);
            v.position = new Vector3(min.x, max.y); vh.AddVert(v);
            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
