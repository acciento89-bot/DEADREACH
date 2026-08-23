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
        public int schemaVersion = 2;
        public int securedScrap;
        public int successfulExtractions;
        public int failedRuns;
        public int currentExtractionStreak;
        public int bestExtractionStreak;
        public List<WeaponInstanceData> stashWeapons = new();
    }

    public static class SaveService
    {
        private const string FileName = "deadreach-profile.json";
        private const int CurrentSchemaVersion = 2;
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

            if (extractedWeapons != null)
            {
                data.stashWeapons ??= new List<WeaponInstanceData>();
                foreach (var weapon in extractedWeapons)
                {
                    if (weapon != null)
                        data.stashWeapons.Add(weapon.Clone());
                }
            }

            Save();
        }

        public static void RegisterFailedRun()
        {
            var data = Data;
            data.failedRuns++;
            data.currentExtractionStreak = 0;
            Save();
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
                stashWeapons = new List<WeaponInstanceData>()
            };
        }

        private static void Migrate(DeadreachProfileData data)
        {
            data.stashWeapons ??= new List<WeaponInstanceData>();
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
