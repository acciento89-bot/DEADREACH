using System;
using System.IO;
using UnityEngine;

namespace Kamilunavo.Deadreach.Persistence
{
    [Serializable]
    public sealed class DeadreachProfileData
    {
        public int schemaVersion = 1;
        public int securedScrap;
        public int successfulExtractions;
        public int failedRuns;
        public int currentExtractionStreak;
        public int bestExtractionStreak;
    }

    public static class SaveService
    {
        private const string FileName = "deadreach-profile.json";
        private static DeadreachProfileData _cached;

        public static DeadreachProfileData Data => _cached ??= Load();
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void RegisterExtraction(int scrap)
        {
            var data = Data;
            data.securedScrap += Mathf.Max(0, scrap);
            data.successfulExtractions++;
            data.currentExtractionStreak++;
            data.bestExtractionStreak = Mathf.Max(data.bestExtractionStreak, data.currentExtractionStreak);
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
                    return new DeadreachProfileData();

                var json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<DeadreachProfileData>(json);
                return data ?? new DeadreachProfileData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"DEADREACH save could not be loaded; starting fresh. {exception.Message}");
                return new DeadreachProfileData();
            }
        }

#if UNITY_EDITOR
        public static void ResetForDevelopment()
        {
            _cached = new DeadreachProfileData();
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
#endif
    }
}
