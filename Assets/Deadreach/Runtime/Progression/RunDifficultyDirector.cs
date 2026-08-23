using System;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Persistence;
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
            boss.Configure(2.55f + tier * 0.08f, 650f + tier * 260f, 24f + tier * 5f, 45 + tier * 20);

            _bossChaser = boss;
            _bossHealth = boss.GetComponent<Damageable>();
            if (_bossHealth != null)
                _bossHealth.Died += HandleBossDeath;
        }

        private void HandleBossDeath()
        {
            BossGateCleared = true;
            BossDefeated?.Invoke();
            Debug.Log($"DEADREACH BOSS DOWN // LEVEL {Level:00} extraction gate unlocked.");
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
