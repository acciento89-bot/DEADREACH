using System;
using System.Collections;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.14 command center. One presentation owner, no 0.13 overlay stack.
    /// </summary>
    public sealed partial class Production14CommandCenterUI : MonoBehaviour
    {
        private static bool _hooked;

        private readonly Color _white = new(0.98f, 0.99f, 1f, 1f);
        private readonly Color _muted = new(0.70f, 0.75f, 0.76f, 1f);
        private readonly Color _cyan = new(0.22f, 0.88f, 0.95f, 1f);
        private readonly Color _amber = new(1.0f, 0.42f, 0.08f, 1f);
        private readonly Color _green = new(0.21f, 0.90f, 0.53f, 1f);
        private readonly Color _danger = new(0.92f, 0.24f, 0.12f, 1f);

        private readonly Dictionary<int, Button> _navButtons = new();

        private Font _font;
        private CanvasScaler _scaler;
        private RectTransform _root;
        private RectTransform _contentRoot;
        private Text _deployInfo;
        private Text _screenTitle;
        private Text _screenSubtitle;
        private int _activeNavIndex;
        private int _campaignSector;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private bool _hasResponsiveFrame;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (_hooked)
                return;

            _hooked = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.name.Contains("Bunker", StringComparison.OrdinalIgnoreCase))
                return;

            if (FindFirstObjectByType<Production14CommandCenterUI>() != null)
                return;

            new GameObject("Production14_CommandCenter").AddComponent<Production14CommandCenterUI>();
        }

        private IEnumerator Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();

            // Stable 0.12 UI gets one frame to initialize its gameplay bindings, then 0.14
            // takes full visual ownership and removes only the legacy presentation canvas.
            yield return null;

            DisableLegacyPresentation();
            _campaignSector = Mathf.Clamp((SaveService.Data.selectedLevel - 1) / 10, 0, 4);
            Production14HoloDiorama.Build();
            BuildInterface();
        }

        private void LateUpdate()
        {
            ApplyResponsiveFrame();
        }

        private void DisableLegacyPresentation()
        {
            var legacy = FindFirstObjectByType<BunkerCommandCenterUI>();
            if (legacy != null)
                legacy.enabled = false;

            var legacyCanvas = GameObject.Find("Bunker_CommandCenter_Canvas");
            if (legacyCanvas != null)
                Destroy(legacyCanvas);
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            var eventObject = new GameObject("P14_UI_EventSystem");
            eventObject.AddComponent<EventSystem>();
            var inputModule = eventObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject("P14_CommandCenter_Canvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 140;
            canvas.pixelPerfect = true;
            canvasObject.AddComponent<GraphicRaycaster>();

            _scaler = canvasObject.AddComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = new Vector2(1440f, 810f);
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _scaler.matchWidthOrHeight = 0.52f;

            _root = CreateRect("Root", canvasObject.transform);
            Fill(_root);
            ApplyResponsiveFrame(true);

            BuildEdgeVignette();
            BuildHeader();
            BuildNavigation();

            // Every tab owns only children of this layer. Header, navigation and footer never get
            // destroyed during a screen switch, which keeps button input stable.
            _contentRoot = CreateRect("ScreenContent", _root);
            Fill(_contentRoot);

            _activeNavIndex = 0;
            BuildOverview();
            BuildFooter();
            RefreshFooter();
        }

        private void ApplyResponsiveFrame(bool force = false)
        {
            if (_root == null || _scaler == null || Screen.width <= 0 || Screen.height <= 0)
                return;

            var safe = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (!force && _hasResponsiveFrame && _lastScreenSize == screenSize && _lastSafeArea == safe)
                return;

            _hasResponsiveFrame = true;
            _lastSafeArea = safe;
            _lastScreenSize = screenSize;

            _scaler.referenceResolution = new Vector2(1440f, 810f);
            _scaler.matchWidthOrHeight = safe.height > 0f && safe.width / safe.height >= 2.15f ? 0.62f : 0.52f;

            _root.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            _root.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
        }

        private void BuildEdgeVignette()
        {
            var topShade = CreateImage("TopShade", _root, new Color(0.01f, 0.015f, 0.017f, 0.54f));
            Place(topShade.rectTransform, 0f, 0.84f, 1f, 1f);

            var leftShade = CreateImage("LeftShade", _root, new Color(0.005f, 0.012f, 0.014f, 0.18f));
            Place(leftShade.rectTransform, 0f, 0.10f, 0.23f, 0.84f);

            var rightShade = CreateImage("RightShade", _root, new Color(0.005f, 0.012f, 0.014f, 0.16f));
            Place(rightShade.rectTransform, 0.77f, 0.10f, 1f, 0.84f);
        }

        private void BuildHeader()
        {
            var header = CreateIndustrialPanel("Header", _root, Production14IndustrialSkin.PlateKind.Header, false);
            Place(header, 0.012f, 0.865f, 0.988f, 0.988f);

            var brand = CreateLabel("Brand", header, "DEADREACH", 31, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(brand.rectTransform, 0.024f, 0.39f, 0.245f, 0.86f);

            var brandSub = CreateLabel("BrandSub", header, "BUNKER // COMMAND CENTER", 11, FontStyle.Bold, _cyan, TextAnchor.UpperLeft);
            Place(brandSub.rectTransform, 0.026f, 0.17f, 0.25f, 0.40f);

            var divider = CreateImage("HeaderDivider", header, new Color(_cyan.r, _cyan.g, _cyan.b, 0.62f));
            Place(divider.rectTransform, 0.278f, 0.20f, 0.2805f, 0.80f);

            _screenTitle = CreateLabel("ScreenTitle", header, "OVERVIEW", 25, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(_screenTitle.rectTransform, 0.305f, 0.44f, 0.55f, 0.80f);

            _screenSubtitle = CreateLabel("ScreenSubtitle", header, "FIELD READINESS // BUNKER INTELLIGENCE", 10, FontStyle.Bold, _muted, TextAnchor.UpperLeft);
            Place(_screenSubtitle.rectTransform, 0.307f, 0.20f, 0.58f, 0.44f);

            var data = SaveService.Data;
            CreateCounterCard(header, "SCRAP", data.securedScrap.ToString("N0"), 0.665f);
            CreateCounterCard(header, "EXTRACTS", data.successfulExtractions.ToString("N0"), 0.775f);
            CreateCounterCard(header, "BOSS KILLS", data.bossKills.ToString("N0"), 0.885f);

            var liveRail = CreateImage("HeaderLiveRail", header, _amber);
            Place(liveRail.rectTransform, 0.018f, 0.94f, 0.16f, 0.965f);
            var liveRail2 = CreateImage("HeaderLiveRailCyan", header, _cyan);
            Place(liveRail2.rectTransform, 0.165f, 0.94f, 0.255f, 0.965f);
        }

        private void CreateCounterCard(Transform parent, string label, string value, float x)
        {
            var card = CreateIndustrialPanel($"Counter_{label}", parent, Production14IndustrialSkin.PlateKind.Counter, false);
            Place(card, x, 0.18f, x + 0.095f, 0.84f);

            var title = CreateLabel("Label", card, label, 8, FontStyle.Bold, _muted, TextAnchor.UpperLeft);
            Place(title.rectTransform, 0.12f, 0.59f, 0.90f, 0.85f);

            var number = CreateLabel("Value", card, value, 23, FontStyle.Bold, _white, TextAnchor.LowerLeft);
            Place(number.rectTransform, 0.12f, 0.12f, 0.90f, 0.63f);

            var status = CreateImage("Status", card, _cyan);
            Place(status.rectTransform, 0.04f, 0.18f, 0.065f, 0.80f);
        }

        private void BuildNavigation()
        {
            var navRoot = CreateRect("Navigation", _root);
            Place(navRoot, 0.012f, 0.752f, 0.988f, 0.852f);

            var labels = new[] { "OVERVIEW", "ARSENAL", "OPERATORS", "CAMPAIGN", "WORKSHOP", "SUPPLY" };
            for (var i = 0; i < labels.Length; i++)
            {
                var left = i / 6f;
                var right = (i + 1) / 6f;
                var button = CreateNavButton(navRoot, $"{i + 1:00}  //  {labels[i]}", i == 0);
                Place(button.GetComponent<RectTransform>(), left + 0.004f, 0.06f, right - 0.004f, 0.94f);
                _navButtons[i] = button;

                var captured = i;
                button.onClick.AddListener(() => HandleNav(captured));
            }
        }

        private Button CreateNavButton(Transform parent, string label, bool active)
        {
            var go = new GameObject($"Nav_{label.Replace(" ", "_").Replace("/", string.Empty)}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.sprite = Production14IndustrialSkin.Get(active ? Production14IndustrialSkin.PlateKind.TabActive : Production14IndustrialSkin.PlateKind.Tab);
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
            colors.pressedColor = new Color(0.76f, 0.86f, 0.88f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            var text = CreateLabel("Text", go.transform, label, 10, FontStyle.Bold, active ? _white : new Color(0.86f, 0.89f, 0.90f, 1f), TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 10f, 8f, 10f, 8f);

            if (active)
                EnsureActiveRail(button, true);

            return button;
        }

        private void HandleNav(int index)
        {
            if (index < 0 || index > 5 || _contentRoot == null)
                return;

            _activeNavIndex = index;
            UpdateNavigationState(index);
            ClearScreenContent();

            var holo = GameObject.Find("P14_HoloDiorama");
            if (holo != null)
                holo.SetActive(index == 0);

            switch (index)
            {
                case 0:
                    SetScreenHeader("OVERVIEW", "FIELD READINESS // BUNKER INTELLIGENCE");
                    BuildOverview();
                    break;
                case 1:
                    SetScreenHeader("ARSENAL", "SECURED WEAPONS // LOADOUT CONTROL");
                    BuildArsenalScreen();
                    break;
                case 2:
                    SetScreenHeader("OPERATORS", "SURVIVOR ROSTER // FIELD SPECIALIZATION");
                    BuildOperatorsScreen();
                    break;
                case 3:
                    SetScreenHeader("CAMPAIGN", "50-LEVEL PROGRESSION // SECTOR SELECT");
                    BuildCampaignScreen();
                    break;
                case 4:
                    SetScreenHeader("WORKSHOP", "BUNKER SYSTEMS // WEAPON CALIBRATION");
                    BuildWorkshopScreen();
                    break;
                default:
                    SetScreenHeader("SUPPLY NETWORK", "COSMETICS // BUNKER THEMES // SEASON CONTENT");
                    BuildSupplyScreen();
                    break;
            }

            RefreshFooter();
        }

        private void SetScreenHeader(string title, string subtitle)
        {
            if (_screenTitle != null)
                _screenTitle.text = title;
            if (_screenSubtitle != null)
                _screenSubtitle.text = subtitle;
        }

        private void UpdateNavigationState(int activeIndex)
        {
            foreach (var pair in _navButtons)
            {
                if (pair.Value == null)
                    continue;

                var active = pair.Key == activeIndex;
                pair.Value.image.sprite = Production14IndustrialSkin.Get(active
                    ? Production14IndustrialSkin.PlateKind.TabActive
                    : Production14IndustrialSkin.PlateKind.Tab);

                var text = pair.Value.transform.Find("Text")?.GetComponent<Text>();
                if (text != null)
                    text.color = active ? _white : new Color(0.86f, 0.89f, 0.90f, 1f);

                EnsureActiveRail(pair.Value, active);
            }
        }

        private void EnsureActiveRail(Button button, bool active)
        {
            var existing = button.transform.Find("ActiveRail");
            if (existing == null && active)
            {
                var rail = CreateImage("ActiveRail", button.transform, _cyan);
                Place(rail.rectTransform, 0.12f, 0.02f, 0.88f, 0.055f);
                existing = rail.transform;
            }

            if (existing != null)
                existing.gameObject.SetActive(active);
        }

        private void ClearScreenContent()
        {
            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = _contentRoot.GetChild(i).gameObject;
                child.SetActive(false);
                Destroy(child);
            }
        }

        private void RefreshFooter()
        {
            if (_deployInfo == null)
                return;

            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);
            _deployInfo.text =
                $"READY // LEVEL {data.selectedLevel:00} // {RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant()}   |   " +
                $"OPERATOR {op.Name.ToUpperInvariant()}   |   {(data.selectedLevel % 10 == 0 ? "BOSS TARGET" : "STANDARD EXPEDITION")}";
        }
    }
}
