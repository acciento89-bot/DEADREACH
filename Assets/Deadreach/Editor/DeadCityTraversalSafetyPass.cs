using Kamilunavo.Deadreach.Extraction;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Final traversal gate for the generated Dead City. Production dressing is allowed to
    /// make the street feel blocked and dangerous, but the extraction objective must always
    /// retain a CharacterController-width approach corridor.
    /// </summary>
    public static class DeadCityTraversalSafetyPass
    {
        private const string EnvironmentRootName = "Production_DeadCity_Environment_0_4";
        private const float ApproachHalfWidth = 2.25f;
        private const float ApproachLength = 7.5f;
        private const float SidePadding = 0.75f;

        public static bool Apply()
        {
            var scene = SceneManager.GetActiveScene();
            var extraction = Object.FindFirstObjectByType<ExtractionZone>();
            var environmentRoot = GameObject.Find(EnvironmentRootName);

            if (!scene.IsValid() || extraction == null || environmentRoot == null)
            {
                Debug.LogError("DEADREACH 0.4 traversal safety could not run because the active Dead City scene, ExtractionZone, or production environment root is missing.");
                return false;
            }

            // The truck was the first real-world blocker found during acceptance: its visual
            // and bounds collider reached into the extraction approach. Keep it as set dressing,
            // but move it to an earlier roadside section where it cannot seal the objective.
            var truck = environmentRoot.transform.Find("Wreck_Truck");
            if (truck != null)
            {
                truck.position = new Vector3(5.35f, 0.04f, -10.4f);
                truck.rotation = Quaternion.Euler(0f, -8f, 0f);
            }

            var extractionPosition = extraction.transform.position;
            var keepoutCenter = extractionPosition + Vector3.back * 3f + Vector3.up;
            var keepout = new Bounds(
                keepoutCenter,
                new Vector3(ApproachHalfWidth * 2f, 6f, ApproachLength));

            var moved = 0;
            foreach (Transform child in environmentRoot.transform)
            {
                if (child == null || child.name.StartsWith("Extraction_"))
                    continue;

                var collisionRoot = child.Find("CollisionBounds");
                var collider = collisionRoot != null ? collisionRoot.GetComponent<BoxCollider>() : null;
                if (collider == null || !keepout.Intersects(collider.bounds))
                    continue;

                if (!TryGetRendererBounds(child.gameObject, out var visualBounds))
                    visualBounds = collider.bounds;

                var deltaX = visualBounds.center.x - keepout.center.x;
                var direction = Mathf.Abs(deltaX) > 0.05f
                    ? Mathf.Sign(deltaX)
                    : (child.name.Contains("West") ? -1f : 1f);

                var requiredCenterDistance = keepout.extents.x + visualBounds.extents.x + SidePadding;
                var currentCenterDistance = Mathf.Abs(deltaX);
                var push = Mathf.Max(0.25f, requiredCenterDistance - currentCenterDistance + 0.05f);
                child.position += Vector3.right * direction * push;
                moved++;
            }

            Physics.SyncTransforms();

            var remainingBlockers = 0;
            foreach (Transform child in environmentRoot.transform)
            {
                if (child == null || child.name.StartsWith("Extraction_"))
                    continue;

                var collisionRoot = child.Find("CollisionBounds");
                var collider = collisionRoot != null ? collisionRoot.GetComponent<BoxCollider>() : null;
                if (collider != null && keepout.Intersects(collider.bounds))
                {
                    remainingBlockers++;
                    Debug.LogError($"DEADREACH 0.4 extraction clearance still intersects '{child.name}'.");
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            if (remainingBlockers > 0)
            {
                Debug.LogError($"DEADREACH 0.4 traversal gate FAILED: {remainingBlockers} production blockers still intersect the extraction approach. Scene generation must not be accepted.");
                return false;
            }

            Debug.Log($"DEADREACH 0.4 traversal gate PASSED: extraction approach corridor is clear. Repositioned blockers: {moved}.");
            return true;
        }

        private static bool TryGetRendererBounds(GameObject root, out Bounds bounds)
        {
            bounds = default;
            var hasBounds = false;
            var renderers = root.GetComponentsInChildren<Renderer>(true);

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
    }
}
