using System;
using System.Collections;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Presentation
{
    /// <summary>
    /// Production 0.14 expedition safety net.
    ///
    /// ProductionVisualBinder intentionally hides the prototype capsule before instantiating the
    /// authored operator. A recovered/imported operator prefab can still contain disabled body
    /// renderers, which would leave the player completely invisible. This guard makes body renderers
    /// visible without touching the artist-authored firearm selection. If no usable body renderer
    /// exists, the original prototype renderer is restored as a last-resort visible fallback.
    /// </summary>
    [DefaultExecutionOrder(250)]
    public sealed class Production14PlayerVisibilityGuard : MonoBehaviour
    {
        private static readonly string[] WeaponTokens =
        {
            "rifle", "smg", "pistol", "shotgun", "gun", "firearm",
            "axe", "knife", "guitar", "spear", "woodenbat", "baseballbat"
        };

        private static bool _hooked;
        private PlayerMotor _player;
        private ProductionVisualBinder _binder;
        private float _nextWatchdogAt;

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
            if (!string.Equals(scene.name, SceneFlowService.ExpeditionSceneName, StringComparison.Ordinal))
                return;

            if (FindFirstObjectByType<Production14PlayerVisibilityGuard>() != null)
                return;

            new GameObject("Systems_P14PlayerVisibilityGuard")
                .AddComponent<Production14PlayerVisibilityGuard>();
        }

        private IEnumerator Start()
        {
            // Let the normal ProductionVisualBinder.Start() complete first.
            yield return null;
            yield return null;
            RepairVisibility(logSuccess: true);
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextWatchdogAt)
                return;

            _nextWatchdogAt = Time.unscaledTime + 1f;
            if (!HasVisiblePlayerBody())
                RepairVisibility(logSuccess: false);
        }

        private void RepairVisibility(bool logSuccess)
        {
            _player ??= FindFirstObjectByType<PlayerMotor>();
            if (_player == null)
            {
                Debug.LogError("DEADREACH 0.14 player-visibility guard could not find PlayerMotor in expedition scene.");
                return;
            }

            _binder ??= _player.GetComponent<ProductionVisualBinder>();
            if (_binder == null)
            {
                _binder = _player.gameObject.AddComponent<ProductionVisualBinder>();
                _binder.Configure(ProductionVisualRole.Survivor);
            }

            _binder.BindNow();
            var visual = _binder.VisualInstance;
            var bodyRendererCount = 0;

            if (visual != null)
            {
                if (!visual.activeSelf)
                    visual.SetActive(true);

                foreach (var renderer in visual.GetComponentsInChildren<Renderer>(true))
                {
                    if (renderer == null || IsWeaponRenderer(renderer.transform, visual.transform))
                        continue;

                    ActivatePath(renderer.transform, visual.transform);
                    renderer.enabled = true;
                    bodyRendererCount++;
                }
            }

            if (bodyRendererCount > 0 && HasVisibleProductionBody())
            {
                var prototype = _player.GetComponent<Renderer>();
                if (prototype != null)
                    prototype.enabled = false;

                if (logSuccess)
                {
                    Debug.Log(
                        $"DEADREACH 0.14 player visibility PASS: production operator body visible " +
                        $"({bodyRendererCount} body renderer(s)); prototype fallback hidden.");
                }
                return;
            }

            // Never leave the player invisible. The root Player_Survivor is the original capsule
            // produced by the stable scene builder, so restoring only its own Renderer is safe and
            // does not mount or alter any weapon geometry.
            var fallback = _player.GetComponent<Renderer>();
            if (fallback != null)
            {
                fallback.enabled = true;
                Debug.LogWarning(
                    "DEADREACH 0.14 production operator body was unavailable/invisible. " +
                    "Restored prototype player renderer as a visible fallback; embedded firearm setup was left untouched.");
            }
            else
            {
                Debug.LogError(
                    "DEADREACH 0.14 player visibility FAILED: no visible production body and no prototype Renderer fallback exists.");
            }
        }

        private bool HasVisiblePlayerBody()
        {
            if (_player == null)
                return false;

            if (HasVisibleProductionBody())
                return true;

            var fallback = _player.GetComponent<Renderer>();
            return fallback != null && fallback.enabled && fallback.gameObject.activeInHierarchy;
        }

        private bool HasVisibleProductionBody()
        {
            var visual = _binder != null ? _binder.VisualInstance : null;
            if (visual == null || !visual.activeInHierarchy)
                return false;

            foreach (var renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || IsWeaponRenderer(renderer.transform, visual.transform))
                    continue;

                if (renderer.enabled && renderer.gameObject.activeInHierarchy)
                    return true;
            }

            return false;
        }

        private static bool IsWeaponRenderer(Transform rendererTransform, Transform visualRoot)
        {
            var current = rendererTransform;
            var depth = 0;
            while (current != null && depth < 8)
            {
                var normalized = NormalizeName(current.name);
                foreach (var token in WeaponTokens)
                {
                    if (normalized.Contains(token))
                        return true;
                }

                if (current == visualRoot)
                    break;

                current = current.parent;
                depth++;
            }

            return false;
        }

        private static void ActivatePath(Transform leaf, Transform root)
        {
            var current = leaf;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                    current.gameObject.SetActive(true);

                if (current == root)
                    break;

                current = current.parent;
            }
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var chars = value.ToCharArray();
            var buffer = new char[chars.Length];
            var length = 0;
            foreach (var character in chars)
            {
                if (!char.IsLetterOrDigit(character))
                    continue;

                buffer[length++] = char.ToLowerInvariant(character);
            }

            return new string(buffer, 0, length);
        }
    }
}
