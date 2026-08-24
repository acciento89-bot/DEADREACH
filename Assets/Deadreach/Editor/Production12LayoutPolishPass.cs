using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.12b layout-polish pass. Q-WARD is intentionally untouched because its real-Unity
    /// gate is already accepted. This pass opens the generated Dead City scene and only declutters
    /// Transit Collapse, Industrial Spill and Blackout Plaza after the normal 0.12 authoring pass.
    /// </summary>
    public static class Production12LayoutPolishPass
    {
        private const string DeadCityScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";
        private const string BunkerScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";

        public static bool Apply()
        {
            if (!File.Exists(DeadCityScenePath))
            {
                Debug.LogError($"DEADREACH 0.12 layout polish could not find {DeadCityScenePath}.");
                return false;
            }

            var scene = EditorSceneManager.OpenScene(DeadCityScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.12 layout polish could not open the generated Dead City scene.");
                return false;
            }

            PolishTransit(scene);
            PolishIndustrial(scene);
            PolishBlackout(scene);

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            if (File.Exists(BunkerScenePath))
                EditorSceneManager.OpenScene(BunkerScenePath, OpenSceneMode.Single);

            Debug.Log("DEADREACH Production 0.12 layout polish applied: Transit / Industrial / Blackout objective arenas and traversal lanes decluttered; Q-WARD unchanged.");
            return true;
        }

        private static void PolishTransit(Scene scene)
        {
            SetPose(scene, "TC_Wreck_Truck", new Vector3(3.8f, 0.04f, 5.8f), 8f);
            SetPose(scene, "TC_Wreck_Sports", new Vector3(-3.8f, 0.04f, 12.6f), -10f);
            SetPose(scene, "TC_Wreck_Pickup", new Vector3(9.2f, 0.04f, 2.1f), 92f);
            SetPose(scene, "TC_Barrier_A", new Vector3(4f, 0.05f, -0.8f), -8f);
            SetPosition(scene, "TC_Wheels", new Vector3(4.7f, 0.03f, 10.8f));
            SetPosition(scene, "TC_Pallet", new Vector3(-4.7f, 0.03f, 7.7f));

            SetPosition(scene, "TC_Objective_01", new Vector3(10f, 0.12f, 3.2f));
            SetPosition(scene, "TC_Objective_02", new Vector3(0f, 0.12f, 9f));
            SetPosition(scene, "TC_Objective_03", new Vector3(-10.2f, 0.12f, 3.2f));
            SetPosition(scene, "TC_Objective_04", new Vector3(0f, 0.12f, 16.2f));
            SetPosition(scene, "TC_Objective_05", new Vector3(0f, 0.12f, 1f));

            SetPosition(scene, "TC_ArcField", new Vector3(-3.8f, 0f, 5.2f));
            SetPosition(scene, "TC_ArcLight", new Vector3(-3.8f, 2.2f, 5.2f));
        }

        private static void PolishIndustrial(Scene scene)
        {
            SetPosition(scene, "IS_Barrel_A", new Vector3(-4.6f, 0.03f, 10.2f));
            SetPosition(scene, "IS_Barrel_B", new Vector3(-4.4f, 0.03f, 11.2f));
            SetPosition(scene, "IS_Barrel_C", new Vector3(4.6f, 0.03f, 5.9f));
            SetPose(scene, "IS_ServiceTruck", new Vector3(12f, 0.04f, 3.2f), 92f);
            SetPose(scene, "IS_Barrier", new Vector3(4.2f, 0.05f, 15.8f), -5f);

            SetPosition(scene, "IS_Objective_01", new Vector3(-10.2f, 0.12f, 3.2f));
            SetPosition(scene, "IS_Objective_02", new Vector3(0f, 0.12f, 4.4f));
            SetPosition(scene, "IS_Objective_03", new Vector3(0f, 0.12f, 12f));
            SetPosition(scene, "IS_Objective_04", new Vector3(0f, 0.12f, 16.8f));
            SetPosition(scene, "IS_Objective_05", new Vector3(8.2f, 0.12f, 3.2f));

            SetPosition(scene, "IS_ChemicalSpill", new Vector3(-3.8f, 0f, 8.2f));
            SetPosition(scene, "IS_SpillLight", new Vector3(-3.8f, 2f, 8.2f));
            SetPosition(scene, "IS_Fireline", new Vector3(3.8f, 0f, 6.2f));
        }

        private static void PolishBlackout(Scene scene)
        {
            SetPosition(scene, "BP_Container_West", new Vector3(-10.2f, 0.02f, 5.1f));
            SetPose(scene, "BP_Wreck_A", new Vector3(4f, 0.04f, 7.2f), -8f);
            SetPose(scene, "BP_Wreck_B", new Vector3(-4f, 0.04f, 14.8f), 8f);
            SetPose(scene, "BP_Barrier_A", new Vector3(-4.3f, 0.05f, 2.1f), 5f);
            SetPose(scene, "BP_Barrier_B", new Vector3(4.3f, 0.05f, 13.6f), -5f);
            SetPosition(scene, "BP_Wheels", new Vector3(-4.7f, 0.03f, 10f));

            SetPosition(scene, "BP_Objective_01", new Vector3(-10.4f, 0.12f, 3.2f));
            SetPosition(scene, "BP_Objective_02", new Vector3(0f, 0.12f, 10.5f));
            SetPosition(scene, "BP_Objective_03", new Vector3(9.8f, 0.12f, 3.2f));
            SetPosition(scene, "BP_Objective_04", new Vector3(0f, 0.12f, 17.4f));
            SetPosition(scene, "BP_Objective_05", new Vector3(0f, 0.12f, 5.5f));

            SetPosition(scene, "BP_Loot_04", new Vector3(4.4f, 0.58f, 9f));
            SetPosition(scene, "BP_Enemy_05", new Vector3(-4.2f, 1f, 8f));
            SetPosition(scene, "BP_Enemy_06", new Vector3(4.2f, 1f, 12.2f));

            SetPosition(scene, "BP_ArcGrid", new Vector3(-5.6f, 0f, 3.2f));
            SetPosition(scene, "BP_Firebreak", new Vector3(3.8f, 0f, 17.6f));
            SetPosition(scene, "BP_PurpleEmergency", new Vector3(-5.6f, 2.4f, 3.2f));
            SetPosition(scene, "BP_RedEmergency", new Vector3(3.8f, 2.2f, 17.6f));
        }

        private static void SetPosition(Scene scene, string objectName, Vector3 position)
        {
            var target = FindSceneTransform(scene, objectName);
            if (target == null)
            {
                Debug.LogWarning($"DEADREACH 0.12 layout polish could not find {objectName}.");
                return;
            }

            target.position = position;
        }

        private static void SetPose(Scene scene, string objectName, Vector3 position, float yaw)
        {
            var target = FindSceneTransform(scene, objectName);
            if (target == null)
            {
                Debug.LogWarning($"DEADREACH 0.12 layout polish could not find {objectName}.");
                return;
            }

            target.position = position;
            target.rotation = Quaternion.Euler(0f, yaw, 0f);
            RefreshCollisionBounds(target);
        }

        private static void RefreshCollisionBounds(Transform target)
        {
            var collision = target.Find("CollisionBounds")?.GetComponent<BoxCollider>();
            if (collision == null)
                return;

            var renderers = target.GetComponentsInChildren<Renderer>(true);
            var hasBounds = false;
            var localBounds = default(Bounds);

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;

                var bounds = renderer.bounds;
                for (var x = 0; x < 2; x++)
                for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    var worldPoint = new Vector3(
                        x == 0 ? bounds.min.x : bounds.max.x,
                        y == 0 ? bounds.min.y : bounds.max.y,
                        z == 0 ? bounds.min.z : bounds.max.z);
                    var localPoint = target.InverseTransformPoint(worldPoint);
                    if (!hasBounds)
                    {
                        localBounds = new Bounds(localPoint, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(localPoint);
                    }
                }
            }

            if (!hasBounds)
                return;

            collision.center = localBounds.center;
            collision.size = localBounds.size;
        }

        private static Transform FindSceneTransform(Scene scene, string objectName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var candidate in transforms)
                {
                    if (candidate != null && candidate.name == objectName)
                        return candidate;
                }
            }

            return null;
        }
    }
}
