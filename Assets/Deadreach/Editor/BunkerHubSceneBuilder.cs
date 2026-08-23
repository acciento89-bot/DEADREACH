using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamilunavo.Deadreach.Editor
{
    public static class BunkerHubSceneBuilder
    {
        private const string SceneFolder = "Assets/Deadreach/Scenes";
        private const string MaterialFolder = "Assets/Deadreach/Art/DevPalette";
        private const string ScenePath = SceneFolder + "/Bunker_Hub.unity";

        [MenuItem("DEADREACH/Dev/Build Bunker Hub Only")]
        public static void Build()
        {
            EnsureFolders();
            var floorMaterial = GetOrCreateMaterial("BunkerFloor", new Color(0.045f, 0.05f, 0.052f));
            var wallMaterial = GetOrCreateMaterial("BunkerWall", new Color(0.11f, 0.12f, 0.115f));
            var consoleMaterial = GetOrCreateMaterial("BunkerConsole", new Color(0.035f, 0.2f, 0.19f));
            var rustMaterial = GetOrCreateMaterial("BunkerRust", new Color(0.34f, 0.11f, 0.055f));
            var hazardMaterial = GetOrCreateMaterial("BunkerHazard", new Color(0.78f, 0.48f, 0.08f));
            var darkMetalMaterial = GetOrCreateMaterial("BunkerDarkMetal", new Color(0.065f, 0.07f, 0.068f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigureAtmosphere();
            CreateShell(floorMaterial, wallMaterial, consoleMaterial, rustMaterial, hazardMaterial, darkMetalMaterial);
            CreateCamera();
            new GameObject("Bunker_CommandCenter_UI").AddComponent<BunkerCommandCenterUI>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            DeadreachBuildSettings.Repair();
            DeadreachPlayModeStart.Configure();
            Debug.Log($"DEADREACH Production Bunker Command Center generated at {ScenePath}");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.5", priority = 1)]
        public static void BuildCompleteSlice()
        {
            if (!DeadCityAssetRepair.EnsureRequiredAssetsReady())
            {
                Debug.LogError("DEADREACH Production Slice 0.5 aborted: required environment imports are not healthy. No partial scene was generated.");
                return;
            }

            if (!Production05OperatorArtSetup.EnsureOperatorAssetsReady())
            {
                Debug.LogError("DEADREACH Production Slice 0.5 aborted: distinct operator art could not be prepared. No partial scene was generated.");
                return;
            }

            VerticalSliceSceneBuilder.Build();
            ProductionSliceEnhancer.EnhanceCurrentDeadCityScene();
            AttachProduction05RuntimeSystems();
            DeadCityEnvironmentPass.EnhanceCurrentDeadCityScene();

            if (!DeadCityTraversalSafetyPass.Apply())
            {
                Debug.LogError("DEADREACH Production Slice 0.5 aborted: extraction traversal safety gate failed.");
                return;
            }

            Build();

            if (!DeadreachBuildSettings.ConfigureCompleteSlice())
                return;

            DeadreachPlayModeStart.Configure();
            EditorSceneManager.OpenScene(DeadreachBuildSettings.BunkerScenePath, OpenSceneMode.Single);
            Debug.Log("DEADREACH Production Slice 0.5 generated: post-apocalyptic Bunker + distinct Sam/Lis/Matt operators + Arsenal/Operators/Campaign/Store + 50-level progression + boss gates + Production 0.4 Dead City baseline.");
        }

        private static void AttachProduction05RuntimeSystems()
        {
            var session = Object.FindFirstObjectByType<RunSession>();
            if (session != null && session.GetComponent<RunDifficultyDirector>() == null)
                session.gameObject.AddComponent<RunDifficultyDirector>();

            var player = Object.FindFirstObjectByType<PlayerMotor>();
            if (player != null && player.GetComponent<OperatorRuntimeApplier>() == null)
                player.gameObject.AddComponent<OperatorRuntimeApplier>();
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(SceneFolder))
                AssetDatabase.CreateFolder("Assets/Deadreach", "Scenes");
            if (!AssetDatabase.IsValidFolder("Assets/Deadreach/Art"))
                AssetDatabase.CreateFolder("Assets/Deadreach", "Art");
            if (!AssetDatabase.IsValidFolder(MaterialFolder))
                AssetDatabase.CreateFolder("Assets/Deadreach/Art", "DevPalette");
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            if (material == null)
            {
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            material.shader = shader;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.25f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.019f;
            RenderSettings.fogColor = new Color(0.018f, 0.025f, 0.024f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.055f, 0.075f, 0.075f);
            RenderSettings.ambientEquatorColor = new Color(0.025f, 0.032f, 0.03f);
            RenderSettings.ambientGroundColor = new Color(0.008f, 0.012f, 0.011f);
        }

        private static void CreateShell(Material floor, Material wall, Material console, Material rust, Material hazard, Material darkMetal)
        {
            CreateCube("Bunker_Floor", new Vector3(0f, -0.35f, 2f), new Vector3(15f, 0.7f, 15f), floor);
            CreateCube("Bunker_BackWall", new Vector3(0f, 3f, 8.5f), new Vector3(15f, 6f, 0.6f), wall);
            CreateCube("Bunker_LeftWall", new Vector3(-7.2f, 3f, 2f), new Vector3(0.6f, 6f, 13f), wall);
            CreateCube("Bunker_RightWall", new Vector3(7.2f, 3f, 2f), new Vector3(0.6f, 6f, 13f), wall);
            CreateCube("Bunker_Ceiling", new Vector3(0f, 6.1f, 2f), new Vector3(15f, 0.35f, 13f), darkMetal);

            CreateCube("CommandTable", new Vector3(0f, 0.75f, 2.5f), new Vector3(4.4f, 1.5f, 2.2f), console);
            CreateCube("CommandTableTop", new Vector3(0f, 1.58f, 2.5f), new Vector3(4.7f, 0.16f, 2.45f), darkMetal);
            CreateCube("Workshop_Left", new Vector3(-5f, 1.1f, 5.7f), new Vector3(2.3f, 2.2f, 1.3f), wall);
            CreateCube("Storage_Right", new Vector3(5f, 1.25f, 5.8f), new Vector3(2.3f, 2.5f, 1.4f), wall);
            CreateCube("Generator", new Vector3(5.2f, 1.05f, -1f), new Vector3(2.5f, 2.1f, 1.8f), wall);

            CreateCube("BlastDoor_Frame_Left", new Vector3(-2.55f, 2.65f, 8.08f), new Vector3(0.45f, 5.1f, 0.35f), rust);
            CreateCube("BlastDoor_Frame_Right", new Vector3(2.55f, 2.65f, 8.08f), new Vector3(0.45f, 5.1f, 0.35f), rust);
            CreateCube("BlastDoor_Frame_Top", new Vector3(0f, 5.05f, 8.08f), new Vector3(5.5f, 0.4f, 0.35f), rust);
            CreateCube("BlastDoor", new Vector3(0f, 2.55f, 8.0f), new Vector3(4.7f, 4.6f, 0.35f), darkMetal);
            CreateCube("BlastDoor_Hazard", new Vector3(0f, 2.2f, 7.78f), new Vector3(4.3f, 0.24f, 0.12f), hazard);

            for (var z = -3; z <= 7; z += 2)
                CreateCube($"CeilingBeam_{z}", new Vector3(0f, 5.72f, z), new Vector3(14.4f, 0.25f, 0.35f), darkMetal);

            CreatePipe("Pipe_Left_A", new Vector3(-6.65f, 4.45f, 2f), new Vector3(0.22f, 0.22f, 5.8f), rust);
            CreatePipe("Pipe_Right_A", new Vector3(6.65f, 4.1f, 1.4f), new Vector3(0.18f, 0.18f, 5.3f), darkMetal);
            CreatePipe("Pipe_Back", new Vector3(-4.7f, 4.85f, 7.85f), new Vector3(2.1f, 0.16f, 0.16f), rust);

            CreateCrateStack(new Vector3(-5.65f, 0.45f, -1.4f), darkMetal, hazard);
            CreateCrateStack(new Vector3(5.6f, 0.45f, 4.2f), darkMetal, rust);
            CreateCrateStack(new Vector3(-4.9f, 0.45f, 4.25f), darkMetal, rust);

            CreateCube("WarningStrip_Left", new Vector3(-7.0f, 0.1f, 1.8f), new Vector3(0.18f, 0.05f, 8.4f), hazard);
            CreateCube("WarningStrip_Right", new Vector3(7.0f, 0.1f, 1.8f), new Vector3(0.18f, 0.05f, 8.4f), hazard);

            CreatePointLight("Bunker_Key", new Vector3(0f, 4.8f, 1.5f), new Color(0.25f, 0.65f, 0.7f), 9f, 12f, LightShadows.Soft);
            CreatePointLight("Workshop_Warm", new Vector3(-4.7f, 3.2f, 4.7f), new Color(1f, 0.32f, 0.12f), 6f, 7f, LightShadows.Soft);
            CreatePointLight("Generator_Green", new Vector3(5f, 2.8f, -0.6f), new Color(0.12f, 0.82f, 0.38f), 4.5f, 6f, LightShadows.None);
            CreatePointLight("BlastDoor_Red", new Vector3(0f, 4.9f, 6.7f), new Color(1f, 0.08f, 0.035f), 4.2f, 6.5f, LightShadows.None);
        }

        private static void CreateCrateStack(Vector3 position, Material primary, Material accent)
        {
            CreateCube("SupplyCrate", position, new Vector3(1.3f, 0.8f, 0.9f), primary);
            CreateCube("SupplyCrate_Top", position + new Vector3(0.25f, 0.75f, 0.05f), new Vector3(0.9f, 0.65f, 0.75f), primary);
            CreateCube("CrateMark", position + new Vector3(0f, 0f, -0.47f), new Vector3(0.72f, 0.16f, 0.04f), accent);
        }

        private static void CreatePipe(string name, Vector3 position, Vector3 scale, Material material)
        {
            var pipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pipe.name = name;
            pipe.transform.position = position;
            pipe.transform.localScale = scale;
            pipe.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            pipe.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 8.4f, -11.8f);
            cameraObject.transform.LookAt(new Vector3(0f, 1.6f, 2.9f));
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 47f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 80f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.012f, 0.018f, 0.018f);
            cameraObject.AddComponent<AudioListener>();
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

        private static void CreatePointLight(string name, Vector3 position, Color color, float intensity, float range, LightShadows shadows)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = shadows;
        }
    }
}
