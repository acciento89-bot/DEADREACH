using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.16 authored placement correction for the imported Meshy city kit.
    /// Keeps the legacy collision blocks authoritative while giving each visual asset a sane
    /// world-space envelope and pushing visible mass away from the central combat corridor.
    /// </summary>
    public static class Production16MeshyDeadCityLayoutPolish
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";
        private const string RootName = "P16_Meshy_DeadCity";

        [MenuItem("DEADREACH/Production 0.16/Polish Meshy Dead City Layout", priority = 14)]
        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.16 layout polish could not open DeadCity_VerticalSlice.");
                return;
            }

            var rootObject = FindSceneObject(scene, RootName);
            if (rootObject == null)
            {
                Debug.LogError("DEADREACH 0.16 layout polish requires P16_Meshy_DeadCity. Run Integrate Meshy Dead City first.");
                return;
            }

            var root = rootObject.transform;

            // Buildings remain tied to their validated collision blocks, but are deliberately
            // smaller than the raw prototype boxes and pushed away from the road centre.
            AdjustBuilding(scene, root, "Meshy_Building_01", "Building_Block_01", 90f, 8.6f, 6.2f, 6.4f, 0.85f, -0.25f);
            AdjustBuilding(scene, root, "Meshy_Building_02", "Building_Block_02", -90f, 7.2f, 6.5f, 6.5f, 1.05f, 0.15f);
            AdjustBuilding(scene, root, "Meshy_Building_03", "Building_Block_03", 90f, 8.0f, 6.2f, 6.4f, 0.95f, 0.35f);
            AdjustBuilding(scene, root, "Meshy_Building_04", "Building_Block_04", -90f, 7.8f, 6.0f, 6.2f, 1.10f, -0.30f);
            AdjustBuilding(scene, root, "Meshy_Storefront", "Building_Block_05", 90f, 4.15f, 5.7f, 5.1f, 0.80f, 0.55f);
            AdjustBuilding(scene, root, "Meshy_Industrial", "Building_Block_06", -90f, 4.45f, 7.1f, 6.2f, 0.95f, -0.45f);

            // Hero dressing moves further to the edges so the north encounter reads instead of
            // becoming one dense pile of silhouettes.
            AdjustFree(root, "Meshy_Rubble_West", new Vector3(-7.45f, 0.02f, 6.9f), 12f, new Vector3(3.25f, 1.75f, 3.0f));
            AdjustFree(root, "Meshy_Checkpoint_East", new Vector3(7.45f, 0.02f, 14.15f), -90f, new Vector3(3.45f, 2.35f, 3.0f));
            AdjustFree(root, "Meshy_AbandonedVan", new Vector3(5.55f, 0.02f, -9.2f), -72f, new Vector3(2.0f, 1.65f, 3.9f));

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = rootObject;

            Debug.Log("DEADREACH 0.16 Meshy Dead City layout polish PASS: building envelopes reduced, facades pushed off the combat corridor, north dressing decluttered.");
        }

        private static void AdjustBuilding(
            Scene scene,
            Transform root,
            string instanceName,
            string prototypeName,
            float yaw,
            float maxHeight,
            float maxWidth,
            float maxDepth,
            float outwardOffset,
            float zOffset)
        {
            var visual = FindChildRecursive(root, instanceName);
            var prototype = FindSceneObject(scene, prototypeName);
            if (visual == null || prototype == null || !TryGetPrototypeBounds(prototype, out var anchor))
                return;

            var side = Mathf.Approximately(anchor.center.x, 0f) ? 1f : Mathf.Sign(anchor.center.x);
            var targetPosition = new Vector3(
                anchor.center.x + side * outwardOffset,
                anchor.min.y,
                anchor.center.z + zOffset);

            visual.position = targetPosition;
            visual.rotation = Quaternion.Euler(0f, yaw, 0f);
            visual.localScale = Vector3.one;

            FitToWorldEnvelope(visual, maxHeight, maxWidth, maxDepth);
            MoveBottomTo(visual, anchor.min.y);
        }

        private static void AdjustFree(Transform root, string name, Vector3 position, float yaw, Vector3 envelope)
        {
            var visual = FindChildRecursive(root, name);
            if (visual == null)
                return;

            visual.position = position;
            visual.rotation = Quaternion.Euler(0f, yaw, 0f);
            visual.localScale = Vector3.one;
            FitToWorldEnvelope(visual, envelope.y, envelope.x, envelope.z);
            MoveBottomTo(visual, position.y);
        }

        private static void FitToWorldEnvelope(Transform target, float maxHeight, float maxWidth, float maxDepth)
        {
            if (!TryGetWorldBounds(target, out var bounds))
                return;

            var size = bounds.size;
            if (size.x < 0.0001f || size.y < 0.0001f || size.z < 0.0001f)
                return;

            var factor = Mathf.Min(
                maxHeight / size.y,
                maxWidth / size.x,
                maxDepth / size.z);

            target.localScale = Vector3.one * Mathf.Max(0.01f, factor);
        }

        private static void MoveBottomTo(Transform target, float worldY)
        {
            if (!TryGetWorldBounds(target, out var bounds))
                return;
            target.position += Vector3.up * (worldY - bounds.min.y);
        }

        private static bool TryGetPrototypeBounds(GameObject prototype, out Bounds bounds)
        {
            var colliders = prototype.GetComponentsInChildren<Collider>(true);
            var hasBounds = false;
            bounds = default;
            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;
                if (!hasBounds)
                {
                    bounds = collider.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            if (hasBounds)
                return true;

            return TryGetWorldBounds(prototype.transform, out bounds);
        }

        private static bool TryGetWorldBounds(Transform root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            var hasBounds = false;
            bounds = default;
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
            return hasBounds;
        }

        private static Transform FindChildRecursive(Transform root, string name)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform != null && transform.name == name)
                    return transform;
            }
            return null;
        }

        private static GameObject FindSceneObject(Scene scene, string name)
        {
            foreach (var sceneRoot in scene.GetRootGameObjects())
            {
                foreach (var transform in sceneRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (transform != null && transform.name == name)
                        return transform.gameObject;
                }
            }
            return null;
        }
    }
}
