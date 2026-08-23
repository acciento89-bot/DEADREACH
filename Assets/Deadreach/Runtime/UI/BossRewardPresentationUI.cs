using System.Collections;
using System.Text;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BossRewardPresentationUI : MonoBehaviour
    {
        private CanvasGroup _group;
        private Image _panel;
        private Text _title;
        private Text _weapon;
        private Text _details;
        private Text _affixes;
        private RectTransform _safeRoot;
        private RectTransform _panelRect;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private Coroutine _routine;
        private RunSession _session;

        private void Start()
        {
            _session = RunSession.Current ?? GetComponent<RunSession>();
            if (_session == null)
                return;

            BuildCanvas();
            _session.BossRewardGranted += HandleReward;
        }

        private void Update()
        {
            if (_safeRoot == null)
                return;

            if (_lastSafeArea != Screen.safeArea || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
                ApplyResponsiveLayout();
        }

        private void OnDestroy()
        {
            if (_session != null)
                _session.BossRewardGranted -= HandleReward;
        }

        private void BuildCanvas()
        {
            var canvasObject = new GameObject("BossRewardCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 82;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var safeObject = new GameObject("BossRewardSafeArea", typeof(RectTransform));
            safeObject.transform.SetParent(canvasObject.transform, false);
            _safeRoot = safeObject.GetComponent<RectTransform>();

            var panelObject = new GameObject("BossRewardToast", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            panelObject.transform.SetParent(safeObject.transform, false);
            _panelRect = panelObject.GetComponent<RectTransform>();

            _group = panelObject.GetComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;

            _panel = panelObject.GetComponent<Image>();
            _panel.color = new Color(0.028f, 0.025f, 0.022f, 0.96f);

            var stripe = new GameObject("RewardStripe", typeof(RectTransform), typeof(Image));
            stripe.transform.SetParent(panelObject.transform, false);
            var stripeRect = stripe.GetComponent<RectTransform>();
            stripeRect.anchorMin = new Vector2(0f, 0f);
            stripeRect.anchorMax = new Vector2(0.012f, 1f);
            stripeRect.offsetMin = Vector2.zero;
            stripeRect.offsetMax = Vector2.zero;
            stripe.GetComponent<Image>().color = new Color(1f, 0.32f, 0.05f, 1f);

            _title = CreateText(panelObject.transform, "RewardTitle", 17, FontStyle.Bold, TextAnchor.MiddleLeft,
                new Vector2(0.055f, 0.72f), new Vector2(0.95f, 0.94f));
            _title.text = "RELIC SECURED";

            _weapon = CreateText(panelObject.transform, "RewardWeapon", 18, FontStyle.Bold, TextAnchor.MiddleLeft,
                new Vector2(0.055f, 0.49f), new Vector2(0.95f, 0.73f));

            _details = CreateText(panelObject.transform, "RewardDetails", 12, FontStyle.Bold, TextAnchor.UpperLeft,
                new Vector2(0.055f, 0.27f), new Vector2(0.95f, 0.49f));

            _affixes = CreateText(panelObject.transform, "RewardAffixes", 11, FontStyle.Normal, TextAnchor.UpperLeft,
                new Vector2(0.055f, 0.07f), new Vector2(0.95f, 0.27f));

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
                SetAnchors(_panelRect, 0.70f, 0.11f, 0.97f, 0.38f);
            else if (aspect <= 1.72f)
                SetAnchors(_panelRect, 0.58f, 0.10f, 0.96f, 0.39f);
            else
                SetAnchors(_panelRect, 0.66f, 0.11f, 0.97f, 0.39f);
        }

        private void HandleReward(WeaponInstanceData reward)
        {
            if (reward == null || _group == null)
                return;

            var rarityColor = reward.rarity switch
            {
                WeaponRarity.Legendary => new Color(1f, 0.55f, 0.08f, 1f),
                WeaponRarity.Epic => new Color(0.72f, 0.28f, 1f, 1f),
                WeaponRarity.Rare => new Color(0.18f, 0.58f, 1f, 1f),
                _ => new Color(0.18f, 0.9f, 0.45f, 1f)
            };

            _title.color = rarityColor;
            _weapon.color = Color.white;
            _weapon.text = reward.displayNameSnapshot;
            _details.color = rarityColor;
            _details.text = $"{reward.rarity.ToString().ToUpperInvariant()} // {reward.family.ToString().ToUpperInvariant()} // PWR {reward.itemPower}\n{WeaponVisualStyle.GetDisplayName(WeaponVisualStyle.ResolveFinishId(reward))}";
            _affixes.color = new Color(0.82f, 0.84f, 0.82f, 1f);
            _affixes.text = BuildAffixText(reward);
            _panel.color = Color.Lerp(new Color(0.018f, 0.022f, 0.02f, 0.96f), rarityColor, 0.10f);

            if (_routine != null)
                StopCoroutine(_routine);
            _routine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            var elapsed = 0f;
            const float fadeIn = 0.16f;
            const float hold = 3.2f;
            const float fadeOut = 0.34f;

            while (elapsed < fadeIn)
            {
                elapsed += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Clamp01(elapsed / fadeIn);
                yield return null;
            }

            _group.alpha = 1f;
            yield return new WaitForSecondsRealtime(hold);

            elapsed = 0f;
            while (elapsed < fadeOut)
            {
                elapsed += Time.unscaledDeltaTime;
                _group.alpha = 1f - Mathf.Clamp01(elapsed / fadeOut);
                yield return null;
            }

            _group.alpha = 0f;
            _routine = null;
        }

        private static string BuildAffixText(WeaponInstanceData reward)
        {
            if (reward.affixes == null || reward.affixes.Count == 0)
                return "NO AFFIXES";

            var builder = new StringBuilder();
            for (var i = 0; i < reward.affixes.Count; i++)
            {
                var affix = reward.affixes[i];
                if (affix == null)
                    continue;

                if (builder.Length > 0)
                    builder.Append("  //  ");

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
                WeaponAffixStat.DamagePercent => "DMG",
                WeaponAffixStat.FireRatePercent => "FIRE RATE",
                WeaponAffixStat.RangePercent => "RANGE",
                WeaponAffixStat.CritChancePercent => "CRIT",
                WeaponAffixStat.CritDamagePercent => "CRIT DMG",
                _ => stat.ToString().ToUpperInvariant()
            };
        }

        private static Text CreateText(Transform parent, string name, int size, FontStyle style, TextAnchor anchor, Vector2 min, Vector2 max)
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
            text.alignment = anchor;
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
