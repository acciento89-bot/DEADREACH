using Kamilunavo.Deadreach.Persistence;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production05DebugTools
    {
        [MenuItem("DEADREACH/Dev/0.5 Unlock Through Boss Level 10", priority = 90)]
        private static void UnlockBoss10()
        {
            UnlockThrough(10);
        }

        [MenuItem("DEADREACH/Dev/0.5 Unlock Full Campaign 50", priority = 91)]
        private static void UnlockFullCampaign()
        {
            UnlockThrough(50);
        }

        [MenuItem("DEADREACH/Dev/0.5 Select Boss Level 10", priority = 92)]
        private static void SelectBoss10()
        {
            var data = SaveService.Data;
            data.highestUnlockedLevel = Mathf.Max(data.highestUnlockedLevel, 10);
            data.selectedLevel = 10;
            SaveService.Save();
            Debug.Log("DEADREACH 0.5 DEV: Boss Level 10 selected. Press Play -> Deploy to validate the first boss gate.");
        }

        private static void UnlockThrough(int level)
        {
            var data = SaveService.Data;
            var target = Mathf.Clamp(level, 1, SaveService.MaxCampaignLevel);
            data.highestUnlockedLevel = Mathf.Max(data.highestUnlockedLevel, target);
            data.selectedLevel = target;
            SaveService.Save();
            Debug.Log($"DEADREACH 0.5 DEV: campaign unlocked through Level {target:00} and selected for testing.");
        }
    }
}
