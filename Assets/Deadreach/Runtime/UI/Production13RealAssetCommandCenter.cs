using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(500)]
    public sealed class Production13RealAssetCommandCenter : MonoBehaviour
    {
        private const string RootName = "Production13b_RealAssetCommandCenter";
        private const string FrameResource = "Production13/Quaternius/Door_Frame_A";
        private const string DoorResource = "Production13/Quaternius/Door_DarkMetal";
        private const string KenneyHeaderResource = "Production13/UI/kenney_header_768x256";
        private const string KenneyBarResource = "Production13/UI/kenney_bar_384x96";

        private static readonly Color Steel = new(0.10f, 0.14f, 0.17f, 1f);
        private static readonly Color DarkSteel = new(0.025f, 0.040f, 0.050f, 1f);
        private static readonly Color Cyan = new(0.08f, 0.75f, 0.88f, 1f);
        private static readonly Color Amber = new(1.0f, 0.38f, 0.055f, 1f);

        private Material _steelMaterial;
        private Material _darkMaterial;
        private Material _cyanMaterial;
        private Material _amberMaterial;
        private Texture2D _kenneyHeader;
        private Texture2D _kenneyBar;
        private Font _font;
        private float _nextUiRefresh;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallAfterSceneLoad()
        {
            var bunker = UnityEngine.Object.FindFirstObjectByType<BunkerCommandCenterUI>();
            if (bunker == null || bunker.GetComponent<Production13RealAssetCommandCenter>() != null)
                return;

            bunker.gameObject.AddComponent<Production13RealAssetCommandCenter>();
        }

        private void Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            CreateMaterials();
            _kenneyHeader = Resources.Load<Texture2D>(KenneyHeaderResource);
            _kenneyBar = Resources.Load<Texture2D>(KenneyBarResource);
            BuildRealAssetCommandCenter();
            InstallKenneyChrome();
            ApplyCinematicComposition();
            RevealCommandCenter();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < _nextUiRefresh)
                return;

            _nextUiRefresh = Time.unscaledTime + 0.20f;
            RevealCommandCenter();
            ApplyCinematicComposition();
            InstallKenneyChrome();
        }

        private void CreateMaterials()
        {
            var lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (lit == null)
                return;

            _steelMaterial = CreateLitMaterial(lit, "P13B_ImportedSteel", Steel, 0.78f, 0.38f);
            _darkMaterial = CreateLitMaterial(lit, "P13B_ImportedDark", DarkSteel, 0.86f, 0.26f);
            _cyanMaterial = CreateEmissionMaterial(lit, "P13B_ImportedCyan", Cyan, 3.5f);
            _amberMaterial = CreateEmissionMaterial(lit, "P13B_ImportedAmber", Amber, 3.0f);
        }

        private static Material CreateLitMaterial(Shader shader, string name, Color color, float metallic, float smoothness)
        {
            var material = new Material(shader) { name = name, color = color };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            return material;
        }

        private static Material CreateEmissionMaterial(Shader shader, string name, Color color, float intensity)
        {
            var material = CreateLitMaterial(shader, name, Color.Lerp(color, Color.black, 0.72f), 0.42f, 0.58f);
            material.EnableKeyword("_EMISSION");
            var emission = color * intensity;
            if (material.HasProperty("_EmissionColor")) material.SetColor("_EmissionColor", emission);
            return material;
        }

        private void BuildRealAssetCommandCenter()
        {
            if (GameObject.Find(RootName) != null)
                return;

            var framePrefab = Resources.Load<GameObject>(FrameResource);
            var doorPrefab = Resources.Load<GameObject>(DoorResource);
            if (framePrefab == null || doorPrefab == null)
            {
                Debug.LogError("Production 0.13b: required Quaternius command-center model(s) could not be loaded from Resources.");
                return;
            }

            var root = new GameObject(RootName);

            // Main rear blast-door assembly: genuine Quaternius geometry, not Unity primitives.
            SpawnModel(framePrefab, root.transform, "P13B_BlastFrame_Center",
                new Vector3(0f, -0.55f, 3.38f), Vector3.zero, Vector3.one * 0.82f,
                _steelMaterial, _cyanMaterial);
            SpawnModel(doorPrefab, root.transform, "P13B_BlastDoor_Left",
                new Vector3(0f, -0.55f, 3.32f), Vector3.zero, Vector3.one * 0.82f,
                _darkMaterial, null);
            SpawnModel(doorPrefab, root.transform, "P13B_BlastDoor_Right",
                new Vector3(0f, -0.55f, 3.32f), new Vector3(0f, 180f, 0f), Vector3.one * 0.82f,
                _darkMaterial, null);

            // Secondary authored bulkheads create a stronger command-deck silhouette around the old support art.
            SpawnModel(framePrefab, root.transform, "P13B_Bulkhead_Left",
                new Vector3(-5.75f, -0.70f, 0.75f), new Vector3(0f, 78f, 0f), Vector3.one * 0.56f,
                _darkMaterial, _cyanMaterial);
            SpawnModel(framePrefab, root.transform, "P13B_Bulkhead_Right",
                new Vector3(5.75f, -0.70f, 0.75f), new Vector3(0f, -78f, 0f), Vector3.one * 0.56f,
                _darkMaterial, _amberMaterial);
            SpawnModel(framePrefab, root.transform, "P13B_Bulkhead_LeftRear",
                new Vector3(-5.15f, -0.82f, 2.85f), new Vector3(0f, 32f, 0f), Vector3.one * 0.47f,
                _steelMaterial, _cyanMaterial);
            SpawnModel(framePrefab, root.transform, "P13B_Bulkhead_RightRear",
                new Vector3(5.15f, -0.82f, 2.85f), new Vector3(0f, -32f, 0f), Vector3.one * 0.47f,
                _steelMaterial, _amberMaterial);

            CreateAccentLight(root.transform, "P13B_CyanDoorLight", new Vector3(-2.0f, 2.25f, 2.75f), Cyan, 4.4f, 5.0f);
            CreateAccentLight(root.transform, "P13B_AmberDoorLight", new Vector3(2.0f, 2.25f, 2.75f), Amber, 3.2f, 4.4f);
            CreateAccentLight(root.transform, "P13B_CommandFill", new Vector3(0f, 1.35f, 0.4f), new Color(0.06f, 0.35f, 0.48f), 2.2f, 8.0f);
        }

        private static GameObject SpawnModel(GameObject prefab, Transform parent, string name,
            Vector3 position, Vector3 euler, Vector3 scale, Material baseMaterial, Material accentMaterial)
        {
            var instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler(euler), parent);
            instance.name = name;
            instance.transform.localScale = scale;

            foreach (var collider in instance.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.Destroy(collider);

            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                var source = renderer.sharedMaterials;
                var count = Mathf.Max(1, source.Length);
                var materials = new Material[count];
                for (var i = 0; i < count; i++)
                    materials[i] = accentMaterial != null && i == count - 1 ? accentMaterial : baseMaterial;
                renderer.sharedMaterials = materials;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }

            return instance;
        }

        private static void CreateAccentLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name, typeof(Light));
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            var light = go.GetComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private void ApplyCinematicComposition()
        {
            // Horizontal operations navigation replaces the prototype-like permanent left sidebar.
            var nav = FindRect("Navigation");
            if (nav != null)
            {
                Place(nav, 0.015f, 0.785f, 0.985f, 0.855f);
                SetNamedActive("NavCallsign", false);
                SetNamedActive("BunkerStatus", false);
                SetNamedActive("P13_Signal", false);
                SetNamedActive("P13_Clearance", false);

                var navNames = new[] { "Nav_OVERVIEW", "Nav_ARSENAL", "Nav_OPERATORS", "Nav_CAMPAIGN", "Nav_WORKSHOP", "Nav_STORE" };
                var activeCount = 0;
                for (var i = 0; i < navNames.Length; i++)
                    if (FindRect(navNames[i]) != null) activeCount++;

                if (activeCount > 0)
                {
                    var margin = 0.012f;
                    var gap = 0.010f;
                    var width = (1f - margin * 2f - gap * (activeCount - 1)) / activeCount;
                    var slot = 0;
                    for (var i = 0; i < navNames.Length; i++)
                    {
                        var buttonRect = FindRect(navNames[i]);
                        if (buttonRect == null) continue;
                        var xMin = margin + slot * (width + gap);
                        Place(buttonRect, xMin, 0.10f, xMin + width, 0.90f);
                        var image = buttonRect.GetComponent<Image>();
                        if (image != null)
                        {
                            var c = image.color;
                            c.a = 0.68f;
                            image.color = c;
                        }
                        slot++;
                    }
                }
            }

            var contentFrame = FindRect("ContentFrame");
            if (contentFrame != null)
                Place(contentFrame, 0.015f, 0.095f, 0.985f, 0.775f);

            var contentViewport = FindRect("ContentViewport");
            if (contentViewport != null)
                Place(contentViewport, 0.012f, 0.025f, 0.988f, 0.972f);

            var deployBar = FindRect("DeployBar");
            if (deployBar != null)
                Place(deployBar, 0.015f, 0.012f, 0.985f, 0.082f);

            // Overview becomes three floating consoles around a visible center hero window.
            var mission = FindRect("MissionPanel");
            var progress = FindRect("ProgressPanel");
            var intel = FindRect("IntelPanel");
            if (mission != null && progress != null && intel != null)
            {
                Place(mission, 0.015f, 0.47f, 0.48f, 0.985f);
                Place(progress, 0.755f, 0.47f, 0.985f, 0.985f);
                Place(intel, 0.015f, 0.015f, 0.48f, 0.435f);
                EnsureHeroWindow(contentViewport);
            }
            else
            {
                var hero = FindDeepChild(transform, "P13B_HeroWindow");
                if (hero != null) hero.gameObject.SetActive(false);
            }
        }

        private void EnsureHeroWindow(RectTransform contentViewport)
        {
            if (contentViewport == null)
                return;

            var existing = FindDeepChild(contentViewport, "P13B_HeroWindow") as RectTransform;
            if (existing != null)
            {
                existing.gameObject.SetActive(true);
                return;
            }

            var go = new GameObject("P13B_HeroWindow", typeof(RectTransform));
            go.transform.SetParent(contentViewport, false);
            go.transform.SetAsFirstSibling();
            var rect = go.GetComponent<RectTransform>();
            Place(rect, 0.505f, 0.12f, 0.735f, 0.965f);

            var top = CreateUiRail(rect, "P13B_HeroTop", Cyan, 0f, 0.985f, 1f, 1f, 0.72f);
            var bottom = CreateUiRail(rect, "P13B_HeroBottom", Amber, 0f, 0f, 0.38f, 0.012f, 0.58f);
            var left = CreateUiRail(rect, "P13B_HeroLeft", Cyan, 0f, 0f, 0.010f, 0.30f, 0.38f);
            top.raycastTarget = false;
            bottom.raycastTarget = false;
            left.raycastTarget = false;

            CreateHeroLabel(rect, "P13B_HeroTag", "COMMAND DECK // LIVE", 11, Cyan, TextAnchor.UpperLeft,
                0.035f, 0.90f, 0.94f, 0.98f);
            CreateHeroLabel(rect, "P13B_HeroSub", "TACTICAL VISUAL FEED", 9, new Color(0.62f, 0.72f, 0.74f, 1f), TextAnchor.LowerRight,
                0.10f, 0.02f, 0.95f, 0.09f);
        }

        private void InstallKenneyChrome()
        {
            if (_kenneyHeader == null) _kenneyHeader = Resources.Load<Texture2D>(KenneyHeaderResource);
            if (_kenneyBar == null) _kenneyBar = Resources.Load<Texture2D>(KenneyBarResource);

            var header = FindRect("Header");
            if (header != null && _kenneyHeader != null)
                EnsureTexturePlate(header, "P13B_KenneyHeader", _kenneyHeader, new Color(1f, 1f, 1f, 0.16f), true);

            var content = FindRect("ContentFrame");
            if (content != null && _kenneyHeader != null)
            {
                var plate = EnsureTexturePlate(content, "P13B_KenneyContentPlate", _kenneyHeader, new Color(0.55f, 0.86f, 1f, 0.07f), true);
                if (plate != null)
                    Place(plate.rectTransform, 0.015f, 0.89f, 0.38f, 0.995f);
            }

            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (_kenneyBar == null || button == null)
                    continue;

                var name = button.gameObject.name;
                if (!name.StartsWith("Nav_", StringComparison.Ordinal) && name != "DeployButton")
                    continue;

                var alpha = name == "DeployButton" ? 0.52f : 0.30f;
                EnsureTexturePlate(button.transform as RectTransform, "P13B_KenneyButton", _kenneyBar,
                    new Color(1f, 1f, 1f, alpha), true);
            }
        }

        private RawImage EnsureTexturePlate(RectTransform parent, string name, Texture texture, Color color, bool firstSibling)
        {
            if (parent == null || texture == null)
                return null;

            var existing = FindDeepChild(parent, name)?.GetComponent<RawImage>();
            if (existing != null)
            {
                existing.texture = texture;
                existing.color = color;
                return existing;
            }

            var go = new GameObject(name, typeof(RectTransform), typeof(RawImage));
            go.transform.SetParent(parent, false);
            if (firstSibling) go.transform.SetAsFirstSibling();
            var rect = go.GetComponent<RectTransform>();
            Place(rect, 0f, 0f, 1f, 1f);
            var image = go.GetComponent<RawImage>();
            image.texture = texture;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private void RevealCommandCenter()
        {
            var tactical = FindDeepChild(transform, "P13_TacticalBackdrop");
            if (tactical != null && tactical.gameObject.activeSelf)
                tactical.gameObject.SetActive(false);

            foreach (var image in GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.gameObject.name.StartsWith("P13_", StringComparison.Ordinal)
                    || image.gameObject.name.StartsWith("P13B_", StringComparison.Ordinal))
                    continue;

                var name = image.gameObject.name;
                var color = image.color;
                if (name == "Backdrop") color.a = 0.18f;
                else if (name == "Header") color.a = 0.80f;
                else if (name == "Navigation") color.a = 0.54f;
                else if (name == "ContentFrame") color.a = 0.26f;
                else if (name == "DeployBar") color.a = 0.74f;
                else if (IsFeaturePanel(name) && image.GetComponent<Button>() == null) color.a = Mathf.Min(color.a, 0.56f);
                image.color = color;
            }
        }

        private static bool IsFeaturePanel(string name)
        {
            return name.Contains("Panel", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Inspector", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Summary", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("List", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Facility_", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("WorkshopWeapon_", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("Product_", StringComparison.OrdinalIgnoreCase)
                   || name == "BunkerSystems"
                   || name == "WeaponCalibration"
                   || name == "SectorNavigation"
                   || name == "SectorInfo";
        }

        private RectTransform FindRect(string objectName)
        {
            foreach (var rect in GetComponentsInChildren<RectTransform>(true))
            {
                if (rect != null && rect.gameObject.name == objectName)
                    return rect;
            }
            return null;
        }

        private void SetNamedActive(string objectName, bool active)
        {
            var child = FindDeepChild(transform, objectName);
            if (child != null && child.gameObject.activeSelf != active)
                child.gameObject.SetActive(active);
        }

        private Text CreateHeroLabel(Transform parent, string name, string value, int size, Color color, TextAnchor alignment,
            float xMin, float yMin, float xMax, float yMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = _font;
            text.text = value;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            Place(text.rectTransform, xMin, yMin, xMax, yMax);
            return text;
        }

        private static Image CreateUiRail(Transform parent, string name, Color color,
            float xMin, float yMin, float xMax, float yMax, float alpha)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            var c = color;
            c.a = alpha;
            image.color = c;
            image.raycastTarget = false;
            Place(image.rectTransform, xMin, yMin, xMax, yMax);
            return image;
        }

        private static Transform FindDeepChild(Transform parent, string objectName)
        {
            if (parent == null)
                return null;

            foreach (var child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == objectName)
                    return child;
            }
            return null;
        }

        private static void Place(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void OnDestroy()
        {
            if (_steelMaterial != null) Destroy(_steelMaterial);
            if (_darkMaterial != null) Destroy(_darkMaterial);
            if (_cyanMaterial != null) Destroy(_cyanMaterial);
            if (_amberMaterial != null) Destroy(_amberMaterial);
        }
    }
}
