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

        private readonly Color _background = new(0.012f, 0.014f, 0.014f, 0.985f);
        private readonly Color _panel = new(0.046f, 0.05f, 0.048f, 0.985f);
        private readonly Color _panelRaised = new(0.073f, 0.073f, 0.066f, 0.99f);
        private readonly Color _panelDark = new(0.025f, 0.029f, 0.028f, 0.99f);
        private readonly Color _rust = new(0.72f, 0.22f, 0.07f, 1f);
        private readonly Color _hazard = new(0.96f, 0.66f, 0.12f, 1f);
        private readonly Color _green = new(0.12f, 0.82f, 0.43f, 1f);
        private readonly Color _muted = new(0.52f, 0.55f, 0.53f, 1f);
        private readonly Color _text = new(0.92f, 0.92f, 0.88f, 1f);
        private readonly Color _line = new(0.16f, 0.17f, 0.16f, 1f);

        private readonly Dictionary<Tab, Image> _navImages = new();

        private Font _font;
        private RectTransform _contentRoot;
        private Text _sectionTitle;
        private Text _sectionSubtitle;
        private Text _deployLabel;
        private Tab _currentTab;
        private int _campaignSector;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            BuildInterface();
            _campaignSector = Mathf.Clamp((SaveService.Data.selectedLevel - 1) / 10, 0, 4);
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

            var backdrop = CreatePanel("Backdrop", canvasObject.transform, _background);
            Fill(backdrop);

            var topStripe = CreatePanel("TopHazardStripe", backdrop, _rust);
            Place(topStripe, 0f, 0.993f, 1f, 1f);

            var header = CreatePanel("Header", backdrop, new Color(0.022f, 0.026f, 0.025f, 0.995f));
            Place(header, 0f, 0.865f, 1f, 0.993f, 18f, 8f, 18f, 8f);

            CreateLabel("Brand", header, "DEADREACH", 34, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.02f, 0.43f, 0.245f, 0.91f);
            CreateLabel("BrandSub", header, "BUNKER // COMMAND CENTER", 13, FontStyle.Bold, _hazard, TextAnchor.UpperLeft,
                0.022f, 0.16f, 0.26f, 0.45f);

            _sectionTitle = CreateLabel("SectionTitle", header, "OVERVIEW", 25, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.29f, 0.48f, 0.59f, 0.9f);
            _sectionSubtitle = CreateLabel("SectionSubtitle", header, "FIELD READINESS / CAMPAIGN STATUS", 12, FontStyle.Bold, _muted, TextAnchor.UpperLeft,
                0.292f, 0.18f, 0.62f, 0.5f);

            var data = SaveService.Data;
            CreateLabel("ProfileSummary", header,
                $"SCRAP  {data.securedScrap:N0}   //   EXTRACTS  {data.successfulExtractions:N0}   //   BOSS KILLS  {data.bossKills:N0}",
                14, FontStyle.Bold, _green, TextAnchor.MiddleRight, 0.62f, 0.27f, 0.985f, 0.78f);

            var nav = CreatePanel("Navigation", backdrop, _panelDark);
            Place(nav, 0.012f, 0.09f, 0.205f, 0.85f);
            CreateLabel("NavCallsign", nav, "BUNKER NETWORK", 11, FontStyle.Bold, _hazard, TextAnchor.MiddleLeft,
                0.07f, 0.91f, 0.92f, 0.965f);

            AddNavButton(nav, "OVERVIEW", Tab.Overview, 0);
            AddNavButton(nav, "ARSENAL", Tab.Arsenal, 1);
            AddNavButton(nav, "OPERATORS", Tab.Operators, 2);
            AddNavButton(nav, "CAMPAIGN", Tab.Campaign, 3);
            AddNavButton(nav, "STORE", Tab.Store, 4);

            CreateLabel("BunkerStatus", nav,
                "BUNKER STATUS\nSEALED // POWER NOMINAL\nEXTERNAL THREAT // CRITICAL",
                11, FontStyle.Bold, _muted, TextAnchor.LowerLeft,
                0.07f, 0.035f, 0.92f, 0.20f);

            var contentFrame = CreatePanel("ContentFrame", backdrop, new Color(0.028f, 0.031f, 0.03f, 0.985f));
            Place(contentFrame, 0.215f, 0.09f, 0.988f, 0.85f);
            var contentAccent = CreatePanel("ContentAccent", contentFrame, _rust);
            Place(contentAccent, 0f, 0.993f, 1f, 1f);

            _contentRoot = CreatePanel("ContentViewport", contentFrame, Color.clear);
            Place(_contentRoot, 0.016f, 0.022f, 0.984f, 0.976f);

            var deployBar = CreatePanel("DeployBar", backdrop, new Color(0.01f, 0.045f, 0.041f, 0.995f));
            Place(deployBar, 0.012f, 0.012f, 0.988f, 0.075f);
            var deployAccent = CreatePanel("DeployAccent", deployBar, _green);
            Place(deployAccent, 0f, 0f, 0.006f, 1f);

            _deployLabel = CreateLabel("DeployInfo", deployBar, string.Empty, 14, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.02f, 0.12f, 0.70f, 0.88f);

            var deployButton = CreateButton("DeployButton", deployBar, "DEPLOY", _rust, () => SceneFlowService.LoadExpedition());
            Place(deployButton.GetComponent<RectTransform>(), 0.72f, 0.13f, 0.985f, 0.87f);
            RefreshDeployLabel();
        }

        private void AddNavButton(Transform parent, string label, Tab tab, int index)
        {
            var yMax = 0.85f - index * 0.115f;
            var yMin = yMax - 0.082f;
            var button = CreateButton($"Nav_{label}", parent, label, new Color(0.055f, 0.06f, 0.058f, 1f), () => ShowTab(tab));
            Place(button.GetComponent<RectTransform>(), 0.055f, yMin, 0.945f, yMax);
            _navImages[tab] = button.image;
        }

        private void ShowTab(Tab tab)
        {
            _currentTab = tab;
            ClearChildren(_contentRoot);
            RefreshDeployLabel();

            foreach (var pair in _navImages)
                pair.Value.color = pair.Key == tab ? new Color(0.22f, 0.12f, 0.055f, 1f) : new Color(0.055f, 0.06f, 0.058f, 1f);

            switch (tab)
            {
                case Tab.Overview:
                    SetHeader("OVERVIEW", "FIELD READINESS // BUNKER INTELLIGENCE");
                    BuildOverview();
                    break;
                case Tab.Arsenal:
                    SetHeader("ARSENAL", "SECURED WEAPONS // AFFIX ANALYSIS // 3D INSPECTOR");
                    BuildArsenal();
                    break;
                case Tab.Operators:
                    SetHeader("OPERATORS", "SURVIVOR ROSTER // FIELD SPECIALIZATION");
                    BuildOperators();
                    break;
                case Tab.Campaign:
                    SetHeader("CAMPAIGN", "50-LEVEL PROGRESSION // MUTATION BOSS EVERY 10");
                    BuildCampaign();
                    break;
                case Tab.Store:
                    SetHeader("SUPPLY NETWORK", "COSMETICS // BUNKER THEMES // SEASON CONTENT");
                    BuildStore();
                    break;
            }
        }

        private void SetHeader(string title, string subtitle)
        {
            _sectionTitle.text = title;
            _sectionSubtitle.text = subtitle;
        }

        private void BuildOverview()
        {
            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);
            var weapon = SaveService.GetEquippedPrimaryWeapon();
            var boss = data.selectedLevel % 10 == 0;

            var mission = CreatePanel("MissionPanel", _contentRoot, _panel);
            Place(mission, 0f, 0.49f, 0.645f, 1f);
            AddPanelHeader(mission, "NEXT DEPLOYMENT", _hazard);
            CreateLabel("Ready", mission, boss ? "MUTATION TARGET" : "DEPLOYMENT READY", 12, FontStyle.Bold, boss ? _rust : _green, TextAnchor.MiddleLeft,
                0.045f, 0.73f, 0.5f, 0.80f);
            CreateLabel("Level", mission, $"LEVEL {data.selectedLevel:00}", 44, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.045f, 0.50f, 0.42f, 0.72f);
            CreateLabel("Zone", mission, RunDifficultyDirector.GetZoneName(data.selectedLevel), 19, FontStyle.Bold, _hazard, TextAnchor.MiddleLeft,
                0.045f, 0.405f, 0.78f, 0.51f);
            CreateLabel("MissionLine", mission,
                boss ? "EXTRACTION SEALED UNTIL MUTATION-CLASS TARGET IS ELIMINATED." : "STANDARD EXPEDITION // LOOT REQUIRED FOR EXTRACTION.",
                13, FontStyle.Bold, boss ? _rust : _green, TextAnchor.MiddleLeft,
                0.045f, 0.30f, 0.94f, 0.39f);
            CreateLabel("Loadout", mission,
                $"OPERATOR  {op.Name} // {op.Role}\nPRIMARY   {(weapon != null ? weapon.displayNameSnapshot : "DR-7 FIELD ISSUE")}\nSTREAK    {data.currentExtractionStreak}   //   BEST {data.bestExtractionStreak}",
                14, FontStyle.Normal, _text, TextAnchor.UpperLeft,
                0.045f, 0.055f, 0.94f, 0.27f);

            var progress = CreatePanel("ProgressPanel", _contentRoot, _panelRaised);
            Place(progress, 0.665f, 0.49f, 1f, 1f);
            AddPanelHeader(progress, "CAMPAIGN STATUS", _green);
            CreateMetric(progress, "HIGHEST UNLOCKED", $"{data.highestUnlockedLevel:00} / {SaveService.MaxCampaignLevel}", 0.62f);
            CreateMetric(progress, "HIGHEST CLEARED", data.highestCompletedLevel.ToString("00"), 0.37f);
            CreateMetric(progress, "BOSS KILLS", data.bossKills.ToString(), 0.12f);

            var intel = CreatePanel("IntelPanel", _contentRoot, _panelDark);
            Place(intel, 0f, 0f, 1f, 0.455f);
            AddPanelHeader(intel, "BUNKER INTEL", _rust);

            var divider = CreatePanel("IntelDivider", intel, _line);
            Place(divider, 0.5f, 0.10f, 0.502f, 0.76f);
            CreateLabel("DirectiveTitle", intel, "SURVIVAL DIRECTIVE", 15, FontStyle.Bold, _text, TextAnchor.UpperLeft,
                0.035f, 0.66f, 0.46f, 0.76f);
            CreateLabel("Directive", intel,
                "Go deeper. Secure weapon cases. Decide when the risk is worth it. Extraction banks Scrap and weapons; death or abandonment destroys everything unsecured.",
                13, FontStyle.Normal, _muted, TextAnchor.UpperLeft,
                0.035f, 0.18f, 0.46f, 0.64f);
            CreateLabel("ProgressionTitle", intel, "PROGRESSION PROTOCOL", 15, FontStyle.Bold, _text, TextAnchor.UpperLeft,
                0.54f, 0.66f, 0.96f, 0.76f);
            CreateLabel("Progression", intel,
                "Each successful level unlocks the next deployment. Every tenth level is a boss operation. Arsenal and operator choices persist between runs.",
                13, FontStyle.Normal, _muted, TextAnchor.UpperLeft,
                0.54f, 0.18f, 0.96f, 0.64f);
        }

        private void BuildArsenal()
        {
            var data = SaveService.Data;
            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
            {
                AddEmptyState("NO SECURED WEAPONS", "Extract weapon cases from Dead City to populate the bunker arsenal.");
                return;
            }

            var listFrame = CreatePanel("ArsenalList", _contentRoot, _panelDark);
            Place(listFrame, 0f, 0f, 0.635f, 1f);
            AddPanelHeader(listFrame, "SECURED INVENTORY", _hazard);

            var scrollHost = CreatePanel("ArsenalScrollHost", listFrame, Color.clear);
            Place(scrollHost, 0.02f, 0.035f, 0.98f, 0.86f);
            var list = CreateScrollArea("ArsenalScroll", scrollHost).content;

            for (var index = data.stashWeapons.Count - 1; index >= 0; index--)
            {
                var weapon = data.stashWeapons[index];
                if (weapon == null)
                    continue;

                var capturedId = weapon.instanceId;
                var equipped = capturedId == data.equippedPrimaryWeaponId;
                var card = CreateLayoutPanel($"Weapon_{index}", list,
                    equipped ? new Color(0.105f, 0.095f, 0.055f, 1f) : _panel, 122f);
                CreateCardStripe(card, RarityColor(weapon.rarity));

                CreateLabel("WeaponTitle", card,
                    $"{weapon.rarity.ToString().ToUpperInvariant()} // {weapon.displayNameSnapshot}",
                    17, FontStyle.Bold, _text, TextAnchor.MiddleLeft, 0.035f, 0.67f, 0.74f, 0.93f);
                CreateLabel("WeaponPower", card,
                    $"ITEM POWER {weapon.itemPower}   {(equipped ? "// EQUIPPED" : string.Empty)}",
                    12, FontStyle.Bold, equipped ? _green : _hazard, TextAnchor.MiddleLeft,
                    0.035f, 0.43f, 0.72f, 0.65f);
                CreateLabel("WeaponAffixes", card, BuildAffixText(weapon), 11, FontStyle.Normal, _muted, TextAnchor.UpperLeft,
                    0.035f, 0.08f, 0.74f, 0.40f);

                var equipButton = CreateButton("Equip", card, equipped ? "EQUIPPED" : "EQUIP", equipped ? _green : _rust, () =>
                {
                    SaveService.EquipPrimaryWeapon(capturedId);
                    ShowTab(Tab.Arsenal);
                });
                equipButton.interactable = !equipped;
                Place(equipButton.GetComponent<RectTransform>(), 0.78f, 0.26f, 0.965f, 0.74f);
            }

            var inspector = CreatePanel("ArsenalInspector", _contentRoot, new Color(0.012f, 0.045f, 0.042f, 0.94f));
            Place(inspector, 0.655f, 0f, 1f, 1f);
            AddPanelHeader(inspector, "3D WEAPON INSPECTOR", _rust);
            CreateLabel("InspectorHint", inspector, "ROTATING PRODUCTION MODEL", 11, FontStyle.Bold, _muted, TextAnchor.MiddleCenter,
                0.05f, 0.75f, 0.95f, 0.82f);

            var equippedWeapon = SaveService.GetEquippedPrimaryWeapon();
            CreateLabel("InspectorWeapon", inspector,
                equippedWeapon != null ? equippedWeapon.displayNameSnapshot.ToUpperInvariant() : "FIELD-ISSUE DR-7",
                18, FontStyle.Bold, _text, TextAnchor.MiddleCenter,
                0.06f, 0.15f, 0.94f, 0.25f);
            CreateLabel("InspectorSub", inspector,
                equippedWeapon != null ? $"{equippedWeapon.rarity.ToString().ToUpperInvariant()} // PWR {equippedWeapon.itemPower}" : "BASE LOADOUT",
                12, FontStyle.Bold, equippedWeapon != null ? RarityColor(equippedWeapon.rarity) : _muted, TextAnchor.MiddleCenter,
                0.06f, 0.08f, 0.94f, 0.15f);
        }

        private void BuildOperators()
        {
            var data = SaveService.Data;
            var active = OperatorCatalog.Get(data.selectedCharacterId);

            var list = CreatePanel("OperatorList", _contentRoot, _panelDark);
            Place(list, 0f, 0f, 0.47f, 1f);
            AddPanelHeader(list, "SURVIVOR ROSTER", _hazard);

            var definitions = OperatorCatalog.All;
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                var selected = definition.Id == data.selectedCharacterId;
                var yMax = 0.84f - i * 0.265f;
                var yMin = yMax - 0.225f;
                var card = CreatePanel($"Operator_{definition.Id}", list,
                    selected ? new Color(0.12f, 0.10f, 0.052f, 1f) : _panel);
                Place(card, 0.035f, yMin, 0.965f, yMax);

                var stripe = CreatePanel("OperatorStripe", card, definition.Accent);
                Place(stripe, 0f, 0f, 0.012f, 1f);
                CreateLabel("Name", card, definition.Name, 22, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                    0.055f, 0.58f, 0.48f, 0.92f);
                CreateLabel("Role", card, definition.Role, 11, FontStyle.Bold, definition.Accent, TextAnchor.MiddleLeft,
                    0.055f, 0.40f, 0.48f, 0.61f);
                CreateLabel("Desc", card, definition.Description, 11, FontStyle.Normal, _muted, TextAnchor.UpperLeft,
                    0.055f, 0.08f, 0.67f, 0.37f);

                var captured = definition.Id;
                var select = CreateButton("Select", card, selected ? "ACTIVE" : "SELECT", selected ? _green : _rust, () =>
                {
                    SaveService.SelectCharacter(captured);
                    ShowTab(Tab.Operators);
                });
                select.interactable = !selected;
                Place(select.GetComponent<RectTransform>(), 0.73f, 0.25f, 0.955f, 0.70f);
            }

            var inspector = CreatePanel("OperatorInspector", _contentRoot, new Color(0.035f, 0.038f, 0.035f, 0.98f));
            Place(inspector, 0.49f, 0f, 1f, 1f);
            AddPanelHeader(inspector, "ACTIVE OPERATOR // 3D FIELD PREVIEW", active.Accent);
            CreateLabel("ActiveName", inspector, active.Name, 34, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.055f, 0.18f, 0.46f, 0.29f);
            CreateLabel("ActiveRole", inspector, active.Role, 14, FontStyle.Bold, active.Accent, TextAnchor.MiddleLeft,
                0.055f, 0.11f, 0.46f, 0.18f);
            CreateLabel("Stats", inspector,
                $"VITALS   x{active.HealthMultiplier:0.00}\nMOBILITY x{active.MoveMultiplier:0.00}\nDAMAGE   x{active.DamageMultiplier:0.00}",
                12, FontStyle.Bold, _text, TextAnchor.UpperLeft,
                0.58f, 0.07f, 0.95f, 0.24f);
        }

        private void BuildCampaign()
        {
            var data = SaveService.Data;
            _campaignSector = Mathf.Clamp(_campaignSector, 0, 4);

            var sectorNav = CreatePanel("SectorNavigation", _contentRoot, _panelDark);
            Place(sectorNav, 0f, 0.79f, 1f, 1f);
            AddPanelHeader(sectorNav, "SECTOR SELECT", _hazard);

            for (var sector = 0; sector < 5; sector++)
            {
                var capturedSector = sector;
                var firstLevel = sector * 10 + 1;
                var selectedSector = sector == _campaignSector;
                var accessible = firstLevel <= data.highestUnlockedLevel;
                var button = CreateButton($"Sector_{sector + 1}", sectorNav,
                    $"SECTOR {sector + 1:00}\n{ShortZoneName(firstLevel)}",
                    selectedSector ? _rust : accessible ? new Color(0.10f, 0.11f, 0.10f, 1f) : new Color(0.045f, 0.048f, 0.047f, 1f),
                    () => { _campaignSector = capturedSector; BuildCampaignRefresh(); });
                Place(button.GetComponent<RectTransform>(), 0.025f + sector * 0.195f, 0.12f, 0.20f + sector * 0.195f, 0.66f);
            }

            var first = _campaignSector * 10 + 1;
            var sectorInfo = CreatePanel("SectorInfo", _contentRoot, _panel);
            Place(sectorInfo, 0f, 0.65f, 1f, 0.77f);
            CreateLabel("SectorName", sectorInfo,
                $"SECTOR {_campaignSector + 1:00} // {RunDifficultyDirector.GetZoneName(first)}",
                20, FontStyle.Bold, _text, TextAnchor.MiddleLeft, 0.025f, 0.22f, 0.60f, 0.82f);
            CreateLabel("SectorProgress", sectorInfo,
                $"UNLOCKED THROUGH {data.highestUnlockedLevel:00}   //   CLEARED THROUGH {data.highestCompletedLevel:00}   //   BOSS LEVEL {first + 9:00}",
                12, FontStyle.Bold, _green, TextAnchor.MiddleRight, 0.48f, 0.22f, 0.975f, 0.82f);

            var grid = CreatePanel("LevelGrid", _contentRoot, Color.clear);
            Place(grid, 0f, 0.02f, 1f, 0.625f);

            for (var slot = 0; slot < 10; slot++)
            {
                var level = first + slot;
                var unlocked = level <= data.highestUnlockedLevel;
                var cleared = level <= data.highestCompletedLevel;
                var selected = level == data.selectedLevel;
                var boss = level % 10 == 0;
                var col = slot % 5;
                var row = slot / 5;

                var xMin = 0.015f + col * 0.197f;
                var xMax = xMin + 0.18f;
                var yMax = row == 0 ? 0.94f : 0.46f;
                var yMin = yMax - 0.38f;
                var status = !unlocked ? "LOCKED" : selected ? "SELECTED" : cleared ? "CLEARED" : boss ? "MUTATION BOSS" : "READY";
                var color = selected ? _green : boss ? new Color(0.42f, 0.06f, 0.035f, 1f) : unlocked ? _panelRaised : new Color(0.035f, 0.038f, 0.037f, 1f);

                var capturedLevel = level;
                var button = CreateButton($"Level_{level}", grid,
                    $"LEVEL {level:00}\n{status}", color,
                    () =>
                    {
                        if (SaveService.SelectLevel(capturedLevel))
                            ShowTab(Tab.Campaign);
                    });
                button.interactable = unlocked;
                Place(button.GetComponent<RectTransform>(), xMin, yMin, xMax, yMax);
            }
        }

        private void BuildCampaignRefresh()
        {
            ClearChildren(_contentRoot);
            BuildCampaign();
        }

        private void BuildStore()
        {
            var intro = CreatePanel("StoreIntro", _contentRoot, _panelDark);
            Place(intro, 0f, 0.80f, 1f, 1f);
            AddPanelHeader(intro, "KAMILUNAVO SUPPLY NETWORK", _hazard);
            CreateLabel("StoreInfo", intro,
                "FAIR CONTENT ONLY // COSMETICS, THEMES AND SEASON CONTENT. NO STAT BOOSTS. VERIFIED STOREKIT / GOOGLE PLAY PURCHASES WILL BE WIRED IN A LATER COMMERCE GATE.",
                12, FontStyle.Bold, _muted, TextAnchor.MiddleLeft, 0.025f, 0.15f, 0.975f, 0.63f);

            var products = new[]
            {
                ("skin_rustwalker", "RUSTWALKER", "OPERATOR SKIN PACK", "3 cosmetic survivor variants", "COSMETIC"),
                ("theme_red_bunker", "REDLINE", "BUNKER THEME", "Red emergency command-center theme", "BUNKER"),
                ("weapon_neon_pack", "WASTELAND", "WEAPON FINISHES", "Finish collection for secured guns", "COSMETIC"),
                ("season_01", "AFTERSHOCK", "SEASON 01", "Challenges, cosmetics and profile rewards", "SEASON")
            };

            for (var i = 0; i < products.Length; i++)
            {
                var product = products[i];
                var row = i / 2;
                var col = i % 2;
                var xMin = col == 0 ? 0f : 0.51f;
                var xMax = col == 0 ? 0.49f : 1f;
                var yMax = row == 0 ? 0.76f : 0.37f;
                var yMin = yMax - 0.34f;
                var card = CreatePanel($"Product_{i}", _contentRoot, _panelRaised);
                Place(card, xMin, yMin, xMax, yMax);
                var stripe = CreatePanel("ProductStripe", card, i % 2 == 0 ? _rust : _hazard);
                Place(stripe, 0f, 0f, 0.012f, 1f);
                CreateLabel("ProductName", card, product.Item2, 23, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                    0.05f, 0.63f, 0.62f, 0.9f);
                CreateLabel("ProductType", card, product.Item3, 12, FontStyle.Bold, _hazard, TextAnchor.MiddleLeft,
                    0.05f, 0.49f, 0.62f, 0.64f);
                CreateLabel("ProductDescription", card, product.Item4, 12, FontStyle.Normal, _muted, TextAnchor.UpperLeft,
                    0.05f, 0.20f, 0.66f, 0.45f);
                CreateLabel("ProductCategory", card, product.Item5, 10, FontStyle.Bold, _muted, TextAnchor.MiddleRight,
                    0.66f, 0.69f, 0.94f, 0.84f);

                var owned = SaveService.OwnsContent(product.Item1);
                var status = CreateButton("Status", card, owned ? "OWNED" : "STORE LINK PENDING", owned ? _green : _rust, null);
                status.interactable = false;
                Place(status.GetComponent<RectTransform>(), 0.67f, 0.16f, 0.94f, 0.38f);
            }
        }

        private void CreateMetric(Transform parent, string label, string value, float y)
        {
            CreateLabel($"{label}_Label", parent, label, 11, FontStyle.Bold, _muted, TextAnchor.MiddleLeft,
                0.08f, y + 0.10f, 0.92f, y + 0.18f);
            CreateLabel($"{label}_Value", parent, value, 26, FontStyle.Bold, _text, TextAnchor.MiddleLeft,
                0.08f, y - 0.01f, 0.92f, y + 0.10f);
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
                WeaponRarity.Uncommon => new Color(0.20f, 0.74f, 0.36f),
                WeaponRarity.Rare => new Color(0.18f, 0.48f, 0.96f),
                WeaponRarity.Epic => new Color(0.68f, 0.25f, 0.92f),
                WeaponRarity.Legendary => new Color(1f, 0.52f, 0.10f),
                _ => _muted
            };
        }

        private static string ShortZoneName(int level)
        {
            if (level <= 10) return "DEAD CITY";
            if (level <= 20) return "FLOODED";
            if (level <= 30) return "ASH";
            if (level <= 40) return "BLACKOUT";
            return "GROUND ZERO";
        }

        private void RefreshDeployLabel()
        {
            if (_deployLabel == null)
                return;
            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);
            _deployLabel.text = $"READY // LEVEL {data.selectedLevel:00} // {RunDifficultyDirector.GetZoneName(data.selectedLevel)}   |   OPERATOR {op.Name}   |   {(data.selectedLevel % 10 == 0 ? "BOSS TARGET" : "STANDARD EXPEDITION")}";
        }

        private void AddEmptyState(string title, string body)
        {
            var panel = CreatePanel("EmptyState", _contentRoot, _panel);
            Place(panel, 0.09f, 0.25f, 0.91f, 0.75f);
            CreateLabel("Title", panel, title, 27, FontStyle.Bold, _hazard, TextAnchor.MiddleCenter,
                0.05f, 0.55f, 0.95f, 0.75f);
            CreateLabel("Body", panel, body, 14, FontStyle.Normal, _muted, TextAnchor.UpperCenter,
                0.08f, 0.30f, 0.92f, 0.52f);
        }

        private (ScrollRect scroll, RectTransform content) CreateScrollArea(string name, Transform parent)
        {
            var root = CreatePanel(name, parent, Color.clear);
            Fill(root);

            var viewport = CreatePanel("Viewport", root, Color.clear);
            Fill(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = CreatePanel("ScrollContent", viewport, Color.clear);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;

            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 10, 4, 16);
            layout.spacing = 10f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = root.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = 28f;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            return (scroll, content);
        }

        private RectTransform CreateLayoutPanel(string name, Transform parent, Color color, float height)
        {
            var rect = CreatePanel(name, parent, color);
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            return rect;
        }

        private void AddPanelHeader(Transform parent, string label, Color color)
        {
            CreateLabel("PanelHeader", parent, label, 12, FontStyle.Bold, color, TextAnchor.MiddleLeft,
                0.025f, 0.87f, 0.97f, 0.97f);
            var line = CreatePanel("PanelHeaderLine", parent, new Color(color.r, color.g, color.b, 0.7f));
            Place(line, 0.025f, 0.855f, 0.975f, 0.862f);
        }

        private void CreateCardStripe(Transform parent, Color color)
        {
            var stripe = CreatePanel("RarityStripe", parent, color);
            Place(stripe, 0f, 0f, 0.009f, 1f);
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

        private Text CreateLabel(string name, Transform parent, string value, int fontSize, FontStyle style, Color color,
            TextAnchor alignment, float xMin, float yMin, float xMax, float yMax)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            Place(text.rectTransform, xMin, yMin, xMax, yMax);
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
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.14f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.25f);
            colors.disabledColor = new Color(color.r * 0.42f, color.g * 0.42f, color.b * 0.42f, 0.82f);
            button.colors = colors;
            if (action != null)
                button.onClick.AddListener(() => action());

            CreateLabel("Label", gameObject.transform, label, 12, FontStyle.Bold, _text, TextAnchor.MiddleCenter,
                0.04f, 0.05f, 0.96f, 0.95f);
            return button;
        }

        private static void ClearChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }

        private static void Fill(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Place(RectTransform rect, float xMin, float yMin, float xMax, float yMax,
            float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }
    }
}
