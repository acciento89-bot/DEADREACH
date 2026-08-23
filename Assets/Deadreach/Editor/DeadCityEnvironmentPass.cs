using Kamilunavo.Deadreach.Extraction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    public static class DeadCityEnvironmentPass
    {
        private const string EnvironmentRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Environment/glTF";
        private const string VehicleRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Vehicles/glTF";
        private const string ProductionRoot = "Assets/Deadreach/Art/Production";
        private const string MaterialRoot = ProductionRoot + "/Materials";
        private const string VolumeRoot = ProductionRoot + "/Volumes";
        private const string EnvironmentMaterialPath = MaterialRoot + "/Quaternius_EnvironmentAtlas.mat";
        private const string EnvironmentAtlasPath = EnvironmentRoot + "/Zombie_Atlas.png";
        private const string VolumeProfilePath = VolumeRoot + "/DeadCity_ProductionProfile.asset";

        [MenuItem("DEADREACH/Production/Apply Dead City Environment 0.4", priority = 40)]
        public static void EnhanceCurrentDeadCityScene()
        {
            EnsureProductionFolders();

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.4 cannot dress Dead City because no valid scene is open.");
                return;
            }

            RemovePreviousPass();
            ConfigureAtmosphere();
            ConfigurePostProcessing();

            var material = EnsureEnvironmentMaterial();
            var root = new GameObject("Production_DeadCity_Environment_0_4");

            var coreMissing = 0;
            coreMissing += Place(false, root.transform, "Street_Crack1.gltf", "Street_Crack_A", new Vector3(0f, 0.045f, -8.5f), 0f, 1.35f, material, false) ? 0 : 1;
            coreMissing += Place(false, root.transform, "Street_Straight.gltf", "Street_Straight_A", new Vector3(0f, 0.047f, -2.5f), 0f, 1.35f, material, false) ? 0 : 1;
            coreMissing += Place(false, root.transform, "Street_4Way.gltf", "Street_Intersection", new Vector3(0f, 0.05f, 3.2f), 0f, 1.35f, material, false) ? 0 : 1;
            coreMissing += Place(false, root.transform, "Street_Crack2.gltf", "Street_Crack_B", new Vector3(0f, 0.045f, 9f), 0f, 1.35f, material, false) ? 0 : 1;
            coreMissing += Place(false, root.transform, "Street_Straight.gltf", "Street_Straight_B", new Vector3(0f, 0.047f, 15f), 0f, 1.35f, material, false) ? 0 : 1;

            // Traversal silhouettes and blockers.
            Place(false, root.transform, "TrafficBarrier_1.gltf", "Barrier_West_A", new Vector3(-2.8f, 0.05f, -2f), 16f, 1.15f, material, true);
            Place(false, root.transform, "TrafficBarrier_1.gltf", "Barrier_East_A", new Vector3(2.6f, 0.05f, 5.5f), -18f, 1.15f, material, true);
            Place(false, root.transform, "PlasticBarrier.gltf", "Barrier_West_B", new Vector3(-3.05f, 0.05f, 8.2f), 12f, 1.05f, material, true);
            Place(false, root.transform, "PlasticBarrier.gltf", "Barrier_East_B", new Vector3(3.1f, 0.05f, 12.6f), -14f, 1.05f, material, true);
            Place(false, root.transform, "Container_Green.gltf", "Container_Green", new Vector3(-7.3f, 0.02f, 7.5f), 88f, 1.05f, material, true);
            Place(false, root.transform, "Container_Red.gltf", "Container_Red", new Vector3(7.5f, 0.02f, 12.4f), -92f, 1.05f, material, true);
            Place(false, root.transform, "WaterTower.gltf", "Landmark_WaterTower", new Vector3(-12.6f, 0.02f, 16.5f), 18f, 1.2f, material, true);

            // Wrecked traffic.
            Place(true, root.transform, "Vehicle_Pickup.gltf", "Wreck_Pickup", new Vector3(1.7f, 0.04f, -3.4f), 17f, 1f, material, true);
            Place(true, root.transform, "Vehicle_Sports.gltf", "Wreck_Sports", new Vector3(-1.9f, 0.04f, 10.3f), -16f, 1f, material, true);
            Place(true, root.transform, "Vehicle_Truck.gltf", "Wreck_Truck", new Vector3(2.3f, 0.04f, 17.2f), 11f, 1f, material, true);

            // Storytelling clutter.
            Place(false, root.transform, "Barrel.gltf", "Barrel_A", new Vector3(-5.1f, 0.03f, 0.6f), 0f, 1f, material, true);
            Place(false, root.transform, "Barrel.gltf", "Barrel_B", new Vector3(-5.7f, 0.03f, 1.2f), 28f, 1f, material, true);
            Place(false, root.transform, "Pallet_Broken.gltf", "Broken_Pallet", new Vector3(5.5f, 0.03f, 7.1f), -22f, 1.1f, material, false);
            Place(false, root.transform, "Pipes.gltf", "Pipes", new Vector3(6.4f, 0.03f, 7.8f), 30f, 1.05f, material, true);
            Place(false, root.transform, "TrashBag_1.gltf", "Trash_A", new Vector3(-5.2f, 0.03f, 13.3f), 18f, 1f, material, false);
            Place(false, root.transform, "TrashBag_2.gltf", "Trash_B", new Vector3(-5.8f, 0.03f, 13.7f), -10f, 1f, material, false);
            Place(false, root.transform, "Wheels_Stack.gltf", "WheelStack", new Vector3(5.8f, 0.03f, -5.4f), 8f, 1f, material, true);
            Place(false, root.transform, "Blood_1.gltf", "Blood_A", new Vector3(1.6f, 0.06f, 5.7f), 31f, 1.1f, material, false);
            Place(false, root.transform, "Blood_2.gltf", "Blood_B", new Vector3(-1.4f, 0.061f, 14.1f), -17f, 1.25f, material, false);

            // Real lamp geometry; cheap prototype point lights remain as functional lighting.
            Place(false, root.transform, "StreetLights.gltf", "StreetLight_Real_A", new Vector3(-4.6f, 0f, -5f), 0f, 1f, material, false);
            Place(false, root.transform, "StreetLights.gltf", "StreetLight_Real_B", new Vector3(4.6f, 0f, 5f), 180f, 1f, material, false);
            Place(false, root.transform, "StreetLights.gltf", "StreetLight_Real_C", new Vector3(-4.6f, 0f, 14f), 0f, 1f, material, false);
            Place(false, root.transform, "TrafficLight_1.gltf", "TrafficLight", new Vector3(4.8f, 0f, 2.4f), 180f, 1f, material, false);

            AddProductionStreetLights(root.transform);
            EnhanceExtractionBeacon(root.transform);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            if (coreMissing > 0)
                Debug.LogWarning($"DEADREACH Production 0.4 atmosphere applied, but {coreMissing} core street assets were missing. Run tools/install-quaternius-deadcity-set.ps1 and rebuild.");
            else
                Debug.Log("DEADREACH Production 0.4 Dead City dressing applied: Quaternius streets/props/vehicles + production fog/lighting/post-processing/extraction beacon.");
        }

        private static bool Place(bool vehicle, Transform parent, string fileName, string instanceName, Vector3 position, float yaw, float scale, Material material, bool addCollider)
        {
            var rootPath = vehicle ? VehicleRoot : EnvironmentRoot;
            var assetPath = $"{rootPath}/{fileName}";
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (source == null)
            {
                Debug.LogWarning($"DEADREACH 0.4 missing environment asset: {assetPath}");
                return false;
            }

            var instance = PrefabUtility.InstantiatePrefab(source) as GameObject ?? Object.Instantiate(source);
            instance.name = instanceName;
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            instance.transform.localScale = Vector3.one * scale;
            SetStaticRecursive(instance);

            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
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

            // Imported glTF/model prefab instances can reject or invalidate components added
            // directly to their root. Keep gameplay collision on a plain DEADREACH-owned child
            // so collider authoring is deterministic and independent of prefab importer state.
            var collisionRoot = new GameObject("CollisionBounds");
            collisionRoot.transform.SetParent(root.transform, false);
            collisionRoot.transform.localPosition = Vector3.zero;
            collisionRoot.transform.localRotation = Quaternion.identity;
            collisionRoot.transform.localScale = Vector3.one;
            GameObjectUtility.SetStaticEditorFlags(
                collisionRoot,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);

            var box = collisionRoot.AddComponent<BoxCollider>();
            if (box == null)
            {
                Debug.LogError($"DEADREACH 0.4 failed to create bounds collider for '{root.name}'.");
                Object.DestroyImmediate(collisionRoot);
                return;
            }

            box.center = root.transform.InverseTransformPoint(bounds.center);
            var lossyScale = root.transform.lossyScale;
            box.size = new Vector3(
                bounds.size.x / Mathf.Max(Mathf.Abs(lossyScale.x), 0.001f),
                bounds.size.y / Mathf.Max(Mathf.Abs(lossyScale.y), 0.001f),
                bounds.size.z / Mathf.Max(Mathf.Abs(lossyScale.z), 0.001f));
        }

        private static void SetStaticRecursive(GameObject root)
        {
            GameObjectUtility.SetStaticEditorFlags(root, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            foreach (Transform child in root.transform)
                SetStaticRecursive(child.gameObject);
        }

        private static Material EnsureEnvironmentMaterial()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(EnvironmentAtlasPath);
            if (atlas == null)
                return null;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                return null;

            var material = AssetDatabase.LoadAssetAtPath<Material>(EnvironmentMaterialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "Quaternius_EnvironmentAtlas" };
                AssetDatabase.CreateAsset(material, EnvironmentMaterialPath);
            }

            material.shader = shader;
            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", atlas);
            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", atlas);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", Color.white);
            if (material.HasProperty("_Color")) material.SetColor("_Color", Color.white);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0f);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.2f);
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
            return material;
        }

        private static void ConfigureAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0125f;
            RenderSettings.fogColor = new Color(0.055f, 0.075f, 0.09f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.11f, 0.15f, 0.19f);
            RenderSettings.ambientEquatorColor = new Color(0.055f, 0.065f, 0.075f);
            RenderSettings.ambientGroundColor = new Color(0.018f, 0.02f, 0.025f);

            var moon = GameObject.Find("Lighting_MoonKey")?.GetComponent<Light>();
            if (moon != null)
            {
                moon.intensity = 0.85f;
                moon.color = new Color(0.55f, 0.68f, 0.9f);
                moon.shadows = LightShadows.Soft;
            }

            var camera = Camera.main;
            if (camera != null)
            {
                camera.backgroundColor = new Color(0.025f, 0.04f, 0.055f);
                camera.farClipPlane = 120f;
            }
        }

        private static void ConfigurePostProcessing()
        {
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, VolumeProfilePath);
            }

            if (!profile.TryGet(out Bloom bloom)) bloom = profile.Add<Bloom>(true);
            bloom.active = true;
            bloom.intensity.Override(0.28f);
            bloom.threshold.Override(1.05f);
            bloom.scatter.Override(0.55f);

            if (!profile.TryGet(out ColorAdjustments color)) color = profile.Add<ColorAdjustments>(true);
            color.active = true;
            color.postExposure.Override(-0.1f);
            color.contrast.Override(14f);
            color.saturation.Override(-10f);
            color.colorFilter.Override(new Color(0.93f, 0.97f, 1f));

            if (!profile.TryGet(out Tonemapping tonemapping)) tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.active = true;
            tonemapping.mode.Override(TonemappingMode.ACES);

            if (!profile.TryGet(out Vignette vignette)) vignette = profile.Add<Vignette>(true);
            vignette.active = true;
            vignette.intensity.Override(0.2f);
            vignette.smoothness.Override(0.42f);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();

            var volumeObject = new GameObject("Production_PostProcessing_0_4");
            var volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 20f;
            volume.sharedProfile = profile;

            var camera = Camera.main;
            if (camera != null)
            {
                var additionalData = camera.GetComponent<UniversalAdditionalCameraData>() ?? camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
                additionalData.renderPostProcessing = true;
            }
        }

        private static void AddProductionStreetLights(Transform parent)
        {
            CreatePointLight(parent, "WarmPool_A", new Vector3(-4.4f, 4.35f, -5f), new Color(1f, 0.54f, 0.25f), 4.2f, 9f);
            CreatePointLight(parent, "WarmPool_B", new Vector3(4.4f, 4.35f, 5f), new Color(1f, 0.48f, 0.2f), 4f, 9f);
            CreatePointLight(parent, "WarmPool_C", new Vector3(-4.4f, 4.35f, 14f), new Color(1f, 0.55f, 0.28f), 4f, 9f);
            CreatePointLight(parent, "ColdFill", new Vector3(1f, 5.5f, 9f), new Color(0.28f, 0.45f, 0.8f), 2.2f, 14f);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(parent, true);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void EnhanceExtractionBeacon(Transform parent)
        {
            var extraction = Object.FindFirstObjectByType<ExtractionZone>();
            if (extraction == null)
                return;

            var beaconRoot = new GameObject("Extraction_Beacon_Production");
            beaconRoot.transform.SetParent(parent, true);
            beaconRoot.transform.position = extraction.transform.position;

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = "BeaconColumn";
            beam.transform.SetParent(beaconRoot.transform, false);
            beam.transform.localPosition = new Vector3(0f, 2.7f, 0f);
            beam.transform.localScale = new Vector3(0.08f, 2.7f, 0.08f);
            Object.DestroyImmediate(beam.GetComponent<Collider>());

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = "Runtime_ExtractionBeacon_0_4" };
            var baseColor = new Color(0.03f, 0.55f, 0.18f);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", baseColor);
            if (material.HasProperty("_Color")) material.SetColor("_Color", baseColor);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", new Color(0.05f, 1f, 0.38f) * 3.5f);
            }
            beam.GetComponent<Renderer>().sharedMaterial = material;

            CreatePointLight(beaconRoot.transform, "BeaconLight", extraction.transform.position + Vector3.up * 1.8f, new Color(0.08f, 1f, 0.38f), 5.5f, 10f);
        }

        private static void RemovePreviousPass()
        {
            var environment = GameObject.Find("Production_DeadCity_Environment_0_4");
            if (environment != null) Object.DestroyImmediate(environment);
            var volume = GameObject.Find("Production_PostProcessing_0_4");
            if (volume != null) Object.DestroyImmediate(volume);
        }

        private static void EnsureProductionFolders()
        {
            EnsureFolder("Assets/Deadreach/Art", "Production");
            EnsureFolder(ProductionRoot, "Materials");
            EnsureFolder(ProductionRoot, "Volumes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
