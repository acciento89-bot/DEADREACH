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
            // Keep the wreck identity, but park the large bodies on the lane edges instead of across
            // objective circles and the central route.
            SetPose(scene, "TC_Wreck_Truck", new Vector3(3.8f, 0.04f, 5.8f), 8f);
            SetPose(scene, "TC_Wreck_Sports", new Vector3(-3.8f, 0.04f, 12.6f), -10f);
            SetPose(scene, "TC_Wreck_Pickup", new Vector3(9.2f, 0.04f, 2.1f), 92f);
            SetPose(scene, "TC_Barrier_A", new Vector3(4f, 0.05f, -0.8f), -8f);
            SetPosition(scene, "TC_Wheels", new Vector3(4.7f, 0.03f, 10.8f));
            SetPosition(scene, "TC_Pallet", new Vector3(-4.7f, 0.03f, 7.7f));

            // Each mission family gets an intentionally open authored point.
            SetPosition(scene, "TC_Objective_01", new Vector3(10f, 0.12f, 3.2f));      // BLACKSITE terminal
            SetPosition(scene, "TC_Objective_02", new Vector3(0f, 0.12f, 9f));        // HOLDOUT arena
            SetPosition(scene, "TC_Objective_03", new Vector3(-10.2f, 0.12f, 3.2f));  // RECOVERY
            SetPosition(scene, "TC_Objective_04", new Vector3(0f, 0.12f, 16.2f));     // BLACKSITE vault
            SetPosition(scene, "TC_Objective_05", new Vector3(0f, 0.12f, 1f));        // PURGE

            SetPosition(scene, "TC_ArcField", new Vector3(-3.8f, 0f, 5.2f));
            SetPosition(scene, "TC_ArcLight", new Vector3(-3.8f, 2.2f, 5.2f));
        }

        private static void PolishIndustrial(Scene scene)
        {
            // Preserve the container-channel silhouette while clearing the actual center lane.
            SetPosition(scene, "IS_Barrel_A", new Vector3(-4.6f, 0.03f, 10.2f));
            SetPosition(scene, "IS_Barrel_B", new Vector3(-4.4f, 0.03f, 11.2f));
            SetPosition(scene, "IS_Barrel_C", new Vector3(4.6f, 0.03f, 5.9f));
            SetPose(scene, "IS_ServiceTruck", new Vector3(12f, 0.04f, 3.2f), 92f);
            SetPose(scene, "IS_Barrier", new Vector3(4.2f, 0.05f, 15.8f), -5f);

            SetPosition(scene, "IS_Objective_01", new Vector3(-10.2f, 0.12f, 3.2f)); // BLACKSITE terminal
            SetPosition(scene, "IS_Objective_02", new Vector3(0f, 0.12f, 4.4f));     // HOLDOUT arena
            SetPosition(scene, "IS_Objective_03", new Vector3(0f, 0.12f, 12f));      // RECOVERY
            SetPosition(scene, "IS_Objective_04", new Vector3(0f, 0.12f, 16.8f));    // BLACKSITE vault
            SetPosition(scene, "IS_Objective_05", new Vector3(8.2f, 0.12f, 3.2f));   // PURGE

            SetPosition(scene, "IS_ChemicalSpill", new Vector3(-3.8f, 0f, 8.2f));
            SetPosition(scene, "IS_SpillLight", new Vector3(-3.8f, 2f, 8.2f));
            SetPosition(scene, "IS_Fireline", new Vector3(3.8f, 0f, 6.2f));
        }

        private static void PolishBlackout(Scene scene)
        {
            // Blackout gets the strongest change: keep the wrecked-plaza look but create a large clear
            // center arena for HOLDOUT / combat instead of surrounding the marker with hard colliders.
            SetPosition(scene, "BP_Container_West", new Vector3(-10.2f, 0.02f, 5.1f));
            SetPose(scene, "BP_Wreck_A", new Vector3(4f, 0.04f, 7.2f), -8f);
            SetPose(scene, "BP_Wreck_B", new Vector3(-4f, 0.04f, 14.8f), 8f);
            SetPose(scene, "BP_Barrier_A", new Vector3(-4.3f, 0.05f, 2.1f), 5f);
            SetPose(scene, "BP_Barrier_B", new Vector3(4.3f, 0.05f, 13.6f), -5f);
            SetPosition(scene, "BP_Wheels", new Vector3(-4.7f, 0.03f, 10f));

            SetPosition(scene, "BP_Objective_01", new Vector3(-10.4f, 0.12f, 3.2f)); // BLACKSITE terminal
            SetPosition(scene, "BP_Objective_02", new Vector3(0f, 0.12f, 10.5f));    // HOLDOUT open plaza
            SetPosition(scene, "BP_Objective_03", new Vector3(9.8f, 0.12f, 3.2f));   // RECOVERY
            SetPosition(scene, "BP_Objective_04", new Vector3(0f, 0.12f, 17.4f));    // BLACKSITE vault
            SetPosition(scene, "BP_Objective_05", new Vector3(0f, 0.12f, 5.5f));     // PURGE

            // Pull non-blocking pickups/spawns out of the holdout circle so the arena reads clearly.
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
