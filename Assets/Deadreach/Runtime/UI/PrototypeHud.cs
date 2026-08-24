using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Missions;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.World;
using UnityEngine;
using UnityEngine.InputSystem;

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
        private int _styleKey;

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
            var safe = Screen.safeArea;
            var mobile = Application.isMobilePlatform || Touchscreen.current != null;
            var scale = mobile ? Mathf.Clamp(safe.height / 844f, 1f, 1.45f) : 1f;
            EnsureStyles(mobile, scale);

            var left = safe.x + (mobile ? 24f * scale : 18f);
            var top = Screen.height - safe.yMax + (mobile ? 20f * scale : 14f);
            var width = mobile
                ? Mathf.Min(620f * scale, safe.width * 0.43f)
                : Mathf.Min(420f, safe.width * 0.36f);
            var session = RunSession.Current;
            var profile = SaveService.Data;
            var inventory = RunInventory.Current;
            var director = RunDifficultyDirector.Current;
            var mission = ExpeditionDirector.Current;
            var sector = SectorDirector.Current;

            DrawDamageFlash(safe);

            var panelHeight = mobile ? 294f * scale : 286f;
            GUI.Box(new Rect(left, top, width, panelHeight), GUIContent.none);
            GUI.Label(new Rect(left + 14f * scale, top + 8f * scale, width - 28f * scale, 29f * scale), "DEADREACH // FIELD OPS", _titleStyle);
            GUI.Label(new Rect(left + 14f * scale, top + 38f * scale, width - 28f * scale, 24f * scale),
                $"LEVEL {session?.RunLevel ?? profile.selectedLevel:00} // {(director != null ? director.ZoneName : RunDifficultyDirector.GetZoneName(profile.selectedLevel))}", _textStyle);

            var sectorText = sector == null
                ? "SECTOR // LEGACY DEAD CITY"
                : sector.PlayerInHazard
                    ? $"SECTOR // {sector.SectorName} // DANGER {sector.ActiveHazardLabel}"
                    : $"SECTOR // {sector.SectorName} // {sector.HazardProfile}";
            GUI.Label(new Rect(left + 14f * scale, top + 64f * scale, width - 28f * scale, 24f * scale), sectorText, _textStyle);

            var hp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.CurrentHealth) : 0;
            var maxHp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.MaxHealth) : 0;
            var healthNormalized = _playerHealth != null ? _playerHealth.NormalizedHealth : 0f;

            GUI.Label(new Rect(left + 14f * scale, top + 92f * scale, width - 28f * scale, 24f * scale), $"VITALS   {hp}/{maxHp}", _textStyle);
            DrawBar(new Rect(left + 14f * scale, top + 118f * scale, width - 28f * scale, (mobile ? 18f : 10f) * scale), healthNormalized,
                new Color(0.22f, 0.78f, 0.36f), new Color(0.75f, 0.15f, 0.12f));

            DrawPrimaryWeapon(left + 14f * scale, top + 144f * scale, width - 28f * scale, scale);
            GUI.Label(new Rect(left + 14f * scale, top + 176f * scale, width - 28f * scale, 24f * scale),
                $"CARRIED {session?.CarriedScrap ?? 0}   //   WEAPON LOOT {inventory?.Weapons.Count ?? 0}/{inventory?.WeaponCapacity ?? 0}", _textStyle);

            if (mission != null)
            {
                GUI.Label(new Rect(left + 14f * scale, top + 203f * scale, width - 28f * scale, 24f * scale),
                    $"MISSION // {mission.MissionName} // THREAT {mission.ThreatLabel}", _textStyle);

                var objectiveText = mission.PrimaryComplete && !mission.SecondaryComplete
                    ? $"{mission.SecondaryObjectiveText} // EXTRACT AVAILABLE"
                    : mission.PrimaryObjectiveText;
                GUI.Label(new Rect(left + 14f * scale, top + 229f * scale, width - 28f * scale, 34f * scale), objectiveText, _objectiveStyle);

                var objectiveProgress = mission.PrimaryComplete && !mission.SecondaryComplete
                    ? mission.SecondaryProgressNormalized
                    : mission.PrimaryProgressNormalized;
                var objectiveColor = mission.PrimaryComplete
                    ? new Color(1f, 0.62f, 0.08f)
                    : new Color(0.12f, 0.82f, 1f);
                DrawBar(new Rect(left + 14f * scale, top + 268f * scale, width - 28f * scale, (mobile ? 14f : 9f) * scale),
                    objectiveProgress, objectiveColor, new Color(0.2f, 0.22f, 0.22f));
            }
            else
            {
                GUI.Label(new Rect(left + 14f * scale, top + 203f * scale, width - 28f * scale, 24f * scale),
                    $"SECURED {profile.securedScrap}   //   STREAK {profile.currentExtractionStreak}", _textStyle);
                GUI.Label(new Rect(left + 14f * scale, top + 231f * scale, width - 28f * scale, 38f * scale),
                    director != null && director.IsBossLevel && !director.BossGateCleared
                        ? "OBJECTIVE // ELIMINATE MUTATION TARGET"
                        : "OBJECTIVE // REACH EXTRACTION WITH LOOT", _objectiveStyle);
            }

            if (!mobile)
            {
                GUI.Label(new Rect(left, safe.yMax - 34f, Mathf.Min(620f, safe.width * 0.58f), 24f),
                    "WASD = MOVE   •   MOUSE = AIM + FIRE", _textStyle);
            }

            if (director != null && director.IsBossLevel && !director.BossGateCleared)
                DrawBossBar(safe, director, mobile, scale);

            if (mission != null && mission.HasActiveAlert)
                DrawMissionAlert(safe, mission, director, mobile, scale);

            if (sector != null && sector.PlayerInHazard)
                DrawSectorHazardAlert(safe, sector, director, mission, mobile, scale);

            if (session == null)
                return;

            DrawExtractionFeedback(safe, session, mission, mobile, scale);
            DrawRunResult(safe, session, mobile, scale);
        }

        private void DrawPrimaryWeapon(float x, float y, float width, float scale)
        {
            if (_weapon == null)
            {
                GUI.Label(new Rect(x, y, width, 28f * scale), "PRIMARY // FIELD DR-7", _textStyle);
                return;
            }

            var instance = _weapon.EquippedInstance;
            var stats = _weapon.RuntimeStats;
            var name = instance != null ? instance.displayNameSnapshot : "DR-7";
            var power = instance != null ? $"PWR {instance.itemPower}" : "BASE";
            GUI.Label(new Rect(x, y, width, 28f * scale),
                $"PRIMARY // {name} // {power} // DMG {stats.Damage:0.#} // CRIT {stats.CritChance * 100f:0.#}%", _textStyle);
        }

        private void DrawBossBar(Rect safe, RunDifficultyDirector director, bool mobile, float scale)
        {
            var width = mobile
                ? Mathf.Min(760f * scale, safe.width * 0.56f)
                : Mathf.Min(610f, safe.width * 0.48f);
            var x = safe.center.x - width * 0.5f;
            var y = Screen.height - safe.yMax + (mobile ? 18f * scale : 12f);
            var tier = Mathf.Clamp(Mathf.Max(1, director.Level / 10), 1, 5);
            var height = mobile ? 76f * scale : 58f;

            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.Label(new Rect(x + 16f * scale, y + 5f * scale, width - 32f * scale, 30f * scale),
                $"MUTATION // TIER {tier} // {GetBossName(tier)}", _centerStyle);
            DrawBar(new Rect(x + 24f * scale, y + 42f * scale, width - 48f * scale, (mobile ? 18f : 12f) * scale),
                director.BossHealthNormalized, new Color(0.92f, 0.2f, 0.08f), new Color(0.35f, 0.02f, 0.01f));
        }

        private void DrawMissionAlert(Rect safe, ExpeditionDirector mission, RunDifficultyDirector director, bool mobile, float scale)
        {
            var width = mobile ? Mathf.Min(390f * scale, safe.width * 0.42f) : Mathf.Min(420f, safe.width * 0.36f);
            var height = mobile ? 52f * scale : 42f;
            var x = safe.center.x - width * 0.5f;
            var bossOffset = director != null && director.IsBossLevel && !director.BossGateCleared ? (mobile ? 88f * scale : 68f) : 0f;
            var y = Screen.height - safe.yMax + 14f * scale + bossOffset;

            var previous = GUI.color;
            GUI.color = new Color(0.04f, 0.055f, 0.06f, 0.92f);
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.color = previous;
            GUI.Label(new Rect(x + 10f * scale, y + 5f * scale, width - 20f * scale, height - 10f * scale), mission.AlertText, _centerStyle);
        }

        private void DrawSectorHazardAlert(Rect safe, SectorDirector sector, RunDifficultyDirector director, ExpeditionDirector mission, bool mobile, float scale)
        {
            var width = mobile ? Mathf.Min(390f * scale, safe.width * 0.42f) : Mathf.Min(420f, safe.width * 0.36f);
            var height = mobile ? 48f * scale : 38f;
            var x = safe.center.x - width * 0.5f;
            var bossOffset = director != null && director.IsBossLevel && !director.BossGateCleared ? (mobile ? 88f * scale : 68f) : 0f;
            var missionOffset = mission != null && mission.HasActiveAlert ? (mobile ? 58f * scale : 48f) : 0f;
            var y = Screen.height - safe.yMax + 14f * scale + bossOffset + missionOffset;

            var previous = GUI.color;
            GUI.color = new Color(0.35f, 0.055f, 0.02f, 0.94f);
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.color = previous;
            GUI.Label(new Rect(x + 10f * scale, y + 4f * scale, width - 20f * scale, height - 8f * scale),
                $"HAZARD // {sector.ActiveHazardLabel} // MOVE CLEAR", _centerStyle);
        }

        private void DrawExtractionFeedback(Rect safe, RunSession session, ExpeditionDirector mission, bool mobile, float scale)
        {
            if (!session.IsInExtractionZone || session.IsCompleted || session.IsFailed)
                return;

            var width = mobile
                ? Mathf.Min(620f * scale, safe.width - 48f * scale)
                : Mathf.Min(440f, safe.width - 40f);
            var x = safe.center.x - width * 0.5f;
            var y = Screen.height - safe.yMax + (mobile ? 104f * scale : 82f);
            var height = mobile ? 104f * scale : 82f;

            GUI.Box(new Rect(x, y, width, height), GUIContent.none);

            if (session.ExtractionBlockedByMission)
            {
                GUI.Label(new Rect(x + 16f * scale, y + 8f * scale, width - 32f * scale, 32f * scale), "EXTRACTION SEALED", _centerStyle);
                GUI.Label(new Rect(x + 16f * scale, y + 47f * scale, width - 32f * scale, 32f * scale),
                    mission != null ? mission.PrimaryObjectiveText : "Primary objective incomplete.", _smallCenterStyle);
                return;
            }

            if (session.ExtractionBlockedByBoss)
            {
                GUI.Label(new Rect(x + 16f * scale, y + 8f * scale, width - 32f * scale, 32f * scale), "EXTRACTION SEALED", _centerStyle);
                GUI.Label(new Rect(x + 16f * scale, y + 47f * scale, width - 32f * scale, 26f * scale), "Mutation target still active.", _smallCenterStyle);
                return;
            }

            if (session.ExtractionBlockedByNoLoot)
            {
                GUI.Label(new Rect(x + 16f * scale, y + 8f * scale, width - 32f * scale, 32f * scale), "EXTRACTION LOCKED", _centerStyle);
                GUI.Label(new Rect(x + 16f * scale, y + 47f * scale, width - 32f * scale, 26f * scale), "Collect Scrap or weapon loot first.", _smallCenterStyle);
                return;
            }

            GUI.Label(new Rect(x + 16f * scale, y + 8f * scale, width - 32f * scale, 30f * scale), "EXTRACTING // HOLD POSITION", _centerStyle);
            DrawBar(new Rect(x + 24f * scale, y + 58f * scale, width - 48f * scale, (mobile ? 20f : 15f) * scale),
                session.ExtractionProgress, new Color(0.12f, 0.92f, 0.48f), new Color(0.12f, 0.92f, 0.48f));
        }

        private void DrawRunResult(Rect safe, RunSession session, bool mobile, float scale)
        {
            if (!session.IsCompleted && !session.IsFailed)
                return;

            var width = mobile ? Mathf.Min(620f * scale, safe.width - 60f * scale) : 440f;
            var height = mobile ? 166f * scale : 132f;
            GUI.Box(new Rect(safe.center.x - width * 0.5f, safe.center.y - height * 0.5f, width, height), GUIContent.none);

            if (session.IsCompleted)
            {
                GUI.Label(new Rect(safe.center.x - width * 0.45f, safe.center.y - 50f * scale, width * 0.9f, 42f * scale), "EXTRACTION SECURED", _centerStyle);
                GUI.Label(new Rect(safe.center.x - width * 0.45f, safe.center.y + 4f * scale, width * 0.9f, 32f * scale), "Loot banked. Returning to bunker...", _smallCenterStyle);
            }
            else
            {
                GUI.Label(new Rect(safe.center.x - width * 0.45f, safe.center.y - 50f * scale, width * 0.9f, 42f * scale), "RUN LOST", _centerStyle);
                GUI.Label(new Rect(safe.center.x - width * 0.45f, safe.center.y + 4f * scale, width * 0.9f, 32f * scale), "Unsecured loot lost. Returning to bunker...", _smallCenterStyle);
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
            GUI.color = new Color(0f, 0f, 0f, 0.68f);
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

        private void EnsureStyles(bool mobile, float scale)
        {
            var key = mobile ? Mathf.RoundToInt(scale * 100f) : 0;
            if (_textStyle != null && _styleKey == key)
                return;

            _styleKey = key;
            var titleSize = mobile ? Mathf.RoundToInt(22f * scale) : 17;
            var textSize = mobile ? Mathf.RoundToInt(16f * scale) : 12;
            var objectiveSize = mobile ? Mathf.RoundToInt(15f * scale) : 11;
            var centerSize = mobile ? Mathf.RoundToInt(21f * scale) : 17;
            var smallCenterSize = mobile ? Mathf.RoundToInt(16f * scale) : 13;

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = titleSize, fontStyle = FontStyle.Bold };
            _textStyle = new GUIStyle(GUI.skin.label) { fontSize = textSize };
            _objectiveStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = objectiveSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.72f, 0.82f, 0.76f, 1f) },
                wordWrap = true
            };
            _centerStyle = new GUIStyle(GUI.skin.label) { fontSize = centerSize, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _smallCenterStyle = new GUIStyle(GUI.skin.label) { fontSize = smallCenterSize, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        }
    }
}
