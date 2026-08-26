using System;
using System.Collections;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Extraction;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Missions;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.16 Field Ops presentation for the Dead City expedition.
    /// Replaces the legacy IMGUI PrototypeHud with a safe-area-aware landscape uGUI HUD while
    /// preserving all accepted 0.12 mission, sector, combat, hazard and extraction gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Production16FieldOpsUI : MonoBehaviour
    {
        private static bool _hooked;

        private readonly Color _white = new(0.96f, 0.985f, 1f, 1f);
        private readonly Color _muted = new(0.56f, 0.66f, 0.69f, 1f);
        private readonly Color _cyan = new(0.13f, 0.83f, 0.91f, 1f);
        private readonly Color _amber = new(1f, 0.43f, 0.08f, 1f);
        private readonly Color _green = new(0.15f, 0.91f, 0.51f, 1f);
        private readonly Color _danger = new(0.95f, 0.19f, 0.08f, 1f);
        private readonly Color _panel = new(0.012f, 0.021f, 0.024f, 0.88f);
        private readonly Color _panelStrong = new(0.008f, 0.014f, 0.017f, 0.95f);

        private Font _font;
        private CanvasScaler _scaler;
        private RectTransform _safeRoot;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private float _nextRefresh;

        private PlayerMotor _player;
        private Damageable _health;
        private HitscanWeapon _weapon;
        private ExtractionZone _extraction;

        private Text _levelText;
        private Text _sectorText;
        private Text _healthText;
        private Image _healthFill;
        private Text _lootText;
        private Text _abilityText;
        private Image _abilityFill;

        private Text _missionText;
        private Text _objectiveText;
        private Image _objectiveFill;
        private Text _secondaryText;
        private Text _navigationText;

        private Text _weaponText;
        private Text _weaponStatsText;
        private Text _threatText;

        private GameObject _alertRoot;
        private Text _alertText;
        private GameObject _hazardRoot;
        private Text _hazardText;
        private GameObject _bossRoot;
        private Text _bossText;
        private Image _bossFill;

        private GameObject _extractionRoot;
        private Text _extractionTitle;
        private Text _extractionDetail;
        private Image _extractionFill;

        private GameObject _resultRoot;
        private Text _resultTitle;
        private Text _resultDetail;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (_hooked)
                return;

            _hooked = true;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!string.Equals(scene.name, SceneFlowService.ExpeditionSceneName, StringComparison.Ordinal))
                return;

            if (FindFirstObjectByType<Production16FieldOpsUI>() != null)
                return;

            new GameObject("Production16_FieldOps").AddComponent<Production16FieldOpsUI>();
        }

        private IEnumerator Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            yield return null;

            DisableLegacyHud();
            ResolveRuntimeReferences();
            BuildInterface();
            Refresh(true);
        }

        private void Update()
        {
            ApplySafeArea();
            if (Time.unscaledTime < _nextRefresh)
                return;

            _nextRefresh = Time.unscaledTime + 0.075f;
            Refresh(false);
        }

        private static void DisableLegacyHud()
        {
            var legacy = FindFirstObjectByType<PrototypeHud>();
            if (legacy != null)
                legacy.enabled = false;
        }

        private void ResolveRuntimeReferences()
        {
            if (_player == null)
                _player = FindFirstObjectByType<PlayerMotor>();
            if (_player != null)
            {
                if (_health == null)
                    _health = _player.GetComponent<Damageable>();
                if (_weapon == null)
                    _weapon = _player.GetComponent<HitscanWeapon>();
            }
            if (_extraction == null)
                _extraction = FindFirstObjectByType<ExtractionZone>();
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject("P16_FieldOps_Canvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 125;
            canvas.pixelPerfect = true;

            _scaler = canvasObject.AddComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = new Vector2(1440f, 810f);
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _scaler.matchWidthOrHeight = 0.52f;
            _scaler.dynamicPixelsPerUnit = 2f;

            _safeRoot = CreateRect("SafeArea", canvasObject.transform);
            Fill(_safeRoot);
            ApplySafeArea(true);

            BuildHeader();
            BuildLeftStatus();
            BuildMissionPanel();
            BuildWeaponPanel();
            BuildAlerts();
            BuildBossBar();
            BuildExtractionPanel();
            BuildResultPanel();
        }

        private void BuildHeader()
        {
            var line = CreateImage("HeaderLine", _safeRoot, new Color(_cyan.r, _cyan.g, _cyan.b, 0.72f));
            Place(line.rectTransform, 0.018f, 0.955f, 0.982f, 0.958f);

            var brand = CreateText("Brand", _safeRoot, "DEADREACH // FIELD OPS", 17, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(brand.rectTransform, 0.022f, 0.958f, 0.34f, 0.995f);

            _levelText = CreateText("Level", _safeRoot, "LEVEL 01", 15, FontStyle.Bold, _muted, TextAnchor.MiddleRight);
            Place(_levelText.rectTransform, 0.72f, 0.958f, 0.978f, 0.995f);
        }

        private void BuildLeftStatus()
        {
            var panel = CreatePanel("StatusPanel", 0.022f, 0.705f, 0.298f, 0.94f);
            var accent = CreateImage("Accent", panel.transform, _cyan);
            Place(accent.rectTransform, 0f, 0f, 0.012f, 1f);

            _sectorText = CreateText("Sector", panel.transform, "SECTOR", 14, FontStyle.Bold, _cyan, TextAnchor.UpperLeft);
            Place(_sectorText.rectTransform, 0.055f, 0.78f, 0.95f, 0.96f);

            _healthText = CreateText("Vitals", panel.transform, "VITALS 100 / 100", 16, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(_healthText.rectTransform, 0.055f, 0.57f, 0.95f, 0.76f);
            CreateBar(panel.transform, "Health", 0.055f, 0.49f, 0.95f, 0.56f, _green, out _healthFill);

            _lootText = CreateText("Loot", panel.transform, "CARRIED 0 // WEAPON LOOT 0/0", 13, FontStyle.Normal, _muted, TextAnchor.MiddleLeft);
            Place(_lootText.rectTransform, 0.055f, 0.29f, 0.95f, 0.45f);

            _abilityText = CreateText("Ability", panel.transform, "ABILITY // READY", 13, FontStyle.Bold, _cyan, TextAnchor.MiddleLeft);
            Place(_abilityText.rectTransform, 0.055f, 0.10f, 0.95f, 0.26f);
            CreateBar(panel.transform, "Ability", 0.055f, 0.045f, 0.95f, 0.09f, _cyan, out _abilityFill);
        }

        private void BuildMissionPanel()
        {
            var panel = CreatePanel("MissionPanel", 0.318f, 0.705f, 0.682f, 0.94f);
            var accent = CreateImage("Accent", panel.transform, _amber);
            Place(accent.rectTransform, 0f, 0f, 0.009f, 1f);

            _missionText = CreateText("Mission", panel.transform, "MISSION // INITIALIZING", 15, FontStyle.Bold, _amber, TextAnchor.UpperLeft);
            Place(_missionText.rectTransform, 0.04f, 0.78f, 0.96f, 0.96f);

            _objectiveText = CreateText("Objective", panel.transform, "ADVANCE OBJECTIVE", 16, FontStyle.Bold, _white, TextAnchor.MiddleLeft);
            Place(_objectiveText.rectTransform, 0.04f, 0.50f, 0.96f, 0.78f);
            CreateBar(panel.transform, "Objective", 0.04f, 0.44f, 0.96f, 0.49f, _cyan, out _objectiveFill);

            _secondaryText = CreateText("Secondary", panel.transform, "OPTIONAL CACHE // LOCKED", 12, FontStyle.Normal, _muted, TextAnchor.MiddleLeft);
            Place(_secondaryText.rectTransform, 0.04f, 0.24f, 0.96f, 0.41f);

            _navigationText = CreateText("Navigation", panel.transform, "NAV // ACQUIRING", 14, FontStyle.Bold, _cyan, TextAnchor.MiddleLeft);
            Place(_navigationText.rectTransform, 0.04f, 0.04f, 0.96f, 0.22f);
        }

        private void BuildWeaponPanel()
        {
            var panel = CreatePanel("WeaponPanel", 0.702f, 0.705f, 0.978f, 0.94f);
            var accent = CreateImage("Accent", panel.transform, _green);
            Place(accent.rectTransform, 0.988f, 0f, 1f, 1f);

            _threatText = CreateText("Threat", panel.transform, "THREAT // ELEVATED", 14, FontStyle.Bold, _amber, TextAnchor.UpperRight);
            Place(_threatText.rectTransform, 0.04f, 0.78f, 0.94f, 0.96f);

            _weaponText = CreateText("Weapon", panel.transform, "PRIMARY // DR-7", 16, FontStyle.Bold, _white, TextAnchor.MiddleRight);
            Place(_weaponText.rectTransform, 0.04f, 0.48f, 0.94f, 0.77f);

            _weaponStatsText = CreateText("WeaponStats", panel.transform, "DMG -- // CRIT --", 13, FontStyle.Normal, _muted, TextAnchor.MiddleRight);
            Place(_weaponStatsText.rectTransform, 0.04f, 0.24f, 0.94f, 0.48f);

            var hint = CreateText("Hint", panel.transform, "AIM / FIRE // STAY MOBILE", 12, FontStyle.Bold, _green, TextAnchor.LowerRight);
            Place(hint.rectTransform, 0.04f, 0.05f, 0.94f, 0.23f);
        }

        private void BuildAlerts()
        {
            _alertRoot = CreatePanel("MissionAlert", 0.355f, 0.615f, 0.645f, 0.685f, _panelStrong).gameObject;
            _alertText = CreateText("Text", _alertRoot.transform, "MISSION UPDATE", 16, FontStyle.Bold, _white, TextAnchor.MiddleCenter);
            Fill(_alertText.rectTransform, 12f, 5f);
            _alertRoot.SetActive(false);

            _hazardRoot = CreatePanel("HazardAlert", 0.355f, 0.545f, 0.645f, 0.607f, new Color(0.26f, 0.035f, 0.012f, 0.96f)).gameObject;
            _hazardText = CreateText("Text", _hazardRoot.transform, "HAZARD", 15, FontStyle.Bold, new Color(1f, 0.76f, 0.36f), TextAnchor.MiddleCenter);
            Fill(_hazardText.rectTransform, 12f, 4f);
            _hazardRoot.SetActive(false);
        }

        private void BuildBossBar()
        {
            _bossRoot = CreatePanel("BossBar", 0.31f, 0.885f, 0.69f, 0.952f, _panelStrong).gameObject;
            _bossText = CreateText("BossText", _bossRoot.transform, "MUTATION TARGET", 14, FontStyle.Bold, _danger, TextAnchor.UpperCenter);
            Place(_bossText.rectTransform, 0.03f, 0.49f, 0.97f, 0.96f);
            CreateBar(_bossRoot.transform, "BossHealth", 0.04f, 0.16f, 0.96f, 0.34f, _danger, out _bossFill);
            _bossRoot.SetActive(false);
        }

        private void BuildExtractionPanel()
        {
            var panel = CreatePanel("Extraction", 0.34f, 0.19f, 0.66f, 0.31f, _panelStrong);
            _extractionRoot = panel.gameObject;
            _extractionTitle = CreateText("Title", panel.transform, "EXTRACTION", 18, FontStyle.Bold, _green, TextAnchor.UpperCenter);
            Place(_extractionTitle.rectTransform, 0.04f, 0.58f, 0.96f, 0.94f);
            _extractionDetail = CreateText("Detail", panel.transform, "HOLD POSITION", 13, FontStyle.Bold, _white, TextAnchor.MiddleCenter);
            Place(_extractionDetail.rectTransform, 0.04f, 0.31f, 0.96f, 0.58f);
            CreateBar(panel.transform, "ExtractionProgress", 0.08f, 0.13f, 0.92f, 0.25f, _green, out _extractionFill);
            _extractionRoot.SetActive(false);
        }

        private void BuildResultPanel()
        {
            var panel = CreatePanel("RunResult", 0.31f, 0.39f, 0.69f, 0.59f, new Color(0.005f, 0.009f, 0.011f, 0.985f));
            _resultRoot = panel.gameObject;
            _resultTitle = CreateText("Title", panel.transform, "EXTRACTION SECURED", 27, FontStyle.Bold, _green, TextAnchor.MiddleCenter);
            Place(_resultTitle.rectTransform, 0.04f, 0.50f, 0.96f, 0.90f);
            _resultDetail = CreateText("Detail", panel.transform, "RUN COMPLETE", 15, FontStyle.Bold, _muted, TextAnchor.MiddleCenter);
            Place(_resultDetail.rectTransform, 0.04f, 0.15f, 0.96f, 0.52f);
            _resultRoot.SetActive(false);
        }

        private void Refresh(bool forceReferences)
        {
            if (forceReferences || _player == null || _health == null || _weapon == null || _extraction == null)
                ResolveRuntimeReferences();

            if (_safeRoot == null)
                return;

            var session = RunSession.Current;
            var profile = SaveService.Data;
            var mission = ExpeditionDirector.Current;
            var sector = SectorDirector.Current;
            var difficulty = RunDifficultyDirector.Current;
            var inventory = RunInventory.Current;
            var ability = OperatorAbilityController.Current;

            var level = session != null ? session.RunLevel : profile.selectedLevel;
            _levelText.text = $"LEVEL {level:00} // {(difficulty != null ? difficulty.ZoneName : RunDifficultyDirector.GetZoneName(level))}";

            if (sector != null)
            {
                _sectorText.text = sector.PlayerInHazard
                    ? $"{sector.SectorName} // {sector.ActiveHazardLabel}"
                    : $"{sector.SectorName} // {sector.HazardProfile}";
                _sectorText.color = sector.PlayerInHazard ? _danger : _cyan;
            }
            else
            {
                _sectorText.text = "DEAD CITY // FIELD SECTOR";
                _sectorText.color = _cyan;
            }

            var hp = _health != null ? Mathf.CeilToInt(_health.CurrentHealth) : 0;
            var maxHp = _health != null ? Mathf.CeilToInt(_health.MaxHealth) : 0;
            var hpN = _health != null ? _health.NormalizedHealth : 0f;
            _healthText.text = $"VITALS {hp} / {maxHp}";
            SetBar(_healthFill, hpN, hpN < 0.3f ? _danger : _green);

            _lootText.text = $"CARRIED {session?.CarriedScrap ?? 0} // WEAPON LOOT {inventory?.Weapons.Count ?? 0}/{inventory?.WeaponCapacity ?? 0}";

            if (ability != null)
            {
                var ready = ability.IsReady;
                _abilityText.text = ready
                    ? $"{ability.AbilityName.ToUpperInvariant()} // READY"
                    : $"{ability.AbilityName.ToUpperInvariant()} // {ability.CooldownRemaining:0.0}s";
                _abilityText.color = ready ? _cyan : _muted;
                var progress = ready || ability.CooldownDuration <= 0.01f
                    ? 1f
                    : 1f - ability.CooldownRemaining / ability.CooldownDuration;
                SetBar(_abilityFill, progress, ready ? _cyan : _muted);
            }
            else
            {
                _abilityText.text = "ABILITY // INITIALIZING";
                SetBar(_abilityFill, 0f, _muted);
            }

            RefreshMission(mission, difficulty);
            RefreshWeapon(mission);
            RefreshNavigation(mission);
            RefreshAlerts(mission, sector);
            RefreshBoss(difficulty);
            RefreshExtraction(session, mission);
            RefreshResult(session);
        }

        private void RefreshMission(ExpeditionDirector mission, RunDifficultyDirector difficulty)
        {
            if (mission == null)
            {
                _missionText.text = "MISSION // INITIALIZING";
                _objectiveText.text = "ACQUIRING FIELD DIRECTIVE";
                _secondaryText.text = "OPTIONAL CACHE // STANDBY";
                SetBar(_objectiveFill, 0f, _cyan);
                return;
            }

            _missionText.text = $"MISSION // {mission.MissionName} // THREAT {mission.ThreatLabel}";
            _objectiveText.text = mission.PrimaryObjectiveText;
            _secondaryText.text = mission.SecondaryObjectiveText;

            var progress = mission.PrimaryComplete ? 1f : mission.PrimaryProgressNormalized;
            SetBar(_objectiveFill, progress, mission.PrimaryComplete ? _green : _cyan);

            if (mission.PrimaryComplete)
                _objectiveText.color = _green;
            else if (difficulty != null && difficulty.IsBossLevel)
                _objectiveText.color = _danger;
            else
                _objectiveText.color = _white;
        }

        private void RefreshWeapon(ExpeditionDirector mission)
        {
            if (_weapon == null)
            {
                _weaponText.text = "PRIMARY // FIELD DR-7";
                _weaponStatsText.text = "DMG -- // CRIT --";
            }
            else
            {
                var instance = _weapon.EquippedInstance;
                var stats = _weapon.RuntimeStats;
                var displayName = instance != null ? instance.displayNameSnapshot : "DR-7";
                var power = instance != null ? $"PWR {instance.itemPower}" : "BASE";
                _weaponText.text = $"PRIMARY // {displayName} // {power}";
                _weaponStatsText.text = $"DMG {stats.Damage:0.#} // CRIT {stats.CritChance * 100f:0.#}% // RNG {stats.Range:0}m";
            }

            var threat = mission?.ThreatLabel ?? "ELEVATED";
            _threatText.text = $"THREAT // {threat}";
            _threatText.color = threat is "HIGH" or "SEVERE" or "HIGH-RISK" ? _danger : _amber;
        }

        private void RefreshNavigation(ExpeditionDirector mission)
        {
            if (_player == null)
            {
                _navigationText.text = "NAV // ACQUIRING";
                return;
            }

            Transform target = null;
            var label = "OBJECTIVE";

            if (mission != null && mission.PrimaryComplete && _extraction != null)
            {
                target = _extraction.transform;
                label = "EXTRACT";
            }
            else
            {
                var markers = FindObjectsByType<ExpeditionObjectiveMarker>(FindObjectsSortMode.None);
                foreach (var marker in markers)
                {
                    if (marker == null || marker.name.Contains("BlackCache", StringComparison.OrdinalIgnoreCase))
                        continue;
                    target = marker.transform;
                    break;
                }
            }

            if (target == null)
            {
                _navigationText.text = "NAV // SIGNAL SEARCH";
                return;
            }

            var delta = target.position - _player.transform.position;
            delta.y = 0f;
            var distance = delta.magnitude;
            var direction = GetScreenDirection(target.position);
            _navigationText.text = distance < 2.2f
                ? $"NAV // {label} // ON SITE"
                : $"NAV // {label} // {distance:0}m // {direction}";
            _navigationText.color = label == "EXTRACT" ? _green : _cyan;
        }

        private string GetScreenDirection(Vector3 worldPosition)
        {
            var camera = Camera.main;
            if (camera == null)
                return "AHEAD";

            var viewport = camera.WorldToViewportPoint(worldPosition);
            var x = viewport.z < 0f ? 1f - viewport.x : viewport.x;
            if (x < 0.42f)
                return "LEFT";
            if (x > 0.58f)
                return "RIGHT";
            return "AHEAD";
        }

        private void RefreshAlerts(ExpeditionDirector mission, SectorDirector sector)
        {
            var missionAlert = mission != null && mission.HasActiveAlert;
            _alertRoot.SetActive(missionAlert);
            if (missionAlert)
                _alertText.text = mission.AlertText;

            var hazard = sector != null && sector.PlayerInHazard;
            _hazardRoot.SetActive(hazard);
            if (hazard)
                _hazardText.text = $"HAZARD // {sector.ActiveHazardLabel} // MOVE CLEAR";
        }

        private void RefreshBoss(RunDifficultyDirector difficulty)
        {
            var active = difficulty != null && difficulty.IsBossLevel && !difficulty.BossGateCleared;
            _bossRoot.SetActive(active);
            if (!active)
                return;

            var tier = Mathf.Clamp(Mathf.Max(1, difficulty.Level / 10), 1, 5);
            _bossText.text = $"MUTATION TARGET // TIER {tier}";
            SetBar(_bossFill, difficulty.BossHealthNormalized, _danger);
        }

        private void RefreshExtraction(RunSession session, ExpeditionDirector mission)
        {
            var active = session != null && session.IsInExtractionZone && !session.IsCompleted && !session.IsFailed;
            _extractionRoot.SetActive(active);
            if (!active)
                return;

            if (session.ExtractionBlockedByMission)
            {
                _extractionTitle.text = "EXTRACTION SEALED";
                _extractionTitle.color = _danger;
                _extractionDetail.text = mission != null ? mission.PrimaryObjectiveText : "PRIMARY OBJECTIVE INCOMPLETE";
                SetBar(_extractionFill, 0f, _danger);
            }
            else if (session.ExtractionBlockedByBoss)
            {
                _extractionTitle.text = "EXTRACTION SEALED";
                _extractionTitle.color = _danger;
                _extractionDetail.text = "MUTATION TARGET STILL ACTIVE";
                SetBar(_extractionFill, 0f, _danger);
            }
            else if (session.ExtractionBlockedByNoLoot)
            {
                _extractionTitle.text = "EXTRACTION LOCKED";
                _extractionTitle.color = _amber;
                _extractionDetail.text = "COLLECT SCRAP OR WEAPON LOOT";
                SetBar(_extractionFill, 0f, _amber);
            }
            else
            {
                _extractionTitle.text = "EXTRACTING // HOLD POSITION";
                _extractionTitle.color = _green;
                _extractionDetail.text = $"TRANSFER {session.ExtractionProgress * 100f:0}%";
                SetBar(_extractionFill, session.ExtractionProgress, _green);
            }
        }

        private void RefreshResult(RunSession session)
        {
            var active = session != null && (session.IsCompleted || session.IsFailed);
            _resultRoot.SetActive(active);
            if (!active)
                return;

            if (session.IsCompleted)
            {
                _resultTitle.text = "EXTRACTION SECURED";
                _resultTitle.color = _green;
                _resultDetail.text = $"SCRAP SECURED // {session.CarriedScrap} // RETURNING TO BUNKER";
            }
            else
            {
                _resultTitle.text = "RUN LOST";
                _resultTitle.color = _danger;
                _resultDetail.text = "FIELD PACKAGE LOST // RETURNING TO BUNKER";
            }
        }

        private RectTransform CreatePanel(string name, float x0, float y0, float x1, float y1, Color? color = null)
        {
            var image = CreateImage(name, _safeRoot, color ?? _panel);
            Place(image.rectTransform, x0, y0, x1, y1);
            return image.rectTransform;
        }

        private Image CreateImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateText(string name, Transform parent, string value, int fontSize, FontStyle style, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private void CreateBar(Transform parent, string name, float x0, float y0, float x1, float y1, Color color, out Image fill)
        {
            var background = CreateImage(name + "_Background", parent, new Color(0.12f, 0.16f, 0.17f, 0.86f));
            Place(background.rectTransform, x0, y0, x1, y1);

            fill = CreateImage(name + "_Fill", background.transform, color);
            Fill(fill.rectTransform);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 1f;
        }

        private static void SetBar(Image image, float normalized, Color color)
        {
            if (image == null)
                return;
            image.fillAmount = Mathf.Clamp01(normalized);
            image.color = color;
        }

        private void ApplySafeArea(bool force = false)
        {
            if (_safeRoot == null || _scaler == null || Screen.width <= 0 || Screen.height <= 0)
                return;

            var safe = Screen.safeArea;
            var size = new Vector2Int(Screen.width, Screen.height);
            if (!force && _lastSafeArea == safe && _lastScreenSize == size)
                return;

            _lastSafeArea = safe;
            _lastScreenSize = size;
            _scaler.matchWidthOrHeight = safe.height > 0f && safe.width / safe.height >= 2.15f ? 0.62f : 0.52f;

            _safeRoot.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            _safeRoot.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            _safeRoot.offsetMin = Vector2.zero;
            _safeRoot.offsetMax = Vector2.zero;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void Place(RectTransform rect, float x0, float y0, float x1, float y1)
        {
            rect.anchorMin = new Vector2(x0, y0);
            rect.anchorMax = new Vector2(x1, y1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Fill(RectTransform rect, float horizontal = 0f, float vertical = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontal, vertical);
            rect.offsetMax = new Vector2(-horizontal, -vertical);
        }
    }
}
