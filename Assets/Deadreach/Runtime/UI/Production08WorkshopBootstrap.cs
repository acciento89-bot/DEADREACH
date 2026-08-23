using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.8 Workshop lifecycle bridge.
    /// RuntimeInitializeOnLoadMethod(AfterSceneLoad) only covers the initial player load;
    /// the Bunker UI is recreated after every expedition. Keep a single sceneLoaded hook
    /// so each newly created Bunker Command Center receives the Workshop extension again.
    /// </summary>
    public static class Production08WorkshopBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneHook()
        {
            // Domain-reload-disabled editor sessions can retain static delegates, so always
            // remove before adding to guarantee exactly one subscription.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInitialScene()
        {
            EnsureWorkshop();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureWorkshop();
        }

        private static void EnsureWorkshop()
        {
            var bunker = Object.FindFirstObjectByType<BunkerCommandCenterUI>();
            if (bunker == null || bunker.GetComponent<Production08WorkshopUI>() != null)
                return;

            bunker.gameObject.AddComponent<Production08WorkshopUI>();
        }
    }
}
