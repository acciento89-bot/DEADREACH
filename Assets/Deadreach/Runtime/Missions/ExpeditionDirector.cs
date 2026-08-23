using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Presentation;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Missions
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionDirector : MonoBehaviour
    {
        private static readonly Vector3[] ObjectivePoints =
        {
            new(-4.0f, 0.12f, 2.2f),
            new(4.0f, 0.12f, 7.6f),
            new(-4.0f, 0.12f, 13.2f),
            new(3.6f, 0.12f, 15.2f),
            new(0f, 0.12f, 5.0f)
        };

        private static readonly Vector3[] ReinforcementPoints =
        {
            new(-5.2f, 1f, -4.2f),
            new(5.2f, 1f, 0.4f),
            new(-5.2f, 1f, 9.8f),
            new(5.2f, 1f, 15.6f),
            new(0f, 1f, 18.1f)
        };

        public static ExpeditionDirector Current { get; private set; }

        public event Action MissionChanged;
        public event Action PrimaryCompleted;
        public event Action SecondaryCompletedEvent;
        public event Action<string> AlertRaised;

        public ExpeditionMissionType MissionType { get; private set; }
        public ExpeditionMissionStage Stage { get; private set; }
        public int Level { get; private set; }
        public bool PrimaryComplete { get; private set; }
        public bool SecondaryAvailable { get; private set; }
        public bool SecondaryComplete { get; private set; }
        public float PrimaryProgressNormalized => Mathf.Clamp01(_primaryProgress);
        public float SecondaryProgressNormalized => Mathf.Clamp01(_secondaryProgress);
        public bool ExtractionAvailable => PrimaryComplete;
        public string AlertText => Time.unscaledTime < _alertUntil ? _alertText : string.Empty;
        public bool HasActiveAlert => !string.IsNullOrWhiteSpace(AlertText);
        public int PrimaryBonusScrap { get; private set; }
        public WeaponInstanceData SecondaryReward => _secondaryReward;

        public string MissionName => MissionType switch
        {
            ExpeditionMissionType.Recovery => "RECOVERY",
            ExpeditionMissionType.Purge => "PURGE",
            ExpeditionMissionType.Holdout => "HOLDOUT",
            _ => "BLACKSITE"
        };

        public string ThreatLabel
        {
            get
            {
                if (PrimaryComplete)
                    return SecondaryAvailable && !SecondaryComplete && _secondaryWaveTriggered ? "HIGH-RISK" : "LOW";

                return MissionType switch
                {
                    ExpeditionMissionType.Purge => "HIGH",
                    ExpeditionMissionType.Holdout when Stage == ExpeditionMissionStage.Hold => "SEVERE",
                    ExpeditionMissionType.Blacksite when Stage == ExpeditionMissionStage.Eliminate => "HIGH",
                    _ => "ELEVATED"
                };
            }
        }

        public string PrimaryObjectiveText
        {
            get
            {
                if (PrimaryComplete)
                    return "PRIMARY COMPLETE // EXTRACTION AVAILABLE";

                return MissionType switch
                {
                    ExpeditionMissionType.Recovery => $"SECURE DATA CORE // {_primaryProgress * 100f:0}%",
                    ExpeditionMissionType.Purge => $"ELIMINATE INFECTED // {_primaryKills}/{_primaryKillTarget}",
                    ExpeditionMissionType.Holdout when Stage == ExpeditionMissionStage.Activate => $"ACTIVATE UPLINK // {_primaryProgress * 100f:0}%",
                    ExpeditionMissionType.Holdout => $"DEFEND UPLINK // {Mathf.Max(0f, _holdDuration - _holdElapsed):0}s",
                    ExpeditionMissionType.Blacksite when Stage == ExpeditionMissionStage.Activate => $"BREACH TERMINAL // {_stageInteractionProgress * 100f:0}%",
                    ExpeditionMissionType.Blacksite when Stage == ExpeditionMissionStage.Eliminate && _difficulty != null && _difficulty.IsBossLevel => "ELIMINATE MUTATION TARGET",
                    ExpeditionMissionType.Blacksite when Stage == ExpeditionMissionStage.Eliminate => $"CLEAR RESPONSE // {_stageKills}/{_stageKillTarget}",
                    ExpeditionMissionType.Blacksite => $"SECURE BLACKSITE CORE // {_stageInteractionProgress * 100f:0}%",
                    _ => "ADVANCE OBJECTIVE"
                };
            }
        }

        public string SecondaryObjectiveText
        {
            get
            {
                if (!PrimaryComplete)
                    return "OPTIONAL CACHE // LOCKED";
                if (SecondaryComplete)
                    return "OPTIONAL CACHE // SECURED";
                return $"OPTIONAL // SECURE BLACK CACHE // {_secondaryProgress * 100f:0}%";
            }
        }

        private readonly Dictionary<Damageable, Action> _deathHandlers = new();
        private readonly Dictionary<Damageable, bool> _bossFlags = new();
        private readonly List<Damageable> _trackedEnemies = new();

        private RunSession _session;
        private RunDifficultyDirector _difficulty;
        private PlayerMotor _player;
        private ExpeditionObjectiveMarker _primaryMarker;
        private ExpeditionObjectiveMarker _secondaryMarker;
        private WeaponInstanceData _secondaryReward;
        private bool _initialized;
        private float _primaryProgress;
        private float _secondaryProgress;
        private float _stageInteractionProgress;
        private float _holdElapsed;
        private float _holdDuration;
        private float _nextWaveTime;
        private bool _wasInsideHold;
        private int _primaryKills;
        private int _primaryKillTarget;
        private int _stageKills;
        private int _stageKillTarget;
        private int _spawnSerial;
        private bool _secondaryWaveTriggered;
        private string _alertText;
        private float _alertUntil;

        private const float InteractionRadius = 2.25f;
        private const float HoldRadius = 5.6f;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
        }

        private void Start()
        {
            InitializeMission();
        }

        private void InitializeMission()
        {
            _session = RunSession.Current;
            _difficulty = RunDifficultyDirector.Current;
            _player = FindFirstObjectByType<PlayerMotor>();
            if (_session == null || _player == null)
                return;

            Level = Mathf.Clamp(_session.RunLevel, 1, SaveService.MaxCampaignLevel);
            RegisterExistingEnemies();
            SelectMission();
            SetupMission();
            _initialized = true;
            RaiseAlert($"MISSION // {MissionName}", 2.2f);
            Debug.Log($"DEADREACH 0.11 expedition online // LEVEL {Level:00} // {MissionName} // extraction gated by primary objective.");
        }

        private void SelectMission()
        {
            if (_difficulty != null && _difficulty.IsBossLevel)
            {
                MissionType = ExpeditionMissionType.Blacksite;
                return;
            }

            var data = SaveService.Data;
            var rotation = Mathf.Abs(Level + data.successfulExtractions + data.failedRuns - 1) % 4;
            MissionType = (ExpeditionMissionType)rotation;
        }

        private void SetupMission()
        {
            PrimaryComplete = false;
            SecondaryAvailable = false;
            SecondaryComplete = false;
            _primaryProgress = 0f;
            _secondaryProgress = 0f;
            _stageInteractionProgress = 0f;

            switch (MissionType)
            {
                case ExpeditionMissionType.Recovery:
                    Stage = ExpeditionMissionStage.Recover;
                    _primaryMarker = CreateMarker("Mission_RecoveryCore", ObjectivePoints[2], new Color(0.12f, 0.82f, 1f, 0.95f));
                    break;

                case ExpeditionMissionType.Purge:
                    Stage = ExpeditionMissionStage.Eliminate;
                    var ordinaryCount = Mathf.Max(1, _bossFlags.Count(pair => !pair.Value));
                    _primaryKillTarget = Mathf.Clamp(4 + Level / 15, 4, ordinaryCount);
                    _primaryMarker = CreateMarker("Mission_PurgeBeacon", ObjectivePoints[4], new Color(1f, 0.22f, 0.08f, 0.95f), 2.35f);
                    break;

                case ExpeditionMissionType.Holdout:
                    Stage = ExpeditionMissionStage.Activate;
                    _holdDuration = Mathf.Clamp(34f + Level * 0.28f, 36f, 48f);
                    _primaryMarker = CreateMarker("Mission_Uplink", ObjectivePoints[1], new Color(1f, 0.72f, 0.12f, 0.95f), HoldRadius);
                    break;

                default:
                    Stage = ExpeditionMissionStage.Activate;
                    _stageKillTarget = _difficulty != null && _difficulty.IsBossLevel ? 1 : Mathf.Clamp(3 + Level / 25, 3, 5);
                    _primaryMarker = CreateMarker("Mission_BlacksiteTerminal", ObjectivePoints[0], new Color(0.74f, 0.2f, 1f, 0.95f));
                    break;
            }

            MissionChanged?.Invoke();
        }

        private void Update()
        {
            if (!_initialized || _session == null || _player == null || _session.IsCompleted || _session.IsFailed)
                return;

            if (!PrimaryComplete)
                UpdatePrimaryMission();
            else if (SecondaryAvailable && !SecondaryComplete)
                UpdateSecondaryObjective();
        }

        private void UpdatePrimaryMission()
        {
            switch (MissionType)
            {
                case ExpeditionMissionType.Recovery:
                    UpdateRecovery();
                    break;
                case ExpeditionMissionType.Purge:
                    UpdatePurge();
                    break;
                case ExpeditionMissionType.Holdout:
                    UpdateHoldout();
                    break;
                case ExpeditionMissionType.Blacksite:
                    UpdateBlacksite();
                    break;
            }
        }

        private void UpdateRecovery()
        {
            _primaryProgress = UpdateInteraction(_primaryMarker, _primaryProgress, 1.55f);
            if (_primaryProgress >= 0.999f)
                CompletePrimaryMission();
        }

        private void UpdatePurge()
        {
            _primaryProgress = _primaryKillTarget <= 0 ? 1f : Mathf.Clamp01(_primaryKills / (float)_primaryKillTarget);
            if (_primaryKills >= _primaryKillTarget)
                CompletePrimaryMission();
        }

        private void UpdateHoldout()
        {
            if (Stage == ExpeditionMissionStage.Activate)
            {
                _primaryProgress = UpdateInteraction(_primaryMarker, _primaryProgress, 1.35f);
                if (_primaryProgress < 0.999f)
                    return;

                Stage = ExpeditionMissionStage.Hold;
                _primaryProgress = 0f;
                _holdElapsed = 0f;
                _nextWaveTime = Time.time + 6.5f;
                RaiseAlert("UPLINK ONLINE // HOLD POSITION", 2.1f);
                SpawnReinforcementWave(2, "UPLINK RESPONSE");
                MissionChanged?.Invoke();
                return;
            }

            var inside = HorizontalDistance(_player.transform.position, _primaryMarker.WorldPosition) <= HoldRadius;
            if (inside)
            {
                _holdElapsed += Time.deltaTime;
                _primaryProgress = Mathf.Clamp01(_holdElapsed / _holdDuration);
            }

            if (inside != _wasInsideHold)
            {
                _wasInsideHold = inside;
                RaiseAlert(inside ? "UPLINK SIGNAL RESTORED" : "UPLINK SIGNAL LOST // RETURN TO ZONE", 1.4f);
            }

            if (Time.time >= _nextWaveTime && _holdElapsed < _holdDuration)
            {
                _nextWaveTime = Time.time + Mathf.Lerp(9.5f, 7.2f, Mathf.Clamp01(Level / 50f));
                SpawnReinforcementWave(Level >= 30 ? 3 : 2, "HOLDOUT REINFORCEMENTS");
            }

            if (_holdElapsed >= _holdDuration)
                CompletePrimaryMission();
        }

        private void UpdateBlacksite()
        {
            switch (Stage)
            {
                case ExpeditionMissionStage.Activate:
                    _stageInteractionProgress = UpdateInteraction(_primaryMarker, _stageInteractionProgress, 1.45f);
                    _primaryProgress = _stageInteractionProgress * 0.22f;
                    if (_stageInteractionProgress < 0.999f)
                        return;

                    Stage = ExpeditionMissionStage.Eliminate;
                    _stageInteractionProgress = 0f;
                    _stageKills = 0;
                    _primaryProgress = 0.22f;
                    RaiseAlert("BLACKSITE BREACHED // RESPONSE INBOUND", 2.2f);
                    if (_difficulty == null || !_difficulty.IsBossLevel)
                        SpawnReinforcementWave(_stageKillTarget, "BLACKSITE RESPONSE");
                    MissionChanged?.Invoke();
                    break;

                case ExpeditionMissionStage.Eliminate:
                {
                    var bossCleared = _difficulty != null && _difficulty.IsBossLevel && _difficulty.BossGateCleared;
                    var ordinaryCleared = (_difficulty == null || !_difficulty.IsBossLevel) && _stageKills >= _stageKillTarget;
                    var t = _difficulty != null && _difficulty.IsBossLevel
                        ? (_difficulty.BossGateCleared ? 1f : 1f - _difficulty.BossHealthNormalized)
                        : Mathf.Clamp01(_stageKills / (float)Mathf.Max(1, _stageKillTarget));
                    _primaryProgress = Mathf.Lerp(0.22f, 0.76f, t);
                    if (!bossCleared && !ordinaryCleared)
                        return;

                    Stage = ExpeditionMissionStage.Recover;
                    _primaryMarker.SetWorldPosition(ObjectivePoints[3]);
                    _primaryMarker.SetTheme(new Color(0.16f, 0.88f, 1f, 0.95f), 1.9f);
                    _stageInteractionProgress = 0f;
                    _primaryProgress = 0.76f;
                    RaiseAlert("VAULT OPEN // SECURE BLACKSITE CORE", 2f);
                    MissionChanged?.Invoke();
                    break;
                }

                case ExpeditionMissionStage.Recover:
                    _stageInteractionProgress = UpdateInteraction(_primaryMarker, _stageInteractionProgress, 1.6f);
                    _primaryProgress = Mathf.Lerp(0.76f, 1f, _stageInteractionProgress);
                    if (_stageInteractionProgress >= 0.999f)
                        CompletePrimaryMission();
                    break;
            }
        }

        private void CompletePrimaryMission()
        {
            if (PrimaryComplete)
                return;

            PrimaryComplete = true;
            Stage = ExpeditionMissionStage.Complete;
            _primaryProgress = 1f;
            PrimaryBonusScrap = 10 + Level + (MissionType == ExpeditionMissionType.Blacksite ? 10 : 0);
            _session.CollectScrap(PrimaryBonusScrap);
            _primaryMarker?.SetCompleted(true);

            SecondaryAvailable = true;
            var secondaryPoint = SelectSecondaryPoint();
            _secondaryMarker = CreateMarker("Mission_OptionalBlackCache", secondaryPoint, new Color(1f, 0.62f, 0.08f, 0.98f), 1.75f);
            RaiseAlert($"PRIMARY COMPLETE // +{PrimaryBonusScrap} SCRAP // EXTRACTION AVAILABLE", 2.8f);
            PrimaryCompleted?.Invoke();
            MissionChanged?.Invoke();
            Debug.Log($"DEADREACH 0.11 primary complete // {MissionName} // extraction unlocked // optional black cache active.");
        }

        private void UpdateSecondaryObjective()
        {
            if (_secondaryMarker == null)
                return;

            var distance = HorizontalDistance(_player.transform.position, _secondaryMarker.WorldPosition);
            if (!_secondaryWaveTriggered && distance <= 4.5f)
            {
                _secondaryWaveTriggered = true;
                SpawnReinforcementWave(Level >= 25 ? 3 : 2, "BLACK CACHE RESPONSE");
                RaiseAlert("OPTIONAL CACHE // HOSTILE RESPONSE", 2f);
            }

            _secondaryProgress = UpdateInteraction(_secondaryMarker, _secondaryProgress, 1.85f);
            if (_secondaryProgress < 0.999f)
                return;

            SecondaryComplete = true;
            _secondaryProgress = 1f;
            _secondaryMarker.SetCompleted(true);
            GrantSecondaryReward();
            RaiseAlert("BLACK CACHE SECURED // BONUS WEAPON RESERVED", 2.6f);
            SecondaryCompletedEvent?.Invoke();
            MissionChanged?.Invoke();
        }

        private void GrantSecondaryReward()
        {
            var seed = unchecked(Environment.TickCount ^ Level * 92821 ^ SaveService.Data.successfulExtractions * 7919);
            var depth = Mathf.Clamp01(Level / (float)SaveService.MaxCampaignLevel + 0.2f);
            var rarity = WeaponLootFactory.RollRarity(depth, seed);
            if ((int)rarity < (int)WeaponRarity.Uncommon)
                rarity = WeaponRarity.Uncommon;
            if (Level >= 25 && (int)rarity < (int)WeaponRarity.Rare)
                rarity = WeaponRarity.Rare;

            var reward = WeaponLootFactory.CreateFieldWeapon(rarity, seed);
            reward.itemPower += 10 + Level / 3;
            reward.displayNameSnapshot = $"BLACK CACHE // {reward.displayNameSnapshot}";
            _secondaryReward = reward.Clone();
            _session.GrantMissionReward(reward);
        }

        private float UpdateInteraction(ExpeditionObjectiveMarker marker, float progress, float requiredSeconds)
        {
            if (marker == null)
                return progress;

            var inside = HorizontalDistance(_player.transform.position, marker.WorldPosition) <= InteractionRadius;
            if (inside)
                progress += Time.deltaTime / Mathf.Max(0.1f, requiredSeconds);
            else
                progress = Mathf.MoveTowards(progress, 0f, Time.deltaTime * 0.42f);

            return Mathf.Clamp01(progress);
        }

        private ExpeditionObjectiveMarker CreateMarker(string objectName, Vector3 position, Color color, float radius = 1.85f)
        {
            var root = new GameObject(objectName);
            var marker = root.AddComponent<ExpeditionObjectiveMarker>();
            marker.Initialize(position, color, radius);
            return marker;
        }

        private Vector3 SelectSecondaryPoint()
        {
            var primaryPosition = _primaryMarker != null ? _primaryMarker.WorldPosition : _player.transform.position;
            return ObjectivePoints
                .OrderByDescending(point => HorizontalDistance(point, primaryPosition) + HorizontalDistance(point, new Vector3(0f, 0f, 20f)) * 0.35f)
                .First();
        }

        private void RegisterExistingEnemies()
        {
            var infected = FindObjectsByType<InfectedChaser>(FindObjectsSortMode.None)
                .OrderBy(item => item.name)
                .ToArray();

            foreach (var enemy in infected)
            {
                if (enemy == null)
                    continue;
                var health = enemy.GetComponent<Damageable>();
                if (health != null)
                    RegisterEnemy(health, enemy.name.StartsWith("BOSS_", StringComparison.Ordinal));
            }
        }

        private void RegisterEnemy(Damageable health, bool boss)
        {
            if (health == null || _deathHandlers.ContainsKey(health))
                return;

            Action handler = () => HandleEnemyDeath(health);
            health.Died += handler;
            _deathHandlers[health] = handler;
            _bossFlags[health] = boss;
            _trackedEnemies.Add(health);
        }

        private void HandleEnemyDeath(Damageable health)
        {
            if (health == null)
                return;

            var isBoss = _bossFlags.TryGetValue(health, out var boss) && boss;

            if (!PrimaryComplete)
            {
                if (MissionType == ExpeditionMissionType.Purge && !isBoss)
                    _primaryKills++;
                else if (MissionType == ExpeditionMissionType.Blacksite && Stage == ExpeditionMissionStage.Eliminate && !isBoss)
                    _stageKills++;
            }
        }

        private void SpawnReinforcementWave(int requestedCount, string reason)
        {
            if (_session == null || _session.IsCompleted || _session.IsFailed)
                return;

            var alive = _trackedEnemies.Count(item => item != null && !item.IsDead);
            var allowed = Mathf.Max(0, 9 - alive);
            var count = Mathf.Min(Mathf.Max(0, requestedCount), allowed);
            if (count <= 0)
                return;

            for (var i = 0; i < count; i++)
            {
                var position = ReinforcementPoints[_spawnSerial % ReinforcementPoints.Length];
                SpawnReinforcement(position, _spawnSerial);
                _spawnSerial++;
            }

            RaiseAlert($"INCOMING // {reason}", 1.5f);
        }

        private void SpawnReinforcement(Vector3 position, int index)
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = $"Reinforcement_{index + 1:00}";
            enemy.transform.position = position;

            var prototypeCollider = enemy.GetComponent<Collider>();
            if (prototypeCollider != null)
                prototypeCollider.enabled = false;

            var controller = enemy.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.44f;
            controller.stepOffset = 0.32f;
            controller.skinWidth = 0.05f;

            var health = enemy.AddComponent<Damageable>();
            var chaser = enemy.AddComponent<InfectedChaser>();
            enemy.AddComponent<InfectedAnimationDriver>();

            var role = (InfectedCombatRole)Mathf.Abs(Level + index) % 4;
            ConfigureReinforcement(chaser, role, index);

            var binder = enemy.AddComponent<ProductionVisualBinder>();
            binder.Configure(ProductionVisualRole.Infected, index);
            binder.BindNow();

            var roleBrain = enemy.AddComponent<InfectedCombatRoleBrain>();
            roleBrain.Configure(role, GetRoleSpecialDamage(role));
            RegisterEnemy(health, false);
        }

        private void ConfigureReinforcement(InfectedChaser enemy, InfectedCombatRole role, int index)
        {
            var healthScale = 1f + (Level - 1) * 0.085f;
            var damageScale = 1f + (Level - 1) * 0.048f;
            var scrap = 4 + Mathf.Min(8, Level / 8);

            switch (role)
            {
                case InfectedCombatRole.Walker:
                    enemy.Configure(2.65f, 58f * healthScale, 10f * damageScale, scrap);
                    enemy.transform.localScale = Vector3.one;
                    enemy.name = $"Infected_Walker_R{index + 1:00}";
                    break;
                case InfectedCombatRole.Runner:
                    enemy.Configure(3.95f, 42f * healthScale, 8.5f * damageScale, scrap);
                    enemy.transform.localScale = Vector3.one * 0.88f;
                    enemy.name = $"Infected_Runner_R{index + 1:00}";
                    break;
                case InfectedCombatRole.Brute:
                    enemy.Configure(2.15f, 112f * healthScale, 16.5f * damageScale, scrap + 2);
                    enemy.transform.localScale = Vector3.one * 1.23f;
                    enemy.name = $"Infected_Brute_R{index + 1:00}";
                    break;
                default:
                    enemy.Configure(3.25f, 72f * healthScale, 12.5f * damageScale, scrap + 1);
                    enemy.transform.localScale = Vector3.one * 1.03f;
                    enemy.name = $"Infected_Stalker_R{index + 1:00}";
                    break;
            }
        }

        private float GetRoleSpecialDamage(InfectedCombatRole role)
        {
            var baseDamage = role switch
            {
                InfectedCombatRole.Runner => 13f,
                InfectedCombatRole.Brute => 23f,
                InfectedCombatRole.Stalker => 10f,
                _ => 1f
            };
            return baseDamage * (1f + (Level - 1) * 0.045f);
        }

        private void RaiseAlert(string text, float duration)
        {
            _alertText = text;
            _alertUntil = Time.unscaledTime + Mathf.Max(0.4f, duration);
            AlertRaised?.Invoke(text);
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private void OnDestroy()
        {
            foreach (var pair in _deathHandlers)
            {
                if (pair.Key != null)
                    pair.Key.Died -= pair.Value;
            }
            _deathHandlers.Clear();
            _bossFlags.Clear();
            _trackedEnemies.Clear();

            if (Current == this)
                Current = null;
        }
    }

    public sealed class Production11ExpeditionBootstrap : MonoBehaviour
    {
        private static Production11ExpeditionBootstrap _instance;
        private Coroutine _bindRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstalled()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_ExpeditionDirector_011_Bootstrap");
            _instance = root.AddComponent<Production11ExpeditionBootstrap>();
            DontDestroyOnLoad(root);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Start()
        {
            ScheduleBind();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ScheduleBind();
        }

        private void ScheduleBind()
        {
            if (_bindRoutine != null)
                StopCoroutine(_bindRoutine);
            _bindRoutine = StartCoroutine(BindDelayed());
        }

        private IEnumerator BindDelayed()
        {
            yield return null;
            yield return null;

            var session = FindFirstObjectByType<RunSession>();
            var player = FindFirstObjectByType<PlayerMotor>();
            if (session == null || player == null || ExpeditionDirector.Current != null)
            {
                _bindRoutine = null;
                yield break;
            }

            var root = new GameObject("Systems_ExpeditionDirector_011");
            root.AddComponent<ExpeditionDirector>();
            _bindRoutine = null;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
