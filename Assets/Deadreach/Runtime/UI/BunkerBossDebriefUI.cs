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
        private RectTransform _safeRoot;
        private RectTransform _panelRect;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void Start()
        {
            var data = SaveService.Data;
            if (!data.bossRewardDebriefPending || data.lastBossReward == null)
                return;

            Build(data.lastBossReward);
        }

        private void Update()
        {
            if (_safeRoot == null)
                return;

            if (_lastSafeArea != Screen.safeArea || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
                ApplyResponsiveLayout();
        }

        private void Build(WeaponInstanceData reward)
        {
            _canvasObject = new GameObject("BunkerBossDebriefCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _canvasObject.transform.SetParent(transform, false);
            var canvas = _canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 96;

            var scaler = _canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.56f;

            var safeObject = new GameObject("DebriefSafeArea", typeof(RectTransform));
            safeObject.transform.SetParent(_canvasObject.transform, false);
            _safeRoot = safeObject.GetComponent<RectTransform>();

            var dim = new GameObject("DebriefDim", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            dim.transform.SetParent(safeObject.transform, false);
            var dimRect = dim.GetComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;
            var dimImage = dim.GetComponent<Image>();
            dimImage.color = new Color(0f, 0f, 0f, 0.88f);
            dimImage.raycastTarget = true;
            var dimGroup = dim.GetComponent<CanvasGroup>();
            dimGroup.interactable = true;
            dimGroup.blocksRaycasts = true;

            var panel = new GameObject("DebriefPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(dim.transform, false);
            _panelRect = panel.GetComponent<RectTransform>();

            var accent = reward.rarity == WeaponRarity.Legendary
                ? new Color(1f, 0.52f, 0.05f, 1f)
                : new Color(0.72f, 0.25f, 1f, 1f);
            panel.GetComponent<Image>().color = new Color(0.018f, 0.027f, 0.025f, 0.995f);

            var stripe = new GameObject("DebriefStripe", typeof(RectTransform), typeof(Image));
            stripe.transform.SetParent(panel.transform, false);
            var stripeRect = stripe.GetComponent<RectTransform>();
            stripeRect.anchorMin = new Vector2(0f, 0.976f);
            stripeRect.anchorMax = Vector2.one;
            stripeRect.offsetMin = Vector2.zero;
            stripeRect.offsetMax = Vector2.zero;
            stripe.GetComponent<Image>().color = accent;

            var eyebrow = CreateText(panel.transform, "DebriefEyebrow", 15, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.055f, 0.84f), new Vector2(0.945f, 0.94f));
            eyebrow.text = "BUNKER RECOVERY DEBRIEF // MUTATION TARGET";
            eyebrow.color = accent;

            var title = CreateText(panel.transform, "DebriefTitle", 34, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.055f, 0.66f), new Vector2(0.945f, 0.84f));
            title.text = "RELIC SECURED";
            title.color = Color.white;

            var weapon = CreateText(panel.transform, "DebriefWeapon", 25, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.055f, 0.49f), new Vector2(0.945f, 0.66f));
            weapon.text = reward.displayNameSnapshot;
            weapon.color = accent;

            var details = CreateText(panel.transform, "DebriefDetails", 16, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.065f, 0.32f), new Vector2(0.935f, 0.49f));
            details.text = $"{reward.rarity.ToString().ToUpperInvariant()} // {reward.family.ToString().ToUpperInvariant()} // ITEM POWER {reward.itemPower}\nFINISH // {WeaponVisualStyle.GetDisplayName(WeaponVisualStyle.ResolveFinishId(reward))}";
            details.color = new Color(0.82f, 0.84f, 0.82f, 1f);

            var affixes = CreateText(panel.transform, "DebriefAffixes", 13, FontStyle.Normal, TextAnchor.UpperCenter,
                new Vector2(0.065f, 0.19f), new Vector2(0.935f, 0.315f));
            affixes.text = BuildAffixText(reward);
            affixes.color = new Color(0.68f, 0.72f, 0.70f, 1f);

            var buttonObject = new GameObject("DebriefAcknowledge", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(panel.transform, false);
            var buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.30f, 0.055f);
            buttonRect.anchorMax = new Vector2(0.70f, 0.155f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = new Color(0.62f, 0.11f, 0.025f, 1f);
            buttonObject.GetComponent<Button>().onClick.AddListener(Acknowledge);

            var buttonLabel = CreateText(buttonObject.transform, "Label", 16, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            buttonLabel.text = "TRANSFER TO ARSENAL";
            buttonLabel.color = Color.white;

            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            if (_safeRoot == null || _panelRect == null || Screen.width <= 0 || Screen.height <= 0)
                return;

            _lastSafeArea = Screen.safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            var safeMin = Screen.safeArea.position;
            var safeMax = Screen.safeArea.position + Screen.safeArea.size;
            safeMin.x /= Screen.width;
            safeMin.y /= Screen.height;
            safeMax.x /= Screen.width;
            safeMax.y /= Screen.height;
            _safeRoot.anchorMin = safeMin;
            _safeRoot.anchorMax = safeMax;
            _safeRoot.offsetMin = Vector2.zero;
            _safeRoot.offsetMax = Vector2.zero;

            var aspect = Screen.safeArea.width / Mathf.Max(1f, Screen.safeArea.height);
            if (aspect >= 2.05f)
                SetAnchors(_panelRect, 0.28f, 0.12f, 0.72f, 0.88f);
            else if (aspect <= 1.72f)
                SetAnchors(_panelRect, 0.12f, 0.10f, 0.88f, 0.90f);
            else
                SetAnchors(_panelRect, 0.22f, 0.13f, 0.78f, 0.87f);
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
                builder.Append(GetAffixLabel(affix.stat));
                builder.Append(" +");
                builder.Append(affix.value.ToString("0.#"));
                builder.Append('%');
            }
            return builder.ToString();
        }

        private static string GetAffixLabel(WeaponAffixStat stat)
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

        private static void SetAnchors(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
