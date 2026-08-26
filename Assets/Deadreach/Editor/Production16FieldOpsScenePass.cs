using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Deterministic scene integration for Production 0.16. Keeps the stable 0.12 expedition
    /// gameplay intact and replaces only the legacy PrototypeHud presentation owner.
    /// </summary>
    public static class Production16FieldOpsScenePass
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";

        [MenuItem("DEADREACH/Production 0.16/Apply Dead City Field Ops", priority = 1)]
        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError($"DEADREACH 0.16 could not open {ScenePath}.");
                return;
            }

            var runSession = Object.FindFirstObjectByType<RunSession>();
            if (runSession == null)
            {
                Debug.LogError("DEADREACH 0.16 requires the existing RunSession in DeadCity_VerticalSlice.");
                return;
            }

            var legacyHuds = Object.FindObjectsByType<PrototypeHud>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var legacy in legacyHuds)
            {
                if (legacy == null)
                    continue;
                legacy.enabled = false;
                EditorUtility.SetDirty(legacy);
            }

            var fieldOps = Object.FindFirstObjectByType<Production16FieldOpsUI>();
            if (fieldOps == null)
                fieldOps = runSession.gameObject.AddComponent<Production16FieldOpsUI>();
            fieldOps.enabled = true;
            EditorUtility.SetDirty(fieldOps);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = runSession.gameObject;

            Debug.Log($"DEADREACH Production 0.16 Field Ops applied: production HUD installed, {legacyHuds.Length} PrototypeHud component(s) disabled, stable expedition gameplay preserved.");
        }

        [MenuItem("DEADREACH/Production 0.16/Validate Dead City Field Ops", priority = 2)]
        public static void Validate()
        {
            var active = SceneManager.GetActiveScene();
            if (!active.IsValid() || !string.Equals(active.name, SceneFlowService.ExpeditionSceneName, System.StringComparison.Ordinal))
            {
                Debug.LogError("DEADREACH 0.16 validation requires DeadCity_VerticalSlice to be open.");
                return;
            }

            var runSession = Object.FindFirstObjectByType<RunSession>();
            var fieldOps = Object.FindFirstObjectByType<Production16FieldOpsUI>();
            var legacyHuds = Object.FindObjectsByType<PrototypeHud>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            var legacyEnabled = 0;
            foreach (var legacy in legacyHuds)
            {
                if (legacy != null && legacy.enabled)
                    legacyEnabled++;
            }

            if (runSession == null || fieldOps == null || !fieldOps.enabled || legacyEnabled > 0)
            {
                Debug.LogError($"DEADREACH 0.16 FIELD OPS CHECK: FAIL // RunSession={(runSession != null)} // FieldOps={(fieldOps != null && fieldOps.enabled)} // enabled legacy HUDs={legacyEnabled}.");
                return;
            }

            Debug.Log("DEADREACH 0.16 FIELD OPS CHECK: PASS // production uGUI HUD owns Dead City presentation and legacy PrototypeHud is disabled.");
        }
    }
}
