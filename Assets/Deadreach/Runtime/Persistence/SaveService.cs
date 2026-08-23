using System;
using System.Collections.Generic;
using System.IO;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Persistence
{
    public enum BunkerUpgradeType
    {
        Workbench = 0,
        Medbay = 1,
        CargoRig = 2,
        ScavengerNetwork = 3
    }

    [Serializable]
    public sealed class DeadreachProfileData
    {
        public int schemaVersion = 6;
        public int securedScrap;
        public int successfulExtractions;
        public int failedRuns;
        public int currentExtractionStreak;
        public int bestExtractionStreak;
        public List<WeaponInstanceData> stashWeapons = new();
        public string equippedPrimaryWeaponId;

        public int highestUnlockedLevel = 1;
        public int selectedLevel = 1;
        public int highestCompletedLevel;
        public int bossKills;
        public string selectedCharacterId = "ranger";
        public List<string> unlockedCharacterIds = new() { "ranger", "scout", "warden" };
        public List<string> ownedContentIds = new();

        // Production 0.6 Bunker debrief.
        public WeaponInstanceData lastBossReward;
        public bool bossRewardDebriefPending;

        // Production 0.8 permanent Bunker progression.
        public int workbenchLevel;
        public int medbayLevel;
        public int cargoRigLevel;
        public int scavengerNetworkLevel;
    }

    public static class SaveService
    {
        private const string FileName = "deadreach-profile.json";
        private const int CurrentSchemaVersion = 6;
        public const int MaxCampaignLevel = 50;
        public const int MaxWeaponUpgradeLevel = 10;
        public const int MaxBunkerUpgradeRank = 5;
        private static DeadreachProfileData _cached;

        public static DeadreachProfileData Data => _cached ??= Load();
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void RegisterExtraction(int scrap)
        {
            RegisterExtraction(scrap, null, Data.selectedLevel);
        }

        public static void RegisterExtraction(int scrap, IReadOnlyList<WeaponInstanceData> extractedWeapons)
        {
            RegisterExtraction(scrap, extractedWeapons, Data.selectedLevel);
        }

        public static void RegisterExtraction(int scrap, IReadOnlyList<WeaponInstanceData> extractedWeapons, int completedLevel)
        {
            var data = Data;
            var securedFromRun = Mathf.Max(0, scrap);
            if (securedFromRun > 0)
                securedFromRun = Mathf.RoundToInt(securedFromRun * GetScrapExtractionMultiplier());

            data.securedScrap += securedFromRun;
            data.successfulExtractions++;
            data.currentExtractionStreak++;
            data.bestExtractionStreak = Mathf.Max(data.bestExtractionStreak, data.currentExtractionStreak);

            WeaponInstanceData bestNewWeapon = null;
            if (extractedWeapons != null)
            {
                data.stashWeapons ??= new List<WeaponInstanceData>();
                foreach (var weapon in extractedWeapons)
                {
                    if (weapon == null)
                        continue;

                    var copy = weapon.Clone();
                    data.stashWeapons.Add(copy);
                    if (bestNewWeapon == null || copy.itemPower > bestNewWeapon.itemPower)
                        bestNewWeapon = copy;
                }
            }

            if (string.IsNullOrWhiteSpace(data.equippedPrimaryWeaponId) && bestNewWeapon != null)
                data.equippedPrimaryWeaponId = bestNewWeapon.instanceId;

            var level = Mathf.Clamp(completedLevel, 1, MaxCampaignLevel);
            data.highestCompletedLevel = Mathf.Max(data.highestCompletedLevel, level);
            if (level % 10 == 0)
                data.bossKills++;

            if (level >= data.highestUnlockedLevel && level < MaxCampaignLevel)
                data.highestUnlockedLevel = Mathf.Min(MaxCampaignLevel, level + 1);

            if (level < MaxCampaignLevel && level + 1 <= data.highestUnlockedLevel)
                data.selectedLevel = level + 1;
            else
                data.selectedLevel = Mathf.Clamp(level, 1, data.highestUnlockedLevel);

            Save();
        }

        public static void RecordSecuredBossReward(WeaponInstanceData reward)
        {
            if (reward == null)
                return;

            var data = Data;
            data.lastBossReward = reward.Clone();
            data.bossRewardDebriefPending = true;
            Save();
        }

        public static void MarkBossRewardDebriefSeen()
        {
            if (!Data.bossRewardDebriefPending)
                return;

            Data.bossRewardDebriefPending = false;
            Save();
        }

        public static void RegisterFailedRun()
        {
            var data = Data;
            data.failedRuns++;
            data.currentExtractionStreak = 0;
            Save();
        }

        public static WeaponInstanceData GetEquippedPrimaryWeapon()
        {
            var data = Data;
            if (string.IsNullOrWhiteSpace(data.equippedPrimaryWeaponId) || data.stashWeapons == null)
                return null;

            return data.stashWeapons.Find(item => item != null && item.instanceId == data.equippedPrimaryWeaponId);
        }

        public static WeaponInstanceData GetWeapon(string instanceId)
        {
            if (string.IsNullOrWhiteSpace(instanceId) || Data.stashWeapons == null)
                return null;

            return Data.stashWeapons.Find(item => item != null && item.instanceId == instanceId);
        }

        public static bool EquipPrimaryWeapon(string instanceId)
        {
            var data = Data;
            if (string.IsNullOrWhiteSpace(instanceId) || data.stashWeapons == null)
                return false;

            var exists = data.stashWeapons.Exists(item => item != null && item.instanceId == instanceId);
            if (!exists)
                return false;

            data.equippedPrimaryWeaponId = instanceId;
            Save();
            return true;
        }

        public static int GetWeaponUpgradeLimit()
        {
            // The sealed field bench permits one calibration rank before permanent Workshop investment.
            return Mathf.Clamp(1 + Data.workbenchLevel * 2, 1, MaxWeaponUpgradeLevel);
        }

        public static int GetWeaponUpgradeCost(WeaponInstanceData weapon)
        {
            if (weapon == null || weapon.upgradeLevel >= GetWeaponUpgradeLimit() || weapon.upgradeLevel >= MaxWeaponUpgradeLevel)
                return 0;

            var rarityCost = Mathf.Max(0, (int)weapon.rarity) * 24;
            var powerCost = Mathf.RoundToInt(Mathf.Max(0, weapon.itemPower) * 0.22f);
            return 55 + rarityCost + powerCost + weapon.upgradeLevel * 42;
        }

        public static bool UpgradeWeapon(string instanceId)
        {
            var weapon = GetWeapon(instanceId);
            if (weapon == null)
                return false;

            var limit = GetWeaponUpgradeLimit();
            if (weapon.upgradeLevel >= limit || weapon.upgradeLevel >= MaxWeaponUpgradeLevel)
                return false;

            var cost = GetWeaponUpgradeCost(weapon);
            if (cost <= 0 || Data.securedScrap < cost)
                return false;

            Data.securedScrap -= cost;
            weapon.upgradeLevel++;
            weapon.itemPower += 8;
            Save();
            return true;
        }

        public static int GetSalvageValue(WeaponInstanceData weapon)
        {
            if (weapon == null)
                return 0;

            var rarityValue = Mathf.Max(0, (int)weapon.rarity) * 13;
            var powerValue = Mathf.RoundToInt(Mathf.Max(0, weapon.itemPower) * 0.18f);
            var calibrationValue = Mathf.Max(0, weapon.upgradeLevel) * 8;
            return Mathf.Max(12, 12 + rarityValue + powerValue + calibrationValue);
        }

        public static bool SalvageWeapon(string instanceId)
        {
            var data = Data;
            if (string.IsNullOrWhiteSpace(instanceId) || data.stashWeapons == null)
                return false;

            // Never allow the active loadout to disappear from under a deployment.
            if (string.Equals(data.equippedPrimaryWeaponId, instanceId, StringComparison.Ordinal))
                return false;

            var index = data.stashWeapons.FindIndex(item => item != null && item.instanceId == instanceId);
            if (index < 0)
                return false;

            var weapon = data.stashWeapons[index];
            data.securedScrap += GetSalvageValue(weapon);
            data.stashWeapons.RemoveAt(index);
            Save();
            return true;
        }

        public static int GetBunkerUpgradeLevel(BunkerUpgradeType type)
        {
            return type switch
            {
                BunkerUpgradeType.Workbench => Data.workbenchLevel,
                BunkerUpgradeType.Medbay => Data.medbayLevel,
                BunkerUpgradeType.CargoRig => Data.cargoRigLevel,
                BunkerUpgradeType.ScavengerNetwork => Data.scavengerNetworkLevel,
                _ => 0
            };
        }

        public static int GetBunkerUpgradeCost(BunkerUpgradeType type)
        {
            var level = GetBunkerUpgradeLevel(type);
            if (level >= MaxBunkerUpgradeRank)
                return 0;

            var baseCost = type switch
            {
                BunkerUpgradeType.Workbench => 170,
                BunkerUpgradeType.Medbay => 210,
                BunkerUpgradeType.CargoRig => 190,
                BunkerUpgradeType.ScavengerNetwork => 230,
                _ => 200
            };

            var stepCost = type switch
            {
                BunkerUpgradeType.Workbench => 145,
                BunkerUpgradeType.Medbay => 165,
                BunkerUpgradeType.CargoRig => 155,
                BunkerUpgradeType.ScavengerNetwork => 185,
                _ => 160
            };

            return baseCost + level * stepCost;
        }

        public static bool PurchaseBunkerUpgrade(BunkerUpgradeType type)
        {
            var level = GetBunkerUpgradeLevel(type);
            if (level >= MaxBunkerUpgradeRank)
                return false;

            var cost = GetBunkerUpgradeCost(type);
            if (cost <= 0 || Data.securedScrap < cost)
                return false;

            Data.securedScrap -= cost;
            switch (type)
            {
                case BunkerUpgradeType.Workbench:
                    Data.workbenchLevel++;
                    break;
                case BunkerUpgradeType.Medbay:
                    Data.medbayLevel++;
                    break;
                case BunkerUpgradeType.CargoRig:
                    Data.cargoRigLevel++;
                    break;
                case BunkerUpgradeType.ScavengerNetwork:
                    Data.scavengerNetworkLevel++;
                    break;
                default:
                    return false;
            }

            Save();
            return true;
        }

        public static float GetPlayerHealthMultiplier()
        {
            return 1f + Mathf.Clamp(Data.medbayLevel, 0, MaxBunkerUpgradeRank) * 0.06f;
        }

        public static int GetRunWeaponCapacity()
        {
            return 6 + Mathf.Clamp(Data.cargoRigLevel, 0, MaxBunkerUpgradeRank);
        }

        public static float GetScrapExtractionMultiplier()
        {
            return 1f + Mathf.Clamp(Data.scavengerNetworkLevel, 0, MaxBunkerUpgradeRank) * 0.08f;
        }

        public static bool SelectLevel(int level)
        {
            var data = Data;
            if (level < 1 || level > Mathf.Clamp(data.highestUnlockedLevel, 1, MaxCampaignLevel))
                return false;

            data.selectedLevel = level;
            Save();
            return true;
        }

        public static bool SelectCharacter(string characterId)
        {
            var data = Data;
            data.unlockedCharacterIds ??= CreateDefaultCharacterUnlocks();
            if (string.IsNullOrWhiteSpace(characterId) || !data.unlockedCharacterIds.Contains(characterId))
                return false;

            data.selectedCharacterId = characterId;
            Save();
            return true;
        }

        public static bool OwnsContent(string contentId)
        {
            return !string.IsNullOrWhiteSpace(contentId) && Data.ownedContentIds != null && Data.ownedContentIds.Contains(contentId);
        }

        public static void GrantContent(string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return;

            var data = Data;
            data.ownedContentIds ??= new List<string>();
            if (!data.ownedContentIds.Contains(contentId))
                data.ownedContentIds.Add(contentId);
            Save();
        }

        public static void Save()
        {
            try
            {
                Data.schemaVersion = CurrentSchemaVersion;
                Data.securedScrap = Mathf.Max(0, Data.securedScrap);
                Data.stashWeapons ??= new List<WeaponInstanceData>();
                Data.unlockedCharacterIds ??= CreateDefaultCharacterUnlocks();
                Data.ownedContentIds ??= new List<string>();
                var json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception exception)
            {
                Debug.LogError($"DEADREACH save failed: {exception.Message}");
            }
        }

        public static DeadreachProfileData Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                    return CreateFreshProfile();

                var json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<DeadreachProfileData>(json) ?? CreateFreshProfile();
                Migrate(data);
                return data;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"DEADREACH save could not be loaded; starting fresh. {exception.Message}");
                return CreateFreshProfile();
            }
        }

        private static DeadreachProfileData CreateFreshProfile()
        {
            return new DeadreachProfileData
            {
                schemaVersion = CurrentSchemaVersion,
                stashWeapons = new List<WeaponInstanceData>(),
                equippedPrimaryWeaponId = string.Empty,
                highestUnlockedLevel = 1,
                selectedLevel = 1,
                highestCompletedLevel = 0,
                bossKills = 0,
                selectedCharacterId = "ranger",
                unlockedCharacterIds = CreateDefaultCharacterUnlocks(),
                ownedContentIds = new List<string>(),
                lastBossReward = null,
                bossRewardDebriefPending = false,
                workbenchLevel = 0,
                medbayLevel = 0,
                cargoRigLevel = 0,
                scavengerNetworkLevel = 0
            };
        }

        private static List<string> CreateDefaultCharacterUnlocks()
        {
            return new List<string> { "ranger", "scout", "warden" };
        }

        private static void Migrate(DeadreachProfileData data)
        {
            data.stashWeapons ??= new List<WeaponInstanceData>();
            data.unlockedCharacterIds ??= CreateDefaultCharacterUnlocks();
            data.ownedContentIds ??= new List<string>();

            if (!data.unlockedCharacterIds.Contains("ranger")) data.unlockedCharacterIds.Add("ranger");
            if (!data.unlockedCharacterIds.Contains("scout")) data.unlockedCharacterIds.Add("scout");
            if (!data.unlockedCharacterIds.Contains("warden")) data.unlockedCharacterIds.Add("warden");

            if (string.IsNullOrWhiteSpace(data.selectedCharacterId) || !data.unlockedCharacterIds.Contains(data.selectedCharacterId))
                data.selectedCharacterId = "ranger";

            data.highestUnlockedLevel = Mathf.Clamp(Mathf.Max(1, data.highestUnlockedLevel), 1, MaxCampaignLevel);
            data.highestCompletedLevel = Mathf.Clamp(data.highestCompletedLevel, 0, MaxCampaignLevel);
            data.selectedLevel = Mathf.Clamp(Mathf.Max(1, data.selectedLevel), 1, data.highestUnlockedLevel);
            data.securedScrap = Mathf.Max(0, data.securedScrap);

            data.workbenchLevel = Mathf.Clamp(data.workbenchLevel, 0, MaxBunkerUpgradeRank);
            data.medbayLevel = Mathf.Clamp(data.medbayLevel, 0, MaxBunkerUpgradeRank);
            data.cargoRigLevel = Mathf.Clamp(data.cargoRigLevel, 0, MaxBunkerUpgradeRank);
            data.scavengerNetworkLevel = Mathf.Clamp(data.scavengerNetworkLevel, 0, MaxBunkerUpgradeRank);

            foreach (var weapon in data.stashWeapons)
            {
                if (weapon == null)
                    continue;
                weapon.upgradeLevel = Mathf.Clamp(weapon.upgradeLevel, 0, MaxWeaponUpgradeLevel);
            }

            if (!string.IsNullOrWhiteSpace(data.equippedPrimaryWeaponId) &&
                !data.stashWeapons.Exists(item => item != null && item.instanceId == data.equippedPrimaryWeaponId))
            {
                data.equippedPrimaryWeaponId = string.Empty;
            }

            // WeaponFamily.Rifle is enum value 0, so legacy 0.5 weapon JSON migrates naturally.
            // Production 0.8 fields all default to zero when loading schema <= 5 JSON.
            data.schemaVersion = CurrentSchemaVersion;
        }

#if UNITY_EDITOR
        public static void ResetForDevelopment()
        {
            _cached = CreateFreshProfile();
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
#endif
    }
}
