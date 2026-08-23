using System.Text;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BunkerBossDebriefUI : MonoBehaviour
    {
        private GameObject _canvasObject;

        private void Start()
        {
            var data = SaveService.Data;
            if (!data.bossRewardDebriefPending || data.lastBossReward == null)
                return;

            Build(data.lastBossReward);
        }

        private void Build(WeaponInstanceData reward)
        {
            _canvasObject = new GameObject("BunkerBossDebriefCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _canvasObject.transform.SetParent(transform, false);
            var canvas = _canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 95;

            var scaler = _canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.6f;

            var safe = Screen.safeArea;
            var safeMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            var safeMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);

            var dim = new GameObject("DebriefDim", typeof(RectTransform), typeof(Image));
            dim.transform.SetParent(_canvasObject.transform, false);
            var dimRect = dim.GetComponent<RectTransform>();
            dimRect.anchorMin = safeMin;
            dimRect.anchorMax = safeMax;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;
            dim.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

            var panel = new GameObject("DebriefPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(dim.transform, false);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.22f, 0.20f);
            panelRect.anchorMax = new Vector2(0.78f, 0.80f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var accent = reward.rarity == WeaponRarity.Legendary
                ? new Color(1f, 0.52f, 0.05f, 1f)
                : new Color(0.72f, 0.25f, 1f, 1f);
            panel.GetComponent<Image>().color = new Color(0.025f, 0.03f, 0.028f, 0.99f);

            var stripe = new GameObject("DebriefStripe", typeof(RectTransform), typeof(Image));
            stripe.transform.SetParent(panel.transform, false);
            var stripeRect = stripe.GetComponent<RectTransform>();
            stripeRect.anchorMin = new Vector2(0f, 0.975f);
            stripeRect.anchorMax = Vector2.one;
            stripeRect.offsetMin = Vector2.zero;
            stripeRect.offsetMax = Vector2.zero;
            stripe.GetComponent<Image>().color = accent;

            var eyebrow = CreateText(panel.transform, "DebriefEyebrow", 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.83f), new Vector2(0.95f, 0.94f));
            eyebrow.text = "BUNKER RECOVERY DEBRIEF // MUTATION TARGET";
            eyebrow.color = accent;

            var title = CreateText(panel.transform, "DebriefTitle", 34, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.65f), new Vector2(0.95f, 0.83f));
            title.text = "RELIC SECURED";
            title.color = Color.white;

            var weapon = CreateText(panel.transform, "DebriefWeapon", 25, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.66f));
            weapon.text = reward.displayNameSnapshot;
            weapon.color = accent;

            var details = CreateText(panel.transform, "DebriefDetails", 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.06f, 0.31f), new Vector2(0.94f, 0.49f));
            details.text = $"{reward.rarity.ToString().ToUpperInvariant()} // {reward.family.ToString().ToUpperInvariant()} // ITEM POWER {reward.itemPower}\nFINISH // {WeaponVisualStyle.GetDisplayName(WeaponVisualStyle.ResolveFinishId(reward))}";
            details.color = new Color(0.82f, 0.84f, 0.82f, 1f);

            var affixes = CreateText(panel.transform, "DebriefAffixes", 13, FontStyle.Normal, TextAnchor.UpperCenter, new Vector2(0.06f, 0.18f), new Vector2(0.94f, 0.31f));
            affixes.text = BuildAffixText(reward);
            affixes.color = new Color(0.66f, 0.7f, 0.68f, 1f);

            var buttonObject = new GameObject("DebriefAcknowledge", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(panel.transform, false);
            var buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.31f, 0.055f);
            buttonRect.anchorMax = new Vector2(0.69f, 0.15f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = new Color(0.55f, 0.08f, 0.025f, 1f);
            buttonObject.GetComponent<Button>().onClick.AddListener(Acknowledge);

            var buttonLabel = CreateText(buttonObject.transform, "Label", 16, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            buttonLabel.text = "TRANSFER TO ARSENAL";
            buttonLabel.color = Color.white;
        }

        private void Acknowledge()
        {
            SaveService.MarkBossRewardDebriefSeen();
            if (_canvasObject != null)
                Destroy(_canvasObject);
        }

        private static string BuildAffixText(WeaponInstanceData reward)
        {
            if (reward.affixes == null || reward.affixes.Count == 0)
                return "NO AFFIXES";

            var builder = new StringBuilder();
            foreach (var affix in reward.affixes)
            {
                if (affix == null)
                    continue;
                if (builder.Length > 0)
                    builder.Append("   //   ");
                builder.Append(affix.stat.ToString().ToUpperInvariant());
                builder.Append(" +");
                builder.Append(affix.value.ToString("0.#"));
                builder.Append('%');
            }
            return builder.ToString();
        }

        private static Text CreateText(Transform parent, string name, int size, FontStyle style, TextAnchor alignment, Vector2 min, Vector2 max)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = obj.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }
    }
}
