using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        private Damageable _playerHealth;
        private HitscanWeapon _weapon;
        private GUIStyle _titleStyle;
        private GUIStyle _textStyle;
        private GUIStyle _centerStyle;
        private GUIStyle _smallCenterStyle;
        private float _damageFlashUntil;

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player == null)
                return;

            _playerHealth = player.GetComponent<Damageable>();
            _weapon = player.GetComponent<HitscanWeapon>();
            if (_playerHealth != null)
                _playerHealth.Damaged += HandlePlayerDamaged;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
                _playerHealth.Damaged -= HandlePlayerDamaged;
        }

        private void HandlePlayerDamaged(DamageInfo info)
        {
            _damageFlashUntil = Time.unscaledTime + 0.16f;
        }

        private void OnGUI()
        {
            EnsureStyles();

            var safe = Screen.safeArea;
            var left = safe.x + 22f;
            var top = Screen.height - safe.yMax + 18f;
            var width = Mathf.Min(470f, safe.width * 0.5f);
            var session = RunSession.Current;
            var profile = SaveService.Data;
            var inventory = RunInventory.Current;

            DrawDamageFlash(safe);

            GUI.Box(new Rect(left, top, width, 232f), GUIContent.none);
            GUI.Label(new Rect(left + 16f, top + 10f, width - 32f, 30f), "DEADREACH // FIELD OPS", _titleStyle);

            var hp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.CurrentHealth) : 0;
            var maxHp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.MaxHealth) : 0;
            var healthNormalized = _playerHealth != null ? _playerHealth.NormalizedHealth : 0f;

            GUI.Label(new Rect(left + 16f, top + 43f, width - 32f, 22f), $"VITALS   {hp}/{maxHp}", _textStyle);
            DrawBar(new Rect(left + 16f, top + 67f, width - 32f, 12f), healthNormalized, new Color(0.22f, 0.78f, 0.36f), new Color(0.75f, 0.15f, 0.12f));

            DrawPrimaryWeapon(left + 16f, top + 88f, width - 32f);
            GUI.Label(new Rect(left + 16f, top + 124f, width - 32f, 24f), $"CARRIED SCRAP   {session?.CarriedScrap ?? 0}", _textStyle);
            GUI.Label(new Rect(left + 16f, top + 150f, width - 32f, 24f), $"WEAPON LOOT   {inventory?.Weapons.Count ?? 0}/{inventory?.WeaponCapacity ?? 0}", _textStyle);
            GUI.Label(new Rect(left + 16f, top + 176f, width - 32f, 24f), $"SECURED   {profile.securedScrap}      STREAK   {profile.currentExtractionStreak}", _textStyle);
            GUI.Label(new Rect(left + 16f, top + 202f, width - 32f, 24f), "Reach the green beacon with any loot to extract.", _textStyle);

            GUI.Label(new Rect(left, safe.yMax - 42f, width + 220f, 28f), "WASD / LEFT THUMB = MOVE   •   MOUSE / RIGHT THUMB = AIM + FIRE", _textStyle);

            if (session == null)
                return;

            DrawExtractionFeedback(safe, session);
            DrawRunResult(safe, session);
        }

        private void DrawPrimaryWeapon(float x, float y, float width)
        {
            if (_weapon == null)
            {
                GUI.Label(new Rect(x, y, width, 32f), "PRIMARY // FIELD DR-7", _textStyle);
                return;
            }

            var instance = _weapon.EquippedInstance;
            var stats = _weapon.RuntimeStats;
            var name = instance != null ? instance.displayNameSnapshot : "Field DR-7 Rifle";
            var power = instance != null ? $"PWR {instance.itemPower}" : "BASE";
            GUI.Label(
                new Rect(x, y, width, 32f),
                $"PRIMARY // {name} // {power} // DMG {stats.Damage:0.#} // CRIT {stats.CritChance * 100f:0.#}%",
                _textStyle);
        }

        private void DrawExtractionFeedback(Rect safe, RunSession session)
        {
            if (!session.IsInExtractionZone || session.IsCompleted || session.IsFailed)
                return;

            var width = Mathf.Min(520f, safe.width - 40f);
            var x = safe.center.x - width * 0.5f;
            var y = Screen.height - safe.yMax + 30f;

            GUI.Box(new Rect(x, y, width, 92f), GUIContent.none);

            if (session.ExtractionBlockedByNoLoot)
            {
                GUI.Label(new Rect(x + 18f, y + 12f, width - 36f, 32f), "EXTRACTION LOCKED", _centerStyle);
                GUI.Label(new Rect(x + 18f, y + 48f, width - 36f, 24f), "Collect Scrap or weapon loot before extracting.", _smallCenterStyle);
                return;
            }

            GUI.Label(new Rect(x + 18f, y + 8f, width - 36f, 30f), "EXTRACTING // HOLD POSITION", _centerStyle);
            DrawBar(new Rect(x + 28f, y + 50f, width - 56f, 18f), session.ExtractionProgress, new Color(0.12f, 0.92f, 0.48f), new Color(0.12f, 0.92f, 0.48f));
        }

        private void DrawRunResult(Rect safe, RunSession session)
        {
            if (!session.IsCompleted && !session.IsFailed)
                return;

            GUI.Box(new Rect(safe.center.x - 240f, safe.center.y - 78f, 480f, 156f), GUIContent.none);

            if (session.IsCompleted)
            {
                GUI.Label(new Rect(safe.center.x - 220f, safe.center.y - 48f, 440f, 42f), "EXTRACTION SECURED", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 220f, safe.center.y + 2f, 440f, 30f), "Loot banked. Returning to bunker...", _smallCenterStyle);
            }
            else
            {
                GUI.Label(new Rect(safe.center.x - 220f, safe.center.y - 48f, 440f, 42f), "RUN LOST", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 220f, safe.center.y + 2f, 440f, 30f), "Unsecured loot lost. Returning to bunker...", _smallCenterStyle);
            }
        }

        private void DrawDamageFlash(Rect safe)
        {
            if (Time.unscaledTime >= _damageFlashUntil)
                return;

            var previous = GUI.color;
            GUI.color = new Color(0.8f, 0.05f, 0.03f, 0.18f);
            GUI.DrawTexture(new Rect(safe.x, Screen.height - safe.yMax, safe.width, safe.height), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private static void DrawBar(Rect rect, float normalized, Color healthyColor, Color criticalColor)
        {
            normalized = Mathf.Clamp01(normalized);
            var previous = GUI.color;

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            var fillColor = Color.Lerp(criticalColor, healthyColor, normalized);
            GUI.color = fillColor;
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(0f, (rect.width - 4f) * normalized), Mathf.Max(0f, rect.height - 4f)), Texture2D.whiteTexture);

            GUI.color = previous;
        }

        private void EnsureStyles()
        {
            if (_textStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13
            };

            _centerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _smallCenterStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
