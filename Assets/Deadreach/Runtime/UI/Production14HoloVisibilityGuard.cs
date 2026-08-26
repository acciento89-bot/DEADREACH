using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Keeps the 0.14 command-center hero visible whenever Overview is active.
    /// Production14CommandCenterUI intentionally hides the hero on the other tabs,
    /// but GameObject.Find cannot find an inactive object when returning to Overview.
    /// This scene-local guard resolves the hero including inactive objects and restores it.
    /// </summary>
    public sealed class Production14HoloVisibilityGuard : MonoBehaviour
    {
        private static bool _hooked;
        private float _nextScan;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (_hooked)
                return;

            _hooked = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.name.Contains("Bunker", System.StringComparison.OrdinalIgnoreCase))
                return;

            var guard = new GameObject("P14_HoloVisibilityGuard");
            guard.AddComponent<Production14HoloVisibilityGuard>();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < _nextScan)
                return;

            _nextScan = Time.unscaledTime + 0.08f;

            if (!TryGetOverviewState(out var overviewActive))
                return;

            var holo = FindHoloRoot();
            if (overviewActive)
            {
                if (holo == null)
                {
                    Production14HoloDiorama.Build();
                    holo = FindHoloRoot();
                }

                if (holo != null && !holo.activeSelf)
                    holo.SetActive(true);
            }
            else if (holo != null && holo.activeSelf)
            {
                holo.SetActive(false);
            }
        }

        private static bool TryGetOverviewState(out bool overviewActive)
        {
            var labels = FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var label in labels)
            {
                if (label == null || label.name != "ScreenTitle")
                    continue;

                var canvas = label.GetComponentInParent<Canvas>();
                if (canvas == null || canvas.name != "P14_CommandCenter_Canvas")
                    continue;

                overviewActive = string.Equals(label.text, "OVERVIEW", System.StringComparison.OrdinalIgnoreCase);
                return true;
            }

            overviewActive = false;
            return false;
        }

        private static GameObject FindHoloRoot()
        {
            var transforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var candidate in transforms)
            {
                if (candidate == null || candidate.name != "P14_HoloDiorama")
                    continue;

                var scene = candidate.gameObject.scene;
                if (scene.IsValid() && scene.isLoaded)
                    return candidate.gameObject;
            }

            return null;
        }
    }
}
