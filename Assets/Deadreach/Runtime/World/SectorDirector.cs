using System;
using System.Collections.Generic;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Extraction;
using Kamilunavo.Deadreach.Loot;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.World
{
    [DisallowMultipleComponent]
    public sealed class SectorDirector : MonoBehaviour
    {
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
        private bool _initialized;
        private RunSession _session;
        private PlayerMotor _player;

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
            ActiveLayout = layouts[selectedIndex];

            for (var i = 0; i < layouts.Length; i++)
                layouts[i].gameObject.SetActive(i == selectedIndex);

            DisableLegacyRouteBlockers();
            ApplyAtmosphere();
            ApplyPlayerSpawn();
            ApplyExtractionAnchor();
            ApplyEnemyAnchors();
            ApplyLootAnchors();

            _initialized = true;
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

        private void OnDestroy()
        {
            _activeHazards.Clear();
            if (Current == this)
                Current = null;
        }
    }
}
