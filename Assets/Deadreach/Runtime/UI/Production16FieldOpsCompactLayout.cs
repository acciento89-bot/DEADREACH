using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.16 presentation polish for the Field Ops HUD.
    /// Keeps the accepted runtime data bindings intact and only compacts/rebalances the generated uGUI layout.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Production16FieldOpsCompactLayout : MonoBehaviour
    {
        private static bool _hooked;
        private GameObject _missionAlert;

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
            if (!string.Equals(scene.name, Kamilunavo.Deadreach.Core.SceneFlowService.ExpeditionSceneName, StringComparison.Ordinal))
                return;

            if (FindFirstObjectByType<Production16FieldOpsCompactLayout>() != null)
                return;

            new GameObject("Production16_FieldOps_CompactLayout").AddComponent<Production16FieldOpsCompactLayout>();
        }

        private IEnumerator Start()
        {
            // Production16FieldOpsUI builds its Canvas one frame after scene load.
            for (var i = 0; i < 12; i++)
            {
                if (TryApply())
                    yield break;
                yield return null;
            }

            Debug.LogWarning("DEADREACH 0.16 compact Field Ops layout could not find the generated HUD canvas.");
        }

        private void LateUpdate()
        {
            // The large central mission banner duplicates information already visible in the mission card.
            // Keep it suppressed even when the base HUD refreshes its transient alert state.
            if (_missionAlert != null && _missionAlert.activeSelf)
                _missionAlert.SetActive(false);
        }

        private bool TryApply()
        {
            var safe = GameObject.Find("P16_FieldOps_Canvas/SafeArea")?.transform as RectTransform;
            if (safe == null)
                return false;

            var status = FindRect(safe, "StatusPanel");
            var mission = FindRect(safe, "MissionPanel");
            var weapon = FindRect(safe, "WeaponPanel");
            if (status == null || mission == null || weapon == null)
                return false;

            // Reduce the primary HUD footprint from ~24% to ~17% of screen height.
            SetAnchors(status, 0.018f, 0.755f, 0.286f, 0.928f);
            SetAnchors(mission, 0.303f, 0.755f, 0.682f, 0.928f);
            SetAnchors(weapon, 0.699f, 0.755f, 0.982f, 0.928f);

            // Mission information already contains the active threat. Remove the duplicate weapon-card threat row.
            var threat = weapon.Find("Threat")?.GetComponent<Text>();
            if (threat != null)
                threat.gameObject.SetActive(false);

            var weaponName = weapon.Find("Weapon")?.GetComponent<Text>();
            if (weaponName != null)
            {
                SetAnchors(weaponName.rectTransform, 0.025f, 0.57f, 0.94f, 0.91f);
                // One deliberate line. At 12 px the longest current family/name/power string fits the card
                // without Unity choosing an ugly mid-name word wrap.
                weaponName.fontSize = 12;
                weaponName.resizeTextForBestFit = false;
                weaponName.horizontalOverflow = HorizontalWrapMode.Overflow;
                weaponName.verticalOverflow = VerticalWrapMode.Truncate;
            }

            var weaponStats = weapon.Find("WeaponStats")?.GetComponent<Text>();
            if (weaponStats != null)
            {
                SetAnchors(weaponStats.rectTransform, 0.035f, 0.30f, 0.94f, 0.56f);
                weaponStats.fontSize = 12;
            }

            var hint = weapon.Find("Hint")?.GetComponent<Text>();
            if (hint != null)
                SetAnchors(hint.rectTransform, 0.035f, 0.06f, 0.94f, 0.29f);

            // Tighten the other cards slightly so the smaller height still breathes.
            SetFontSize(status, "Sector", 13);
            SetFontSize(status, "Vitals", 15);
            SetFontSize(status, "Loot", 12);
            SetFontSize(status, "Ability", 12);
            SetFontSize(mission, "Mission", 14);
            SetFontSize(mission, "Objective", 15);
            SetFontSize(mission, "Secondary", 11);
            SetFontSize(mission, "Navigation", 13);

            _missionAlert = safe.Find("MissionAlert")?.gameObject;
            if (_missionAlert != null)
                _missionAlert.SetActive(false);

            var hazard = FindRect(safe, "HazardAlert");
            if (hazard != null)
                SetAnchors(hazard, 0.36f, 0.685f, 0.64f, 0.735f);

            var boss = FindRect(safe, "BossBar");
            if (boss != null)
                SetAnchors(boss, 0.32f, 0.868f, 0.68f, 0.932f);

            Debug.Log("DEADREACH 0.16 Field Ops compact presentation applied: reduced HUD footprint, duplicate mission alert removed, weapon card forced to a clean single line.");
            return true;
        }

        private static RectTransform FindRect(Transform parent, string name)
        {
            return parent.Find(name) as RectTransform;
        }

        private static void SetFontSize(Transform parent, string childName, int size)
        {
            var text = parent.Find(childName)?.GetComponent<Text>();
            if (text != null)
                text.fontSize = size;
        }

        private static void SetAnchors(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
