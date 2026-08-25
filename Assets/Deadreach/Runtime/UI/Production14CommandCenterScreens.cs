using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed partial class Production14CommandCenterUI
    {
        private void BuildArsenalScreen()
        {
            var data = SaveService.Data;
            var inventory = CreateIndustrialPanel("ArsenalInventory", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(inventory, 0.035f, 0.15f, 0.615f, 0.71f);
            AddSectionHeader(inventory, "SECURED INVENTORY", _amber);

            var count = CreateLabel("InventoryCount", inventory,
                $"WEAPONS SECURED  {(data.stashWeapons?.Count ?? 0):00}", 10, FontStyle.Bold, _muted, TextAnchor.MiddleRight);
            Place(count.rectTransform, 0.56f, 0.84f, 0.92f, 0.91f);

            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
            {
                var empty = CreateLabel("Empty", inventory,
                    "NO SECURED WEAPONS\n\nEXTRACT WEAPON CASES FROM DEAD CITY TO POPULATE THE BUNKER ARSENAL.",
                    16, FontStyle.Bold, _muted, TextAnchor.MiddleCenter);
                Place(empty.rectTransform, 0.10f, 0.22f, 0.90f, 0.72f);
            }
            else
            {
                var scrollHost = CreateRect("InventoryScrollHost", inventory);
                Place(scrollHost, 0.045f, 0.06f, 0.955f, 0.79f);
                var scroll = CreateScreenScrollArea(scrollHost);

                for (var i = data.stashWeapons.Count - 1; i >= 0; i--)
                {
                    var weapon = data.stashWeapons[i];
                    if (weapon == null)
                        continue;

                    var equipped = string.Equals(weapon.instanceId, data.equippedPrimaryWeaponId, StringComparison.Ordinal);
                    var card = CreateLayoutIndustrialPanel($"Weapon_{i:00}", scroll.content,
                        equipped ? Production14IndustrialSkin.PlateKind.TabActive : Production14IndustrialSkin.PlateKind.Tag, 106f);

                    var stripe = CreateImage("RarityStripe", card, P14RarityColor(weapon.rarity));
                    Place(stripe.rectTransform, 0.018f, 0.12f, 0.028f, 0.88f);

                    var title = CreateLabel("Title", card,
                        $"{weapon.rarity.ToString().ToUpperInvariant()} // {weapon.displayNameSnapshot.ToUpperInvariant()}",
                        14, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
                    Place(title.rectTransform, 0.055f, 0.58f, 0.70f, 0.88f);

                    var power = CreateLabel("Power", card,
                        $"ITEM POWER {weapon.itemPower:000}   //   CAL {weapon.upgradeLevel:00}",
                        10, FontStyle.Bold, equipped ? _green : _amber, TextAnchor.MiddleLeft);
                    Place(power.rectTransform, 0.055f, 0.37f, 0.70f, 0.59f);

                    var affixes = CreateLabel("Affixes", card, BuildP14AffixText(weapon), 9, FontStyle.Normal, _muted, TextAnchor.UpperLeft);
                    Place(affixes.rectTransform, 0.055f, 0.08f, 0.72f, 0.36f);

                    var capturedId = weapon.instanceId;
                    var equip = CreateScreenButton("Equip", card, equipped ? "EQUIPPED" : "EQUIP", equipped ? _green : _cyan,
                        () =>
                        {
                            if (SaveService.EquipPrimaryWeapon(capturedId))
                                HandleNav(1);
                        });
                    equip.interactable = !equipped;
                    Place(equip.GetComponent<RectTransform>(), 0.76f, 0.24f, 0.95f, 0.76f);
                }
            }

            var inspector = CreateIndustrialPanel("ArsenalInspector", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(inspector, 0.635f, 0.15f, 0.965f, 0.71f);
            AddSectionHeader(inspector, "ACTIVE LOADOUT", _cyan);

            var active = SaveService.GetEquippedPrimaryWeapon();
            var activeName = CreateLabel("WeaponName", inspector,
                active != null ? active.displayNameSnapshot.ToUpperInvariant() : "DR-7 FIELD ISSUE",
                24, FontStyle.Bold, _white, TextAnchor.MiddleCenter);
            Place(activeName.rectTransform, 0.08f, 0.59f, 0.92f, 0.76f);

            var activeState = CreateLabel("WeaponState", inspector,
                active != null
                    ? $"{active.rarity.ToString().ToUpperInvariant()} // ITEM POWER {active.itemPower} // CAL {active.upgradeLevel}"
                    : "BASE LOADOUT // NO SECURED PRIMARY EQUIPPED",
                11, FontStyle.Bold, active != null ? P14RarityColor(active.rarity) : _muted, TextAnchor.MiddleCenter);
            Place(activeState.rectTransform, 0.08f, 0.50f, 0.92f, 0.61f);

            var divider = CreateImage("InspectorDivider", inspector, new Color(_cyan.r, _cyan.g, _cyan.b, 0.55f));
            Place(divider.rectTransform, 0.10f, 0.46f, 0.90f, 0.465f);

            var details = CreateLabel("WeaponDetails", inspector,
                active != null
                    ? BuildP14AffixText(active) + $"\n\nWORKSHOP LIMIT  {SaveService.GetWeaponUpgradeLimit():00} / {SaveService.MaxWeaponUpgradeLevel:00}"
                    : "SECURE A WEAPON CASE AND EXTRACT TO ADD A PRIMARY TO YOUR LOADOUT.",
                11, FontStyle.Bold, _muted, TextAnchor.UpperCenter);
            Place(details.rectTransform, 0.10f, 0.16f, 0.90f, 0.43f);
        }

        private void BuildOperatorsScreen()
        {
            var data = SaveService.Data;
            var active = OperatorCatalog.Get(data.selectedCharacterId);

            var roster = CreateIndustrialPanel("OperatorRoster", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(roster, 0.035f, 0.15f, 0.62f, 0.71f);
            AddSectionHeader(roster, "SURVIVOR ROSTER", _amber);

            var definitions = OperatorCatalog.All;
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                var selected = string.Equals(definition.Id, data.selectedCharacterId, StringComparison.OrdinalIgnoreCase);
                var yMax = 0.77f - i * 0.245f;
                var yMin = yMax - 0.205f;

                var card = CreateIndustrialPanel($"Operator_{definition.Id}", roster,
                    selected ? Production14IndustrialSkin.PlateKind.TabActive : Production14IndustrialSkin.PlateKind.Tag, false);
                Place(card, 0.055f, yMin, 0.945f, yMax);

                var accent = CreateImage("Accent", card, definition.Accent);
                Place(accent.rectTransform, 0.02f, 0.12f, 0.03f, 0.88f);

                var name = CreateLabel("Name", card, definition.Name.ToUpperInvariant(), 18, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
                Place(name.rectTransform, 0.065f, 0.57f, 0.47f, 0.88f);

                var role = CreateLabel("Role", card, definition.Role.ToUpperInvariant(), 10, FontStyle.Bold, definition.Accent, TextAnchor.MiddleLeft);
                Place(role.rectTransform, 0.065f, 0.39f, 0.47f, 0.58f);

                var ability = CreateLabel("Ability", card, $"ACTIVE // {definition.AbilityName}", 9, FontStyle.Bold, _muted, TextAnchor.MiddleLeft);
                Place(ability.rectTransform, 0.065f, 0.12f, 0.63f, 0.37f);

                var captured = definition.Id;
                var select = CreateScreenButton("Select", card, selected ? "ACTIVE" : "SELECT", selected ? _green : _cyan,
                    () =>
                    {
                        if (SaveService.SelectCharacter(captured))
                            HandleNav(2);
                    });
                select.interactable = !selected;
                Place(select.GetComponent<RectTransform>(), 0.72f, 0.22f, 0.95f, 0.78f);
            }

            var inspector = CreateIndustrialPanel("OperatorInspector", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(inspector, 0.64f, 0.15f, 0.965f, 0.71f);
            AddSectionHeader(inspector, "ACTIVE OPERATOR", active.Accent);

            var nameLabel = CreateLabel("ActiveName", inspector, active.Name.ToUpperInvariant(), 31, FontStyle.Bold, _white, TextAnchor.MiddleCenter);
            Place(nameLabel.rectTransform, 0.08f, 0.67f, 0.92f, 0.82f);

            var roleLabel = CreateLabel("ActiveRole", inspector, active.Role.ToUpperInvariant(), 13, FontStyle.Bold, active.Accent, TextAnchor.MiddleCenter);
            Place(roleLabel.rectTransform, 0.08f, 0.59f, 0.92f, 0.68f);

            var stats = CreateLabel("Stats", inspector,
                $"VITALS     x{active.HealthMultiplier:0.00}\nMOBILITY   x{active.MoveMultiplier:0.00}\nDAMAGE     x{active.DamageMultiplier:0.00}\n\n{active.AbilityName.ToUpperInvariant()}\n{active.AbilityDescription}",
                12, FontStyle.Bold, _muted, TextAnchor.UpperCenter);
            Place(stats.rectTransform, 0.09f, 0.18f, 0.91f, 0.55f);
        }

        private void BuildCampaignScreen()
        {
            var data = SaveService.Data;
            _campaignSector = Mathf.Clamp(_campaignSector, 0, 4);

            var sectorPanel = CreateIndustrialPanel("SectorPanel", _contentRoot, Production14IndustrialSkin.PlateKind.Header, false);
            Place(sectorPanel, 0.035f, 0.60f, 0.965f, 0.71f);
            AddSectionHeader(sectorPanel, "SECTOR SELECT", _cyan);

            for (var sector = 0; sector < 5; sector++)
            {
                var capturedSector = sector;
                var firstLevel = sector * 10 + 1;
                var accessible = firstLevel <= data.highestUnlockedLevel;
                var selected = sector == _campaignSector;
                var button = CreateScreenButton($"Sector_{sector + 1:00}", sectorPanel,
                    $"SECTOR {sector + 1:00}\n{P14ShortZoneName(firstLevel)}", selected ? _cyan : _muted,
                    () =>
                    {
                        _campaignSector = capturedSector;
                        HandleNav(3);
                    });
                button.interactable = accessible;
                Place(button.GetComponent<RectTransform>(), 0.025f + sector * 0.195f, 0.12f, 0.20f + sector * 0.195f, 0.67f);
            }

            var first = _campaignSector * 10 + 1;
            var gridPanel = CreateIndustrialPanel("CampaignGrid", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(gridPanel, 0.035f, 0.15f, 0.965f, 0.575f);
            AddSectionHeader(gridPanel,
                $"SECTOR {_campaignSector + 1:00} // {RunDifficultyDirector.GetZoneName(first).ToUpperInvariant()}", _amber);

            for (var slot = 0; slot < 10; slot++)
            {
                var level = first + slot;
                var unlocked = level <= data.highestUnlockedLevel;
                var cleared = level <= data.highestCompletedLevel;
                var selected = level == data.selectedLevel;
                var boss = level % 10 == 0;
                var col = slot % 5;
                var row = slot / 5;

                var status = !unlocked ? "LOCKED" : selected ? "SELECTED" : cleared ? "CLEARED" : boss ? "MUTATION" : "READY";
                var accent = selected ? _green : boss ? _danger : unlocked ? _cyan : _muted;
                var capturedLevel = level;
                var button = CreateScreenButton($"Level_{level:00}", gridPanel, $"LEVEL {level:00}\n{status}", accent,
                    () =>
                    {
                        if (SaveService.SelectLevel(capturedLevel))
                            HandleNav(3);
                    });
                button.interactable = unlocked;

                var xMin = 0.045f + col * 0.19f;
                var xMax = xMin + 0.16f;
                var yMin = row == 0 ? 0.49f : 0.12f;
                var yMax = yMin + 0.25f;
                Place(button.GetComponent<RectTransform>(), xMin, yMin, xMax, yMax);
            }
        }

        private void BuildWorkshopScreen()
        {
            var data = SaveService.Data;
            var systems = CreateIndustrialPanel("BunkerSystems", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(systems, 0.035f, 0.15f, 0.60f, 0.71f);
            AddSectionHeader(systems, "PERMANENT BUNKER SYSTEMS", _amber);

            var status = CreateLabel("WorkshopStatus", systems,
                $"SECURED SCRAP {data.securedScrap:N0}   //   CALIBRATION CAP {SaveService.GetWeaponUpgradeLimit():00}/{SaveService.MaxWeaponUpgradeLevel:00}",
                10, FontStyle.Bold, _green, TextAnchor.MiddleLeft);
            Place(status.rectTransform, 0.06f, 0.75f, 0.94f, 0.82f);

            var types = new[]
            {
                BunkerUpgradeType.Workbench,
                BunkerUpgradeType.Medbay,
                BunkerUpgradeType.CargoRig,
                BunkerUpgradeType.ScavengerNetwork
            };

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var row = i / 2;
                var col = i % 2;
                var xMin = col == 0 ? 0.055f : 0.515f;
                var xMax = col == 0 ? 0.485f : 0.945f;
                var yMax = row == 0 ? 0.70f : 0.37f;
                var yMin = yMax - 0.27f;

                var card = CreateIndustrialPanel($"Facility_{type}", systems, Production14IndustrialSkin.PlateKind.Tag, false);
                Place(card, xMin, yMin, xMax, yMax);

                var level = SaveService.GetBunkerUpgradeLevel(type);
                var cost = SaveService.GetBunkerUpgradeCost(type);
                var maxed = level >= SaveService.MaxBunkerUpgradeRank;
                var canBuy = !maxed && cost > 0 && data.securedScrap >= cost;

                var title = CreateLabel("Title", card, FacilityTitle(type), 13, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
                Place(title.rectTransform, 0.07f, 0.66f, 0.65f, 0.90f);

                var rank = CreateLabel("Rank", card, $"RANK {level} / {SaveService.MaxBunkerUpgradeRank}", 9, FontStyle.Bold, _cyan, TextAnchor.MiddleLeft);
                Place(rank.rectTransform, 0.07f, 0.49f, 0.65f, 0.66f);

                var effect = CreateLabel("Effect", card, FacilityEffect(type), 9, FontStyle.Normal, _muted, TextAnchor.UpperLeft);
                Place(effect.rectTransform, 0.07f, 0.12f, 0.64f, 0.46f);

                var capturedType = type;
                var upgrade = CreateScreenButton("Upgrade", card,
                    maxed ? "MAX RANK" : $"UPGRADE\n{cost:N0} SCRAP", canBuy ? _green : _amber,
                    () =>
                    {
                        if (SaveService.PurchaseBunkerUpgrade(capturedType))
                        {
                            RefreshHeaderCounters();
                            HandleNav(4);
                        }
                    });
                upgrade.interactable = canBuy;
                Place(upgrade.GetComponent<RectTransform>(), 0.68f, 0.20f, 0.95f, 0.78f);
            }

            var calibration = CreateIndustrialPanel("Calibration", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(calibration, 0.62f, 0.15f, 0.965f, 0.71f);
            AddSectionHeader(calibration, "WEAPON CALIBRATION", _cyan);

            var equipped = SaveService.GetEquippedPrimaryWeapon();
            if (equipped == null)
            {
                var empty = CreateLabel("NoWeapon", calibration,
                    "NO SECURED PRIMARY\n\nEQUIP A WEAPON IN ARSENAL BEFORE CALIBRATION.",
                    14, FontStyle.Bold, _muted, TextAnchor.MiddleCenter);
                Place(empty.rectTransform, 0.10f, 0.30f, 0.90f, 0.70f);
                return;
            }

            var weaponName = CreateLabel("Weapon", calibration, equipped.displayNameSnapshot.ToUpperInvariant(), 22, FontStyle.Bold, _white, TextAnchor.MiddleCenter);
            Place(weaponName.rectTransform, 0.08f, 0.67f, 0.92f, 0.80f);

            var weaponStats = CreateLabel("Stats", calibration,
                $"{equipped.rarity.ToString().ToUpperInvariant()} // ITEM POWER {equipped.itemPower}\nCALIBRATION {equipped.upgradeLevel:00} / {SaveService.GetWeaponUpgradeLimit():00}\n\n{BuildP14AffixText(equipped)}",
                11, FontStyle.Bold, _muted, TextAnchor.UpperCenter);
            Place(weaponStats.rectTransform, 0.08f, 0.33f, 0.92f, 0.64f);

            var upgradeCost = SaveService.GetWeaponUpgradeCost(equipped);
            var canUpgrade = upgradeCost > 0 && data.securedScrap >= upgradeCost;
            var capturedWeapon = equipped.instanceId;
            var calibrate = CreateScreenButton("Calibrate", calibration,
                upgradeCost <= 0 ? "CALIBRATION LIMIT" : $"CALIBRATE // {upgradeCost:N0} SCRAP", canUpgrade ? _green : _amber,
                () =>
                {
                    if (SaveService.UpgradeWeapon(capturedWeapon))
                    {
                        RefreshHeaderCounters();
                        HandleNav(4);
                    }
                });
            calibrate.interactable = canUpgrade;
            Place(calibrate.GetComponent<RectTransform>(), 0.13f, 0.13f, 0.87f, 0.28f);
        }

        private void BuildSupplyScreen()
        {
            var intro = CreateIndustrialPanel("SupplyIntro", _contentRoot, Production14IndustrialSkin.PlateKind.Header, false);
            Place(intro, 0.035f, 0.60f, 0.965f, 0.71f);
            AddSectionHeader(intro, "KAMILUNAVO SUPPLY NETWORK", _cyan);

            var introText = CreateLabel("Intro", intro,
                "FAIR CONTENT ONLY // COSMETICS, BUNKER THEMES AND SEASON CONTENT // NO STAT BOOSTS",
                10, FontStyle.Bold, _muted, TextAnchor.MiddleLeft);
            Place(introText.rectTransform, 0.06f, 0.16f, 0.94f, 0.67f);

            var products = new[]
            {
                ("skin_rustwalker", "RUSTWALKER", "OPERATOR SKIN PACK", "3 cosmetic survivor variants"),
                ("theme_red_bunker", "REDLINE", "BUNKER THEME", "Emergency command-center theme"),
                ("weapon_neon_pack", "WASTELAND", "WEAPON FINISHES", "Finish collection for secured guns"),
                ("season_01", "AFTERSHOCK", "SEASON 01", "Challenges, cosmetics and profile rewards")
            };

            for (var i = 0; i < products.Length; i++)
            {
                var product = products[i];
                var row = i / 2;
                var col = i % 2;
                var xMin = col == 0 ? 0.035f : 0.51f;
                var xMax = col == 0 ? 0.49f : 0.965f;
                var yMax = row == 0 ? 0.56f : 0.34f;
                var yMin = yMax - 0.18f;

                var card = CreateIndustrialPanel($"Supply_{i:00}", _contentRoot,
                    i % 2 == 0 ? Production14IndustrialSkin.PlateKind.Mission : Production14IndustrialSkin.PlateKind.Glass, false);
                Place(card, xMin, yMin, xMax, yMax);

                var name = CreateLabel("Name", card, product.Item2, 18, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
                Place(name.rectTransform, 0.06f, 0.58f, 0.56f, 0.88f);

                var type = CreateLabel("Type", card, product.Item3, 9, FontStyle.Bold, i % 2 == 0 ? _amber : _cyan, TextAnchor.MiddleLeft);
                Place(type.rectTransform, 0.06f, 0.42f, 0.60f, 0.60f);

                var description = CreateLabel("Description", card, product.Item4, 10, FontStyle.Normal, _muted, TextAnchor.UpperLeft);
                Place(description.rectTransform, 0.06f, 0.12f, 0.62f, 0.40f);

                var owned = SaveService.OwnsContent(product.Item1);
                var state = CreatePill("State", card, owned ? "OWNED" : "STORE LINK PENDING", owned ? _green : _muted);
                Place(state, 0.67f, 0.28f, 0.94f, 0.72f);
            }
        }

        private Button CreateScreenButton(string name, Transform parent, string label, Color accent, Action action)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.sprite = Production14IndustrialSkin.Get(Production14IndustrialSkin.PlateKind.Tab);
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
            colors.pressedColor = new Color(0.72f, 0.83f, 0.85f, 1f);
            colors.disabledColor = new Color(0.40f, 0.42f, 0.42f, 0.72f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            if (action != null)
                button.onClick.AddListener(() => action());

            var text = CreateLabel("Text", go.transform, label, 10, FontStyle.Bold, accent, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 8f, 4f, 8f, 4f);
            return button;
        }

        private (ScrollRect scroll, RectTransform content) CreateScreenScrollArea(RectTransform host)
        {
            var viewport = CreateRect("Viewport", host);
            Fill(viewport);
            var viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            viewportImage.raycastTarget = true;
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = CreateRect("ScrollContent", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;

            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 8, 4, 12);
            layout.spacing = 9f;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = host.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = 26f;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            return (scroll, content);
        }

        private RectTransform CreateLayoutIndustrialPanel(string name, Transform parent, Production14IndustrialSkin.PlateKind kind, float height)
        {
            var panel = CreateIndustrialPanel(name, parent, kind, false);
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            return panel;
        }

        private void RefreshHeaderCounters()
        {
            var header = _root?.Find("Header");
            if (header == null)
                return;

            SetCounterValue(header, "Counter_SCRAP", SaveService.Data.securedScrap.ToString("N0"));
            SetCounterValue(header, "Counter_EXTRACTS", SaveService.Data.successfulExtractions.ToString("N0"));
            SetCounterValue(header, "Counter_BOSS KILLS", SaveService.Data.bossKills.ToString("N0"));
        }

        private static void SetCounterValue(Transform header, string cardName, string value)
        {
            var card = header.Find(cardName);
            var text = card?.Find("Value")?.GetComponent<Text>();
            if (text != null)
                text.text = value;
        }

        private string BuildP14AffixText(WeaponInstanceData weapon)
        {
            if (weapon?.affixes == null || weapon.affixes.Count == 0)
                return "AFFIXES // NONE";

            var parts = new List<string>();
            foreach (var affix in weapon.affixes)
            {
                if (affix == null)
                    continue;
                parts.Add($"{P14AffixName(affix.stat)} +{affix.value:0.#}%");
            }

            return parts.Count == 0 ? "AFFIXES // NONE" : "AFFIXES // " + string.Join("  |  ", parts);
        }

        private static string P14AffixName(WeaponAffixStat stat)
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

        private Color P14RarityColor(WeaponRarity rarity)
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

        private static string P14ShortZoneName(int level)
        {
            if (level <= 10) return "DEAD CITY";
            if (level <= 20) return "FLOODED";
            if (level <= 30) return "ASH";
            if (level <= 40) return "BLACKOUT";
            return "GROUND ZERO";
        }

        private static string FacilityTitle(BunkerUpgradeType type)
        {
            return type switch
            {
                BunkerUpgradeType.Workbench => "WORKBENCH",
                BunkerUpgradeType.Medbay => "MEDBAY",
                BunkerUpgradeType.CargoRig => "CARGO RIG",
                BunkerUpgradeType.ScavengerNetwork => "SCAVENGER NET",
                _ => type.ToString().ToUpperInvariant()
            };
        }

        private static string FacilityEffect(BunkerUpgradeType type)
        {
            return type switch
            {
                BunkerUpgradeType.Workbench => "Raises weapon calibration cap.",
                BunkerUpgradeType.Medbay => "+6% field health per rank.",
                BunkerUpgradeType.CargoRig => "+1 run weapon capacity per rank.",
                BunkerUpgradeType.ScavengerNetwork => "+8% extracted Scrap per rank.",
                _ => string.Empty
            };
        }
    }
}
