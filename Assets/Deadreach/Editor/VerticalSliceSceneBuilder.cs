using System.Collections.Generic;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.CameraSystem;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Extraction;
using Kamilunavo.Deadreach.Input;
using Kamilunavo.Deadreach.Loot;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamilunavo.Deadreach.Editor
{
    public static class VerticalSliceSceneBuilder
    {
        private const string SceneFolder = "Assets/Deadreach/Scenes";
        private const string MaterialFolder = "Assets/Deadreach/Art/DevPalette";
        private const string ScenePath = SceneFolder + "/DeadCity_VerticalSlice.unity";

        [MenuItem("DEADREACH/Build Vertical Slice 0.1")]
        public static void Build()
        {
            EnsureFolders();
            var palette = LoadOrCreatePalette();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            ConfigureAtmosphere();
            CreateSystems();
            CreateEnvironment(palette);
            var player = CreatePlayer(palette);
            CreateCamera(player.transform);
            CreateEnemies(palette);
            CreateStartingLoot(palette);
            CreateExtraction(palette);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings();
            Selection.activeGameObject = player;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log($"DEADREACH Vertical Slice 0.1 generated at {ScenePath}");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/Deadreach", "Scenes");
            EnsureFolder("Assets/Deadreach", "Art");
            EnsureFolder("Assets/Deadreach/Art", "DevPalette");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }

        private static Palette LoadOrCreatePalette()
        {
            return new Palette
            {
                Ground = GetOrCreateMaterial("Ground", new Color(0.055f, 0.065f, 0.07f), 0.15f, 0.1f),
                Road = GetOrCreateMaterial("Road", new Color(0.025f, 0.03f, 0.035f), 0.05f, 0.05f),
                Concrete = GetOrCreateMaterial("Concrete", new Color(0.18f, 0.2f, 0.21f), 0.05f, 0.35f),
                Survivor = GetOrCreateMaterial("Survivor", new Color(0.05f, 0.45f, 0.48f), 0.25f, 0.55f),
                Infected = GetOrCreateMaterial("Infected", new Color(0.42f, 0.055f, 0.045f), 0.05f, 0.3f),
                Scrap = GetOrCreateMaterial("Scrap", new Color(0.9f, 0.55f, 0.08f), 0.75f, 0.5f),
                Extraction = GetOrCreateMaterial("Extraction", new Color(0.05f, 0.8f, 0.35f), 0.15f, 0.65f)
            };
        }

        private static Material GetOrCreateMaterial(string name, Color color, float metallic, float smoothness)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = name };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            else
                material.color = color;

            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void ConfigureAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0085f;
            RenderSettings.fogColor = new Color(0.085f, 0.105f, 0.12f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.14f, 0.17f, 0.2f);
            RenderSettings.ambientEquatorColor = new Color(0.075f, 0.08f, 0.085f);
            RenderSettings.ambientGroundColor = new Color(0.025f, 0.025f, 0.03f);

            var sunObject = new GameObject("Lighting_MoonKey");
            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.15f;
            sun.color = new Color(0.62f, 0.72f, 0.9f);
            sun.shadows = LightShadows.Soft;
            sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static void CreateSystems()
        {
            var input = new GameObject("Systems_Input");
            input.AddComponent<DeadreachInput>();

            var run = new GameObject("Systems_RunSession");
            run.AddComponent<RunSession>();
            run.AddComponent<PrototypeHud>();
        }

        private static GameObject CreatePlayer(Palette palette)
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Survivor";
            player.transform.position = new Vector3(0f, 1f, -11f);
            Object.DestroyImmediate(player.GetComponent<Collider>());

            var controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.46f;
            controller.stepOffset = 0.35f;
            controller.skinWidth = 0.05f;

            var damageable = player.AddComponent<Damageable>();
            damageable.Configure(CombatFaction.Survivor, 100f);
            player.AddComponent<PlayerMotor>();
            player.AddComponent<HitscanWeapon>();
            player.GetComponent<Renderer>().sharedMaterial = palette.Survivor;
            return player;
        }

        private static void CreateCamera(Transform player)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 140f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.05f, 0.065f);
            cameraObject.AddComponent<AudioListener>();
            var rig = cameraObject.AddComponent<HighAngleCameraRig>();
            rig.SetTarget(player);
        }

        private static void CreateEnvironment(Palette palette)
        {
            CreateCube("World_Ground", new Vector3(0f, -0.5f, 0f), new Vector3(34f, 1f, 38f), palette.Ground);
            CreateCube("Road_Main", new Vector3(0f, 0.015f, 1f), new Vector3(8f, 0.03f, 36f), palette.Road);
            CreateCube("Road_Cross", new Vector3(0f, 0.02f, 3f), new Vector3(30f, 0.035f, 7f), palette.Road);

            var buildings = new[]
            {
                (new Vector3(-12f, 3f, -9f), new Vector3(7f, 6f, 9f)),
                (new Vector3(12.5f, 4f, -8f), new Vector3(8f, 8f, 8f)),
                (new Vector3(-13f, 4.5f, 9f), new Vector3(6f, 9f, 9f)),
                (new Vector3(13f, 3.5f, 10f), new Vector3(7f, 7f, 8f)),
                (new Vector3(-10f, 2.5f, 16f), new Vector3(9f, 5f, 5f)),
                (new Vector3(11f, 5f, 17f), new Vector3(10f, 10f, 5f))
            };

            for (var i = 0; i < buildings.Length; i++)
                CreateCube($"Building_Block_{i + 1:00}", buildings[i].Item1, buildings[i].Item2, palette.Concrete);

            var barricades = new[]
            {
                new Vector3(-2.7f, 0.55f, -2f), new Vector3(2.5f, 0.55f, 5.5f),
                new Vector3(-3f, 0.55f, 8f), new Vector3(3f, 0.55f, 12.5f)
            };

            foreach (var position in barricades)
            {
                var barrier = CreateCube("Concrete_Barrier", position, new Vector3(2.8f, 1.1f, 0.65f), palette.Concrete);
                barrier.transform.rotation = Quaternion.Euler(0f, position.x > 0f ? -16f : 16f, 0f);
            }

            CreateStreetLight(new Vector3(-4.6f, 0f, -5f));
            CreateStreetLight(new Vector3(4.6f, 0f, 5f));
            CreateStreetLight(new Vector3(-4.6f, 0f, 14f));
        }

        private static void CreateStreetLight(Vector3 position)
        {
            var root = new GameObject("StreetLight");
            root.transform.position = position;
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(root.transform, false);
            pole.transform.localPosition = new Vector3(0f, 2.4f, 0f);
            pole.transform.localScale = new Vector3(0.08f, 2.4f, 0.08f);

            var lightObject = new GameObject("Lamp");
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.localPosition = new Vector3(0f, 4.65f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 9f;
            light.intensity = 5.5f;
            light.color = new Color(1f, 0.63f, 0.34f);
            light.shadows = LightShadows.Soft;
        }

        private static void CreateEnemies(Palette palette)
        {
            var positions = new[]
            {
                new Vector3(-2f, 1f, -1f), new Vector3(3f, 1f, 2f),
                new Vector3(-1.5f, 1f, 7f), new Vector3(2.8f, 1f, 10f),
                new Vector3(-3f, 1f, 16f), new Vector3(2f, 1f, 18f)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = $"Infected_{i + 1:00}";
                enemy.transform.position = positions[i];
                Object.DestroyImmediate(enemy.GetComponent<Collider>());
                var controller = enemy.AddComponent<CharacterController>();
                controller.height = 2f;
                controller.radius = 0.44f;
                var damageable = enemy.AddComponent<Damageable>();
                damageable.Configure(CombatFaction.Infected, 55f);
                var ai = enemy.AddComponent<InfectedChaser>();
                ai.Configure(2.6f + i * 0.08f, 55f + i * 4f, 10f + i, 3 + i);
                enemy.GetComponent<Renderer>().sharedMaterial = palette.Infected;
            }
        }

        private static void CreateStartingLoot(Palette palette)
        {
            var loot = new[]
            {
                (new Vector3(-4.1f, 0.55f, 1.8f), 3),
                (new Vector3(4.2f, 0.55f, 7.8f), 5),
                (new Vector3(-4.2f, 0.55f, 13.2f), 8)
            };

            foreach (var entry in loot)
            {
                var pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pickup.name = $"ScrapCache_{entry.Item2}";
                pickup.transform.position = entry.Item1;
                pickup.transform.localScale = Vector3.one * 0.46f;
                pickup.GetComponent<Collider>().isTrigger = true;
                pickup.GetComponent<Renderer>().sharedMaterial = palette.Scrap;
                pickup.AddComponent<LootPickup>().Configure(entry.Item2);
            }
        }

        private static void CreateExtraction(Palette palette)
        {
            var zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "ExtractionZone_Alpha";
            zone.transform.position = new Vector3(0f, 0.08f, 20f);
            zone.transform.localScale = new Vector3(2.8f, 0.08f, 2.8f);
            zone.GetComponent<Collider>().isTrigger = true;
            zone.GetComponent<Renderer>().sharedMaterial = palette.Extraction;
            zone.AddComponent<ExtractionZone>();

            var beacon = new GameObject("Extraction_BeaconLight");
            beacon.transform.position = new Vector3(0f, 3.5f, 20f);
            var light = beacon.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 11f;
            light.intensity = 7f;
            light.color = new Color(0.1f, 1f, 0.42f);
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static void AddSceneToBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.Exists(scene => scene.path == ScenePath))
                return;

            scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private sealed class Palette
        {
            public Material Ground;
            public Material Road;
            public Material Concrete;
            public Material Survivor;
            public Material Infected;
            public Material Scrap;
            public Material Extraction;
        }
    }
}
