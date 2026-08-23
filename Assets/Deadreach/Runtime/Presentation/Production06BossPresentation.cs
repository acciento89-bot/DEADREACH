using System.Collections;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.Presentation
{
    public sealed class Production06BossPresentation : MonoBehaviour
    {
        private RunDifficultyDirector _director;
        private CanvasGroup _overlayGroup;
        private Text _nameText;
        private Text _phaseText;
        private Light _aura;
        private Renderer[] _bossRenderers;
        private MaterialPropertyBlock _block;
        private Color _accent;

        private IEnumerator Start()
        {
            yield return null;
            _director = RunDifficultyDirector.Current;
            if (_director == null || !_director.IsBossLevel)
                yield break;

            var wait = 0f;
            while (_director.BossHealth == null && wait < 3f)
            {
                wait += Time.unscaledDeltaTime;
                yield return null;
            }

            if (_director.BossHealth == null)
                yield break;

            var tier = Mathf.Clamp(Mathf.Max(1, _director.Level / 10), 1, 5);
            _accent = GetTierColor(tier);
            BuildBossVisual(_director.BossHealth.gameObject, tier);
            BuildOverlay(tier);

            _director.BossPhaseChanged += HandlePhase;
            _director.BossDefeated += HandleDefeated;
        }

        private void OnDestroy()
        {
            if (_director != null)
            {
                _director.BossPhaseChanged -= HandlePhase;
                _director.BossDefeated -= HandleDefeated;
            }
        }

        private void BuildBossVisual(GameObject boss, int tier)
        {
            _bossRenderers = boss.GetComponentsInChildren<Renderer>(true);
            _block = new MaterialPropertyBlock();
            ApplyBossTint(_accent, 0.34f);

            var auraObject = new GameObject("MutationBossAura");
            auraObject.transform.SetParent(boss.transform, false);
            auraObject.transform.localPosition = Vector3.up * 1.25f;
            _aura = auraObject.AddComponent<Light>();
            _aura.type = LightType.Point;
            _aura.color = _accent;
            _aura.intensity = 2.7f + tier * 0.45f;
            _aura.range = 5.5f + tier * 0.45f;
            _aura.shadows = LightShadows.None;

            var particleObject = new GameObject("MutationBossParticles");
            particleObject.transform.SetParent(boss.transform, false);
            particleObject.transform.localPosition = Vector3.up * 1.1f;
            var particles = particleObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.loop = true;
            main.startLifetime = 0.75f;
            main.startSpeed = 1.15f;
            main.startSize = 0.085f + tier * 0.01f;
            main.startColor = _accent;
            main.maxParticles = 90;
            var emission = particles.emission;
            emission.rateOverTime = 22f + tier * 4f;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.25f + tier * 0.08f;
        }

        private void BuildOverlay(int tier)
        {
            var canvasObject = new GameObject("MutationBossIdentityCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 54;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var panel = new GameObject("BossIdentityPanel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            panel.transform.SetParent(canvasObject.transform, false);
            var rect = panel.GetComponent<RectTransform>();
            // The existing PrototypeHud owns the centered boss HP bar at the very top. Keep the
            // identity card directly below it so both remain readable on desktop and phone landscape.
            rect.anchorMin = new Vector2(0.37f, 0.745f);
            rect.anchorMax = new Vector2(0.63f, 0.835f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.015f, 0.015f, 0.018f, 0.88f);
            _overlayGroup = panel.GetComponent<CanvasGroup>();
            _overlayGroup.blocksRaycasts = false;
            _overlayGroup.interactable = false;

            _nameText = CreateText(panel.transform, "BossName", 21, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.03f, 0.43f), new Vector2(0.97f, 0.92f));
            _nameText.color = _accent;
            _nameText.text = $"TIER {tier} // {GetBossName(tier)}";

            _phaseText = CreateText(panel.transform, "BossPhase", 12, FontStyle.Bold, TextAnchor.MiddleCenter,
                new Vector2(0.03f, 0.08f), new Vector2(0.97f, 0.44f));
            _phaseText.color = Color.white;
            _phaseText.text = "MUTATION STATE // STABLE";
        }

        private void HandlePhase(int phase)
        {
            if (_phaseText == null)
                return;

            _phaseText.text = phase switch
            {
                1 => "MUTATION STATE // ENRAGED",
                2 => "MUTATION STATE // TERMINAL OVERDRIVE",
                _ => "MUTATION STATE // STABLE"
            };

            if (_aura != null)
                _aura.intensity *= 1.28f;

            ApplyBossTint(Color.Lerp(_accent, Color.white, Mathf.Clamp01(phase * 0.18f)), 0.48f + phase * 0.12f);
        }

        private void HandleDefeated()
        {
            if (_phaseText != null)
            {
                _phaseText.text = "TARGET ELIMINATED // RELIC RECOVERED";
                _phaseText.color = new Color(0.15f, 1f, 0.48f, 1f);
            }

            if (_aura != null)
                _aura.intensity = 0f;
        }

        private void ApplyBossTint(Color color, float strength)
        {
            if (_bossRenderers == null)
                return;

            foreach (var renderer in _bossRenderers)
            {
                if (renderer == null)
                    continue;

                renderer.GetPropertyBlock(_block);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
                    _block.SetColor("_BaseColor", Color.Lerp(Color.white, color, strength));
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color"))
                    _block.SetColor("_Color", Color.Lerp(Color.white, color, strength));
                renderer.SetPropertyBlock(_block);
            }
        }

        private static Color GetTierColor(int tier)
        {
            return tier switch
            {
                1 => new Color(0.25f, 0.48f, 1f, 1f),
                2 => new Color(0.05f, 0.95f, 0.66f, 1f),
                3 => new Color(1f, 0.32f, 0.04f, 1f),
                4 => new Color(0.62f, 0.25f, 1f, 1f),
                _ => new Color(1f, 0.04f, 0.05f, 1f)
            };
        }

        private static string GetBossName(int tier)
        {
            return tier switch
            {
                1 => "THE BREAKER",
                2 => "FLOOD MAW",
                3 => "ASH TITAN",
                4 => "BLACKOUT WRAITH",
                _ => "GROUND ZERO PRIME"
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
            text.raycastTarget = false;
            return text;
        }
    }
}
