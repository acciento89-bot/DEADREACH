using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        private Damageable _playerHealth;
        private HitscanWeapon _weapon;
        private GUIStyle _titleStyle;
        private GUIStyle _textStyle;
        private GUIStyle _objectiveStyle;
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
            var left = safe.x + 18f;
            var top = Screen.height - safe.yMax + 14f;
            var width = Mathf.Min(420f, safe.width * 0.36f);
            var session = RunSession.Current;
            var profile = SaveService.Data;
            var inventory = RunInventory.Current;
            var director = RunDifficultyDirector.Current;

            DrawDamageFlash(safe);

            const float panelHeight = 202f;
            GUI.Box(new Rect(left, top, width, panelHeight), GUIContent.none);
            GUI.Label(new Rect(left + 14f, top + 8f, width - 28f, 25f), "DEADREACH // FIELD OPS", _titleStyle);
            GUI.Label(new Rect(left + 14f, top + 31f, width - 28f, 19f),
                $"LEVEL {session?.RunLevel ?? profile.selectedLevel:00} // {(director != null ? director.ZoneName : RunDifficultyDirector.GetZoneName(profile.selectedLevel))}", _textStyle);

            var hp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.CurrentHealth) : 0;
            var maxHp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.MaxHealth) : 0;
            var healthNormalized = _playerHealth != null ? _playerHealth.NormalizedHealth : 0f;

            GUI.Label(new Rect(left + 14f, top + 52f, width - 28f, 18f), $"VITALS   {hp}/{maxHp}", _textStyle);
            DrawBar(new Rect(left + 14f, top + 72f, width - 28f, 10f), healthNormalized, new Color(0.22f, 0.78f, 0.36f), new Color(0.75f, 0.15f, 0.12f));

            DrawPrimaryWeapon(left + 14f, top + 89f, width - 28f);
            GUI.Label(new Rect(left + 14f, top + 116f, width - 28f, 19f),
                $"CARRIED {session?.CarriedScrap ?? 0}   //   WEAPON LOOT {inventory?.Weapons.Count ?? 0}/{inventory?.WeaponCapacity ?? 0}", _textStyle);
            GUI.Label(new Rect(left + 14f, top + 136f, width - 28f, 19f),
                $"SECURED {profile.securedScrap}   //   STREAK {profile.currentExtractionStreak}", _textStyle);
            GUI.Label(new Rect(left + 14f, top + 158f, width - 28f, 34f),
                director != null && director.IsBossLevel && !director.BossGateCleared
                    ? "OBJECTIVE // ELIMINATE MUTATION TARGET"
                    : "OBJECTIVE // REACH EXTRACTION WITH LOOT", _objectiveStyle);

            if (!Application.isMobilePlatform)
            {
                GUI.Label(new Rect(left, safe.yMax - 34f, Mathf.Min(620f, safe.width * 0.58f), 24f),
                    "WASD = MOVE   •   MOUSE = AIM + FIRE", _textStyle);
            }

            if (director != null && director.IsBossLevel && !director.BossGateCleared)
                DrawBossBar(safe, director);

            if (session == null)
                return;

            DrawExtractionFeedback(safe, session);
            DrawRunResult(safe, session);
        }

        private void DrawPrimaryWeapon(float x, float y, float width)
        {
            if (_weapon == null)
            {
                GUI.Label(new Rect(x, y, width, 24f), "PRIMARY // FIELD DR-7", _textStyle);
                return;
            }

            var instance = _weapon.EquippedInstance;
            var stats = _weapon.RuntimeStats;
            var name = instance != null ? instance.displayNameSnapshot : "DR-7";
            var power = instance != null ? $"PWR {instance.itemPower}" : "BASE";
            GUI.Label(new Rect(x, y, width, 24f),
                $"PRIMARY // {name} // {power} // DMG {stats.Damage:0.#} // CRIT {stats.CritChance * 100f:0.#}%", _textStyle);
        }

        private void DrawBossBar(Rect safe, RunDifficultyDirector director)
        {
            var width = Mathf.Min(610f, safe.width * 0.48f);
            var x = safe.center.x - width * 0.5f;
            var y = Screen.height - safe.yMax + 12f;
            var tier = Mathf.Clamp(Mathf.Max(1, director.Level / 10), 1, 5);

            GUI.Box(new Rect(x, y, width, 58f), GUIContent.none);
            GUI.Label(new Rect(x + 16f, y + 5f, width - 32f, 23f),
                $"MUTATION // TIER {tier} // {GetBossName(tier)}", _centerStyle);
            DrawBar(new Rect(x + 24f, y + 34f, width - 48f, 12f), director.BossHealthNormalized,
                new Color(0.92f, 0.2f, 0.08f), new Color(0.35f, 0.02f, 0.01f));
        }

        private void DrawExtractionFeedback(Rect safe, RunSession session)
        {
            if (!session.IsInExtractionZone || session.IsCompleted || session.IsFailed)
                return;

            var width = Mathf.Min(440f, safe.width - 40f);
            var x = safe.center.x - width * 0.5f;
            var y = Screen.height - safe.yMax + 82f;

            GUI.Box(new Rect(x, y, width, 76f), GUIContent.none);

            if (session.ExtractionBlockedByBoss)
            {
                GUI.Label(new Rect(x + 16f, y + 8f, width - 32f, 28f), "EXTRACTION SEALED", _centerStyle);
                GUI.Label(new Rect(x + 16f, y + 40f, width - 32f, 22f), "Mutation target still active.", _smallCenterStyle);
                return;
            }

            if (session.ExtractionBlockedByNoLoot)
            {
                GUI.Label(new Rect(x + 16f, y + 8f, width - 32f, 28f), "EXTRACTION LOCKED", _centerStyle);
                GUI.Label(new Rect(x + 16f, y + 40f, width - 32f, 22f), "Collect Scrap or weapon loot first.", _smallCenterStyle);
                return;
            }

            GUI.Label(new Rect(x + 16f, y + 5f, width - 32f, 25f), "EXTRACTING // HOLD POSITION", _centerStyle);
            DrawBar(new Rect(x + 24f, y + 44f, width - 48f, 15f), session.ExtractionProgress,
                new Color(0.12f, 0.92f, 0.48f), new Color(0.12f, 0.92f, 0.48f));
        }

        private void DrawRunResult(Rect safe, RunSession session)
        {
            if (!session.IsCompleted && !session.IsFailed)
                return;

            GUI.Box(new Rect(safe.center.x - 220f, safe.center.y - 66f, 440f, 132f), GUIContent.none);

            if (session.IsCompleted)
            {
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y - 40f, 400f, 36f), "EXTRACTION SECURED", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y + 2f, 400f, 26f), "Loot banked. Returning to bunker...", _smallCenterStyle);
            }
            else
            {
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y - 40f, 400f, 36f), "RUN LOST", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y + 2f, 400f, 26f), "Unsecured loot lost. Returning to bunker...", _smallCenterStyle);
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
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.Lerp(criticalColor, healthyColor, normalized);
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(0f, (rect.width - 4f) * normalized), Mathf.Max(0f, rect.height - 4f)), Texture2D.whiteTexture);
            GUI.color = previous;
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

        private void EnsureStyles()
        {
            if (_textStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold };
            _textStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            _objectiveStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.72f, 0.82f, 0.76f, 1f) },
                wordWrap = true
            };
            _centerStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _smallCenterStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, alignment = TextAnchor.MiddleCenter };
        }
    }
}
