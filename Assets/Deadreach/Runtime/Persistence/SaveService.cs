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
        public int schemaVersion = 3;
        public int securedScrap;
        public int successfulExtractions;
        public int failedRuns;
        public int currentExtractionStreak;
        public int bestExtractionStreak;
        public List<WeaponInstanceData> stashWeapons = new();
        public string equippedPrimaryWeaponId;
    }

    public static class SaveService
    {
        private const string FileName = "deadreach-profile.json";
        private const int CurrentSchemaVersion = 3;
        private static DeadreachProfileData _cached;

        public static DeadreachProfileData Data => _cached ??= Load();
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void RegisterExtraction(int scrap)
        {
            RegisterExtraction(scrap, null);
        }

        public static void RegisterExtraction(int scrap, IReadOnlyList<WeaponInstanceData> extractedWeapons)
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

        public static void Save()
        {
            try
            {
                Data.schemaVersion = CurrentSchemaVersion;
                Data.stashWeapons ??= new List<WeaponInstanceData>();
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
                equippedPrimaryWeaponId = string.Empty
            };
        }

        private static void Migrate(DeadreachProfileData data)
        {
            data.stashWeapons ??= new List<WeaponInstanceData>();

            if (!string.IsNullOrWhiteSpace(data.equippedPrimaryWeaponId) &&
                !data.stashWeapons.Exists(item => item != null && item.instanceId == data.equippedPrimaryWeaponId))
            {
                data.equippedPrimaryWeaponId = string.Empty;
            }

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
