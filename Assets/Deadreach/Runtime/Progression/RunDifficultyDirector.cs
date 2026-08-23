using System;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Progression
{
    public sealed class RunDifficultyDirector : MonoBehaviour
    {
        public static RunDifficultyDirector Current { get; private set; }

        public event Action BossDefeated;
        public event Action<int> BossPhaseChanged;

        public int Level { get; private set; }
        public bool IsBossLevel { get; private set; }
        public bool BossGateCleared { get; private set; } = true;
        public Damageable BossHealth => _bossHealth;
        public float BossHealthNormalized => _bossHealth == null ? 0f : _bossHealth.NormalizedHealth;
        public string ZoneName => GetZoneName(Level);

        private Damageable _bossHealth;
        private InfectedChaser _bossChaser;
        private int _bossPhase;

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
            Level = Mathf.Clamp(SaveService.Data.selectedLevel, 1, SaveService.MaxCampaignLevel);
            IsBossLevel = Level % 10 == 0;
            BossGateCleared = !IsBossLevel;
            ApplySectorAtmosphere();
            ConfigureEncounter();
        }

        private void OnDestroy()
        {
            if (_bossHealth != null)
                _bossHealth.Died -= HandleBossDeath;
            if (Current == this)
                Current = null;
        }

        private void Update()
        {
            if (!IsBossLevel || BossGateCleared || _bossHealth == null || _bossHealth.IsDead || _bossChaser == null)
                return;

            var normalized = _bossHealth.NormalizedHealth;
            if (_bossPhase < 1 && normalized <= 0.66f)
            {
                _bossPhase = 1;
                _bossChaser.ApplyMutation(1.12f, 1.12f, 1.18f);
                _bossChaser.transform.localScale *= 1.06f;
                BossPhaseChanged?.Invoke(_bossPhase);
            }
            else if (_bossPhase < 2 && normalized <= 0.33f)
            {
                _bossPhase = 2;
                _bossChaser.ApplyMutation(1.18f, 1.2f, 1.25f);
                _bossChaser.transform.localScale *= 1.08f;
                BossPhaseChanged?.Invoke(_bossPhase);
            }
        }

        private void ConfigureEncounter()
        {
            var infected = FindObjectsByType<InfectedChaser>(FindObjectsSortMode.None)
                .OrderBy(item => item.name)
                .ToArray();

            if (infected.Length == 0)
                return;

            var levelHealthScale = 1f + (Level - 1) * 0.085f;
            var levelDamageScale = 1f + (Level - 1) * 0.048f;

            for (var i = 0; i < infected.Length; i++)
                ConfigureArchetype(infected[i], (Level + i) % 4, levelHealthScale, levelDamageScale, i);

            if (IsBossLevel)
                PromoteBoss(infected[infected.Length - 1]);

            Debug.Log($"DEADREACH campaign encounter configured: LEVEL {Level:00} // {ZoneName} // boss={IsBossLevel}.");
        }

        private static void ConfigureArchetype(InfectedChaser enemy, int archetype, float healthScale, float damageScale, int index)
        {
            switch (archetype)
            {
                case 0:
                    enemy.Configure(2.65f, 58f * healthScale, 10f * damageScale, 4 + index);
                    enemy.transform.localScale = Vector3.one;
                    enemy.name = $"Infected_Walker_{index + 1:00}";
                    break;
                case 1:
                    enemy.Configure(3.95f, 42f * healthScale, 8.5f * damageScale, 4 + index);
                    enemy.transform.localScale = Vector3.one * 0.88f;
                    enemy.name = $"Infected_Runner_{index + 1:00}";
                    break;
                case 2:
                    enemy.Configure(2.15f, 112f * healthScale, 16.5f * damageScale, 7 + index);
                    enemy.transform.localScale = Vector3.one * 1.23f;
                    enemy.name = $"Infected_Brute_{index + 1:00}";
                    break;
                default:
                    enemy.Configure(3.25f, 72f * healthScale, 12.5f * damageScale, 5 + index);
                    enemy.transform.localScale = Vector3.one * 1.03f;
                    enemy.name = $"Infected_Stalker_{index + 1:00}";
                    break;
            }
        }

        private void PromoteBoss(InfectedChaser boss)
        {
            var tier = Mathf.Max(1, Level / 10);
            boss.name = $"BOSS_Mutation_Tier_{tier}";
            boss.transform.localScale = Vector3.one * (1.58f + tier * 0.09f);

            // Bosses never pay ordinary Scrap. Their guaranteed mutation weapon reward is granted
            // in HandleBossDeath and only becomes secured after successful extraction.
            boss.Configure(2.55f + tier * 0.08f, 650f + tier * 260f, 24f + tier * 5f, 0);

            _bossChaser = boss;
            _bossHealth = boss.GetComponent<Damageable>();
            if (_bossHealth != null)
                _bossHealth.Died += HandleBossDeath;
        }

        private void ApplySectorAtmosphere()
        {
            Color fog;
            Color key;
            float density;
            float intensity;

            if (Level <= 10)
            {
                fog = new Color(0.055f, 0.075f, 0.09f);
                key = new Color(0.55f, 0.68f, 0.9f);
                density = 0.0125f;
                intensity = 0.85f;
            }
            else if (Level <= 20)
            {
                fog = new Color(0.035f, 0.085f, 0.075f);
                key = new Color(0.42f, 0.74f, 0.63f);
                density = 0.015f;
                intensity = 0.72f;
            }
            else if (Level <= 30)
            {
                fog = new Color(0.13f, 0.07f, 0.035f);
                key = new Color(0.92f, 0.58f, 0.31f);
                density = 0.018f;
                intensity = 0.78f;
            }
            else if (Level <= 40)
            {
                fog = new Color(0.035f, 0.027f, 0.06f);
                key = new Color(0.42f, 0.38f, 0.72f);
                density = 0.021f;
                intensity = 0.58f;
            }
            else
            {
                fog = new Color(0.09f, 0.018f, 0.018f);
                key = new Color(0.82f, 0.24f, 0.18f);
                density = 0.023f;
                intensity = 0.68f;
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fog;
            RenderSettings.fogDensity = density;
            RenderSettings.ambientSkyColor = Color.Lerp(Color.black, key, 0.22f);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.black, fog, 0.35f);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.black, fog, 0.12f);

            var moon = GameObject.Find("Lighting_MoonKey")?.GetComponent<Light>();
            if (moon != null)
            {
                moon.color = key;
                moon.intensity = intensity;
            }

            var camera = Camera.main;
            if (camera != null)
                camera.backgroundColor = Color.Lerp(Color.black, fog, 0.55f);
        }

        private void HandleBossDeath()
        {
            BossGateCleared = true;

            var tier = Mathf.Clamp(Mathf.Max(1, Level / 10), 1, 5);
            var seed = unchecked(Environment.TickCount ^ Level * 48611 ^ SaveService.Data.successfulExtractions * 3571);
            var reward = WeaponLootFactory.CreateBossReward(Level, seed);
            RunSession.Current?.GrantBossReward(reward);

            BossDefeated?.Invoke();
            Debug.Log($"DEADREACH BOSS DOWN // LEVEL {Level:00} // MUTATION T{tier} REWARD GRANTED // extraction gate unlocked.");
        }

        public static string GetZoneName(int level)
        {
            if (level <= 10) return "DEAD CITY";
            if (level <= 20) return "FLOODED INDUSTRIAL";
            if (level <= 30) return "ASH DISTRICT";
            if (level <= 40) return "BLACKOUT SECTOR";
            return "GROUND ZERO";
        }
    }
}
