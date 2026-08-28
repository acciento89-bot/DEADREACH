using System;
using System.Collections;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Presentation
{
    /// <summary>
    /// Production 0.16 Dead City presentation layer. It deliberately leaves the validated
    /// 0.12 gameplay geometry/colliders in place and upgrades only the visible city shell.
    /// Everything generated here is decoration-only and owns no gameplay collision.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Production16DeadCityVisuals : MonoBehaviour
    {
        private static bool _hooked;

        private readonly List<Material> _runtimeMaterials = new();
        private Transform _generatedRoot;

        private Material _buildingMaterial;
        private Material _buildingAccentMaterial;
        private Material _roadMaterial;
        private Material _sidewalkMaterial;
        private Material _laneMaterial;
        private Material _windowDarkMaterial;
        private Material _windowLitMaterial;
        private Material _roofMaterial;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (_hooked)
                return;

            _hooked = true;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!string.Equals(scene.name, SceneFlowService.ExpeditionSceneName, StringComparison.Ordinal))
                return;

            if (FindFirstObjectByType<Production16DeadCityVisuals>() != null)
                return;

            new GameObject("Production16_DeadCity_Visuals").AddComponent<Production16DeadCityVisuals>();
        }

        private IEnumerator Start()
        {
            // Let the stable scene/sector presentation finish its own Start pass first.
            yield return null;
            yield return null;
            ApplyVisualPass();
        }

        private void ApplyVisualPass()
        {
            if (_generatedRoot != null)
                return;

            BuildMaterials();
            var root = new GameObject("P16_DeadCity_VisualLayer");
            _generatedRoot = root.transform;
            _generatedRoot.SetParent(transform, false);

            RestyleBaseSurfaces();
            BuildStreetReadability();
            BuildFacadeDetails();
            BuildStreetFurniture();
            ImproveLightingReadability();

            Debug.Log("DEADREACH 0.16 Dead City visual reboot applied: production facades, sidewalks, street markings, roof detail and readability lighting active; gameplay collision unchanged.");
        }

        private void BuildMaterials()
        {
            _buildingMaterial = MakeLit("P16_Building", new Color(0.105f, 0.115f, 0.135f), 0.04f, 0.18f);
            _buildingAccentMaterial = MakeLit("P16_BuildingAccent", new Color(0.17f, 0.15f, 0.16f), 0.12f, 0.22f);
            _roadMaterial = MakeLit("P16_Asphalt", new Color(0.055f, 0.05f, 0.075f), 0.02f, 0.12f);
            _sidewalkMaterial = MakeLit("P16_Sidewalk", new Color(0.19f, 0.17f, 0.20f), 0.02f, 0.17f);
            _laneMaterial = MakeLit("P16_LanePaint", new Color(0.53f, 0.49f, 0.39f), 0f, 0.12f);
            _windowDarkMaterial = MakeLit("P16_WindowDark", new Color(0.025f, 0.035f, 0.05f), 0.08f, 0.55f);
            _windowLitMaterial = MakeLit("P16_WindowLit", new Color(0.22f, 0.30f, 0.34f), 0.05f, 0.45f);
            _roofMaterial = MakeLit("P16_Roof", new Color(0.07f, 0.075f, 0.085f), 0.18f, 0.20f);

            if (_windowLitMaterial != null)
            {
                if (_windowLitMaterial.HasProperty("_EmissionColor"))
                {
                    _windowLitMaterial.EnableKeyword("_EMISSION");
                    _windowLitMaterial.SetColor("_EmissionColor", new Color(0.14f, 0.42f, 0.50f) * 1.35f);
                }
            }
        }

        private Material MakeLit(string name, Color color, float metallic, float smoothness)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                return null;

            var material = new Material(shader) { name = name };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            _runtimeMaterials.Add(material);
            return material;
        }

        private void RestyleBaseSurfaces()
        {
            SetMaterial(GameObject.Find("Road_Main"), _roadMaterial);
            SetMaterial(GameObject.Find("Road_Cross"), _roadMaterial);

            var ground = GameObject.Find("World_Ground");
            if (ground != null)
            {
                var renderer = ground.GetComponent<Renderer>();
                if (renderer != null && _buildingMaterial != null)
                    renderer.sharedMaterial = _buildingMaterial;
            }

            for (var i = 1; i <= 6; i++)
            {
                var building = GameObject.Find($"Building_Block_{i:00}");
                SetMaterial(building, i % 3 == 0 ? _buildingAccentMaterial : _buildingMaterial);
            }
        }

        private static void SetMaterial(GameObject target, Material material)
        {
            if (target == null || material == null)
                return;

            var renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }

        private void BuildStreetReadability()
        {
            // Main-road sidewalks: decorative only, slightly above the old ground so the street gets
            // a readable profile without changing CharacterController collision.
            CreateDecorCube("Sidewalk_West", new Vector3(-5.05f, 0.075f, 2.3f), new Vector3(1.65f, 0.10f, 39.5f), _sidewalkMaterial);
            CreateDecorCube("Sidewalk_East", new Vector3(5.05f, 0.075f, 2.3f), new Vector3(1.65f, 0.10f, 39.5f), _sidewalkMaterial);
            CreateDecorCube("Curb_West", new Vector3(-4.18f, 0.13f, 2.3f), new Vector3(0.13f, 0.17f, 39.5f), _buildingAccentMaterial);
            CreateDecorCube("Curb_East", new Vector3(4.18f, 0.13f, 2.3f), new Vector3(0.13f, 0.17f, 39.5f), _buildingAccentMaterial);

            // Faded broken center line. Keep it sparse so combat silhouettes remain clear.
            for (var z = -15f; z <= 20f; z += 3.25f)
                CreateDecorCube($"Lane_{z:0.00}", new Vector3(0f, 0.075f, z), new Vector3(0.10f, 0.018f, 1.25f), _laneMaterial);

            // A few cross-street remnants make the central intersection read as an actual city block.
            for (var x = -11f; x <= 11f; x += 3.5f)
            {
                if (Mathf.Abs(x) < 4.5f)
                    continue;
                CreateDecorCube($"CrossLane_{x:0.00}", new Vector3(x, 0.078f, 3.0f), new Vector3(1.35f, 0.016f, 0.09f), _laneMaterial);
            }

            CreateManhole(new Vector3(-1.65f, 0.09f, -5.7f));
            CreateManhole(new Vector3(1.9f, 0.09f, 8.6f));
        }

        private void BuildFacadeDetails()
        {
            var decorated = 0;
            for (var i = 1; i <= 6; i++)
            {
                var building = GameObject.Find($"Building_Block_{i:00}");
                var renderer = building != null ? building.GetComponent<Renderer>() : null;
                if (building == null || renderer == null)
                    continue;

                DecorateBuilding(building.transform, renderer.bounds, i);
                decorated++;
            }

            if (decorated == 0)
                Debug.LogWarning("DEADREACH 0.16 visual reboot found no legacy Building_Block geometry to dress.");
        }

        private void DecorateBuilding(Transform building, Bounds bounds, int index)
        {
            var side = bounds.center.x < 0f ? 1f : -1f;
            var facadeX = side > 0f ? bounds.max.x + 0.035f : bounds.min.x - 0.035f;
            var root = new GameObject($"Facade_{index:00}").transform;
            root.SetParent(_generatedRoot, false);

            // Roof cap and parapet break the original giant-box silhouette from the high-angle camera.
            CreateDecorCube($"RoofCap_{index:00}", new Vector3(bounds.center.x, bounds.max.y + 0.10f, bounds.center.z),
                new Vector3(bounds.size.x + 0.18f, 0.20f, bounds.size.z + 0.18f), _roofMaterial, root);

            var windowRows = Mathf.Clamp(Mathf.FloorToInt(bounds.size.y / 2.2f), 2, 4);
            var windowColumns = Mathf.Clamp(Mathf.FloorToInt(bounds.size.z / 2.4f), 2, 4);
            var yMin = bounds.min.y + 1.05f;
            var yStep = windowRows <= 1 ? 0f : Mathf.Max(1.25f, (bounds.size.y - 2.0f) / windowRows);
            var zPadding = Mathf.Min(1.15f, bounds.size.z * 0.18f);
            var zSpan = Mathf.Max(0.5f, bounds.size.z - zPadding * 2f);

            for (var row = 0; row < windowRows; row++)
            {
                var y = Mathf.Min(bounds.max.y - 0.75f, yMin + row * yStep);
                for (var column = 0; column < windowColumns; column++)
                {
                    var t = windowColumns == 1 ? 0.5f : column / (float)(windowColumns - 1);
                    var z = bounds.min.z + zPadding + zSpan * t;
                    var lit = (index * 7 + row * 3 + column) % 5 == 0;
                    CreateDecorCube(
                        $"Window_{index:00}_{row:00}_{column:00}",
                        new Vector3(facadeX, y, z),
                        new Vector3(0.075f, 0.66f, 0.72f),
                        lit ? _windowLitMaterial : _windowDarkMaterial,
                        root);
                }
            }

            // Vertical service spine + one sign plate gives each block an authored asymmetry.
            var spineZ = index % 2 == 0 ? bounds.min.z + bounds.size.z * 0.18f : bounds.max.z - bounds.size.z * 0.18f;
            CreateDecorCube($"Spine_{index:00}", new Vector3(facadeX + side * 0.045f, bounds.center.y, spineZ),
                new Vector3(0.11f, bounds.size.y * 0.78f, 0.22f), _buildingAccentMaterial, root);

            if (index is 2 or 5)
            {
                CreateDecorCube($"Sign_{index:00}", new Vector3(facadeX + side * 0.08f, bounds.min.y + 1.35f, bounds.center.z),
                    new Vector3(0.12f, 0.75f, Mathf.Min(2.8f, bounds.size.z * 0.55f)), _buildingAccentMaterial, root);
            }
        }

        private void BuildStreetFurniture()
        {
            // Small production-owned pieces only. Existing Quaternius wrecks/containers remain the hero props.
            CreateBollard(new Vector3(-4.0f, 0.1f, -6.8f));
            CreateBollard(new Vector3(-4.0f, 0.1f, -5.9f));
            CreateBollard(new Vector3(4.0f, 0.1f, 11.0f));
            CreateBollard(new Vector3(4.0f, 0.1f, 11.9f));

            CreateDecorCube("ServiceBox_A", new Vector3(-5.55f, 0.58f, 4.5f), new Vector3(0.55f, 1.05f, 0.62f), _buildingAccentMaterial);
            CreateDecorCube("ServiceBox_B", new Vector3(5.55f, 0.50f, -1.6f), new Vector3(0.48f, 0.90f, 0.55f), _buildingAccentMaterial);
        }

        private void ImproveLightingReadability()
        {
            var moon = GameObject.Find("Lighting_MoonKey")?.GetComponent<Light>();
            if (moon != null)
            {
                moon.intensity = Mathf.Max(moon.intensity, 0.92f);
                moon.shadows = LightShadows.Soft;
            }

            var fillObject = new GameObject("P16_Street_Fill");
            fillObject.transform.SetParent(_generatedRoot, false);
            fillObject.transform.rotation = Quaternion.Euler(58f, 24f, 0f);
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.intensity = 0.24f;
            fill.color = new Color(0.40f, 0.48f, 0.68f);
            fill.shadows = LightShadows.None;

            AddPointLight("P16_Intersection_Glow", new Vector3(0f, 4.2f, 3.2f), new Color(0.45f, 0.38f, 0.60f), 3.0f, 10f);
            AddPointLight("P16_North_Glow", new Vector3(-3.7f, 3.2f, 14.2f), new Color(0.25f, 0.55f, 0.62f), 2.2f, 8f);
        }

        private void AddPointLight(string name, Vector3 position, Color color, float intensity, float range)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(_generatedRoot, false);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.intensity = intensity;
            light.range = range;
            light.color = color;
            light.shadows = LightShadows.None;
        }

        private void CreateManhole(Vector3 position)
        {
            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = "Manhole";
            cylinder.transform.SetParent(_generatedRoot, false);
            cylinder.transform.position = position;
            cylinder.transform.localScale = new Vector3(0.52f, 0.018f, 0.52f);
            StripCollider(cylinder);
            SetMaterial(cylinder, _roofMaterial);
        }

        private void CreateBollard(Vector3 position)
        {
            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = "Bollard";
            cylinder.transform.SetParent(_generatedRoot, false);
            cylinder.transform.position = position + Vector3.up * 0.38f;
            cylinder.transform.localScale = new Vector3(0.11f, 0.38f, 0.11f);
            StripCollider(cylinder);
            SetMaterial(cylinder, _buildingAccentMaterial);
        }

        private GameObject CreateDecorCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent != null ? parent : _generatedRoot, false);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            StripCollider(cube);
            SetMaterial(cube, material);
            return cube;
        }

        private static void StripCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);
        }

        private void OnDestroy()
        {
            foreach (var material in _runtimeMaterials)
            {
                if (material != null)
                    Destroy(material);
            }
            _runtimeMaterials.Clear();
        }
    }
}
