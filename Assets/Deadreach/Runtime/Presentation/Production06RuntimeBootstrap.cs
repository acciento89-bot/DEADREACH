using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Presentation
{
    public static class Production06RuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var session = Object.FindFirstObjectByType<RunSession>();
            if (session != null)
            {
                Ensure<Production06SectorIdentity>(session.gameObject);
                Ensure<Production06BossPresentation>(session.gameObject);
                Ensure<BossRewardPresentationUI>(session.gameObject);
            }

            var bunker = Object.FindFirstObjectByType<BunkerCommandCenterUI>();
            if (bunker != null)
            {
                Ensure<BunkerMobileResponsiveUI>(bunker.gameObject);
                Ensure<BunkerBossDebriefUI>(bunker.gameObject);
            }
        }

        private static void Ensure<T>(GameObject root) where T : Component
        {
            if (root.GetComponent<T>() == null)
                root.AddComponent<T>();
        }
    }
}
