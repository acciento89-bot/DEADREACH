using System.Collections.Generic;
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

        [MenuItem("DEADREACH/Build Bunker Hub 0.1")]
        public static void Build()
        {
            EnsureFolders();
            var floorMaterial = GetOrCreateMaterial("BunkerFloor", new Color(0.055f, 0.065f, 0.07f));
            var wallMaterial = GetOrCreateMaterial("BunkerWall", new Color(0.16f, 0.18f, 0.19f));
            var consoleMaterial = GetOrCreateMaterial("BunkerConsole", new Color(0.04f, 0.28f, 0.3f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigureAtmosphere();
            CreateShell(floorMaterial, wallMaterial, consoleMaterial);
            CreateCamera();
            new GameObject("Bunker_Menu").AddComponent<BunkerPrototypeMenu>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            PutBunkerFirstInBuildSettings();
            Debug.Log($"DEADREACH Bunker Hub generated at {ScenePath}");
        }

        [MenuItem("DEADREACH/Build Complete Vertical Slice 0.1")]
        public static void BuildCompleteSlice()
        {
            VerticalSliceSceneBuilder.Build();
            Build();
            Debug.Log("DEADREACH complete Vertical Slice 0.1 generated. Press Play from Bunker_Hub.");
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
            if (material != null)
                return material;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader) { name = name };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            else
                material.color = color;
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.4f);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void ConfigureAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.014f;
            RenderSettings.fogColor = new Color(0.025f, 0.04f, 0.045f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.07f, 0.1f, 0.11f);
            RenderSettings.ambientEquatorColor = new Color(0.035f, 0.045f, 0.05f);
            RenderSettings.ambientGroundColor = new Color(0.015f, 0.02f, 0.02f);
        }

        private static void CreateShell(Material floor, Material wall, Material console)
        {
            CreateCube("Bunker_Floor", new Vector3(0f, -0.35f, 2f), new Vector3(15f, 0.7f, 15f), floor);
            CreateCube("Bunker_BackWall", new Vector3(0f, 3f, 8.5f), new Vector3(15f, 6f, 0.6f), wall);
            CreateCube("Bunker_LeftWall", new Vector3(-7.2f, 3f, 2f), new Vector3(0.6f, 6f, 13f), wall);
            CreateCube("Bunker_RightWall", new Vector3(7.2f, 3f, 2f), new Vector3(0.6f, 6f, 13f), wall);

            CreateCube("CommandTable", new Vector3(0f, 0.75f, 2.5f), new Vector3(4.4f, 1.5f, 2.2f), console);
            CreateCube("Workshop_Left", new Vector3(-5f, 1.1f, 5.7f), new Vector3(2.3f, 2.2f, 1.3f), wall);
            CreateCube("Storage_Right", new Vector3(5f, 1.25f, 5.8f), new Vector3(2.3f, 2.5f, 1.4f), wall);
            CreateCube("Generator", new Vector3(5.2f, 1.05f, -1f), new Vector3(2.5f, 2.1f, 1.8f), wall);

            CreatePointLight("Bunker_Key", new Vector3(0f, 4.8f, 1.5f), new Color(0.32f, 0.72f, 0.8f), 8.5f, 12f);
            CreatePointLight("Workshop_Warm", new Vector3(-4.7f, 3.2f, 4.7f), new Color(1f, 0.45f, 0.18f), 5f, 7f);
            CreatePointLight("Generator_Green", new Vector3(5f, 2.8f, -0.6f), new Color(0.15f, 0.8f, 0.4f), 4f, 6f);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 8.2f, -11.5f);
            cameraObject.transform.LookAt(new Vector3(0f, 1.3f, 2.7f));
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 47f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 80f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.018f, 0.028f, 0.03f);
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

        private static void CreatePointLight(string name, Vector3 position, Color color, float intensity, float range)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.Soft;
        }

        private static void PutBunkerFirstInBuildSettings()
        {
            var existing = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            existing.RemoveAll(scene => scene.path == ScenePath);
            existing.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = existing.ToArray();
        }
    }
}
