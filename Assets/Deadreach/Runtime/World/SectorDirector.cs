using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Extraction;
using Kamilunavo.Deadreach.Loot;
using Kamilunavo.Deadreach.Missions;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.World
{
    [DisallowMultipleComponent]
    public sealed class SectorDirector : MonoBehaviour
    {
        public const string EditorSectorOverrideKey = "DEADREACH_DEV_SECTOR_012";

        private static readonly Vector3[] FallbackObjectives =
        {
            new(-4f, 0.12f, 2.2f),
            new(4f, 0.12f, 7.6f),
            new(-4f, 0.12f, 13.2f),
            new(3.6f, 0.12f, 15.2f),
            new(0f, 0.12f, 5f)
        };

        private static readonly Vector3[] FallbackReinforcements =
        {
            new(-5.2f, 1f, -4.2f),
            new(5.2f, 1f, 0.4f),
            new(-5.2f, 1f, 9.8f),
            new(5.2f, 1f, 15.6f),
            new(0f, 1f, 18.1f)
        };

        private static readonly string[] LegacyRouteBlockers =
        {
            "Barrier_West_A",
            "Barrier_East_A",
            "Barrier_West_B",
            "Barrier_East_B",
            "Container_Green",
            "Container_Red",
            "Wreck_Pickup",
            "Wreck_Sports",
            "Wreck_Truck"
        };

        public static SectorDirector Current { get; private set; }

        public event Action SectorChanged;
        public event Action HazardStateChanged;

        public SectorLayoutDefinition ActiveLayout { get; private set; }
        public SectorArchetype Archetype => ActiveLayout != null ? ActiveLayout.Archetype : SectorArchetype.QuarantineWard;
        public string SectorName => ActiveLayout != null ? ActiveLayout.DisplayName : "DEAD CITY";
        public string HazardProfile => ActiveLayout != null ? ActiveLayout.HazardProfile : "CLEAR";
        public int RiskBonusScrap => ActiveLayout != null ? ActiveLayout.RiskBonusScrap : 0;
        public int RewardPowerBonus => ActiveLayout != null ? ActiveLayout.RewardPowerBonus : 0;
        public bool PlayerInHazard => _activeHazards.Count > 0;
        public string ActiveHazardLabel => _activeHazards.Count == 0
            ? "CLEAR"
            : _activeHazards.FirstOrDefault(item => item != null)?.DisplayName ?? "HAZARD";
        public string SectorCode => Archetype switch
        {
            SectorArchetype.TransitCollapse => "TRANSIT",
            SectorArchetype.IndustrialSpill => "SPILL",
            SectorArchetype.BlackoutPlaza => "BLACKOUT",
            _ => "Q-WARD"
        };

        private readonly HashSet<SectorHazardZone> _activeHazards = new();
        private readonly HashSet<int> _positionedReinforcements = new();
        private bool _initialized;
        private bool _primaryRiskBonusGranted;
        private bool _secondaryAnchored;
        private float _nextWorldSync;
        private int _reinforcementSerial;
        private RunSession _session;
        private PlayerMotor _player;
        private ExpeditionDirector _mission;

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
            EnsureInitialized();
        }

        private void Update()
        {
            if (!_initialized)
            {
                EnsureInitialized();
                return;
            }

            TryBindMission();

            if (_mission != null && _mission.PrimaryComplete && !_primaryRiskBonusGranted)
                GrantPrimaryRiskBonus();

            if (Time.time >= _nextWorldSync)
            {
                _nextWorldSync = Time.time + 0.2f;
                RelocateNewReinforcements();
            }
        }

        private void LateUpdate()
        {
            if (_initialized)
                ApplyMissionAnchors();
        }

        public void EnsureInitialized()
        {
            if (_initialized)
                return;

            _session = RunSession.Current;
            _player = FindFirstObjectByType<PlayerMotor>();
            if (_session == null || _player == null)
                return;

            var layouts = FindObjectsByType<SectorLayoutDefinition>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(item => item != null && item.gameObject.scene.IsValid())
                .OrderBy(item => (int)item.Archetype)
                .ToArray();

            if (layouts.Length == 0)
            {
                _initialized = true;
                Debug.LogWarning("DEADREACH 0.12 sector director found no authored sector layouts; using 0.11 fallback coordinates.");
                return;
            }

            var profile = SaveService.Data;
            var seed = unchecked(_session.RunLevel * 17 + profile.successfulExtractions * 7 + profile.failedRuns * 11);
            var selectedIndex = Mathf.Abs(seed) % layouts.Length;
#if UNITY_EDITOR
            var forcedIndex = PlayerPrefs.GetInt(EditorSectorOverrideKey, -1);
            if (forcedIndex >= 0 && forcedIndex < layouts.Length)
                selectedIndex = forcedIndex;
#endif
            ActiveLayout = layouts[selectedIndex];

            for (var i = 0; i < layouts.Length; i++)
                layouts[i].gameObject.SetActive(i == selectedIndex);

            DisableLegacyRouteBlockers();
            ApplyAtmosphere();
            ApplyPlayerSpawn();
            ApplyExtractionAnchor();
            ApplyEnemyAnchors();
            ApplyLootAnchors();

            _session.MissionRewardGranted += HandleMissionRewardGranted;
            _initialized = true;
            TryBindMission();
            SectorChanged?.Invoke();
            Debug.Log($"DEADREACH 0.12 sector online // {SectorName} // hazard={HazardProfile} // risk +{RiskBonusScrap} scrap // reward +{RewardPowerBonus} power.");
        }

        public Vector3 GetObjectivePoint(int index)
        {
            var fallback = FallbackObjectives[Mathf.Abs(index) % FallbackObjectives.Length];
            return ActiveLayout != null ? ActiveLayout.GetObjectivePoint(index, fallback) : fallback;
        }

        public Vector3[] GetObjectivePoints()
        {
            if (ActiveLayout == null)
                return FallbackObjectives.ToArray();

            var points = ActiveLayout.GetObjectivePoints();
            return points.Length > 0 ? points : FallbackObjectives.ToArray();
        }

        public Vector3 GetReinforcementPoint(int index)
        {
            var fallback = FallbackReinforcements[Mathf.Abs(index) % FallbackReinforcements.Length];
            return ActiveLayout != null ? ActiveLayout.GetReinforcementPoint(index, fallback) : fallback;
        }

        public void NotifyHazardEnter(SectorHazardZone hazard)
        {
            if (hazard == null || !_activeHazards.Add(hazard))
                return;

            HazardStateChanged?.Invoke();
        }

        public void NotifyHazardExit(SectorHazardZone hazard)
        {
            if (hazard == null || !_activeHazards.Remove(hazard))
                return;

            HazardStateChanged?.Invoke();
        }

        private void TryBindMission()
        {
            if (_mission != null || ExpeditionDirector.Current == null)
                return;

            _mission = ExpeditionDirector.Current;
            _mission.PrimaryCompleted += HandlePrimaryCompleted;
            ApplyMissionAnchors();
        }

        private void HandlePrimaryCompleted()
        {
            GrantPrimaryRiskBonus();
        }

        private void GrantPrimaryRiskBonus()
        {
            if (_primaryRiskBonusGranted || _session == null || RiskBonusScrap <= 0)
                return;

            _primaryRiskBonusGranted = true;
            _session.CollectScrap(RiskBonusScrap);
            Debug.Log($"DEADREACH 0.12 sector risk bonus // {SectorName} // +{RiskBonusScrap} unsecured Scrap.");
        }

        private void HandleMissionRewardGranted(WeaponInstanceData reward)
        {
            if (reward == null || RewardPowerBonus <= 0)
                return;

            reward.itemPower += RewardPowerBonus;
            Debug.Log($"DEADREACH 0.12 sector cache bonus // {SectorName} // +{RewardPowerBonus} Item Power.");
        }

        private void ApplyMissionAnchors()
        {
            if (_mission == null || ActiveLayout == null)
                return;

            var primaryName = _mission.MissionType switch
            {
                ExpeditionMissionType.Recovery => "Mission_RecoveryCore",
                ExpeditionMissionType.Purge => "Mission_PurgeBeacon",
                ExpeditionMissionType.Holdout => "Mission_Uplink",
                _ => "Mission_BlacksiteTerminal"
            };

            var primaryObject = GameObject.Find(primaryName);
            var primaryMarker = primaryObject != null ? primaryObject.GetComponent<ExpeditionObjectiveMarker>() : null;
            if (primaryMarker != null && !_mission.PrimaryComplete)
            {
                var objectiveIndex = _mission.MissionType switch
                {
                    ExpeditionMissionType.Recovery => 2,
                    ExpeditionMissionType.Purge => 4,
                    ExpeditionMissionType.Holdout => 1,
                    ExpeditionMissionType.Blacksite when _mission.Stage == ExpeditionMissionStage.Recover => 3,
                    _ => 0
                };
                primaryMarker.SetWorldPosition(GetObjectivePoint(objectiveIndex));
            }

            if (!_mission.PrimaryComplete || _secondaryAnchored)
                return;

            var secondaryObject = GameObject.Find("Mission_OptionalBlackCache");
            var secondaryMarker = secondaryObject != null ? secondaryObject.GetComponent<ExpeditionObjectiveMarker>() : null;
            if (secondaryMarker == null)
                return;

            var primaryPosition = primaryMarker != null ? primaryMarker.WorldPosition : _player.transform.position;
            var extractionPosition = ActiveLayout.ExtractionAnchor;
            var candidates = GetObjectivePoints();
            if (candidates.Length == 0)
                return;

            var selected = candidates
                .OrderByDescending(point => HorizontalDistance(point, primaryPosition) + HorizontalDistance(point, extractionPosition) * 0.45f)
                .First();
            secondaryMarker.SetWorldPosition(selected);
            _secondaryAnchored = true;
        }

        private void RelocateNewReinforcements()
        {
            if (ActiveLayout == null)
                return;

            var reinforcements = FindObjectsByType<InfectedChaser>(FindObjectsSortMode.None)
                .Where(item => item != null && IsRuntimeReinforcementName(item.name))
                .OrderBy(item => item.GetInstanceID())
                .ToArray();

            foreach (var reinforcement in reinforcements)
            {
                var id = reinforcement.GetInstanceID();
                if (!_positionedReinforcements.Add(id))
                    continue;

                var point = GetReinforcementPoint(_reinforcementSerial++);
                var controller = reinforcement.GetComponent<CharacterController>();
                if (controller != null)
                    controller.enabled = false;
                reinforcement.transform.position = point;
                if (controller != null)
                    controller.enabled = true;
            }

            if (reinforcements.Length > 0)
                Physics.SyncTransforms();
        }

        private static bool IsRuntimeReinforcementName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                return false;

            var marker = objectName.LastIndexOf("_R", StringComparison.Ordinal);
            if (marker < 0 || marker + 2 >= objectName.Length)
                return false;

            for (var i = marker + 2; i < objectName.Length; i++)
            {
                if (!char.IsDigit(objectName[i]))
                    return false;
            }

            return true;
        }

        private void DisableLegacyRouteBlockers()
        {
            foreach (var objectName in LegacyRouteBlockers)
            {
                var item = GameObject.Find(objectName);
                if (item != null)
                    item.SetActive(false);
            }
        }

        private void ApplyPlayerSpawn()
        {
            if (ActiveLayout == null || _player == null)
                return;

            var controller = _player.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;
            _player.transform.position = ActiveLayout.PlayerSpawn;
            if (controller != null)
                controller.enabled = true;
        }

        private void ApplyExtractionAnchor()
        {
            if (ActiveLayout == null)
                return;

            var extraction = FindFirstObjectByType<ExtractionZone>();
            if (extraction != null)
                extraction.transform.position = ActiveLayout.ExtractionAnchor;

            var devBeacon = GameObject.Find("Extraction_BeaconLight");
            if (devBeacon != null)
                devBeacon.transform.position = ActiveLayout.ExtractionAnchor + Vector3.up * 3.42f;

            var productionBeacon = GameObject.Find("Extraction_Beacon_Production");
            if (productionBeacon != null)
                productionBeacon.transform.position = ActiveLayout.ExtractionAnchor;

            Physics.SyncTransforms();
        }

        private void ApplyEnemyAnchors()
        {
            if (ActiveLayout == null || ActiveLayout.EnemyAnchorCount == 0)
                return;

            var enemies = FindObjectsByType<InfectedChaser>(FindObjectsSortMode.None)
                .Where(item => item != null)
                .OrderBy(item => item.name)
                .ToArray();

            for (var i = 0; i < enemies.Length; i++)
            {
                var fallback = enemies[i].transform.position;
                var point = ActiveLayout.GetEnemyPoint(i, fallback);
                var controller = enemies[i].GetComponent<CharacterController>();
                if (controller != null)
                    controller.enabled = false;
                enemies[i].transform.position = point;
                if (controller != null)
                    controller.enabled = true;
            }

            Physics.SyncTransforms();
        }

        private void ApplyLootAnchors()
        {
            if (ActiveLayout == null || ActiveLayout.LootAnchorCount == 0)
                return;

            var transforms = new List<Transform>();
            transforms.AddRange(FindObjectsByType<LootPickup>(FindObjectsSortMode.None)
                .Where(item => item != null)
                .OrderBy(item => item.name)
                .Select(item => item.transform));
            transforms.AddRange(FindObjectsByType<WeaponLootPickup>(FindObjectsSortMode.None)
                .Where(item => item != null)
                .OrderBy(item => item.name)
                .Select(item => item.transform));

            for (var i = 0; i < transforms.Count; i++)
            {
                var transformToMove = transforms[i];
                transformToMove.position = ActiveLayout.GetLootPoint(i, transformToMove.position);
            }

            Physics.SyncTransforms();
        }

        private void ApplyAtmosphere()
        {
            if (ActiveLayout == null)
                return;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = ActiveLayout.FogColor;
            RenderSettings.fogDensity = ActiveLayout.FogDensity;
            RenderSettings.ambientSkyColor = Color.Lerp(Color.black, ActiveLayout.KeyColor, 0.22f);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.black, ActiveLayout.FogColor, 0.38f);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.black, ActiveLayout.FogColor, 0.15f);

            var moon = GameObject.Find("Lighting_MoonKey")?.GetComponent<Light>();
            if (moon != null)
            {
                moon.color = ActiveLayout.KeyColor;
                moon.intensity = ActiveLayout.KeyIntensity;
            }

            var camera = Camera.main;
            if (camera != null)
                camera.backgroundColor = Color.Lerp(Color.black, ActiveLayout.FogColor, 0.58f);
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private void OnDestroy()
        {
            if (_mission != null)
                _mission.PrimaryCompleted -= HandlePrimaryCompleted;
            if (_session != null)
                _session.MissionRewardGranted -= HandleMissionRewardGranted;

            _activeHazards.Clear();
            _positionedReinforcements.Clear();
            if (Current == this)
                Current = null;
        }
    }

    public sealed class Production12SectorBootstrap : MonoBehaviour
    {
        private static Production12SectorBootstrap _instance;
        private Coroutine _bindRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstalled()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_SectorDirector_012_Bootstrap");
            _instance = root.AddComponent<Production12SectorBootstrap>();
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

            var session = FindFirstObjectByType<RunSession>();
            var player = FindFirstObjectByType<PlayerMotor>();
            if (session == null || player == null || SectorDirector.Current != null)
            {
                _bindRoutine = null;
                yield break;
            }

            var root = new GameObject("Systems_SectorDirector_012");
            var director = root.AddComponent<SectorDirector>();
            director.EnsureInitialized();
            _bindRoutine = null;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
