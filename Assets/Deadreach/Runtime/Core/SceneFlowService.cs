using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Core
{
    public static class SceneFlowService
    {
        public const string BunkerSceneName = "Bunker_Hub";
        public const string ExpeditionSceneName = "DeadCity_VerticalSlice";

        public static bool LoadBunker() => Load(BunkerSceneName);
        public static bool LoadExpedition() => Load(ExpeditionSceneName);

        public static bool ReloadCurrent()
        {
            var active = SceneManager.GetActiveScene();
            return Load(active.name);
        }

        private static bool Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"DEADREACH scene '{sceneName}' is not available in Build Settings. Generate the complete vertical slice first.");
                return false;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return true;
        }
    }
}
