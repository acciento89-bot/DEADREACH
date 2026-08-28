using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.16 authored Meshy Dead City layout. The accepted 0.12 gameplay geometry,
    /// mission anchors and colliders remain authoritative. Meshy replaces visible city mass only.
    /// </summary>
    public static class Production16MeshyDeadCityLayout
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";
        private const string RootName = "P16_Meshy_DeadCity";
        private static readonly List<string> MissingAssets = new();

        [MenuItem("DEADREACH/Production 0.16/Validate Meshy Dead City Assets", priority = 12)]
        public static void ValidateAssets()
        {
            MissingAssets.Clear();
            foreach (var stem in Production16MeshyDeadCityGlbBridge.Required)
            {
                if (FindMeshyAsset(stem) == null)
                    MissingAssets.Add(stem);
            }

            var vehicleFound = FindMeshyAsset(Production16MeshyDeadCityGlbBridge.OptionalVehicle) != null;
            if (MissingAssets.Count == 0)
            {
                Debug.Log(
                    $"DEADREACH 0.16 Meshy Dead City assets: PASS ({Production16MeshyDeadCityGlbBridge.Required.Length}/{Production16MeshyDeadCityGlbBridge.Required.Length} required found" +
                    (vehicleFound ? " + VehicleVan optional)." : ")."));
            }
            else
            {
                Debug.LogWarning($"DEADREACH 0.16 Meshy Dead City assets missing: {string.Join(", ", MissingAssets)}");
            }
        }

        [MenuItem("DEADREACH/Production 0.16/Build Meshy Dead City Layout", priority = 13)]
        public static void Build()
        {
            if (!File.Exists(ScenePath))
            {
                Debug.LogError($"DEADREACH 0.16 could not find {ScenePath}.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.16 could not open the Dead City scene.");
                return;
            }

            var previous = FindSceneObject(scene, RootName);
            if (previous != null)
                UnityEngine.Object.DestroyImmediate(previous);

            MissingAssets.Clear();
            var root = new GameObject(RootName);

            ReplaceBuilding(scene, root.transform, "RuinedBuilding_A", "Meshy_Building_01", "Building_Block_01", 90f);
            ReplaceBuilding(scene, root.transform, "CornerBuilding", "Meshy_Building_02", "Building_Block_02", -90f);
            ReplaceBuilding(scene, root.transform, "RuinedBuilding_B", "Meshy_Building_03", "Building_Block_03", 90f);
            ReplaceBuilding(scene, root.transform, "RuinedBuilding_A", "Meshy_Building_04", "Building_Block_04", -90f);
            ReplaceBuilding(scene, root.transform, "CollapsedStorefront", "Meshy_Storefront", "Building_Block_05", 90f);
            ReplaceBuilding(scene, root.transform, "IndustrialBuilding", "Meshy_Industrial", "Building_Block_06", -90f);

            // Large dressing stays outside the validated central traversal corridor.
            PlaceFree("RubbleLarge", "Meshy_Rubble_West", root.transform,
                new Vector3(-6.35f, 0.02f, 5.9f), 18f, new Vector3(3.4f, 2.25f, 3.1f));
            PlaceFree("MilitaryCheckpoint", "Meshy_Checkpoint_East", root.transform,
                new Vector3(6.35f, 0.02f, 13.2f), -90f, new Vector3(3.7f, 2.65f, 3.3f));

            if (FindMeshyAsset(Production16MeshyDeadCityGlbBridge.OptionalVehicle) != null)
            {
                PlaceFree(Production16MeshyDeadCityGlbBridge.OptionalVehicle, "Meshy_AbandonedVan", root.transform,
                    new Vector3(6.2f, 0.02f, -8.6f), -78f, new Vector3(2.15f, 1.75f, 4.25f));
            }

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = root;

            if (MissingAssets.Count == 0)
            {
                Debug.Log("DEADREACH Production 0.16 Meshy Dead City layout built: 6 building visuals replaced, rubble/checkpoint dressed, gameplay collision preserved.");
            }
            else
            {
                Debug.LogWarning(
                    "DEADREACH Production 0.16 Meshy Dead City layout built with legacy visual fallbacks. Missing: " +
                    string.Join(", ", MissingAssets));
            }
        }

        private static void ReplaceBuilding(
            Scene scene,
            Transform root,
            string stem,
            string instanceName,
            string prototypeName,
            float yaw)
        {
            var prototype = FindSceneObject(scene, prototypeName);
            if (prototype == null)
            {
                Debug.LogWarning($"DEADREACH 0.16 could not find legacy collision block {prototypeName}.");
                return;
            }

            if (!TryGetPrototypeBounds(prototype, out var bounds))
            {
                Debug.LogWarning($"DEADREACH 0.16 could not resolve bounds for {prototypeName}.");
                return;
            }

            var placed = Place(
                stem,
                instanceName,
                root,
                new Vector3(bounds.center.x, bounds.min.y, bounds.center.z),
                yaw,
                bounds.size,
                true);

            if (placed == null)
                return;

            // Legacy blocks remain the gameplay collision shell but no longer render.
            foreach (var renderer in prototype.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = false;
            EditorUtility.SetDirty(prototype);
        }

        private static GameObject PlaceFree(
            string stem,
            string instanceName,
            Transform root,
            Vector3 position,
            float yaw,
            Vector3 targetSize)
        {
            return Place(stem, instanceName, root, position, yaw, targetSize, true);
        }

        private static GameObject Place(
            string stem,
            string instanceName,
            Transform parent,
            Vector3 position,
            float yaw,
            Vector3 targetSize,
            bool uniform)
        {
            var prefab = FindMeshyAsset(stem);
            if (prefab == null)
            {
                RegisterMissing(stem);
                return null;
            }

            var scene = parent.gameObject.scene;
            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                instance = UnityEngine.Object.Instantiate(prefab);
            if (instance == null)
            {
                RegisterMissing(stem);
                return null;
            }

            instance.name = instanceName;
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            instance.transform.localScale = Vector3.one;

            FitToLocalBox(instance.transform, targetSize, uniform);
            MoveBottomTo(instance.transform, position.y);
            RemoveImportedColliders(instance);
            SetStaticRecursive(instance);
            return instance;
        }

        private static GameObject FindMeshyAsset(string stem)
        {
            var preparedPath = $"{Production16MeshyDeadCityGlbBridge.PreparedRoot}/{stem}.prefab";
            var prepared = AssetDatabase.LoadAssetAtPath<GameObject>(preparedPath);
            if (prepared != null)
                return prepared;

            var sourcePath = Production16MeshyDeadCityGlbBridge.ResolveSourcePath(stem);
            if (string.IsNullOrWhiteSpace(sourcePath))
                return null;

            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source != null)
                return source;

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(sourcePath))
            {
                if (asset is GameObject gameObject)
                    return gameObject;
            }
            return null;
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

        private static void FitToLocalBox(Transform target, Vector3 targetSize, bool uniform)
        {
            if (!TryGetLocalBounds(target, out var bounds))
                return;

            var size = bounds.size;
            if (size.x < 0.0001f || size.y < 0.0001f || size.z < 0.0001f)
                return;

            if (uniform)
            {
                var factor = Mathf.Min(targetSize.x / size.x, targetSize.y / size.y, targetSize.z / size.z);
                target.localScale = Vector3.one * factor;
            }
            else
            {
                target.localScale = new Vector3(
                    targetSize.x / size.x,
                    targetSize.y / size.y,
                    targetSize.z / size.z);
            }
        }

        private static void MoveBottomTo(Transform target, float worldY)
        {
            if (!TryGetWorldBounds(target, out var bounds))
                return;
            target.position += Vector3.up * (worldY - bounds.min.y);
        }

        private static bool TryGetLocalBounds(Transform root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            var hasBounds = false;
            bounds = default;

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;

                var world = renderer.bounds;
                for (var x = 0; x < 2; x++)
                for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    var point = root.InverseTransformPoint(new Vector3(
                        x == 0 ? world.min.x : world.max.x,
                        y == 0 ? world.min.y : world.max.y,
                        z == 0 ? world.min.z : world.max.z));
                    if (!hasBounds)
                    {
                        bounds = new Bounds(point, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(point);
                    }
                }
            }
            return hasBounds;
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

        private static void RemoveImportedColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(collider);
        }

        private static void SetStaticRecursive(GameObject root)
        {
            var flags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic;
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                GameObjectUtility.SetStaticEditorFlags(transform.gameObject, flags);
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

        private static void RegisterMissing(string stem)
        {
            if (!MissingAssets.Contains(stem))
                MissingAssets.Add(stem);
        }
    }
}
