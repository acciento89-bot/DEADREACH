using System;
using Kamilunavo.Deadreach.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Presentation
{
    /// <summary>
    /// When the authored Meshy Dead City scene root exists, suppress only the earlier primitive
    /// facade dressing. Road readability, lighting and all gameplay systems remain untouched.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Production16MeshyDeadCityRuntimeGuard : MonoBehaviour
    {
        private static bool _hooked;
        private int _framesRemaining = 180;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (_hooked)
                return;

            _hooked = true;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!string.Equals(scene.name, SceneFlowService.ExpeditionSceneName, StringComparison.Ordinal))
                return;

            if (FindFirstObjectByType<Production16MeshyDeadCityRuntimeGuard>() != null)
                return;

            new GameObject("Production16_MeshyDeadCity_Guard").AddComponent<Production16MeshyDeadCityRuntimeGuard>();
        }

        private void LateUpdate()
        {
            if (--_framesRemaining <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (GameObject.Find("P16_Meshy_DeadCity") == null)
                return;

            var legacyLayer = GameObject.Find("P16_DeadCity_VisualLayer");
            if (legacyLayer == null)
                return;

            var removed = 0;
            for (var i = legacyLayer.transform.childCount - 1; i >= 0; i--)
            {
                var child = legacyLayer.transform.GetChild(i);
                if (child != null && child.name.StartsWith("Facade_", StringComparison.Ordinal))
                {
                    Destroy(child.gameObject);
                    removed++;
                }
            }

            if (removed > 0)
                Debug.Log($"DEADREACH 0.16 Meshy city guard removed {removed} legacy primitive facade group(s).");

            Destroy(gameObject);
        }
    }
}
