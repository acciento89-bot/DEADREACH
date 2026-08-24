using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production12SectorScenePass
    {
        private const string DeadCityScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";
        private const string BunkerScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        private const string EnvironmentRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Environment/glTF";
        private const string VehicleRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Vehicles/glTF";
        private const string EnvironmentMaterialPath = "Assets/Deadreach/Art/Production/Materials/Quaternius_EnvironmentAtlas.mat";
        private const string SharedRootName = "Production_SectorNetwork_0_12";
        private const string LayoutRootName = "Production_SectorLayouts_0_12";
        private const string WorldSafetyRootName = "DeadCity_WorldSafety_0_5";

        public static bool Apply()
        {
            if (!System.IO.File.Exists(DeadCityScenePath))
            {
                Debug.LogError($"DEADREACH 0.12 sector pass could not find {DeadCityScenePath}.");
                return false;
            }

            var scene = EditorSceneManager.OpenScene(DeadCityScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.12 sector pass could not open the generated Dead City scene.");
                return false;
            }

            RemovePreviousPass();
            ExpandWorldSafety();

            var material = AssetDatabase.LoadAssetAtPath<Material>(EnvironmentMaterialPath);
            BuildSharedCrossStreet(material);
            BuildLayouts(material);

            var fallSafety = UnityEngine.Object.FindFirstObjectByType<PlayerFallSafety>();
            if (fallSafety != null)
                fallSafety.ConfigureBounds(new Vector2(-16.2f, 16.2f), new Vector2(-10.5f, 21.4f));

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            if (System.IO.File.Exists(BunkerScenePath))
                EditorSceneManager.OpenScene(BunkerScenePath, OpenSceneMode.Single);

            Debug.Log("DEADREACH Production 0.12 sector world generated: four alternate layouts, expanded cross-street traversal, authored objective/enemy/loot/reinforcement anchors and gameplay hazards.");
            return true;
        }

        private static void RemovePreviousPass()
        {
            var shared = GameObject.Find(SharedRootName);
            if (shared != null)
                UnityEngine.Object.DestroyImmediate(shared);
            var layouts = GameObject.Find(LayoutRootName);
            if (layouts != null)
                UnityEngine.Object.DestroyImmediate(layouts);
        }

        private static void BuildSharedCrossStreet(Material material)
        {
            var root = new GameObject(SharedRootName);

            Place(false, root.transform, "Street_Straight.gltf", "West_Spur_A", new Vector3(-6.2f, 0.048f, 3.2f), 90f, 1.35f, material, false);
            Place(false, root.transform, "Street_Crack1.gltf", "West_Spur_B", new Vector3(-12.1f, 0.048f, 3.2f), 90f, 1.35f, material, false);
            Place(false, root.transform, "Street_Straight.gltf", "East_Spur_A", new Vector3(6.2f, 0.048f, 3.2f), 90f, 1.35f, material, false);
            Place(false, root.transform, "Street_Crack2.gltf", "East_Spur_B", new Vector3(12.1f, 0.048f, 3.2f), 90f, 1.35f, material, false);

            Place(false, root.transform, "StreetLights.gltf", "West_Spur_Lamp", new Vector3(-11.6f, 0f, 0.7f), 90f, 1f, material, false);
            Place(false, root.transform, "StreetLights.gltf", "East_Spur_Lamp", new Vector3(11.6f, 0f, 5.7f), -90f, 1f, material, false);
            Place(false, root.transform, "TrafficLight_1.gltf", "West_Spur_Traffic", new Vector3(-5.1f, 0f, 6.15f), 90f, 1f, material, false);
            Place(false, root.transform, "TrafficLight_1.gltf", "East_Spur_Traffic", new Vector3(5.1f, 0f, 0.25f), -90f, 1f, material, false);

            CreatePointLight(root.transform, "West_Spur_Key", new Vector3(-10.4f, 3.1f, 3.2f), new Color(0.18f, 0.48f, 0.72f), 3.6f, 9f);
            CreatePointLight(root.transform, "East_Spur_Key", new Vector3(10.4f, 3.1f, 3.2f), new Color(0.18f, 0.48f, 0.72f), 3.6f, 9f);
        }

        private static void BuildLayouts(Material material)
        {
            var root = new GameObject(LayoutRootName);
            BuildQuarantineWard(root.transform, material);
            BuildTransitCollapse(root.transform, material);
            BuildIndustrialSpill(root.transform, material);
            BuildBlackoutPlaza(root.transform, material);
        }

        private static void BuildQuarantineWard(Transform parent, Material material)
        {
            var root = new GameObject("SectorLayout_QuarantineWard");
            root.transform.SetParent(parent, false);

            Place(false, root.transform, "TrafficBarrier_1.gltf", "QW_Checkpoint_A", new Vector3(-2.7f, 0.05f, -1.8f), 17f, 1.2f, material, true);
            Place(false, root.transform, "TrafficBarrier_1.gltf", "QW_Checkpoint_B", new Vector3(2.65f, 0.05f, 4.9f), -19f, 1.2f, material, true);
            Place(false, root.transform, "PlasticBarrier.gltf", "QW_Checkpoint_C", new Vector3(-2.9f, 0.05f, 10.8f), 14f, 1.15f, material, true);
            Place(false, root.transform, "Container_Green.gltf", "QW_Container_West", new Vector3(-10.8f, 0.02f, 5.9f), 90f, 1.02f, material, true);
            Place(false, root.transform, "Container_Red.gltf", "QW_Container_North", new Vector3(6.5f, 0.02f, 15.6f), 6f, 1.02f, material, true);
            Place(true, root.transform, "Vehicle_Pickup.gltf", "QW_Response_Pickup", new Vector3(5.3f, 0.04f, 2.25f), 72f, 1f, material, true);
            Place(false, root.transform, "Barrel.gltf", "QW_Bio_Barrel_A", new Vector3(7.2f, 0.03f, 3.8f), 0f, 1f, material, true);
            Place(false, root.transform, "Barrel.gltf", "QW_Bio_Barrel_B", new Vector3(7.8f, 0.03f, 4.3f), 22f, 1f, material, true);
            Place(false, root.transform, "TrashBag_1.gltf", "QW_Waste_A", new Vector3(-7.2f, 0.03f, 2.1f), 20f, 1f, material, false);
            CreatePointLight(root.transform, "QW_BioLight", new Vector3(7.4f, 1.8f, 4.1f), new Color(0.15f, 1f, 0.42f), 4.8f, 7.5f);

            var objectives = Anchors(root.transform, "QW_Objective", 0.12f,
                new Vector3(-9.2f, 0f, 3.15f), new Vector3(0f, 0f, 7.4f), new Vector3(8.6f, 0f, 3.15f),
                new Vector3(2.8f, 0f, 15.2f), new Vector3(-3.1f, 0f, 11.8f));
            var reinforcements = Anchors(root.transform, "QW_Reinforcement", 1f,
                new Vector3(-13.2f, 0f, 3.2f), new Vector3(13.2f, 0f, 3.2f), new Vector3(-4.8f, 0f, -5.2f),
                new Vector3(4.7f, 0f, 8.8f), new Vector3(-4.4f, 0f, 16.8f), new Vector3(4.2f, 0f, 19.4f));
            var enemies = Anchors(root.transform, "QW_Enemy", 1f,
                new Vector3(-2.2f, 0f, -1.2f), new Vector3(2.4f, 0f, 2.1f), new Vector3(-8.2f, 0f, 3.2f),
                new Vector3(8.4f, 0f, 3.2f), new Vector3(-2.8f, 0f, 8.9f), new Vector3(2.7f, 0f, 12.8f),
                new Vector3(-2.9f, 0f, 16.7f), new Vector3(2.5f, 0f, 18.4f));
            var loot = Anchors(root.transform, "QW_Loot", 0.58f,
                new Vector3(-10.6f, 0f, 2.7f), new Vector3(10.2f, 0f, 3.8f), new Vector3(-3.4f, 0f, 5.6f),
                new Vector3(3.2f, 0f, 11.2f), new Vector3(-2.7f, 0f, 15.3f));
            var spawn = Anchor(root.transform, "QW_PlayerSpawn", new Vector3(0f, 1f, -8.5f));
            var extraction = Anchor(root.transform, "QW_Extraction", new Vector3(12f, 0.08f, 3.2f));
            var hazard = CreateHazard(root.transform, "QW_Contamination", new Vector3(7.5f, 0f, 4f), SectorHazardKind.Contamination,
                new Color(0.18f, 1f, 0.42f, 0.95f), new Vector3(4.5f, 2.5f, 4.2f), 3f, 1.25f, 2.2f);

            var definition = root.AddComponent<SectorLayoutDefinition>();
            definition.Configure(SectorArchetype.QuarantineWard, "QUARANTINE WARD", "BIOHAZARD", 4, 2,
                new Color(0.035f, 0.09f, 0.07f), 0.0155f, new Color(0.38f, 0.82f, 0.64f), 0.72f,
                spawn, extraction, objectives, reinforcements, enemies, loot, new[] { hazard });
            root.SetActive(false);
        }

        private static void BuildTransitCollapse(Transform parent, Material material)
        {
            var root = new GameObject("SectorLayout_TransitCollapse");
            root.transform.SetParent(parent, false);

            Place(true, root.transform, "Vehicle_Truck.gltf", "TC_Wreck_Truck", new Vector3(0.6f, 0.04f, 5.8f), 78f, 1.04f, material, true);
            Place(true, root.transform, "Vehicle_Sports.gltf", "TC_Wreck_Sports", new Vector3(-1.6f, 0.04f, 11.4f), -24f, 1f, material, true);
            Place(true, root.transform, "Vehicle_Pickup.gltf", "TC_Wreck_Pickup", new Vector3(7.6f, 0.04f, 3.0f), 92f, 1f, material, true);
            Place(false, root.transform, "TrafficBarrier_1.gltf", "TC_Barrier_A", new Vector3(2.7f, 0.05f, -0.8f), -20f, 1.1f, material, true);
            Place(false, root.transform, "PlasticBarrier.gltf", "TC_Barrier_B", new Vector3(-7.5f, 0.05f, 4.9f), 8f, 1.1f, material, true);
            Place(false, root.transform, "Wheels_Stack.gltf", "TC_Wheels", new Vector3(4.8f, 0.03f, 9.8f), 0f, 1.05f, material, true);
            Place(false, root.transform, "Pallet_Broken.gltf", "TC_Pallet", new Vector3(-4.9f, 0.03f, 8.2f), -20f, 1.1f, material, false);
            CreatePointLight(root.transform, "TC_ArcLight", new Vector3(-1f, 2.2f, 5.4f), new Color(0.15f, 0.72f, 1f), 4.5f, 8.5f);

            var objectives = Anchors(root.transform, "TC_Objective", 0.12f,
                new Vector3(9.5f, 0f, 3.2f), new Vector3(0f, 0f, -0.2f), new Vector3(-9.8f, 0f, 3.2f),
                new Vector3(3.2f, 0f, 14.8f), new Vector3(-3f, 0f, 9.2f));
            var reinforcements = Anchors(root.transform, "TC_Reinforcement", 1f,
                new Vector3(-13.4f, 0f, 3.2f), new Vector3(13.4f, 0f, 3.2f), new Vector3(-4.5f, 0f, -5f),
                new Vector3(4.5f, 0f, 8.5f), new Vector3(-4.4f, 0f, 15.6f), new Vector3(4f, 0f, 19f));
            var enemies = Anchors(root.transform, "TC_Enemy", 1f,
                new Vector3(-2.5f, 0f, -1f), new Vector3(2.8f, 0f, 1.8f), new Vector3(-10f, 0f, 3.1f),
                new Vector3(9.4f, 0f, 3.5f), new Vector3(-3.2f, 0f, 8.2f), new Vector3(3.5f, 0f, 12f),
                new Vector3(-2.8f, 0f, 16.2f), new Vector3(2.4f, 0f, 18f));
            var loot = Anchors(root.transform, "TC_Loot", 0.58f,
                new Vector3(-11.2f, 0f, 3.8f), new Vector3(10.8f, 0f, 2.5f), new Vector3(-3.1f, 0f, 6.6f),
                new Vector3(3.4f, 0f, 10.4f), new Vector3(-2.2f, 0f, 15.8f));
            var spawn = Anchor(root.transform, "TC_PlayerSpawn", new Vector3(0f, 1f, -8.6f));
            var extraction = Anchor(root.transform, "TC_Extraction", new Vector3(-12.1f, 0.08f, 3.2f));
            var hazard = CreateHazard(root.transform, "TC_ArcField", new Vector3(-0.8f, 0f, 5.4f), SectorHazardKind.ElectricalArc,
                new Color(0.12f, 0.75f, 1f, 0.95f), new Vector3(4.2f, 2.5f, 3.4f), 4f, 1.05f, 2f);

            var definition = root.AddComponent<SectorLayoutDefinition>();
            definition.Configure(SectorArchetype.TransitCollapse, "TRANSIT COLLAPSE", "ARC GRID", 6, 3,
                new Color(0.04f, 0.065f, 0.095f), 0.017f, new Color(0.42f, 0.68f, 0.96f), 0.68f,
                spawn, extraction, objectives, reinforcements, enemies, loot, new[] { hazard });
            root.SetActive(false);
        }

        private static void BuildIndustrialSpill(Transform parent, Material material)
        {
            var root = new GameObject("SectorLayout_IndustrialSpill");
            root.transform.SetParent(parent, false);

            Place(false, root.transform, "Container_Green.gltf", "IS_Channel_West", new Vector3(-5.9f, 0.02f, 8.8f), 4f, 1.05f, material, true);
            Place(false, root.transform, "Container_Red.gltf", "IS_Channel_East", new Vector3(5.9f, 0.02f, 12.1f), -4f, 1.05f, material, true);
            Place(false, root.transform, "Pipes.gltf", "IS_Pipes_A", new Vector3(-8.8f, 0.03f, 3.2f), 90f, 1.08f, material, true);
            Place(false, root.transform, "Barrel.gltf", "IS_Barrel_A", new Vector3(-2.6f, 0.03f, 10.4f), 0f, 1f, material, true);
            Place(false, root.transform, "Barrel.gltf", "IS_Barrel_B", new Vector3(-2.0f, 0.03f, 10.9f), 28f, 1f, material, true);
            Place(false, root.transform, "Barrel.gltf", "IS_Barrel_C", new Vector3(2.8f, 0.03f, 5.7f), -18f, 1f, material, true);
            Place(true, root.transform, "Vehicle_Truck.gltf", "IS_ServiceTruck", new Vector3(8.5f, 0.04f, 3.25f), 92f, 1f, material, true);
            Place(false, root.transform, "PlasticBarrier.gltf", "IS_Barrier", new Vector3(2.8f, 0.05f, 15.4f), -12f, 1.15f, material, true);
            CreatePointLight(root.transform, "IS_SpillLight", new Vector3(-2.3f, 2f, 10.6f), new Color(1f, 0.62f, 0.08f), 5f, 8f);

            var objectives = Anchors(root.transform, "IS_Objective", 0.12f,
                new Vector3(-9.4f, 0f, 3.2f), new Vector3(9.5f, 0f, 3.2f), new Vector3(-3.2f, 0f, 9.2f),
                new Vector3(3.2f, 0f, 15.2f), new Vector3(0f, 0f, 5.2f));
            var reinforcements = Anchors(root.transform, "IS_Reinforcement", 1f,
                new Vector3(-13.2f, 0f, 3.2f), new Vector3(13.2f, 0f, 3.2f), new Vector3(-4.7f, 0f, -5.1f),
                new Vector3(4.6f, 0f, 8.4f), new Vector3(-4.6f, 0f, 16.4f), new Vector3(4.2f, 0f, 19.2f));
            var enemies = Anchors(root.transform, "IS_Enemy", 1f,
                new Vector3(-2.4f, 0f, -0.8f), new Vector3(2.6f, 0f, 2.1f), new Vector3(-9f, 0f, 3.5f),
                new Vector3(9.1f, 0f, 2.8f), new Vector3(-3f, 0f, 7.8f), new Vector3(3.1f, 0f, 11.4f),
                new Vector3(-2.7f, 0f, 15.8f), new Vector3(2.8f, 0f, 18f));
            var loot = Anchors(root.transform, "IS_Loot", 0.58f,
                new Vector3(-10.5f, 0f, 3.2f), new Vector3(10.6f, 0f, 3.2f), new Vector3(-3.4f, 0f, 6.7f),
                new Vector3(3.5f, 0f, 12.6f), new Vector3(-2.2f, 0f, 17f));
            var spawn = Anchor(root.transform, "IS_PlayerSpawn", new Vector3(0f, 1f, -8.6f));
            var extraction = Anchor(root.transform, "IS_Extraction", new Vector3(0f, 0.08f, 20f));
            var hazardA = CreateHazard(root.transform, "IS_ChemicalSpill", new Vector3(-2.25f, 0f, 10.6f), SectorHazardKind.Contamination,
                new Color(0.82f, 0.95f, 0.08f, 0.95f), new Vector3(4.1f, 2.5f, 4.4f), 4.5f, 1.2f, 2.1f);
            var hazardB = CreateHazard(root.transform, "IS_Fireline", new Vector3(3.1f, 0f, 5.8f), SectorHazardKind.Fireline,
                new Color(1f, 0.28f, 0.04f, 0.95f), new Vector3(3.4f, 2.5f, 3.2f), 5f, 1f, 1.7f);

            var definition = root.AddComponent<SectorLayoutDefinition>();
            definition.Configure(SectorArchetype.IndustrialSpill, "INDUSTRIAL SPILL", "CHEMICAL / FIRE", 8, 5,
                new Color(0.12f, 0.07f, 0.025f), 0.019f, new Color(0.95f, 0.54f, 0.2f), 0.76f,
                spawn, extraction, objectives, reinforcements, enemies, loot, new[] { hazardA, hazardB });
            root.SetActive(false);
        }

        private static void BuildBlackoutPlaza(Transform parent, Material material)
        {
            var root = new GameObject("SectorLayout_BlackoutPlaza");
            root.transform.SetParent(parent, false);

            Place(false, root.transform, "Container_Red.gltf", "BP_Container_West", new Vector3(-8.9f, 0.02f, 4.8f), 92f, 1.02f, material, true);
            Place(true, root.transform, "Vehicle_Sports.gltf", "BP_Wreck_A", new Vector3(1.8f, 0.04f, 7.8f), -18f, 1f, material, true);
            Place(true, root.transform, "Vehicle_Pickup.gltf", "BP_Wreck_B", new Vector3(-2.1f, 0.04f, 14.1f), 16f, 1f, material, true);
            Place(false, root.transform, "TrafficBarrier_1.gltf", "BP_Barrier_A", new Vector3(-2.8f, 0.05f, 2.4f), 14f, 1.15f, material, true);
            Place(false, root.transform, "TrafficBarrier_1.gltf", "BP_Barrier_B", new Vector3(2.8f, 0.05f, 12.3f), -14f, 1.15f, material, true);
            Place(false, root.transform, "TrashBag_2.gltf", "BP_Trash", new Vector3(8.7f, 0.03f, 2.2f), -12f, 1f, material, false);
            Place(false, root.transform, "Wheels_Stack.gltf", "BP_Wheels", new Vector3(-5.2f, 0.03f, 10.1f), 8f, 1f, material, true);
            CreatePointLight(root.transform, "BP_PurpleEmergency", new Vector3(-8f, 2.4f, 3.2f), new Color(0.52f, 0.18f, 1f), 3.8f, 8f);
            CreatePointLight(root.transform, "BP_RedEmergency", new Vector3(3f, 2.2f, 13f), new Color(1f, 0.08f, 0.05f), 3.8f, 8f);

            var objectives = Anchors(root.transform, "BP_Objective", 0.12f,
                new Vector3(-10f, 0f, 3.2f), new Vector3(0f, 0f, 13.8f), new Vector3(9.8f, 0f, 3.2f),
                new Vector3(-3f, 0f, 16.2f), new Vector3(3.2f, 0f, 7.5f));
            var reinforcements = Anchors(root.transform, "BP_Reinforcement", 1f,
                new Vector3(-13.4f, 0f, 3.2f), new Vector3(13.4f, 0f, 3.2f), new Vector3(-4.5f, 0f, -5.2f),
                new Vector3(4.5f, 0f, 9.3f), new Vector3(-4.4f, 0f, 16.7f), new Vector3(4.1f, 0f, 19.3f));
            var enemies = Anchors(root.transform, "BP_Enemy", 1f,
                new Vector3(-2.4f, 0f, -0.7f), new Vector3(2.4f, 0f, 2f), new Vector3(-9.5f, 0f, 3.2f),
                new Vector3(9.2f, 0f, 3.2f), new Vector3(-3.2f, 0f, 8.2f), new Vector3(3f, 0f, 11.8f),
                new Vector3(-2.8f, 0f, 16.3f), new Vector3(2.6f, 0f, 18.2f));
            var loot = Anchors(root.transform, "BP_Loot", 0.58f,
                new Vector3(-11f, 0f, 2.8f), new Vector3(10.8f, 0f, 3.6f), new Vector3(-3.1f, 0f, 5.7f),
                new Vector3(3.3f, 0f, 11.1f), new Vector3(-2.5f, 0f, 15.7f));
            var spawn = Anchor(root.transform, "BP_PlayerSpawn", new Vector3(0f, 1f, -8.7f));
            var extraction = Anchor(root.transform, "BP_Extraction", new Vector3(11.8f, 0.08f, 3.2f));
            var hazardA = CreateHazard(root.transform, "BP_ArcGrid", new Vector3(0f, 0f, 3.2f), SectorHazardKind.ElectricalArc,
                new Color(0.55f, 0.18f, 1f, 0.95f), new Vector3(4.4f, 2.5f, 4.1f), 5f, 0.95f, 2.2f);
            var hazardB = CreateHazard(root.transform, "BP_Firebreak", new Vector3(2.8f, 0f, 13.2f), SectorHazardKind.Fireline,
                new Color(1f, 0.12f, 0.05f, 0.95f), new Vector3(3.6f, 2.5f, 3.2f), 5.5f, 0.9f, 1.8f);

            var definition = root.AddComponent<SectorLayoutDefinition>();
            definition.Configure(SectorArchetype.BlackoutPlaza, "BLACKOUT PLAZA", "ARC / FIRE", 10, 6,
                new Color(0.035f, 0.02f, 0.065f), 0.022f, new Color(0.5f, 0.35f, 0.82f), 0.52f,
                spawn, extraction, objectives, reinforcements, enemies, loot, new[] { hazardA, hazardB });
            root.SetActive(false);
        }

        private static Transform[] Anchors(Transform parent, string prefix, float y, params Vector3[] positions)
        {
            var result = new Transform[positions.Length];
            for (var i = 0; i < positions.Length; i++)
            {
                var p = positions[i];
                p.y = y;
                result[i] = Anchor(parent, $"{prefix}_{i + 1:00}", p);
            }
            return result;
        }

        private static Transform Anchor(Transform parent, string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            return go.transform;
        }

        private static SectorHazardZone CreateHazard(
            Transform parent,
            string name,
            Vector3 position,
            SectorHazardKind kind,
            Color accent,
            Vector3 size,
            float damage,
            float interval,
            float radius)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            var hazard = go.AddComponent<SectorHazardZone>();
            hazard.Configure(kind, accent, size, damage, interval, radius);
            return hazard;
        }

        private static bool Place(
            bool vehicle,
            Transform parent,
            string fileName,
            string instanceName,
            Vector3 position,
            float yaw,
            float scale,
            Material material,
            bool addCollider)
        {
            var rootPath = vehicle ? VehicleRoot : EnvironmentRoot;
            var source = AssetDatabase.LoadAssetAtPath<GameObject>($"{rootPath}/{fileName}");
            if (source == null)
            {
                Debug.LogWarning($"DEADREACH 0.12 missing sector asset: {rootPath}/{fileName}");
                return false;
            }

            var instance = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
            instance.name = instanceName;
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            instance.transform.localScale = Vector3.one * scale;
            SetStaticRecursive(instance);

            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;
                if (material != null)
                    renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }

            if (addCollider)
                AddBoundsCollider(instance, renderers);
            return true;
        }

        private static void AddBoundsCollider(GameObject root, Renderer[] renderers)
        {
            if (root == null || renderers == null || renderers.Length == 0)
                return;

            var hasBounds = false;
            var bounds = default(Bounds);
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
            if (!hasBounds)
                return;

            var collisionRoot = new GameObject("CollisionBounds");
            collisionRoot.transform.SetParent(root.transform, false);
            GameObjectUtility.SetStaticEditorFlags(collisionRoot,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            var box = collisionRoot.AddComponent<BoxCollider>();
            box.center = root.transform.InverseTransformPoint(bounds.center);
            var lossy = root.transform.lossyScale;
            box.size = new Vector3(
                bounds.size.x / Mathf.Max(Mathf.Abs(lossy.x), 0.001f),
                bounds.size.y / Mathf.Max(Mathf.Abs(lossy.y), 0.001f),
                bounds.size.z / Mathf.Max(Mathf.Abs(lossy.z), 0.001f));
        }

        private static void SetStaticRecursive(GameObject root)
        {
            GameObjectUtility.SetStaticEditorFlags(root,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            foreach (Transform child in root.transform)
                SetStaticRecursive(child.gameObject);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void ExpandWorldSafety()
        {
            var old = GameObject.Find(WorldSafetyRootName);
            if (old != null)
                UnityEngine.Object.DestroyImmediate(old);

            var root = new GameObject(WorldSafetyRootName);
            CreateBoundary(root.transform, "Boundary_West", new Vector3(-16.7f, 1.5f, 5.45f), new Vector3(0.6f, 4f, 33.5f));
            CreateBoundary(root.transform, "Boundary_East", new Vector3(16.7f, 1.5f, 5.45f), new Vector3(0.6f, 4f, 33.5f));
            CreateBoundary(root.transform, "Boundary_South", new Vector3(0f, 1.5f, -11.15f), new Vector3(34f, 4f, 0.6f));
            CreateBoundary(root.transform, "Boundary_North", new Vector3(0f, 1.5f, 22.05f), new Vector3(34f, 4f, 0.6f));
            CreateBoundary(root.transform, "Emergency_CatchFloor", new Vector3(0f, -1.25f, 5.45f), new Vector3(34f, 0.25f, 33.5f));
        }

        private static void CreateBoundary(Transform parent, string name, Vector3 position, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            var box = go.AddComponent<BoxCollider>();
            box.size = size;
            GameObjectUtility.SetStaticEditorFlags(go,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
        }
    }
}
