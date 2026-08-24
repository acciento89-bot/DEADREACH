using System.IO;
using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production13BunkerScenePass
    {
        private const string BunkerScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        private const string MaterialFolder = "Assets/Deadreach/Art/Production/Materials";
        private const string RootName = "Production_BunkerVisual_0_13";

        public static bool Apply()
        {
            if (!File.Exists(BunkerScenePath))
            {
                Debug.LogError($"DEADREACH 0.13 bunker pass could not find {BunkerScenePath}.");
                return false;
            }

            EnsureMaterialFolder();
            var scene = EditorSceneManager.OpenScene(BunkerScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.13 bunker pass could not open the generated Bunker scene.");
                return false;
            }

            var previous = GameObject.Find(RootName);
            if (previous != null)
                Object.DestroyImmediate(previous);

            var graphite = GetMaterial("P13_Graphite", new Color(0.018f, 0.026f, 0.031f), 0.60f, Color.black);
            var steel = GetMaterial("P13_Steel", new Color(0.050f, 0.066f, 0.071f), 0.72f, Color.black);
            var cyan = GetMaterial("P13_CyanEmissive", new Color(0.025f, 0.13f, 0.16f), 0.34f, new Color(0.08f, 1.6f, 2.1f));
            var amber = GetMaterial("P13_AmberEmissive", new Color(0.16f, 0.050f, 0.012f), 0.32f, new Color(2.3f, 0.62f, 0.06f));
            var red = GetMaterial("P13_RedEmissive", new Color(0.13f, 0.010f, 0.008f), 0.28f, new Color(2.2f, 0.08f, 0.04f));
            var screen = GetMaterial("P13_Screen", new Color(0.010f, 0.040f, 0.050f), 0.22f, new Color(0.10f, 0.85f, 1.15f));

            var root = new GameObject(RootName);
            root.AddComponent<Production13BunkerAtmosphere>();

            BuildArchitecturalRibs(root.transform, graphite, steel, cyan);
            BuildCommandTableUpgrade(root.transform, graphite, steel, cyan, amber);
            BuildServerBanks(root.transform, graphite, steel, cyan, amber);
            BuildTacticalWall(root.transform, graphite, steel, screen, cyan, amber);
            BuildFloorGuides(root.transform, cyan, amber);
            BuildDoorWarningArray(root.transform, graphite, red, amber);
            BuildLighting(root.transform);
            TuneExistingCamera();
            TuneAtmosphere();

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("DEADREACH Production 0.13 bunker scene authored: cinematic command-center ribs, holo table, tactical wall, server banks, floor guidance and animated emergency lighting.");
            return true;
        }

        private static void BuildArchitecturalRibs(Transform parent, Material graphite, Material steel, Material cyan)
        {
            var root = new GameObject("P13_ArchitecturalRibs");
            root.transform.SetParent(parent, false);

            for (var z = -2.8f; z <= 7.3f; z += 2.05f)
            {
                CreateCube(root.transform, $"Rib_Left_{z:0.0}", new Vector3(-6.72f, 3.15f, z), new Vector3(0.18f, 5.9f, 0.22f), graphite);
                CreateCube(root.transform, $"Rib_Right_{z:0.0}", new Vector3(6.72f, 3.15f, z), new Vector3(0.18f, 5.9f, 0.22f), graphite);
                CreateCube(root.transform, $"Rib_Ceiling_{z:0.0}", new Vector3(0f, 5.62f, z), new Vector3(13.6f, 0.16f, 0.20f), steel);

                var leftAccent = CreateCube(root.transform, $"RibAccent_L_{z:0.0}", new Vector3(-6.57f, 3.20f, z - 0.03f), new Vector3(0.035f, 2.6f, 0.04f), cyan);
                var rightAccent = CreateCube(root.transform, $"RibAccent_R_{z:0.0}", new Vector3(6.57f, 3.20f, z - 0.03f), new Vector3(0.035f, 2.6f, 0.04f), cyan);
                DisableCollider(leftAccent);
                DisableCollider(rightAccent);
            }
        }

        private static void BuildCommandTableUpgrade(Transform parent, Material graphite, Material steel, Material cyan, Material amber)
        {
            var root = new GameObject("P13_CommandTableUpgrade");
            root.transform.SetParent(parent, false);

            CreateCube(root.transform, "Table_BaseShadow", new Vector3(0f, 0.30f, 2.5f), new Vector3(5.2f, 0.20f, 3.0f), graphite);
            CreateCube(root.transform, "Table_EdgeFront", new Vector3(0f, 1.55f, 1.25f), new Vector3(4.8f, 0.08f, 0.07f), amber);
            CreateCube(root.transform, "Table_EdgeLeft", new Vector3(-2.38f, 1.55f, 2.50f), new Vector3(0.07f, 0.08f, 2.45f), cyan);
            CreateCube(root.transform, "Table_EdgeRight", new Vector3(2.38f, 1.55f, 2.50f), new Vector3(0.07f, 0.08f, 2.45f), cyan);

            var rotor = new GameObject("P13_HoloRotor");
            rotor.transform.SetParent(root.transform, false);
            rotor.transform.position = new Vector3(0f, 1.82f, 2.5f);

            const int segments = 20;
            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var radius = i % 2 == 0 ? 1.42f : 1.26f;
                var pos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                var segment = CreateCube(rotor.transform, $"HoloSegment_{i:00}", pos, new Vector3(0.34f, 0.025f, 0.055f), i % 5 == 0 ? amber : cyan);
                segment.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
                DisableCollider(segment);
            }

            CreateCube(rotor.transform, "HoloCore", Vector3.zero, new Vector3(0.18f, 0.55f, 0.18f), cyan);
            CreatePointLight(root.transform, "P13_HoloLight", new Vector3(0f, 2.25f, 2.5f), new Color(0.08f, 0.78f, 0.95f), 3.4f, 5.0f, LightShadows.None);
        }

        private static void BuildServerBanks(Transform parent, Material graphite, Material steel, Material cyan, Material amber)
        {
            var left = new GameObject("P13_ServerBank_Left");
            left.transform.SetParent(parent, false);
            var right = new GameObject("P13_ServerBank_Right");
            right.transform.SetParent(parent, false);

            BuildServerStack(left.transform, new Vector3(-5.45f, 1.55f, 5.90f), graphite, steel, cyan, amber, false);
            BuildServerStack(right.transform, new Vector3(5.45f, 1.55f, 5.90f), graphite, steel, cyan, amber, true);
        }

        private static void BuildServerStack(Transform parent, Vector3 center, Material graphite, Material steel, Material cyan, Material amber, bool mirror)
        {
            for (var i = 0; i < 3; i++)
            {
                var x = center.x + (i - 1) * 0.78f;
                CreateCube(parent, $"Rack_{i}", new Vector3(x, center.y, center.z), new Vector3(0.66f, 2.85f, 0.74f), graphite);
                CreateCube(parent, $"RackFrame_{i}", new Vector3(x, center.y + 0.05f, center.z - 0.385f), new Vector3(0.54f, 2.45f, 0.035f), steel);

                for (var slot = 0; slot < 7; slot++)
                {
                    var y = center.y - 1.0f + slot * 0.32f;
                    var material = (slot + i) % 4 == 0 ? amber : cyan;
                    var led = CreateCube(parent, $"RackLed_{i}_{slot}", new Vector3(x + (mirror ? 0.20f : -0.20f), y, center.z - 0.41f), new Vector3(0.09f, 0.025f, 0.018f), material);
                    DisableCollider(led);
                }
            }

            CreatePointLight(parent, "P13_Monitor_Server", center + new Vector3(0f, 0.8f, -1.0f), new Color(0.10f, 0.55f, 0.68f), 1.7f, 3.6f, LightShadows.None);
        }

        private static void BuildTacticalWall(Transform parent, Material graphite, Material steel, Material screen, Material cyan, Material amber)
        {
            var root = new GameObject("P13_TacticalWall");
            root.transform.SetParent(parent, false);

            var baseZ = 7.63f;
            CreateCube(root.transform, "TacWall_Backplate", new Vector3(0f, 3.35f, baseZ), new Vector3(6.9f, 2.55f, 0.12f), graphite);

            var centers = new[]
            {
                new Vector3(-2.05f, 3.55f, baseZ - 0.08f),
                new Vector3(0f, 3.55f, baseZ - 0.08f),
                new Vector3(2.05f, 3.55f, baseZ - 0.08f)
            };

            for (var i = 0; i < centers.Length; i++)
            {
                CreateCube(root.transform, $"ScreenFrame_{i}", centers[i], new Vector3(1.82f, 1.48f, 0.10f), steel);
                var face = CreateCube(root.transform, $"ScreenFace_{i}", centers[i] + new Vector3(0f, 0f, -0.065f), new Vector3(1.60f, 1.24f, 0.025f), screen);
                DisableCollider(face);

                for (var line = 0; line < 4; line++)
                {
                    var width = 1.10f - line * 0.13f;
                    var bar = CreateCube(root.transform, $"ScreenData_{i}_{line}", centers[i] + new Vector3(-0.12f, 0.36f - line * 0.22f, -0.085f),
                        new Vector3(width, 0.025f, 0.012f), line == 0 ? amber : cyan);
                    DisableCollider(bar);
                }
            }

            CreatePointLight(root.transform, "P13_Monitor_Wall", new Vector3(0f, 3.7f, 6.8f), new Color(0.08f, 0.58f, 0.70f), 2.3f, 5.2f, LightShadows.None);
        }

        private static void BuildFloorGuides(Transform parent, Material cyan, Material amber)
        {
            var root = new GameObject("P13_FloorGuides");
            root.transform.SetParent(parent, false);

            for (var i = 0; i < 9; i++)
            {
                var z = -2.4f + i * 1.12f;
                var mat = i % 4 == 0 ? amber : cyan;
                var left = CreateCube(root.transform, $"FloorGuide_L_{i}", new Vector3(-3.25f, 0.035f, z), new Vector3(0.46f, 0.025f, 0.06f), mat);
                var right = CreateCube(root.transform, $"FloorGuide_R_{i}", new Vector3(3.25f, 0.035f, z), new Vector3(0.46f, 0.025f, 0.06f), mat);
                DisableCollider(left);
                DisableCollider(right);
            }
        }

        private static void BuildDoorWarningArray(Transform parent, Material graphite, Material red, Material amber)
        {
            var root = new GameObject("P13_DoorWarningArray");
            root.transform.SetParent(parent, false);

            CreateCube(root.transform, "DoorStatusHousing", new Vector3(0f, 5.34f, 7.72f), new Vector3(2.3f, 0.32f, 0.10f), graphite);
            for (var i = 0; i < 6; i++)
            {
                var x = -0.86f + i * 0.34f;
                var lamp = CreateCube(root.transform, $"DoorStatus_{i}", new Vector3(x, 5.34f, 7.64f), new Vector3(0.15f, 0.08f, 0.035f), i < 4 ? red : amber);
                DisableCollider(lamp);
            }

            CreatePointLight(root.transform, "P13_Emergency_Door_Left", new Vector3(-2.8f, 4.8f, 6.65f), new Color(1f, 0.05f, 0.025f), 3.0f, 4.5f, LightShadows.None);
            CreatePointLight(root.transform, "P13_Emergency_Door_Right", new Vector3(2.8f, 4.8f, 6.65f), new Color(1f, 0.05f, 0.025f), 3.0f, 4.5f, LightShadows.None);
        }

        private static void BuildLighting(Transform parent)
        {
            CreatePointLight(parent, "P13_Monitor_LeftWash", new Vector3(-4.6f, 3.5f, 2.0f), new Color(0.08f, 0.34f, 0.44f), 2.0f, 5.8f, LightShadows.None);
            CreatePointLight(parent, "P13_Monitor_RightWash", new Vector3(4.6f, 3.5f, 2.0f), new Color(0.08f, 0.34f, 0.44f), 2.0f, 5.8f, LightShadows.None);
            CreatePointLight(parent, "P13_CommandWarm", new Vector3(0f, 3.0f, 0.2f), new Color(1f, 0.26f, 0.08f), 1.65f, 5.4f, LightShadows.None);
        }

        private static void TuneExistingCamera()
        {
            var camera = Camera.main ?? Object.FindFirstObjectByType<Camera>();
            if (camera == null)
                return;

            camera.backgroundColor = new Color(0.002f, 0.007f, 0.011f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.fieldOfView = 45f;
        }

        private static void TuneAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.023f;
            RenderSettings.fogColor = new Color(0.006f, 0.016f, 0.021f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.025f, 0.065f, 0.078f);
            RenderSettings.ambientEquatorColor = new Color(0.012f, 0.025f, 0.029f);
            RenderSettings.ambientGroundColor = new Color(0.003f, 0.007f, 0.009f);
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            return go;
        }

        private static void DisableCollider(GameObject go)
        {
            var collider = go != null ? go.GetComponent<Collider>() : null;
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range, LightShadows shadows)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = shadows;
        }

        private static Material GetMaterial(string name, Color baseColor, float smoothness, Color emission)
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
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", baseColor);
            if (material.HasProperty("_Color")) material.SetColor("_Color", baseColor);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (emission.maxColorComponent > 0.001f)
            {
                material.EnableKeyword("_EMISSION");
                if (material.HasProperty("_EmissionColor")) material.SetColor("_EmissionColor", emission);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureMaterialFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Deadreach/Art/Production"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Deadreach/Art"))
                    AssetDatabase.CreateFolder("Assets/Deadreach", "Art");
                AssetDatabase.CreateFolder("Assets/Deadreach/Art", "Production");
            }
            if (!AssetDatabase.IsValidFolder(MaterialFolder))
                AssetDatabase.CreateFolder("Assets/Deadreach/Art/Production", "Materials");
        }
    }
}
