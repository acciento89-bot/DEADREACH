using System;
using System.Collections;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Core
{
    public sealed class RunSession : MonoBehaviour
    {
        public static RunSession Current { get; private set; }

        [SerializeField, Min(0f)] private float resultScreenDuration = 2.2f;

        public event Action<int> ScrapChanged;
        public event Action ExtractionCompleted;
        public event Action RunFailed;
        public event Action<WeaponInstanceData> BossRewardGranted;
        public event Action<WeaponInstanceData> MissionRewardGranted;

        public int CarriedScrap { get; private set; }
        public int RunLevel { get; private set; }
        public float ExtractionProgress { get; private set; }
        public bool IsInExtractionZone { get; private set; }
        public bool ExtractionBlockedByNoLoot { get; private set; }
        public bool ExtractionBlockedByBoss { get; private set; }
        public bool ExtractionBlockedByMission { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsFailed { get; private set; }
        public WeaponInstanceData PendingBossReward => _pendingBossReward;
        public IReadOnlyList<WeaponInstanceData> PendingMissionRewards => _pendingMissionRewards;

        private Damageable _playerHealth;
        private Coroutine _returnRoutine;
        private WeaponInstanceData _pendingBossReward;
        private WeaponInstanceData _bossRewardGrantedThisRun;
        private readonly List<WeaponInstanceData> _pendingMissionRewards = new();

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
            RunLevel = Mathf.Clamp(SaveService.Data.selectedLevel, 1, SaveService.MaxCampaignLevel);
        }

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player == null)
                return;

            _playerHealth = player.GetComponent<Damageable>();
            if (_playerHealth != null)
                _playerHealth.Died += HandlePlayerDeath;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
                _playerHealth.Died -= HandlePlayerDeath;

            if (Current == this)
                Current = null;
        }

        public void CollectScrap(int amount)
        {
            if (IsCompleted || IsFailed || amount <= 0)
                return;

            CarriedScrap += amount;
            ScrapChanged?.Invoke(CarriedScrap);
        }

        public void GrantBossReward(WeaponInstanceData reward)
        {
            if (reward == null || IsCompleted || IsFailed || _bossRewardGrantedThisRun != null)
                return;

            _bossRewardGrantedThisRun = reward.Clone();
            _pendingBossReward = reward.Clone();

            // Show the reward immediately in the carried weapon count when capacity allows. If the
            // inventory is full, the reward remains reserved and is still secured on extraction.
            if (RunInventory.Current != null && !RunInventory.Current.IsFull)
            {
                RunInventory.Current.TryAddWeapon(_pendingBossReward);
                _pendingBossReward = null;
            }

            BossRewardGranted?.Invoke(reward);
            Debug.Log($"DEADREACH BOSS REWARD ACQUIRED // {reward.displayNameSnapshot} // {WeaponVisualStyle.GetDisplayName(WeaponVisualStyle.ResolveFinishId(reward))}");
        }

        public void GrantMissionReward(WeaponInstanceData reward)
        {
            if (reward == null || IsCompleted || IsFailed)
                return;

            var copy = reward.Clone();
            if (RunInventory.Current != null && !RunInventory.Current.IsFull)
            {
                RunInventory.Current.TryAddWeapon(copy);
            }
            else
            {
                _pendingMissionRewards.Add(copy);
            }

            MissionRewardGranted?.Invoke(copy);
            Debug.Log($"DEADREACH MISSION REWARD ACQUIRED // {copy.displayNameSnapshot} // PWR {copy.itemPower} // reserved until extraction.");
        }

        public void SetExtractionProgress(float normalized)
        {
            if (IsCompleted || IsFailed)
                return;

            ExtractionProgress = Mathf.Clamp01(normalized);
        }

        public void SetExtractionPresence(bool inside, bool blockedByNoLoot)
        {
            SetExtractionPresence(inside, blockedByNoLoot, false, false);
        }

        public void SetExtractionPresence(bool inside, bool blockedByNoLoot, bool blockedByBoss)
        {
            SetExtractionPresence(inside, blockedByNoLoot, blockedByBoss, false);
        }

        public void SetExtractionPresence(bool inside, bool blockedByNoLoot, bool blockedByBoss, bool blockedByMission)
        {
            if (IsCompleted || IsFailed)
                return;

            IsInExtractionZone = inside;
            ExtractionBlockedByNoLoot = inside && blockedByNoLoot;
            ExtractionBlockedByBoss = inside && blockedByBoss;
            ExtractionBlockedByMission = inside && blockedByMission;

            if (!inside)
                ExtractionProgress = 0f;
        }

        public void CompleteExtraction()
        {
            if (IsCompleted || IsFailed)
                return;

            IsCompleted = true;
            IsInExtractionZone = false;
            ExtractionBlockedByNoLoot = false;
            ExtractionBlockedByBoss = false;
            ExtractionBlockedByMission = false;
            ExtractionProgress = 1f;

            var extractedWeapons = RunInventory.Current?.CreateExtractionSnapshot() ?? new List<WeaponInstanceData>();
            if (_pendingBossReward != null)
            {
                extractedWeapons.Add(_pendingBossReward.Clone());
                _pendingBossReward = null;
            }

            foreach (var reward in _pendingMissionRewards)
            {
                if (reward != null)
                    extractedWeapons.Add(reward.Clone());
            }
            _pendingMissionRewards.Clear();

            SaveService.RegisterExtraction(CarriedScrap, extractedWeapons, RunLevel);
            if (_bossRewardGrantedThisRun != null)
                SaveService.RecordSecuredBossReward(_bossRewardGrantedThisRun);

            RunInventory.Current?.Clear();

            CarriedScrap = 0;
            ScrapChanged?.Invoke(CarriedScrap);
            ExtractionCompleted?.Invoke();
            BeginReturnToBunker();
        }

        public void AbandonRun()
        {
            FailRun();
        }

        private void HandlePlayerDeath()
        {
            FailRun();
        }

        private void FailRun()
        {
            if (IsCompleted || IsFailed)
                return;

            IsFailed = true;
            IsInExtractionZone = false;
            ExtractionBlockedByNoLoot = false;
            ExtractionBlockedByBoss = false;
            ExtractionBlockedByMission = false;
            CarriedScrap = 0;
            ExtractionProgress = 0f;
            _pendingBossReward = null;
            _bossRewardGrantedThisRun = null;
            _pendingMissionRewards.Clear();
            RunInventory.Current?.Clear();
            SaveService.RegisterFailedRun();
            ScrapChanged?.Invoke(CarriedScrap);
            RunFailed?.Invoke();
            BeginReturnToBunker();
        }

        private void BeginReturnToBunker()
        {
            if (_returnRoutine == null)
                _returnRoutine = StartCoroutine(ReturnToBunkerAfterDelay());
        }

        private IEnumerator ReturnToBunkerAfterDelay()
        {
            yield return new WaitForSecondsRealtime(resultScreenDuration);
            SceneFlowService.LoadBunker();
        }
    }
}
