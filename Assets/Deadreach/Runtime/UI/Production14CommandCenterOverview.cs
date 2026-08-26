using System;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed partial class Production14CommandCenterUI
    {
        private void BuildOverview()
        {
            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);
            var weapon = SaveService.GetEquippedPrimaryWeapon();
            var boss = data.selectedLevel % 10 == 0;
            var zoneName = RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant();

            var mission = CreateIndustrialPanel("MissionConsole", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(mission, 0.012f, 0.145f, 0.275f, 0.710f);
            AddSectionHeader(mission, "NEXT DEPLOYMENT", _cyan);

            var state = CreatePill("MissionState", mission, boss ? "MUTATION TARGET" : "DEPLOYMENT READY", boss ? _danger : _green);
            Place(state, 0.065f, 0.735f, 0.62f, 0.805f);

            var level = CreateLabel("Level", mission, $"LEVEL {data.selectedLevel:00}", 40, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(level.rectTransform, 0.065f, 0.575f, 0.90f, 0.725f);

            var zone = CreateLabel("Zone", mission, zoneName, 21, FontStyle.Bold, _amber, TextAnchor.MiddleLeft);
            Place(zone.rectTransform, 0.065f, 0.505f, 0.90f, 0.59f);

            var line = CreateImage("MissionLine", mission, _amber);
            Place(line.rectTransform, 0.065f, 0.482f, 0.94f, 0.487f);

            var objectiveTitle = CreateLabel("ObjectiveTitle", mission, "OBJECTIVE", 12, FontStyle.Bold, _muted, TextAnchor.MiddleLeft);
            Place(objectiveTitle.rectTransform, 0.065f, 0.415f, 0.90f, 0.47f);
            var objective = CreateLabel("Objective", mission,
                boss ? "ELIMINATE MUTATION TARGET" : "SECURE LOOT AND REACH EXTRACTION",
                16, FontStyle.Bold, _white, TextAnchor.UpperLeft);
            Place(objective.rectTransform, 0.065f, 0.325f, 0.94f, 0.42f);

            var loadout = CreateLabel("Loadout", mission,
                $"OPERATOR  {op.Name.ToUpperInvariant()}\n" +
                $"PRIMARY   {(weapon != null ? weapon.displayNameSnapshot.ToUpperInvariant() : "DR-7 FIELD ISSUE")}\n" +
                $"STREAK    {data.currentExtractionStreak}   •   BEST {data.bestExtractionStreak}",
                14, FontStyle.Bold, new Color(0.93f, 0.95f, 0.95f, 1f), TextAnchor.UpperLeft);
            Place(loadout.rectTransform, 0.065f, 0.105f, 0.94f, 0.30f);

            var heroFrame = CreateIndustrialPanel("BunkerHero", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(heroFrame, 0.285f, 0.145f, 0.720f, 0.710f);
            var heroImage = heroFrame.GetComponent<Image>();
            if (heroImage != null)
                heroImage.color = new Color(1f, 1f, 1f, 0.30f);

            var heroTopShade = CreateImage("HeroTopShade", heroFrame, new Color(0.005f, 0.012f, 0.014f, 0.76f));
            Place(heroTopShade.rectTransform, 0.02f, 0.86f, 0.98f, 0.98f);
            var heroTitle = CreateLabel("HeroTitle", heroFrame, "BUNKER COMMAND", 24, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(heroTitle.rectTransform, 0.055f, 0.885f, 0.60f, 0.965f);
            var heroState = CreateLabel("HeroState", heroFrame, "TACTICAL SYSTEMS ONLINE", 13, FontStyle.Bold, _cyan, TextAnchor.MiddleRight);
            Place(heroState.rectTransform, 0.52f, 0.885f, 0.945f, 0.965f);

            var heroBottom = CreateIndustrialPanel("HeroBottom", heroFrame, Production14IndustrialSkin.PlateKind.Tag, false);
            Place(heroBottom, 0.055f, 0.055f, 0.945f, 0.145f);
            var heroBottomText = CreateLabel("HeroBottomText", heroBottom,
                $"LEVEL {data.selectedLevel:00}   •   {zoneName}   •   {op.Name.ToUpperInvariant()} READY",
                14, FontStyle.Bold, _cyan, TextAnchor.MiddleCenter);
            Fill(heroBottomText.rectTransform, 10f, 4f, 10f, 4f);

            var campaign = CreateIndustrialPanel("CampaignConsole", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(campaign, 0.730f, 0.420f, 0.988f, 0.710f);
            AddSectionHeader(campaign, "CAMPAIGN STATUS", _cyan);

            CreateStatusMetric(campaign, "HIGHEST UNLOCKED", $"LEVEL {data.highestUnlockedLevel:00}", 0.57f,
                (float)data.highestUnlockedLevel / SaveService.MaxCampaignLevel);
            CreateStatusMetric(campaign, "HIGHEST CLEARED", data.highestCompletedLevel > 0 ? $"LEVEL {data.highestCompletedLevel:00}" : "—", 0.31f,
                Mathf.Clamp01(data.highestCompletedLevel / (float)Mathf.Max(1, data.highestUnlockedLevel)));
            CreateStatusMetric(campaign, "BOSS KILLS", data.bossKills.ToString("N0"), 0.055f, Mathf.Clamp01(data.bossKills / 5f));

            var operatorPanel = CreateIndustrialPanel("ActiveOperator", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(operatorPanel, 0.730f, 0.145f, 0.988f, 0.405f);
            AddSectionHeader(operatorPanel, "ACTIVE OPERATOR", op.Accent);

            var opName = CreateLabel("OperatorName", operatorPanel, op.Name.ToUpperInvariant(), 30, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(opName.rectTransform, 0.07f, 0.55f, 0.68f, 0.78f);
            var opRole = CreateLabel("OperatorRole", operatorPanel, op.Role.ToUpperInvariant(), 15, FontStyle.Bold, op.Accent, TextAnchor.MiddleLeft);
            Place(opRole.rectTransform, 0.07f, 0.45f, 0.68f, 0.58f);

            var opStats = CreateLabel("OperatorStats", operatorPanel,
                $"VITALS x{op.HealthMultiplier:0.00}   •   MOBILITY x{op.MoveMultiplier:0.00}   •   DAMAGE x{op.DamageMultiplier:0.00}",
                12, FontStyle.Bold, _muted, TextAnchor.MiddleLeft);
            Place(opStats.rectTransform, 0.07f, 0.31f, 0.94f, 0.44f);

            var change = CreateActionButton("ChangeOperator", operatorPanel, "CHANGE OPERATOR", () => HandleNav(2));
            Place(change.GetComponent<RectTransform>(), 0.49f, 0.08f, 0.94f, 0.28f);
        }

        private void CreateStatusMetric(Transform parent, string label, string value, float y, float fill)
        {
            var title = CreateLabel($"Metric_{label}_Label", parent, label, 11, FontStyle.Bold, _muted, TextAnchor.UpperLeft);
            Place(title.rectTransform, 0.08f, y + 0.105f, 0.92f, y + 0.18f);

            var number = CreateLabel($"Metric_{label}_Value", parent, value, 20, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(number.rectTransform, 0.08f, y + 0.035f, 0.92f, y + 0.115f);

            var track = CreateImage($"Metric_{label}_Track", parent, new Color(0.12f, 0.17f, 0.18f, 0.95f));
            Place(track.rectTransform, 0.08f, y, 0.92f, y + 0.016f);

            var bar = CreateImage($"Metric_{label}_Fill", parent, _cyan);
            Place(bar.rectTransform, 0.08f, y, Mathf.Lerp(0.08f, 0.92f, Mathf.Clamp01(fill)), y + 0.016f);
        }

        private void BuildFooter()
        {
            var footer = CreateIndustrialPanel("Footer", _root, Production14IndustrialSkin.PlateKind.Footer, false);
            Place(footer, 0.012f, 0.018f, 0.988f, 0.120f);

            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);

            var feedTitle = CreateLabel("FeedTitle", footer, "BUNKER FEED", 15, FontStyle.Bold, _cyan, TextAnchor.MiddleLeft);
            Place(feedTitle.rectTransform, 0.025f, 0.47f, 0.18f, 0.88f);

            _deployInfo = CreateLabel("ReadyLine", footer,
                $"SYSTEMS ONLINE   •   LEVEL {data.selectedLevel:00} {RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant()}   •   OPERATOR {op.Name.ToUpperInvariant()}",
                13, FontStyle.Bold, _muted, TextAnchor.MiddleLeft);
            Place(_deployInfo.rectTransform, 0.025f, 0.08f, 0.70f, 0.50f);

            var deploy = CreateActionButton("Deploy", footer, "DEPLOY  ›", () => SceneFlowService.LoadExpedition());
            Place(deploy.GetComponent<RectTransform>(), 0.715f, 0.08f, 0.985f, 0.92f);

            var deployImage = deploy.image;
            if (deployImage != null)
                deployImage.color = new Color(1f, 0.60f, 0.42f, 1f);

            var deployText = deploy.transform.Find("Text")?.GetComponent<Text>();
            if (deployText != null)
            {
                deployText.fontSize = 25;
                deployText.color = _white;
            }
        }

        private Button CreateActionButton(string name, Transform parent, string label, Action onClick)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.sprite = Production14IndustrialSkin.Get(Production14IndustrialSkin.PlateKind.Deploy);
            image.type = Image.Type.Sliced;

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.75f, 0.88f, 0.90f, 1f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            button.onClick.AddListener(() => onClick?.Invoke());

            var text = CreateLabel("Text", go.transform, label, 15, FontStyle.Bold, _cyan, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 8f, 4f, 8f, 4f);
            return button;
        }

        private RectTransform CreatePill(string name, Transform parent, string label, Color accent)
        {
            var root = CreateRect(name, parent);
            var bg = CreateImage("Background", root, new Color(accent.r * 0.16f, accent.g * 0.16f, accent.b * 0.16f, 0.94f));
            Fill(bg.rectTransform);

            var rail = CreateImage("Rail", root, accent);
            Place(rail.rectTransform, 0f, 0f, 0.018f, 1f);

            var text = CreateLabel("Text", root, label, 12, FontStyle.Bold, accent, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 6f, 2f, 6f, 2f);
            return root;
        }

        private void AddSectionHeader(Transform parent, string text, Color accent)
        {
            var label = CreateLabel("HeaderLabel", parent, text, 14, FontStyle.Bold, accent, TextAnchor.MiddleLeft);
            Place(label.rectTransform, 0.06f, 0.84f, 0.90f, 0.94f);

            var line = CreateImage("HeaderLine", parent, accent);
            Place(line.rectTransform, 0.06f, 0.815f, 0.94f, 0.823f);
        }

        private void AddBoltPair(Transform parent)
        {
            CreateBolt(parent, 0.035f, 0.055f);
            CreateBolt(parent, 0.935f, 0.055f);
            CreateBolt(parent, 0.035f, 0.93f);
            CreateBolt(parent, 0.935f, 0.93f);
        }

        private void CreateBolt(Transform parent, float x, float y)
        {
            var bolt = CreateImage("Bolt", parent, new Color(0.54f, 0.58f, 0.58f, 0.82f));
            Place(bolt.rectTransform, x, y, x + 0.018f, y + 0.018f);
        }

        private RectTransform CreateIndustrialPanel(string name, Transform parent, Production14IndustrialSkin.PlateKind kind, bool raycast)
        {
            var root = CreateRect(name, parent);
            var image = root.gameObject.AddComponent<Image>();
            image.sprite = Production14IndustrialSkin.Get(kind);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = raycast;
            return root;
        }

        private Text CreateLabel(string name, Transform parent, string value, int size, FontStyle style, Color color, TextAnchor anchor)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(Shadow));
            go.transform.SetParent(parent, false);

            var text = go.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = ReadableFontSize(size);
            text.fontStyle = style;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            var shadow = go.GetComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.72f);
            shadow.effectDistance = new Vector2(1f, -1f);
            shadow.useGraphicAlpha = true;
            return text;
        }

        private static int ReadableFontSize(int requested)
        {
            if (requested <= 9)
                return requested + 3;
            if (requested <= 12)
                return requested + 2;
            if (requested <= 16)
                return requested + 1;
            return requested;
        }

        private Image CreateImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void Fill(RectTransform rect, float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
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
