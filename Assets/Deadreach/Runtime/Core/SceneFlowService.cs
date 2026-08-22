using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Core
{
    public static class SceneFlowService
    {
        public const string BunkerSceneName = "Bunker_Hub";
        public const string ExpeditionSceneName = "DeadCity_VerticalSlice";

        public static void LoadBunker()
        {
            SceneManager.LoadScene(BunkerSceneName, LoadSceneMode.Single);
        }

        public static void LoadExpedition()
        {
            SceneManager.LoadScene(ExpeditionSceneName, LoadSceneMode.Single);
        }

        public static void ReloadCurrent()
        {
            var active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name, LoadSceneMode.Single);
        }
    }
}
