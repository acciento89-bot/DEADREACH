using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BunkerCommandCenterUI : MonoBehaviour
    {
        private enum Tab
        {
            Overview,
            Arsenal,
            Operators,
            Campaign,
            Store
        }

        private readonly Color _black = new(0.018f, 0.022f, 0.024f, 0.96f);
        private readonly Color _panel = new(0.055f, 0.061f, 0.063f, 0.96f);
        private readonly Color _panelRaised = new(0.085f, 0.088f, 0.082f, 0.97f);
        private readonly Color _rust = new(0.72f, 0.28f, 0.12f, 1f);
        private readonly Color _hazard = new(0.93f, 0.68f, 0.18f, 1f);
        private readonly Color _green = new(0.18f, 0.8f, 0.46f, 1f);
        private readonly Color _muted = new(0.58f, 0.61f, 0.6f, 1f);
        private readonly Color _text = new(0.9f, 0.91f, 0.87f, 1f);

        private Font _font;
        private RectTransform _contentRoot;
        private Text _headerTitle;
        private Text _headerSubtitle;
        private Text _deployLabel;
        private Tab _currentTab = Tab.Overview;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            BuildInterface();
            ShowTab(Tab.Overview);
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            var eventObject = new GameObject("UI_EventSystem");
            eventObject.AddComponent<EventSystem>();
            var inputModule = eventObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject("Bunker_CommandCenter_Canvas");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 40;
            canvasObject.AddComponent<GraphicRaycaster>();

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var backdrop = CreatePanel("Backdrop", canvasObject.transform, _black);
            Stretch(backdrop);

            var topStripe = CreatePanel("HazardStripe", backdrop, _rust);
            SetRect(topStripe, 0f, 1f, 1f, 1f, 0f, -8f, 0f, 0f);

            var header = CreatePanel("Header", backdrop, new Color(0.035f, 0.04f, 0.041f, 0.98f));
            SetRect(header, 0f, 0.865f, 1f, 1f, 0f, 0f, 0f, -8f);

            CreateText("Brand", header, "DEADREACH", 34, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                new Vector2(34f, -8f), new Vector2(330f, -58f));
            CreateText("BrandSub", header, "BUNKER // COMMAND CENTER", 15, FontStyle.Bold, _hazard, TextAnchor.MiddleLeft,
                new Vector2(37f, -58f), new Vector2(360f, -86f));

            _headerTitle = CreateText("SectionTitle", header, "OVERVIEW", 25, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                new Vector2(405f, -20f), new Vector2(980f, -54f));
            _headerSubtitle = CreateText("SectionSub", header, "FIELD READINESS / CAMPAIGN STATUS", 13, FontStyle.Normal, _muted, TextAnchor.MiddleLeft,
                new Vector2(407f, -56f), new Vector2(1000f, -82f));

            var data = SaveService.Data;
            var rightSummary = $"SCRAP  {data.securedScrap:N0}     //     EXTRACTS  {data.successfulExtractions:N0}     //     BOSS KILLS  {data.bossKills:N0}";
            CreateText("Summary", header, rightSummary, 15, FontStyle.Bold, _green, TextAnchor.MiddleRight,
                new Vector2(930f, -25f), new Vector2(-30f, -65f), true);

            var nav = CreatePanel("Navigation", backdrop, new Color(0.03f, 0.034f, 0.035f, 0.98f));
            SetRect(nav, 0f, 0.09f, 0.22f, 0.865f, 18f, 18f, -8f, -8f);
            AddNavButton(nav, "OVERVIEW", Tab.Overview, 0);
            AddNavButton(nav, "ARSENAL", Tab.Arsenal, 1);
            AddNavButton(nav, "OPERATORS", Tab.Operators, 2);
            AddNavButton(nav, "CAMPAIGN", Tab.Campaign, 3);
            AddNavButton(nav, "STORE", Tab.Store, 4);

            var warning = CreateText("Warning", nav,
                "BUNKER STATUS\nSEALED / POWER NOMINAL\nEXTERNAL THREAT: CRITICAL",
                12, FontStyle.Bold, _muted, TextAnchor.LowerLeft,
                new Vector2(20f, 18f), new Vector2(-18f, 120f), true);
            warning.horizontalOverflow = HorizontalWrapMode.Wrap;

            var content = CreatePanel("ContentFrame", backdrop, new Color(0.035f, 0.039f, 0.04f, 0.94f));
            SetRect(content, 0.22f, 0.09f, 1f, 0.865f, 6f, 18f, -18f, -8f);

            var contentViewport = CreatePanel("ContentViewport", content, Color.clear);
            SetRect(contentViewport, 0f, 0f, 1f, 1f, 20f, 18f, -20f, -18f);
            _contentRoot = contentViewport;

            var deployBar = CreatePanel("DeployBar", backdrop, new Color(0.025f, 0.03f, 0.03f, 0.99f));
            SetRect(deployBar, 0f, 0f, 1f, 0.09f, 18f, 10f, -18f, -4f);
            _deployLabel = CreateText("DeployInfo", deployBar, string.Empty, 15, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                new Vector2(24f, 0f), new Vector2(1010f, 0f), true);
            SetTextVerticalStretch(_deployLabel.rectTransform, 10f, -10f);

            var deploy = CreateButton("DeployButton", deployBar, "DEPLOY TO DEAD CITY", _rust, () => SceneFlowService.LoadExpedition());
            SetRect(deploy.GetComponent<RectTransform>(), 0.72f, 0.14f, 0.985f, 0.86f, 0f, 0f, 0f, 0f);
            RefreshDeployLabel();
        }

        private void AddNavButton(Transform parent, string label, Tab tab, int index)
        {
            var button = CreateButton($"Nav_{label}", parent, label, new Color(0.09f, 0.095f, 0.09f, 1f), () => ShowTab(tab));
            var rect = button.GetComponent<RectTransform>();
            var top = 0.94f - index * 0.112f;
            SetRect(rect, 0.06f, top - 0.085f, 0.94f, top, 0f, 0f, 0f, 0f);
        }

        private void ShowTab(Tab tab)
        {
            _currentTab = tab;
            ClearChildren(_contentRoot);
            RefreshDeployLabel();

            switch (tab)
            {
                case Tab.Overview:
                    _headerTitle.text = "OVERVIEW";
                    _headerSubtitle.text = "FIELD READINESS / CAMPAIGN STATUS";
                    BuildOverview();
                    break;
                case Tab.Arsenal:
                    _headerTitle.text = "ARSENAL";
                    _headerSubtitle.text = "SECURED WEAPONS / AFFIX ANALYSIS";
                    BuildArsenal();
                    break;
                case Tab.Operators:
                    _headerTitle.text = "OPERATORS";
                    _headerSubtitle.text = "SURVIVOR LOADOUT / FIELD SPECIALIZATION";
                    BuildOperators();
                    break;
                case Tab.Campaign:
                    _headerTitle.text = "CAMPAIGN";
                    _headerSubtitle.text = "50-LEVEL DEADREACH PROGRESSION / BOSS EVERY 10";
                    BuildCampaign();
                    break;
                case Tab.Store:
                    _headerTitle.text = "STORE";
                    _headerSubtitle.text = "COSMETICS / BUNKER THEMES / SEASON CONTENT";
                    BuildStore();
                    break;
            }
        }

        private void BuildOverview()
        {
            var data = SaveService.Data;
            var selectedOperator = OperatorCatalog.Get(data.selectedCharacterId);
            var weapon = SaveService.GetEquippedPrimaryWeapon();

            var readiness = CreatePanel("Readiness", _contentRoot, _panel);
            SetRect(readiness, 0f, 0.54f, 0.62f, 1f, 0f, 0f, -12f, 0f);
            AddSectionLabel(readiness, "NEXT DEPLOYMENT", _hazard);
            CreateText("NextLevel", readiness,
                $"LEVEL {data.selectedLevel:00}  //  {RunDifficultyDirector.GetZoneName(data.selectedLevel)}",
                27, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                new Vector2(26f, -72f), new Vector2(-24f, -116f), true);

            var boss = data.selectedLevel % 10 == 0;
            CreateText("Threat", readiness,
                boss ? "BOSS ENCOUNTER // EXTRACTION LOCKED UNTIL TARGET ELIMINATED" : "STANDARD EXPEDITION // EXTRACTION AVAILABLE WITH LOOT",
                14, FontStyle.Bold, boss ? _rust : _green, TextAnchor.MiddleLeft,
                new Vector2(28f, -122f), new Vector2(-24f, -154f), true);

            CreateText("Operator", readiness,
                $"OPERATOR  {selectedOperator.Name} // {selectedOperator.Role}\nPRIMARY   {(weapon != null ? weapon.displayNameSnapshot : "DR-7 FIELD ISSUE")}\nSTREAK    {data.currentExtractionStreak}   //   BEST {data.bestExtractionStreak}",
                16, FontStyle.Normal, _text, TextAnchor.UpperLeft,
                new Vector2(28f, -185f), new Vector2(-26f, -310f), true);

            var progress = CreatePanel("Progress", _contentRoot, _panel);
            SetRect(progress, 0.62f, 0.54f, 1f, 1f, 12f, 0f, 0f, 0f);
            AddSectionLabel(progress, "CAMPAIGN PROGRESS", _green);
            CreateText("ProgressText", progress,
                $"HIGHEST UNLOCKED\nLEVEL {data.highestUnlockedLevel:00} / {SaveService.MaxCampaignLevel}\n\nHIGHEST CLEARED\nLEVEL {data.highestCompletedLevel:00}\n\nBOSS KILLS\n{data.bossKills}",
                17, FontStyle.Bold, _text, TextAnchor.UpperLeft,
                new Vector2(26f, -78f), new Vector2(-22f, -300f), true);

            var intel = CreatePanel("Intel", _contentRoot, _panelRaised);
            SetRect(intel, 0f, 0f, 1f, 0.5f, 0f, 0f, 0f, -14f);
            AddSectionLabel(intel, "BUNKER INTEL", _rust);
            CreateText("IntelBody", intel,
                "SURVIVAL DIRECTIVE\nGo deeper, secure weapon cases, extract before the run collapses. Every tenth level contains a mutation-class target. Boss extraction stays sealed until the target is eliminated.\n\nPROGRESSION\nExtracting a level unlocks the next deployment. Weapons remain persistent only after successful extraction. Operator choice and campaign selection are stored in your bunker profile.",
                15, FontStyle.Normal, _text, TextAnchor.UpperLeft,
                new Vector2(28f, -72f), new Vector2(-28f, -250f), true);
        }

        private void BuildArsenal()
        {
            var data = SaveService.Data;
            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
            {
                AddEmptyState("NO SECURED WEAPONS", "Extract weapon cases from Dead City to populate the bunker arsenal.");
                return;
            }

            var scroll = CreateScrollArea("ArsenalScroll");
            var list = scroll.content;

            for (var index = data.stashWeapons.Count - 1; index >= 0; index--)
            {
                var weapon = data.stashWeapons[index];
                if (weapon == null)
                    continue;

                var capturedId = weapon.instanceId;
                var equipped = capturedId == data.equippedPrimaryWeaponId;
                var card = CreateLayoutPanel($"Weapon_{index}", list, equipped ? new Color(0.13f, 0.12f, 0.075f, 1f) : _panel, 142f);
                CreateCardStripe(card, RarityColor(weapon.rarity));

                var title = $"{weapon.rarity.ToString().ToUpperInvariant()} // {weapon.displayNameSnapshot}";
                CreateAnchoredText(card, title, 19, FontStyle.Bold, _text, 22f, -15f, 680f, 34f);
                CreateAnchoredText(card, $"ITEM POWER {weapon.itemPower}   {(equipped ? "// EQUIPPED" : string.Empty)}", 13, FontStyle.Bold, equipped ? _green : _hazard, 22f, -50f, 650f, 28f);
                CreateAnchoredText(card, BuildAffixText(weapon), 13, FontStyle.Normal, _muted, 22f, -82f, 780f, 50f);

                var button = CreateButton("Equip", card, equipped ? "EQUIPPED" : "EQUIP", equipped ? _green : _rust, () =>
                {
                    SaveService.EquipPrimaryWeapon(capturedId);
                    ShowTab(Tab.Arsenal);
                });
                button.interactable = !equipped;
                SetRect(button.GetComponent<RectTransform>(), 0.81f, 0.24f, 0.975f, 0.76f, 0f, 0f, 0f, 0f);
            }
        }

        private void BuildOperators()
        {
            var data = SaveService.Data;
            var holder = CreatePanel("OperatorHolder", _contentRoot, Color.clear);
            Stretch(holder);

            var definitions = OperatorCatalog.All;
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                var selected = definition.Id == data.selectedCharacterId;
                var xMin = 0.015f + i * 0.33f;
                var xMax = xMin + 0.305f;
                var card = CreatePanel($"Operator_{definition.Id}", holder, selected ? new Color(0.12f, 0.11f, 0.075f, 0.98f) : _panel);
                SetRect(card, xMin, 0.08f, xMax, 0.95f, 0f, 0f, 0f, 0f);

                var portrait = CreatePanel("Portrait", card, new Color(definition.Accent.r * 0.25f, definition.Accent.g * 0.25f, definition.Accent.b * 0.25f, 1f));
                SetRect(portrait, 0.08f, 0.58f, 0.92f, 0.92f, 0f, 0f, 0f, 0f);
                CreateText("Monogram", portrait, definition.Name.Substring(0, 1), 66, FontStyle.Bold, definition.Accent, TextAnchor.MiddleCenter,
                    Vector2.zero, Vector2.zero, true);
                Stretch(portrait.GetComponentInChildren<Text>().rectTransform);

                CreateAnchoredText(card, definition.Name, 27, FontStyle.Bold, _text, 24f, -315f, 340f, 40f);
                CreateAnchoredText(card, definition.Role, 14, FontStyle.Bold, definition.Accent, 24f, -355f, 340f, 28f);
                CreateAnchoredText(card, definition.Description, 13, FontStyle.Normal, _muted, 24f, -398f, 340f, 68f);
                CreateAnchoredText(card,
                    $"VITALS  x{definition.HealthMultiplier:0.00}\nMOBILITY x{definition.MoveMultiplier:0.00}\nDAMAGE   x{definition.DamageMultiplier:0.00}",
                    13, FontStyle.Bold, _text, 24f, -478f, 340f, 78f);

                var captured = definition.Id;
                var selectButton = CreateButton("Select", card, selected ? "ACTIVE" : "SELECT OPERATOR", selected ? _green : _rust, () =>
                {
                    SaveService.SelectCharacter(captured);
                    ShowTab(Tab.Operators);
                });
                selectButton.interactable = !selected;
                SetRect(selectButton.GetComponent<RectTransform>(), 0.08f, 0.05f, 0.92f, 0.14f, 0f, 0f, 0f, 0f);
            }
        }

        private void BuildCampaign()
        {
            var scroll = CreateScrollArea("CampaignScroll");
            var root = scroll.content;
            var data = SaveService.Data;

            for (var sector = 0; sector < 5; sector++)
            {
                var firstLevel = sector * 10 + 1;
                var sectorPanel = CreateLayoutPanel($"Sector_{sector}", root, _panel, 250f);
                CreateAnchoredText(sectorPanel,
                    $"SECTOR {sector + 1:00} // {RunDifficultyDirector.GetZoneName(firstLevel)}",
                    19, FontStyle.Bold, _hazard, 22f, -15f, 820f, 34f);

                for (var slot = 0; slot < 10; slot++)
                {
                    var level = firstLevel + slot;
                    var unlocked = level <= data.highestUnlockedLevel;
                    var selected = level == data.selectedLevel;
                    var boss = level % 10 == 0;
                    var col = slot % 5;
                    var row = slot / 5;
                    var x = 22f + col * 174f;
                    var y = -62f - row * 78f;
                    var label = boss ? $"LEVEL {level:00}\nBOSS" : $"LEVEL {level:00}";
                    if (!unlocked) label += "\nLOCKED";

                    var captured = level;
                    var color = selected ? _green : boss ? _rust : unlocked ? new Color(0.16f, 0.17f, 0.16f, 1f) : new Color(0.06f, 0.065f, 0.065f, 1f);
                    var button = CreateButton($"Level_{level}", sectorPanel, label, color, () =>
                    {
                        if (SaveService.SelectLevel(captured))
                            ShowTab(Tab.Campaign);
                    });
                    button.interactable = unlocked;
                    SetFixedRect(button.GetComponent<RectTransform>(), x, y, 158f, 62f);
                }
            }
        }

        private void BuildStore()
        {
            var intro = CreatePanel("StoreIntro", _contentRoot, _panel);
            SetRect(intro, 0f, 0.76f, 1f, 1f, 0f, 0f, 0f, 0f);
            AddSectionLabel(intro, "SUPPLY NETWORK", _hazard);
            CreateText("StoreInfo", intro,
                "Fair-content storefront. No stat boosts, no pay-to-win. Purchase verification is intentionally not enabled in this 0.5 gameplay pass; these cards define the production store surface for StoreKit / Google Play integration.",
                14, FontStyle.Normal, _text, TextAnchor.UpperLeft,
                new Vector2(28f, -72f), new Vector2(-28f, -145f), true);

            var products = new[]
            {
                ("skin_rustwalker", "RUSTWALKER OPERATOR SKINS", "3 cosmetic operator variants", "COSMETIC"),
                ("theme_red_bunker", "REDLINE BUNKER THEME", "Post-apocalyptic red emergency bunker set", "BUNKER THEME"),
                ("weapon_neon_pack", "WASTELAND WEAPON FINISHES", "Weapon finish collection for secured guns", "COSMETIC"),
                ("season_01", "SEASON 01 // AFTERSHOCK", "Challenges, cosmetics and profile rewards", "SEASON")
            };

            for (var i = 0; i < products.Length; i++)
            {
                var product = products[i];
                var row = i / 2;
                var col = i % 2;
                var xMin = col == 0 ? 0f : 0.51f;
                var xMax = col == 0 ? 0.49f : 1f;
                var yMax = 0.71f - row * 0.34f;
                var yMin = yMax - 0.3f;
                var card = CreatePanel($"Product_{i}", _contentRoot, _panelRaised);
                SetRect(card, xMin, yMin, xMax, yMax, 0f, 0f, 0f, 0f);
                CreateAnchoredText(card, product.Item2, 17, FontStyle.Bold, _text, 22f, -18f, 520f, 34f);
                CreateAnchoredText(card, product.Item4, 11, FontStyle.Bold, _hazard, 22f, -52f, 480f, 24f);
                CreateAnchoredText(card, product.Item3, 13, FontStyle.Normal, _muted, 22f, -84f, 480f, 48f);

                var owned = SaveService.OwnsContent(product.Item1);
                var status = CreateButton("Status", card, owned ? "OWNED" : "COMING SOON", owned ? _green : _rust, null);
                status.interactable = false;
                SetRect(status.GetComponent<RectTransform>(), 0.66f, 0.12f, 0.95f, 0.34f, 0f, 0f, 0f, 0f);
            }
        }

        private string BuildAffixText(WeaponInstanceData weapon)
        {
            if (weapon.affixes == null || weapon.affixes.Count == 0)
                return "AFFIXES // NONE";

            var parts = new List<string>();
            foreach (var affix in weapon.affixes)
            {
                if (affix == null)
                    continue;
                parts.Add($"{AffixName(affix.stat)} +{affix.value:0.#}%");
            }

            return "AFFIXES // " + string.Join("   |   ", parts);
        }

        private static string AffixName(WeaponAffixStat stat)
        {
            return stat switch
            {
                WeaponAffixStat.DamagePercent => "DAMAGE",
                WeaponAffixStat.FireRatePercent => "FIRE RATE",
                WeaponAffixStat.RangePercent => "RANGE",
                WeaponAffixStat.CritChancePercent => "CRIT CHANCE",
                WeaponAffixStat.CritDamagePercent => "CRIT DAMAGE",
                _ => stat.ToString().ToUpperInvariant()
            };
        }

        private Color RarityColor(WeaponRarity rarity)
        {
            return rarity switch
            {
                WeaponRarity.Common => new Color(0.55f, 0.58f, 0.56f),
                WeaponRarity.Uncommon => new Color(0.22f, 0.72f, 0.36f),
                WeaponRarity.Rare => new Color(0.18f, 0.5f, 0.92f),
                WeaponRarity.Epic => new Color(0.68f, 0.25f, 0.9f),
                WeaponRarity.Legendary => new Color(1f, 0.55f, 0.12f),
                _ => _muted
            };
        }

        private void RefreshDeployLabel()
        {
            if (_deployLabel == null)
                return;

            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);
            _deployLabel.text = $"READY // LEVEL {data.selectedLevel:00} // {RunDifficultyDirector.GetZoneName(data.selectedLevel)}   |   OPERATOR {op.Name}   |   {(data.selectedLevel % 10 == 0 ? "BOSS TARGET ACTIVE" : "STANDARD EXPEDITION")}";
        }

        private void AddEmptyState(string title, string body)
        {
            var panel = CreatePanel("EmptyState", _contentRoot, _panel);
            SetRect(panel, 0.08f, 0.25f, 0.92f, 0.75f, 0f, 0f, 0f, 0f);
            CreateText("Title", panel, title, 25, FontStyle.Bold, _hazard, TextAnchor.MiddleCenter,
                new Vector2(30f, -80f), new Vector2(-30f, -130f), true);
            CreateText("Body", panel, body, 15, FontStyle.Normal, _muted, TextAnchor.UpperCenter,
                new Vector2(40f, -145f), new Vector2(-40f, -215f), true);
        }

        private (ScrollRect scroll, RectTransform content) CreateScrollArea(string name)
        {
            var root = CreatePanel(name, _contentRoot, Color.clear);
            Stretch(root);

            var viewport = CreatePanel("Viewport", root, Color.clear);
            Stretch(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = CreatePanel("ScrollContent", viewport, Color.clear);
            var contentRect = content;
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = new Vector2(0f, 0f);
            contentRect.offsetMax = new Vector2(0f, 0f);

            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 14, 4, 18);
            layout.spacing = 12f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = root.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = 28f;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            return (scroll, contentRect);
        }

        private RectTransform CreateLayoutPanel(string name, Transform parent, Color color, float height)
        {
            var rect = CreatePanel(name, parent, color);
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            return rect;
        }

        private void AddSectionLabel(Transform parent, string label, Color color)
        {
            CreateAnchoredText(parent, label, 13, FontStyle.Bold, color, 26f, -18f, 720f, 28f);
        }

        private void CreateCardStripe(Transform parent, Color color)
        {
            var stripe = CreatePanel("RarityStripe", parent, color);
            SetRect(stripe, 0f, 0f, 0f, 1f, 0f, 0f, 7f, 0f);
        }

        private RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = color.a > 0.01f;
            return gameObject.GetComponent<RectTransform>();
        }

        private Text CreateText(string name, Transform parent, string value, int fontSize, FontStyle style, Color color, TextAnchor anchor, Vector2 offsetMin, Vector2 offsetMax, bool anchorsStretch = false)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = anchor;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var rect = text.rectTransform;
            if (anchorsStretch)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
            }
            else
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
            }
            rect.pivot = new Vector2(0f, 1f);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return text;
        }

        private Text CreateAnchoredText(Transform parent, string value, int fontSize, FontStyle style, Color color, float x, float y, float width, float height)
        {
            var text = CreateText("Text", parent, value, fontSize, style, color, TextAnchor.UpperLeft, Vector2.zero, Vector2.zero);
            SetFixedRect(text.rectTransform, x, y, width, height);
            return text;
        }

        private Button CreateButton(string name, Transform parent, string label, Color color, Action action)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            var button = gameObject.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.22f);
            colors.disabledColor = new Color(color.r * 0.45f, color.g * 0.45f, color.b * 0.45f, 0.75f);
            button.colors = colors;
            if (action != null)
                button.onClick.AddListener(() => action());

            var labelText = CreateText("Label", gameObject.transform, label, 14, FontStyle.Bold, _text, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero, true);
            Stretch(labelText.rectTransform, 8f, 8f, -8f, -8f);
            return button;
        }

        private static void SetRect(RectTransform rect, float xMin, float yMin, float xMax, float yMax, float left, float bottom, float right, float top)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(right, top);
        }

        private static void SetFixedRect(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void Stretch(RectTransform rect, float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(right, top);
        }

        private static void SetTextVerticalStretch(RectTransform rect, float top, float bottom)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.offsetMin = new Vector2(rect.offsetMin.x, top);
            rect.offsetMax = new Vector2(rect.offsetMax.x, bottom);
        }

        private static void ClearChildren(Transform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }
    }
}
