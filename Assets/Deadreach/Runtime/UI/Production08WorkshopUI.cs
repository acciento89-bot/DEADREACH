using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    [DisallowMultipleComponent]
    public sealed class Production08WorkshopUI : MonoBehaviour
    {
        private readonly Color _panel = new(0.046f, 0.05f, 0.048f, 0.985f);
        private readonly Color _panelRaised = new(0.073f, 0.073f, 0.066f, 0.99f);
        private readonly Color _panelDark = new(0.025f, 0.029f, 0.028f, 0.99f);
        private readonly Color _rust = new(0.72f, 0.22f, 0.07f, 1f);
        private readonly Color _hazard = new(0.96f, 0.66f, 0.12f, 1f);
        private readonly Color _green = new(0.12f, 0.82f, 0.43f, 1f);
        private readonly Color _cyan = new(0.20f, 0.72f, 0.70f, 1f);
        private readonly Color _muted = new(0.52f, 0.55f, 0.53f, 1f);
        private readonly Color _text = new(0.92f, 0.92f, 0.88f, 1f);
        private readonly Color _navIdle = new(0.055f, 0.06f, 0.058f, 1f);
        private readonly Color _navActive = new(0.22f, 0.12f, 0.055f, 1f);

        private Font _font;
        private RectTransform _navigation;
        private RectTransform _contentRoot;
        private RectTransform _workshopRoot;
        private Text _sectionTitle;
        private Text _sectionSubtitle;
        private Text _profileSummary;
        private Button _workshopButton;
        private string _pendingSalvageId;
        private bool _workshopOpen;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallAfterSceneLoad()
        {
            var bunker = UnityEngine.Object.FindFirstObjectByType<BunkerCommandCenterUI>();
            if (bunker == null || bunker.GetComponent<Production08WorkshopUI>() != null)
                return;

            bunker.gameObject.AddComponent<Production08WorkshopUI>();
        }

        private void Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _navigation = FindDeepChild(transform, "Navigation") as RectTransform;
            _contentRoot = FindDeepChild(transform, "ContentViewport") as RectTransform;
            _sectionTitle = FindDeepChild(transform, "SectionTitle")?.GetComponent<Text>();
            _sectionSubtitle = FindDeepChild(transform, "SectionSubtitle")?.GetComponent<Text>();
            _profileSummary = FindDeepChild(transform, "ProfileSummary")?.GetComponent<Text>();

            if (_navigation == null || _contentRoot == null || _sectionTitle == null || _sectionSubtitle == null)
            {
                Debug.LogWarning("DEADREACH 0.8 Workshop could not bind to the Bunker Command Center UI.");
                Destroy(this);
                return;
            }

            InstallWorkshopNavigation();
            RefreshProfileSummary();
        }

        private void InstallWorkshopNavigation()
        {
            var existing = FindDeepChild(_navigation, "Nav_WORKSHOP");
            if (existing != null)
            {
                _workshopButton = existing.GetComponent<Button>();
            }
            else
            {
                _workshopButton = CreateButton("Nav_WORKSHOP", _navigation, "WORKSHOP", _navIdle, OpenWorkshop);
                Place(_workshopButton.GetComponent<RectTransform>(), 0.055f, 0.308f, 0.945f, 0.390f);
            }

            var store = FindDeepChild(_navigation, "Nav_STORE") as RectTransform;
            if (store != null)
                Place(store, 0.055f, 0.193f, 0.945f, 0.275f);

            var bunkerStatus = FindDeepChild(_navigation, "BunkerStatus") as RectTransform;
            if (bunkerStatus != null)
                Place(bunkerStatus, 0.07f, 0.02f, 0.92f, 0.16f);

            foreach (var button in _navigation.GetComponentsInChildren<Button>(true))
            {
                if (button == null || button == _workshopButton)
                    continue;
                button.onClick.AddListener(HideWorkshop);
            }
        }

        private void OpenWorkshop()
        {
            // Normalize the previous tab first. This also releases any Arsenal/operator preview state
            // owned by the validated Bunker UI before the Workshop takes over the content viewport.
            var overview = FindDeepChild(_navigation, "Nav_OVERVIEW")?.GetComponent<Button>();
            if (overview != null)
                overview.onClick.Invoke();

            _workshopOpen = true;
            _pendingSalvageId = null;

            foreach (var button in _navigation.GetComponentsInChildren<Button>(true))
            {
                if (button?.image != null)
                    button.image.color = button == _workshopButton ? _navActive : _navIdle;
            }

            _sectionTitle.text = "WORKSHOP";
            _sectionSubtitle.text = "WEAPON CALIBRATION // SALVAGE // PERMANENT BUNKER SYSTEMS";

            for (var i = 0; i < _contentRoot.childCount; i++)
                _contentRoot.GetChild(i).gameObject.SetActive(false);

            BuildWorkshop();
        }

        private void HideWorkshop()
        {
            _workshopOpen = false;
            _pendingSalvageId = null;

            if (_workshopButton?.image != null)
                _workshopButton.image.color = _navIdle;

            if (_workshopRoot != null)
                _workshopRoot.gameObject.SetActive(false);
        }

        private void RebuildWorkshop()
        {
            if (!_workshopOpen)
                return;

            if (_workshopRoot != null)
                Destroy(_workshopRoot.gameObject);

            BuildWorkshop();
        }

        private void BuildWorkshop()
        {
            RefreshProfileSummary();

            _workshopRoot = CreatePanel("WorkshopRoot", _contentRoot, Color.clear);
            Fill(_workshopRoot);

            var summary = CreatePanel("WorkshopSummary", _workshopRoot, new Color(0.035f, 0.055f, 0.046f, 0.99f));
            Place(summary, 0f, 0.865f, 1f, 1f);
            AddSectionHeader(summary, "WORKSHOP STATUS", _green);

            CreateLabel("Scrap", summary,
                $"SECURED SCRAP  {SaveService.Data.securedScrap:N0}", 20, FontStyle.Bold, _hazard,
                TextAnchor.MiddleLeft, 0.025f, 0.12f, 0.30f, 0.70f);
            CreateLabel("CalibrationCap", summary,
                $"CALIBRATION CAP  {SaveService.GetWeaponUpgradeLimit():00} / {SaveService.MaxWeaponUpgradeLevel:00}",
                13, FontStyle.Bold, _text, TextAnchor.MiddleCenter, 0.30f, 0.12f, 0.58f, 0.70f);
            CreateLabel("PermanentEffects", summary,
                $"FIELD HP +{SaveService.Data.medbayLevel * 6}%   //   LOOT CAP {SaveService.GetRunWeaponCapacity()}   //   EXTRACTION SCRAP +{SaveService.Data.scavengerNetworkLevel * 8}%",
                12, FontStyle.Bold, _green, TextAnchor.MiddleRight, 0.56f, 0.12f, 0.975f, 0.70f);

            var systems = CreatePanel("BunkerSystems", _workshopRoot, _panelDark);
            Place(systems, 0f, 0f, 0.365f, 0.84f);
            AddSectionHeader(systems, "PERMANENT BUNKER SYSTEMS", _hazard);
            BuildFacilityCard(systems, BunkerUpgradeType.Workbench, 0);
            BuildFacilityCard(systems, BunkerUpgradeType.Medbay, 1);
            BuildFacilityCard(systems, BunkerUpgradeType.CargoRig, 2);
            BuildFacilityCard(systems, BunkerUpgradeType.ScavengerNetwork, 3);

            var weapons = CreatePanel("WeaponCalibration", _workshopRoot, _panelDark);
            Place(weapons, 0.385f, 0f, 1f, 0.84f);
            AddSectionHeader(weapons, "WEAPON CALIBRATION / SALVAGE", _rust);
            CreateLabel("WeaponProtocol", weapons,
                "ITEM POWER NOW DRIVES REAL DAMAGE. CALIBRATION ADDS +8 POWER PER RANK PLUS SMALL RANGE / CRIT HANDLING GAINS.",
                11, FontStyle.Bold, _muted, TextAnchor.MiddleLeft, 0.025f, 0.845f, 0.975f, 0.91f);

            var scrollViewport = CreatePanel("WeaponScrollViewport", weapons, Color.clear);
            Place(scrollViewport, 0.018f, 0.025f, 0.982f, 0.83f);
            BuildWeaponScroll(scrollViewport);
        }

        private void BuildFacilityCard(RectTransform parent, BunkerUpgradeType type, int index)
        {
            var yMax = 0.84f - index * 0.205f;
            var yMin = yMax - 0.175f;
            var accent = FacilityAccent(type);
            var card = CreatePanel($"Facility_{type}", parent, _panel);
            Place(card, 0.025f, yMin, 0.975f, yMax);

            var stripe = CreatePanel("Accent", card, accent);
            Place(stripe, 0f, 0f, 0.014f, 1f);

            var level = SaveService.GetBunkerUpgradeLevel(type);
            var maxed = level >= SaveService.MaxBunkerUpgradeRank;
            var cost = SaveService.GetBunkerUpgradeCost(type);

            CreateLabel("Title", card, FacilityTitle(type), 16, FontStyle.Bold, _text,
                TextAnchor.MiddleLeft, 0.045f, 0.61f, 0.58f, 0.91f);
            CreateLabel("Rank", card, $"RANK {level} / {SaveService.MaxBunkerUpgradeRank}", 10, FontStyle.Bold, accent,
                TextAnchor.MiddleLeft, 0.045f, 0.43f, 0.58f, 0.62f);
            CreateLabel("Effect", card, FacilityEffect(type), 11, FontStyle.Normal, _muted,
                TextAnchor.UpperLeft, 0.045f, 0.08f, 0.63f, 0.42f);

            var canBuy = !maxed && cost > 0 && SaveService.Data.securedScrap >= cost;
            var buttonText = maxed ? "MAX RANK" : $"UPGRADE // {cost:N0}";
            var purchase = CreateButton("Upgrade", card, buttonText, canBuy ? _green : _rust, () =>
            {
                if (SaveService.PurchaseBunkerUpgrade(type))
                {
                    _pendingSalvageId = null;
                    RefreshProfileSummary();
                    RebuildWorkshop();
                }
            });
            purchase.interactable = canBuy;
            Place(purchase.GetComponent<RectTransform>(), 0.66f, 0.24f, 0.955f, 0.74f);
        }

        private void BuildWeaponScroll(RectTransform viewport)
        {
            var viewportImage = viewport.gameObject.GetComponent<Image>() ?? viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            viewport.gameObject.AddComponent<RectMask2D>();

            var contentObject = new GameObject("WeaponScrollContent", typeof(RectTransform));
            contentObject.transform.SetParent(viewport, false);
            var content = contentObject.GetComponent<RectTransform>();
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            var layout = contentObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(2, 8, 2, 8);
            layout.spacing = 10f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var fitter = contentObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 28f;

            var weapons = new List<WeaponInstanceData>();
            if (SaveService.Data.stashWeapons != null)
            {
                foreach (var weapon in SaveService.Data.stashWeapons)
                {
                    if (weapon != null)
                        weapons.Add(weapon);
                }
            }

            weapons.Sort((left, right) =>
            {
                var leftEquipped = left.instanceId == SaveService.Data.equippedPrimaryWeaponId;
                var rightEquipped = right.instanceId == SaveService.Data.equippedPrimaryWeaponId;
                if (leftEquipped != rightEquipped)
                    return leftEquipped ? -1 : 1;
                return right.itemPower.CompareTo(left.itemPower);
            });

            if (weapons.Count == 0)
            {
                var empty = CreatePanel("EmptyWeapons", content, _panel);
                empty.gameObject.AddComponent<LayoutElement>().preferredHeight = 150f;
                CreateLabel("EmptyTitle", empty, "NO SECURED WEAPONS", 20, FontStyle.Bold, _text,
                    TextAnchor.MiddleCenter, 0.05f, 0.52f, 0.95f, 0.83f);
                CreateLabel("EmptyBody", empty, "EXTRACT WEAPON CASES OR USE THE 0.8 DEV TEST PROFILE TO VALIDATE THE WORKSHOP.", 12,
                    FontStyle.Bold, _muted, TextAnchor.UpperCenter, 0.08f, 0.18f, 0.92f, 0.50f);
                return;
            }

            foreach (var weapon in weapons)
                BuildWeaponCard(content, weapon);
        }

        private void BuildWeaponCard(RectTransform parent, WeaponInstanceData weapon)
        {
            var equipped = weapon.instanceId == SaveService.Data.equippedPrimaryWeaponId;
            var card = CreatePanel($"WorkshopWeapon_{weapon.instanceId}", parent,
                equipped ? new Color(0.105f, 0.095f, 0.055f, 1f) : _panel);
            card.gameObject.AddComponent<LayoutElement>().preferredHeight = 132f;

            var stripe = CreatePanel("RarityStripe", card, RarityColor(weapon.rarity));
            Place(stripe, 0f, 0f, 0.012f, 1f);

            var calibrationLimit = SaveService.GetWeaponUpgradeLimit();
            var upgradeCost = SaveService.GetWeaponUpgradeCost(weapon);
            var powerBonus = (WeaponStatResolver.GetItemPowerDamageMultiplier(weapon) - 1f) * 100f;
            var powerPrefix = powerBonus >= 0f ? "+" : string.Empty;

            CreateLabel("Title", card,
                $"{(equipped ? "ACTIVE // " : string.Empty)}{weapon.rarity.ToString().ToUpperInvariant()} // {weapon.displayNameSnapshot}",
                15, FontStyle.Bold, _text, TextAnchor.MiddleLeft, 0.035f, 0.69f, 0.68f, 0.94f);
            CreateLabel("Stats", card,
                $"{weapon.family.ToString().ToUpperInvariant()}   //   PWR {weapon.itemPower}   //   POWER DMG {powerPrefix}{powerBonus:0.#}%   //   CAL {weapon.upgradeLevel}/{calibrationLimit}",
                11, FontStyle.Bold, _hazard, TextAnchor.MiddleLeft, 0.035f, 0.48f, 0.68f, 0.70f);
            CreateLabel("Affixes", card, BuildAffixText(weapon), 10, FontStyle.Normal, _muted,
                TextAnchor.UpperLeft, 0.035f, 0.09f, 0.68f, 0.46f);

            var atAbsoluteMax = weapon.upgradeLevel >= SaveService.MaxWeaponUpgradeLevel;
            var atWorkbenchLimit = !atAbsoluteMax && weapon.upgradeLevel >= calibrationLimit;
            var canAffordUpgrade = upgradeCost > 0 && SaveService.Data.securedScrap >= upgradeCost;
            var upgradeLabel = atAbsoluteMax
                ? "MAX CALIBRATION"
                : atWorkbenchLimit
                    ? "WORKBENCH REQUIRED"
                    : $"CALIBRATE // {upgradeCost:N0}";

            var capturedId = weapon.instanceId;
            var calibrate = CreateButton("Calibrate", card, upgradeLabel,
                !atAbsoluteMax && !atWorkbenchLimit && canAffordUpgrade ? _green : _rust, () =>
                {
                    if (SaveService.UpgradeWeapon(capturedId))
                    {
                        _pendingSalvageId = null;
                        RefreshProfileSummary();
                        RebuildWorkshop();
                    }
                });
            calibrate.interactable = !atAbsoluteMax && !atWorkbenchLimit && canAffordUpgrade;
            Place(calibrate.GetComponent<RectTransform>(), 0.71f, 0.54f, 0.97f, 0.87f);

            var salvageValue = SaveService.GetSalvageValue(weapon);
            var pending = string.Equals(_pendingSalvageId, capturedId, StringComparison.Ordinal);
            var salvageLabel = equipped
                ? "ACTIVE LOADOUT"
                : pending
                    ? $"CONFIRM SALVAGE +{salvageValue:N0}"
                    : $"SALVAGE +{salvageValue:N0}";
            var salvage = CreateButton("Salvage", card, salvageLabel, pending ? _hazard : _rust, () =>
            {
                if (equipped)
                    return;

                if (string.Equals(_pendingSalvageId, capturedId, StringComparison.Ordinal))
                {
                    if (SaveService.SalvageWeapon(capturedId))
                    {
                        _pendingSalvageId = null;
                        RefreshProfileSummary();
                        RebuildWorkshop();
                    }
                    return;
                }

                _pendingSalvageId = capturedId;
                RebuildWorkshop();
            });
            salvage.interactable = !equipped;
            Place(salvage.GetComponent<RectTransform>(), 0.71f, 0.13f, 0.97f, 0.46f);
        }

        private void RefreshProfileSummary()
        {
            if (_profileSummary == null)
                return;

            var data = SaveService.Data;
            _profileSummary.text = $"SCRAP  {data.securedScrap:N0}   //   EXTRACTS  {data.successfulExtractions:N0}   //   BOSS KILLS  {data.bossKills:N0}";
        }

        private string FacilityTitle(BunkerUpgradeType type)
        {
            return type switch
            {
                BunkerUpgradeType.Workbench => "WORKBENCH",
                BunkerUpgradeType.Medbay => "MEDBAY",
                BunkerUpgradeType.CargoRig => "CARGO RIG",
                BunkerUpgradeType.ScavengerNetwork => "SCAVENGER NETWORK",
                _ => "BUNKER SYSTEM"
            };
        }

        private string FacilityEffect(BunkerUpgradeType type)
        {
            var level = SaveService.GetBunkerUpgradeLevel(type);
            return type switch
            {
                BunkerUpgradeType.Workbench => $"Calibration ceiling {SaveService.GetWeaponUpgradeLimit()}/{SaveService.MaxWeaponUpgradeLevel}. Each rank unlocks two additional calibration levels.",
                BunkerUpgradeType.Medbay => $"Permanent operator max HP +{level * 6}% across every deployment.",
                BunkerUpgradeType.CargoRig => $"Expedition weapon capacity {SaveService.GetRunWeaponCapacity()}. Each rank adds one secured-case slot.",
                BunkerUpgradeType.ScavengerNetwork => $"Extraction Scrap yield +{level * 8}%. Applied only when a run is successfully banked.",
                _ => string.Empty
            };
        }

        private Color FacilityAccent(BunkerUpgradeType type)
        {
            return type switch
            {
                BunkerUpgradeType.Workbench => _hazard,
                BunkerUpgradeType.Medbay => _green,
                BunkerUpgradeType.CargoRig => _cyan,
                BunkerUpgradeType.ScavengerNetwork => _rust,
                _ => _text
            };
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

            return parts.Count == 0 ? "AFFIXES // NONE" : "AFFIXES // " + string.Join("   //   ", parts);
        }

        private string AffixName(WeaponAffixStat stat)
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
                WeaponRarity.Uncommon => new Color(0.20f, 0.80f, 0.40f, 1f),
                WeaponRarity.Rare => new Color(0.20f, 0.52f, 0.96f, 1f),
                WeaponRarity.Epic => new Color(0.73f, 0.25f, 0.96f, 1f),
                WeaponRarity.Legendary => new Color(1f, 0.55f, 0.10f, 1f),
                _ => new Color(0.65f, 0.68f, 0.66f, 1f)
            };
        }

        private void AddSectionHeader(Transform parent, string text, Color color)
        {
            CreateLabel("Header", parent, text, 11, FontStyle.Bold, color, TextAnchor.MiddleLeft,
                0.025f, 0.90f, 0.975f, 0.99f);
            var line = CreatePanel("HeaderLine", parent, color);
            Place(line, 0.025f, 0.885f, 0.975f, 0.892f);
        }

        private RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = color.a > 0.01f;
            return go.GetComponent<RectTransform>();
        }

        private Text CreateLabel(
            string name,
            Transform parent,
            string text,
            int size,
            FontStyle style,
            Color color,
            TextAnchor anchor,
            float xMin,
            float yMin,
            float xMax,
            float yMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var label = go.GetComponent<Text>();
            label.font = _font;
            label.text = text;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = color;
            label.alignment = anchor;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
            Place(label.rectTransform, xMin, yMin, xMax, yMax);
            return label;
        }

        private Button CreateButton(string name, Transform parent, string label, Color color, Action action)
        {
            var panel = CreatePanel(name, parent, color);
            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel.GetComponent<Image>();
            if (action != null)
                button.onClick.AddListener(() => action());

            var text = CreateLabel("Label", panel, label, 11, FontStyle.Bold, _text,
                TextAnchor.MiddleCenter, 0.03f, 0.05f, 0.97f, 0.95f);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
            text.resizeTextMaxSize = 12;
            return button;
        }

        private static Transform FindDeepChild(Transform root, string name)
        {
            if (root == null)
                return null;

            if (root.name == name)
                return root;

            for (var i = 0; i < root.childCount; i++)
            {
                var found = FindDeepChild(root.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
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
            if (rect == null)
                return;

            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
