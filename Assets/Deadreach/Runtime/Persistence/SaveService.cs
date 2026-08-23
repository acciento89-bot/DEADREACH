using System;
using System.Collections.Generic;
using System.IO;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Persistence
{
    [Serializable]
    public sealed class DeadreachProfileData
    {
        public int schemaVersion = 5;
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
    }

    public static class SaveService
    {
        private const string FileName = "deadreach-profile.json";
        private const int CurrentSchemaVersion = 5;
        public const int MaxCampaignLevel = 50;
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
            data.securedScrap += Mathf.Max(0, scrap);
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
                bossRewardDebriefPending = false
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

            if (!string.IsNullOrWhiteSpace(data.equippedPrimaryWeaponId) &&
                !data.stashWeapons.Exists(item => item != null && item.instanceId == data.equippedPrimaryWeaponId))
            {
                data.equippedPrimaryWeaponId = string.Empty;
            }

            // WeaponFamily.Rifle is enum value 0, so legacy 0.5 weapon JSON migrates naturally.
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
