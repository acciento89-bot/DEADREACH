using System;
using System.Collections;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Persistence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.14 Pass 1. One presentation owner, no 0.13 overlay stack.
    /// </summary>
    public sealed partial class Production14CommandCenterUI : MonoBehaviour
    {
        private static bool _hooked;

        private readonly Color _white = new(0.93f, 0.95f, 0.95f, 1f);
        private readonly Color _muted = new(0.51f, 0.58f, 0.60f, 1f);
        private readonly Color _cyan = new(0.22f, 0.88f, 0.95f, 1f);
        private readonly Color _amber = new(1.0f, 0.42f, 0.08f, 1f);
        private readonly Color _green = new(0.21f, 0.90f, 0.53f, 1f);
        private readonly Color _danger = new(0.86f, 0.16f, 0.08f, 1f);

        private readonly Dictionary<int, Button> _navButtons = new();

        private Font _font;
        private RectTransform _root;
        private Text _deployInfo;
        private Text _screenTitle;
        private Text _screenSubtitle;

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
            Production14HoloDiorama.Build();
            BuildInterface();
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
            canvasObject.AddComponent<GraphicRaycaster>();

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            _root = CreateRect("Root", canvasObject.transform);
            Fill(_root);

            BuildEdgeVignette();
            BuildHeader();
            BuildNavigation();
            BuildOverview();
            BuildFooter();
        }

        private void BuildEdgeVignette()
        {
            var topShade = CreateImage("TopShade", _root, new Color(0.01f, 0.015f, 0.017f, 0.72f));
            Place(topShade.rectTransform, 0f, 0.84f, 1f, 1f);

            var leftShade = CreateImage("LeftShade", _root, new Color(0.005f, 0.012f, 0.014f, 0.34f));
            Place(leftShade.rectTransform, 0f, 0.10f, 0.23f, 0.84f);

            var rightShade = CreateImage("RightShade", _root, new Color(0.005f, 0.012f, 0.014f, 0.26f));
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

            var text = CreateLabel("Text", go.transform, label, 10, FontStyle.Bold, active ? _white : new Color(0.78f, 0.82f, 0.82f, 1f), TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 10f, 8f, 10f, 8f);

            if (active)
            {
                var rail = CreateImage("ActiveRail", go.transform, _cyan);
                Place(rail.rectTransform, 0.12f, 0.02f, 0.88f, 0.055f);
            }

            return button;
        }

        private void HandleNav(int index)
        {
            // Pass 1 is intentionally Overview-only. Do not resurrect the old DEV dashboard
            // behind unfinished tabs while the visual language is still being accepted.
            if (index == 0)
                return;

            _screenTitle.text = index switch
            {
                1 => "ARSENAL",
                2 => "OPERATORS",
                3 => "CAMPAIGN",
                4 => "WORKSHOP",
                _ => "SUPPLY NETWORK"
            };
            _screenSubtitle.text = "PRODUCTION 0.14 // PREMIUM SCREEN PASS PENDING";
            StartCoroutine(FlashPending(index));
        }

        private IEnumerator FlashPending(int index)
        {
            if (!_navButtons.TryGetValue(index, out var button))
                yield break;

            var image = button.image;
            var old = image.color;
            image.color = new Color(0.82f, 0.44f, 0.16f, 1f);
            yield return new WaitForSecondsRealtime(0.16f);
            image.color = old;
        }
    }
}
