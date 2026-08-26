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
    /// Production 0.15 bunker dressing pass. Keeps the accepted 0.14 UI/camera/gameplay intact and
    /// replaces only visible prototype bunker dressing when the corresponding Meshy asset exists.
    /// Missing Meshy assets fall back to the existing scene instead of breaking the Bunker.
    /// </summary>
    public static class Production15MeshyBunkerLayout
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        private const string AssetRoot = "Assets/Deadreach/Art/Production15/Meshy/Bunker";
        private const string RootName = "P15_Meshy_Bunker";

        private static readonly List<string> MissingAssets = new();

        [MenuItem("DEADREACH/Production 0.15/Build Meshy Bunker Layout", priority = 1)]
        public static void Build()
        {
            if (!File.Exists(ScenePath))
            {
                Debug.LogError($"DEADREACH 0.15 could not find {ScenePath}.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.15 could not open the Bunker scene.");
                return;
            }

            var previous = FindSceneObject(scene, RootName);
            if (previous != null)
                UnityEngine.Object.DestroyImmediate(previous);

            MissingAssets.Clear();
            var root = new GameObject(RootName);

            BuildArchitecture(scene, root.transform);
            BuildStations(scene, root.transform);
            BuildUtilityProps(root.transform);
            BuildLighting(root.transform);

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = root;

            if (MissingAssets.Count == 0)
            {
                Debug.Log("DEADREACH Production 0.15 Meshy Bunker layout built: all expected custom assets are present.");
            }
            else
            {
                Debug.LogWarning(
                    $"DEADREACH Production 0.15 Meshy Bunker layout built with existing-scene fallbacks. " +
                    $"Missing: {string.Join(", ", MissingAssets)}");
            }
        }

        [MenuItem("DEADREACH/Production 0.15/Validate Meshy Bunker Assets", priority = 2)]
        public static void ValidateAssets()
        {
            MissingAssets.Clear();
            var required = new[]
            {
                "Wall_Standard", "Wall_Technical", "Wall_Utility", "Wall_Corner", "Door_Blast",
                "Floor_A", "Pillar_A", "CommandConsole", "HologramTable", "ArsenalRack",
                "WorkshopBench", "SupplyStation", "OperatorBay", "Generator", "Crates",
                "VentUnit", "PowerBox", "LightFixture"
            };

            foreach (var stem in required)
            {
                if (FindMeshyAsset(stem) == null)
                    MissingAssets.Add(stem);
            }

            if (MissingAssets.Count == 0)
                Debug.Log("DEADREACH 0.15 Meshy Bunker assets: PASS (18/18 found).");
            else
                Debug.LogWarning($"DEADREACH 0.15 Meshy Bunker assets missing: {string.Join(", ", MissingAssets)}");
        }

        private static void BuildArchitecture(Scene scene, Transform root)
        {
            var floorFound = FindMeshyAsset("Floor_A") != null;
            if (floorFound)
            {
                const float tile = 3.65f;
                for (var x = 0; x < 4; x++)
                for (var z = 0; z < 4; z++)
                {
                    var position = new Vector3(
                        (x - 1.5f) * tile,
                        0.01f,
                        2f + (z - 1.5f) * tile);
                    Place("Floor_A", $"Floor_{x}_{z}", root, position, Vector3.zero,
                        new Vector3(tile, 0.12f, tile), false);
                }
                DisablePrototypeVisual(scene, "Bunker_Floor", keepCollider: true);
            }
            else
            {
                RegisterMissing("Floor_A");
            }

            // Back wall leaves the accepted central blast-door opening unobstructed.
            var leftBack = PlaceFirstAvailable(
                new[] { "Wall_Technical", "Wall_Standard" }, "BackWall_Left", root,
                new Vector3(-4.9f, 0f, 8.12f), Vector3.zero, new Vector3(4.5f, 5.65f, 0.35f), false);
            var rightBack = PlaceFirstAvailable(
                new[] { "Wall_Utility", "Wall_Standard" }, "BackWall_Right", root,
                new Vector3(4.9f, 0f, 8.12f), Vector3.zero, new Vector3(4.5f, 5.65f, 0.35f), false);

            if (leftBack != null || rightBack != null)
                DisablePrototypeVisual(scene, "Bunker_BackWall", keepCollider: true);

            var leftPlaced = false;
            var rightPlaced = false;
            var sideZ = new[] { -3.0f, 1.4f, 5.8f };
            for (var i = 0; i < sideZ.Length; i++)
            {
                var stem = i == 1 ? "Wall_Technical" : "Wall_Standard";
                leftPlaced |= PlaceFirstAvailable(
                    new[] { stem, "Wall_Standard" }, $"LeftWall_{i}", root,
                    new Vector3(-6.92f, 0f, sideZ[i]), new Vector3(0f, 90f, 0f),
                    new Vector3(4.25f, 5.65f, 0.35f), false) != null;
                rightPlaced |= PlaceFirstAvailable(
                    new[] { stem, "Wall_Standard" }, $"RightWall_{i}", root,
                    new Vector3(6.92f, 0f, sideZ[i]), new Vector3(0f, -90f, 0f),
                    new Vector3(4.25f, 5.65f, 0.35f), false) != null;
            }

            if (leftPlaced)
                DisablePrototypeVisual(scene, "Bunker_LeftWall", keepCollider: true);
            if (rightPlaced)
                DisablePrototypeVisual(scene, "Bunker_RightWall", keepCollider: true);

            Place("Wall_Corner", "Corner_BackLeft", root, new Vector3(-6.75f, 0f, 7.95f),
                Vector3.zero, new Vector3(2.0f, 5.65f, 2.0f), false);
            Place("Wall_Corner", "Corner_BackRight", root, new Vector3(6.75f, 0f, 7.95f),
                new Vector3(0f, -90f, 0f), new Vector3(2.0f, 5.65f, 2.0f), false);

            var door = Place("Door_Blast", "BlastDoor", root, new Vector3(0f, 0f, 7.92f),
                Vector3.zero, new Vector3(4.7f, 4.9f, 0.7f), false);
            if (door != null)
            {
                DisablePrototypeVisual(scene, "BlastDoor", keepCollider: true);
                DisablePrototypeVisual(scene, "BlastDoor_Frame_Left", keepCollider: false);
                DisablePrototypeVisual(scene, "BlastDoor_Frame_Right", keepCollider: false);
                DisablePrototypeVisual(scene, "BlastDoor_Frame_Top", keepCollider: false);
                DisablePrototypeVisual(scene, "BlastDoor_Hazard", keepCollider: false);
            }

            var pillarPositions = new[]
            {
                new Vector3(-6.15f, 0f, -3.65f), new Vector3(6.15f, 0f, -3.65f),
                new Vector3(-6.15f, 0f, 6.7f), new Vector3(6.15f, 0f, 6.7f)
            };
            for (var i = 0; i < pillarPositions.Length; i++)
                Place("Pillar_A", $"Pillar_{i}", root, pillarPositions[i], Vector3.zero,
                    new Vector3(1.4f, 5.55f, 1.4f), true);

            // Ceiling remains deterministic Unity geometry by design. Meshy is only used for fixtures/dressing.
        }

        private static void BuildStations(Scene scene, Transform root)
        {
            var holo = Place("HologramTable", "HologramTable", root, new Vector3(0f, 0.02f, 2.35f),
                Vector3.zero, new Vector3(4.35f, 1.45f, 2.75f), true);
            if (holo != null)
            {
                DisablePrototypeVisual(scene, "CommandTable", keepCollider: false);
                DisablePrototypeVisual(scene, "CommandTableTop", keepCollider: false);
                AddHologramGlow(holo.transform);
            }

            Place("CommandConsole", "CommandConsole", root, new Vector3(0f, 0.02f, 5.55f),
                new Vector3(0f, 180f, 0f), new Vector3(3.8f, 2.5f, 1.8f), true);

            Place("ArsenalRack", "ArsenalRack", root, new Vector3(-5.25f, 0.02f, 1.45f),
                new Vector3(0f, 90f, 0f), new Vector3(2.45f, 3.25f, 1.55f), true);

            Place("OperatorBay", "OperatorBay", root, new Vector3(5.25f, 0.02f, 1.95f),
                new Vector3(0f, -90f, 0f), new Vector3(2.65f, 3.25f, 1.6f), true);

            var workshop = Place("WorkshopBench", "WorkshopBench", root, new Vector3(-4.75f, 0.02f, 5.55f),
                new Vector3(0f, 180f, 0f), new Vector3(3.3f, 2.4f, 1.85f), true);
            if (workshop != null)
                DisablePrototypeVisual(scene, "Workshop_Left", keepCollider: false);

            var supply = Place("SupplyStation", "SupplyStation", root, new Vector3(4.75f, 0.02f, 5.55f),
                new Vector3(0f, 180f, 0f), new Vector3(3.15f, 2.9f, 1.9f), true);
            if (supply != null)
                DisablePrototypeVisual(scene, "Storage_Right", keepCollider: false);

            var generator = Place("Generator", "Generator", root, new Vector3(5.25f, 0.02f, -1.25f),
                new Vector3(0f, -90f, 0f), new Vector3(2.55f, 2.35f, 2.0f), true);
            if (generator != null)
                DisablePrototypeVisual(scene, "Generator", keepCollider: false);
        }

        private static void BuildUtilityProps(Transform root)
        {
            Place("Crates", "Crates_LeftFront", root, new Vector3(-5.45f, 0.02f, -1.65f),
                new Vector3(0f, 18f, 0f), new Vector3(2.5f, 1.7f, 1.8f), true);
            Place("Crates", "Crates_Supply", root, new Vector3(5.45f, 0.02f, 4.05f),
                new Vector3(0f, -12f, 0f), new Vector3(2.2f, 1.55f, 1.65f), true);

            Place("VentUnit", "VentUnit", root, new Vector3(-6.55f, 0.08f, -0.55f),
                new Vector3(0f, 90f, 0f), new Vector3(1.8f, 2.35f, 1.1f), true);
            Place("PowerBox", "PowerBox", root, new Vector3(6.58f, 0.65f, 4.25f),
                new Vector3(0f, -90f, 0f), new Vector3(1.35f, 1.8f, 0.75f), true);
        }

        private static void BuildLighting(Transform root)
        {
            var positions = new[]
            {
                new Vector3(-4.5f, 5.62f, 0.4f), new Vector3(4.5f, 5.62f, 0.4f),
                new Vector3(-4.5f, 5.62f, 5.0f), new Vector3(4.5f, 5.62f, 5.0f)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var fixture = Place("LightFixture", $"LightFixture_{i}", root, positions[i], Vector3.zero,
                    new Vector3(2.2f, 0.45f, 1.0f), true);
                if (fixture == null)
                    continue;

                var lightObject = new GameObject($"Light_{i}");
                lightObject.transform.SetParent(root, false);
                lightObject.transform.localPosition = positions[i] + new Vector3(0f, -0.35f, 0f);
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(0.72f, 0.90f, 0.94f);
                light.intensity = 2.3f;
                light.range = 5.8f;
                light.shadows = i == 0 || i == 3 ? LightShadows.Soft : LightShadows.None;
            }
        }

        private static void AddHologramGlow(Transform table)
        {
            var glow = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glow.name = "Hologram_Emitter_Glow";
            glow.transform.SetParent(table, false);
            glow.transform.localPosition = new Vector3(0f, 0.58f, 0f);
            glow.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            glow.transform.localScale = new Vector3(0.62f, 0.42f, 1f);
            var collider = glow.GetComponent<Collider>();
            if (collider != null)
                UnityEngine.Object.DestroyImmediate(collider);

            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            if (shader == null)
                return;

            var material = new Material(shader) { name = "P15_HologramGlow_Runtime" };
            var color = new Color(0.08f, 0.86f, 0.95f, 0.28f);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            glow.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static GameObject PlaceFirstAvailable(
            string[] stems,
            string instanceName,
            Transform parent,
            Vector3 position,
            Vector3 euler,
            Vector3 targetSize,
            bool uniform)
        {
            foreach (var stem in stems)
            {
                if (FindMeshyAsset(stem) == null)
                    continue;
                return Place(stem, instanceName, parent, position, euler, targetSize, uniform);
            }

            foreach (var stem in stems)
                RegisterMissing(stem);
            return null;
        }

        private static GameObject Place(
            string stem,
            string instanceName,
            Transform parent,
            Vector3 position,
            Vector3 euler,
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

            instance.name = instanceName;
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(euler);
            instance.transform.localScale = Vector3.one;

            FitToLocalBox(instance.transform, targetSize, uniform);
            MoveBottomTo(instance.transform, position.y);
            SetStaticRecursive(instance);
            return instance;
        }

        private static GameObject FindMeshyAsset(string stem)
        {
            if (!AssetDatabase.IsValidFolder(AssetRoot))
                return null;

            var guids = AssetDatabase.FindAssets(stem, new[] { AssetRoot });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.Equals(Path.GetFileNameWithoutExtension(path), stem, StringComparison.OrdinalIgnoreCase))
                    continue;

                var extension = Path.GetExtension(path).ToLowerInvariant();
                if (extension != ".glb" && extension != ".gltf" && extension != ".fbx" && extension != ".prefab")
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset != null)
                    return asset;
            }

            return null;
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

        private static void DisablePrototypeVisual(Scene scene, string name, bool keepCollider)
        {
            var target = FindSceneObject(scene, name);
            if (target == null)
                return;

            foreach (var renderer in target.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = false;

            if (!keepCollider)
            {
                foreach (var collider in target.GetComponentsInChildren<Collider>(true))
                    collider.enabled = false;
            }
        }

        private static GameObject FindSceneObject(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                {
                    if (transform != null && transform.name == name)
                        return transform.gameObject;
                }
            }
            return null;
        }

        private static void SetStaticRecursive(GameObject root)
        {
            var flags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic;
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                GameObjectUtility.SetStaticEditorFlags(transform.gameObject, flags);
        }

        private static void RegisterMissing(string stem)
        {
            if (!MissingAssets.Contains(stem))
                MissingAssets.Add(stem);
        }
    }
}
