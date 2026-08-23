using Kamilunavo.Deadreach.Persistence;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production06DebugTools
    {
        [MenuItem("DEADREACH/Dev/0.6 Select Sector 01 Level 01", priority = 100)]
        private static void Sector01() => Select(1);

        [MenuItem("DEADREACH/Dev/0.6 Select Sector 02 Level 11", priority = 101)]
        private static void Sector02() => Select(11);

        [MenuItem("DEADREACH/Dev/0.6 Select Sector 03 Level 21", priority = 102)]
        private static void Sector03() => Select(21);

        [MenuItem("DEADREACH/Dev/0.6 Select Sector 04 Level 31", priority = 103)]
        private static void Sector04() => Select(31);

        [MenuItem("DEADREACH/Dev/0.6 Select Sector 05 Level 41", priority = 104)]
        private static void Sector05() => Select(41);

        [MenuItem("DEADREACH/Dev/0.6 Select Boss Tier 1 Level 10", priority = 110)]
        private static void Boss01() => Select(10);

        [MenuItem("DEADREACH/Dev/0.6 Select Boss Tier 2 Level 20", priority = 111)]
        private static void Boss02() => Select(20);

        [MenuItem("DEADREACH/Dev/0.6 Select Boss Tier 3 Level 30", priority = 112)]
        private static void Boss03() => Select(30);

        [MenuItem("DEADREACH/Dev/0.6 Select Boss Tier 4 Level 40", priority = 113)]
        private static void Boss04() => Select(40);

        [MenuItem("DEADREACH/Dev/0.6 Select Boss Tier 5 Level 50", priority = 114)]
        private static void Boss05() => Select(50);

        private static void Select(int level)
        {
            var target = Mathf.Clamp(level, 1, SaveService.MaxCampaignLevel);
            var data = SaveService.Data;
            data.highestUnlockedLevel = Mathf.Max(data.highestUnlockedLevel, target);
            data.selectedLevel = target;
            SaveService.Save();
            Debug.Log($"DEADREACH 0.6 DEV: Level {target:00} selected and unlocked for direct visual/runtime testing.");
        }
    }
}
