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

            var mission = CreateIndustrialPanel("MissionConsole", _contentRoot, Production14IndustrialSkin.PlateKind.Mission, false);
            Place(mission, 0.035f, 0.205f, 0.30f, 0.708f);
            AddSectionHeader(mission, "NEXT DEPLOYMENT", _amber);

            var state = CreatePill("MissionState", mission, boss ? "MUTATION TARGET" : "DEPLOYMENT READY", boss ? _danger : _green);
            Place(state, 0.075f, 0.71f, 0.57f, 0.785f);

            var level = CreateLabel("Level", mission, $"LEVEL {data.selectedLevel:00}", 39, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(level.rectTransform, 0.075f, 0.52f, 0.70f, 0.70f);

            var zone = CreateLabel("Zone", mission, RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant(), 18, FontStyle.Bold, _amber, TextAnchor.MiddleLeft);
            Place(zone.rectTransform, 0.075f, 0.43f, 0.80f, 0.53f);

            var rule = CreateImage("MissionRule", mission, new Color(_amber.r, _amber.g, _amber.b, 0.78f));
            Place(rule.rectTransform, 0.075f, 0.405f, 0.92f, 0.412f);

            var expedition = CreateLabel("Expedition", mission,
                boss ? "EXTRACTION SEALED // MUTATION CLASS TARGET" : "STANDARD EXPEDITION // LOOT REQUIRED FOR EXTRACTION",
                10, FontStyle.Bold, boss ? _danger : _green, TextAnchor.UpperLeft);
            Place(expedition.rectTransform, 0.075f, 0.315f, 0.92f, 0.395f);

            var loadout = CreateLabel("Loadout", mission,
                $"OPERATOR   {op.Name.ToUpperInvariant()} // {op.Role.ToUpperInvariant()}\n" +
                $"PRIMARY    {(weapon != null ? weapon.displayNameSnapshot.ToUpperInvariant() : "DR-7 FIELD ISSUE")}\n" +
                $"STREAK     {data.currentExtractionStreak}   //   BEST {data.bestExtractionStreak}",
                11, FontStyle.Bold, new Color(0.94f, 0.96f, 0.96f, 1f), TextAnchor.UpperLeft);
            Place(loadout.rectTransform, 0.075f, 0.10f, 0.92f, 0.295f);
            AddBoltPair(mission);

            var campaign = CreateIndustrialPanel("CampaignConsole", _contentRoot, Production14IndustrialSkin.PlateKind.Glass, false);
            Place(campaign, 0.755f, 0.32f, 0.955f, 0.708f);
            AddSectionHeader(campaign, "CAMPAIGN STATUS", _cyan);

            CreateStatusMetric(campaign, "HIGHEST UNLOCKED", $"{data.highestUnlockedLevel:00} / {SaveService.MaxCampaignLevel}", 0.61f,
                (float)data.highestUnlockedLevel / SaveService.MaxCampaignLevel);
            CreateStatusMetric(campaign, "HIGHEST CLEARED", data.highestCompletedLevel.ToString("00"), 0.36f,
                Mathf.Clamp01(data.highestCompletedLevel / (float)Mathf.Max(1, data.highestUnlockedLevel)));
            CreateStatusMetric(campaign, "BOSS KILLS", data.bossKills.ToString(), 0.11f, Mathf.Clamp01(data.bossKills / 5f));

            var centerTag = CreateIndustrialPanel("HoloTag", _contentRoot, Production14IndustrialSkin.PlateKind.Tag, false);
            Place(centerTag, 0.42f, 0.18f, 0.58f, 0.224f);
            var centerTagText = CreateLabel("TagText", centerTag, "BUNKER 07 // TACTICAL FEED", 8, FontStyle.Bold, _cyan, TextAnchor.MiddleCenter);
            Fill(centerTagText.rectTransform, 4f, 2f, 4f, 2f);

            var heroTop = CreateImage("HeroTopRail", _contentRoot, new Color(_cyan.r, _cyan.g, _cyan.b, 0.65f));
            Place(heroTop.rectTransform, 0.33f, 0.705f, 0.70f, 0.709f);

            var heroLeft = CreateImage("HeroLeftRail", _contentRoot, new Color(_cyan.r, _cyan.g, _cyan.b, 0.20f));
            Place(heroLeft.rectTransform, 0.325f, 0.225f, 0.327f, 0.70f);

            var heroRight = CreateImage("HeroRightRail", _contentRoot, new Color(_cyan.r, _cyan.g, _cyan.b, 0.20f));
            Place(heroRight.rectTransform, 0.704f, 0.225f, 0.706f, 0.70f);
        }

        private void CreateStatusMetric(Transform parent, string label, string value, float y, float fill)
        {
            var title = CreateLabel($"Metric_{label}_Label", parent, label, 8, FontStyle.Bold, _muted, TextAnchor.UpperLeft);
            Place(title.rectTransform, 0.10f, y + 0.105f, 0.88f, y + 0.16f);

            var number = CreateLabel($"Metric_{label}_Value", parent, value, 25, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(number.rectTransform, 0.10f, y + 0.02f, 0.88f, y + 0.11f);

            var track = CreateImage($"Metric_{label}_Track", parent, new Color(0.14f, 0.18f, 0.19f, 0.90f));
            Place(track.rectTransform, 0.10f, y, 0.90f, y + 0.014f);

            var bar = CreateImage($"Metric_{label}_Fill", parent, _cyan);
            Place(bar.rectTransform, 0.10f, y, Mathf.Lerp(0.10f, 0.90f, Mathf.Clamp01(fill)), y + 0.014f);
        }

        private void BuildFooter()
        {
            var footer = CreateIndustrialPanel("Footer", _root, Production14IndustrialSkin.PlateKind.Footer, false);
            Place(footer, 0.012f, 0.018f, 0.988f, 0.092f);

            var data = SaveService.Data;
            var op = OperatorCatalog.Get(data.selectedCharacterId);

            _deployInfo = CreateLabel("ReadyLine", footer,
                $"READY // LEVEL {data.selectedLevel:00} // {RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant()}   |   " +
                $"OPERATOR {op.Name.ToUpperInvariant()}   |   STANDARD EXPEDITION",
                11, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(_deployInfo.rectTransform, 0.025f, 0.12f, 0.70f, 0.88f);

            var greenRail = CreateImage("ReadyRail", footer, _green);
            Place(greenRail.rectTransform, 0.004f, 0.10f, 0.009f, 0.90f);

            var deploy = CreateActionButton("Deploy", footer, "DEPLOY  //  ENTER", () => SceneFlowService.LoadExpedition());
            Place(deploy.GetComponent<RectTransform>(), 0.73f, 0.12f, 0.982f, 0.88f);
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

            var text = CreateLabel("Text", go.transform, label, 12, FontStyle.Bold, _cyan, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 8f, 4f, 8f, 4f);
            return button;
        }

        private RectTransform CreatePill(string name, Transform parent, string label, Color accent)
        {
            var root = CreateRect(name, parent);
            var bg = CreateImage("Background", root, new Color(accent.r * 0.16f, accent.g * 0.16f, accent.b * 0.16f, 0.92f));
            Fill(bg.rectTransform);

            var rail = CreateImage("Rail", root, accent);
            Place(rail.rectTransform, 0f, 0f, 0.018f, 1f);

            var text = CreateLabel("Text", root, label, 9, FontStyle.Bold, accent, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 6f, 2f, 6f, 2f);
            return root;
        }

        private void AddSectionHeader(Transform parent, string text, Color accent)
        {
            var label = CreateLabel("HeaderLabel", parent, text, 10, FontStyle.Bold, accent, TextAnchor.MiddleLeft);
            Place(label.rectTransform, 0.06f, 0.84f, 0.88f, 0.91f);

            var line = CreateImage("HeaderLine", parent, accent);
            Place(line.rectTransform, 0.06f, 0.815f, 0.93f, 0.823f);

            var tick = CreateImage("HeaderTick", parent, accent);
            Place(tick.rectTransform, 0.91f, 0.86f, 0.925f, 0.91f);
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
