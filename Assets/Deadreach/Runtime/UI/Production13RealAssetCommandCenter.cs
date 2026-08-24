using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(1000)]
    public sealed class Production13RealAssetCommandCenter : MonoBehaviour
    {
        private const string RuntimeRootName = "Production13c_AuthoredCommandCenter";
        private const string OldRuntimeRootName = "Production13b_RealAssetCommandCenter";
        private const string FrameResource = "Production13/Quaternius/Door_Frame_A";
        private const string DoorResource = "Production13/Quaternius/Door_DarkMetal";

        private static readonly Color Void = new(0.003f, 0.006f, 0.009f, 1f);
        private static readonly Color Glass = new(0.010f, 0.018f, 0.024f, 0.92f);
        private static readonly Color GlassSoft = new(0.013f, 0.024f, 0.031f, 0.86f);
        private static readonly Color GlassDeep = new(0.005f, 0.010f, 0.014f, 0.94f);
        private static readonly Color Steel = new(0.085f, 0.105f, 0.120f, 1f);
        private static readonly Color DarkSteel = new(0.018f, 0.026f, 0.032f, 1f);
        private static readonly Color Cyan = new(0.08f, 0.70f, 0.84f, 1f);
        private static readonly Color CyanMuted = new(0.20f, 0.46f, 0.52f, 1f);
        private static readonly Color Amber = new(0.96f, 0.34f, 0.055f, 1f);
        private static readonly Color Green = new(0.16f, 0.82f, 0.46f, 1f);
        private static readonly Color Red = new(0.80f, 0.09f, 0.055f, 1f);
        private static readonly Color White = new(0.91f, 0.95f, 0.96f, 1f);
        private static readonly Color Muted = new(0.47f, 0.56f, 0.59f, 1f);
        private static readonly Color NavIdle = new(0.010f, 0.018f, 0.024f, 0.92f);
        private static readonly Color NavActive = new(0.030f, 0.085f, 0.102f, 0.96f);

        private Material _steelMaterial;
        private Material _darkMaterial;
        private Material _cyanMaterial;
        private Material _amberMaterial;
        private Font _font;
        private float _nextRefresh;
        private Image _deployPulse;
        private bool _legacyUiRemoved;

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
            DisableSupersededUiLayers();
            DisablePrototypeSceneDress();
            CreateMaterials();
            BuildAuthoredCommandCenter();
            TuneCameraAndAtmosphere();
            ApplyPremiumUi();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime >= _nextRefresh)
            {
                _nextRefresh = Time.unscaledTime + 0.16f;
                DisableSupersededUiLayers();
                ApplyPremiumUi();
            }

            if (_deployPulse != null)
            {
                var c = Amber;
                c.a = 0.42f + Mathf.Sin(Time.unscaledTime * 2.4f) * 0.10f;
                _deployPulse.color = c;
            }
        }

        private void DisableSupersededUiLayers()
        {
            var oldPremium = GetComponent<Production13PremiumBunkerUI>();
            if (oldPremium != null)
                oldPremium.enabled = false;

            if (_legacyUiRemoved)
                return;

            foreach (var rect in GetComponentsInChildren<RectTransform>(true))
            {
                if (rect == null || rect == transform)
                    continue;

                var name = rect.gameObject.name;
                if (!name.StartsWith("P13_", StringComparison.Ordinal)
                    && !name.StartsWith("P13B_", StringComparison.Ordinal))
                    continue;

                rect.gameObject.SetActive(false);
                Destroy(rect.gameObject);
            }

            _legacyUiRemoved = true;
        }

        private static void DisablePrototypeSceneDress()
        {
            var oldPass = GameObject.Find("Production_BunkerVisual_0_13");
            if (oldPass != null)
                oldPass.SetActive(false);

            var oldRuntime = GameObject.Find(OldRuntimeRootName);
            if (oldRuntime != null)
                oldRuntime.SetActive(false);

            // These are the deliberately primitive DEV-shell props from the old Bunker builder.
            // Keep the actual room shell, but clear the center/rear sightline for authored geometry.
            var oldNames = new[]
            {
                "CommandTable", "CommandTableTop", "Workshop_Left", "Storage_Right", "Generator",
                "BlastDoor_Frame_Left", "BlastDoor_Frame_Right", "BlastDoor_Frame_Top", "BlastDoor", "BlastDoor_Hazard"
            };

            foreach (var objectName in oldNames)
            {
                var go = GameObject.Find(objectName);
                if (go != null)
                    go.SetActive(false);
            }
        }

        private void CreateMaterials()
        {
            var lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (lit == null)
                return;

            _steelMaterial = CreateLitMaterial(lit, "P13C_Steel", Steel, 0.82f, 0.36f);
            _darkMaterial = CreateLitMaterial(lit, "P13C_DarkSteel", DarkSteel, 0.90f, 0.25f);
            _cyanMaterial = CreateEmissionMaterial(lit, "P13C_Cyan", Cyan, 2.3f);
            _amberMaterial = CreateEmissionMaterial(lit, "P13C_Amber", Amber, 1.9f);
        }

        private static Material CreateLitMaterial(Shader shader, string name, Color color, float metallic, float smoothness)
        {
            var material = new Material(shader) { name = name, color = color };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            return material;
        }

        private static Material CreateEmissionMaterial(Shader shader, string name, Color color, float intensity)
        {
            var baseColor = Color.Lerp(color, Color.black, 0.80f);
            var material = CreateLitMaterial(shader, name, baseColor, 0.45f, 0.50f);
            material.EnableKeyword("_EMISSION");
            if (material.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", color * intensity);
            return material;
        }

        private void BuildAuthoredCommandCenter()
        {
            var existing = GameObject.Find(RuntimeRootName);
            if (existing != null)
                return;

            var framePrefab = Resources.Load<GameObject>(FrameResource);
            var doorPrefab = Resources.Load<GameObject>(DoorResource);
            if (framePrefab == null || doorPrefab == null)
            {
                Debug.LogError("Production 0.13c: Quaternius blast-door resources are missing.");
                return;
            }

            var root = new GameObject(RuntimeRootName);

            // One coherent rear blast-door assembly. 0.13b incorrectly doubled the door and placed it
            // much too close to the camera, which created the black intersecting shapes seen in review.
            SpawnModel(framePrefab, root.transform, "P13C_RearFrame",
                new Vector3(0f, -0.30f, 7.72f), Vector3.zero, Vector3.one * 0.96f,
                _steelMaterial, _cyanMaterial);
            SpawnModel(doorPrefab, root.transform, "P13C_RearDoor",
                new Vector3(0f, -0.30f, 7.66f), Vector3.zero, Vector3.one * 0.96f,
                _darkMaterial, null);

            // Authored side bulkheads provide silhouette without filling the whole screen with geometry.
            SpawnModel(framePrefab, root.transform, "P13C_LeftBulkhead",
                new Vector3(-5.55f, -0.48f, 5.75f), new Vector3(0f, 70f, 0f), Vector3.one * 0.55f,
                _darkMaterial, _cyanMaterial);
            SpawnModel(framePrefab, root.transform, "P13C_RightBulkhead",
                new Vector3(5.55f, -0.48f, 5.75f), new Vector3(0f, -70f, 0f), Vector3.one * 0.55f,
                _darkMaterial, _amberMaterial);

            CreatePointLight(root.transform, "P13C_RearCold", new Vector3(-2.25f, 2.8f, 6.65f), Cyan, 3.2f, 5.6f);
            CreatePointLight(root.transform, "P13C_RearWarm", new Vector3(2.25f, 2.4f, 6.45f), Amber, 2.4f, 4.8f);
            CreatePointLight(root.transform, "P13C_FrontFill", new Vector3(0f, 3.3f, -0.8f), new Color(0.08f, 0.24f, 0.30f), 2.7f, 10f);
        }

        private static GameObject SpawnModel(GameObject prefab, Transform parent, string name,
            Vector3 position, Vector3 euler, Vector3 scale, Material baseMaterial, Material accentMaterial)
        {
            var instance = Instantiate(prefab, position, Quaternion.Euler(euler), parent);
            instance.name = name;
            instance.transform.localScale = scale;

            foreach (var collider in instance.GetComponentsInChildren<Collider>(true))
                Destroy(collider);

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

        private static void CreatePointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
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

        private static void TuneCameraAndAtmosphere()
        {
            var camera = Camera.main ?? FindFirstObjectByType<Camera>();
            if (camera != null)
            {
                camera.transform.position = new Vector3(0f, 5.25f, -11.4f);
                camera.transform.LookAt(new Vector3(0f, 2.05f, 6.15f));
                camera.fieldOfView = 40f;
                camera.backgroundColor = Void;
                camera.clearFlags = CameraClearFlags.SolidColor;
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.0085f;
            RenderSettings.fogColor = new Color(0.004f, 0.009f, 0.012f);
            RenderSettings.ambientSkyColor = new Color(0.035f, 0.055f, 0.065f);
            RenderSettings.ambientEquatorColor = new Color(0.016f, 0.025f, 0.030f);
            RenderSettings.ambientGroundColor = new Color(0.004f, 0.007f, 0.009f);
        }

        private void ApplyPremiumUi()
        {
            var canvas = FindNamed<Canvas>("Bunker_CommandCenter_Canvas");
            if (canvas == null)
                return;

            StyleShell();
            StyleNavigation();
            StylePanels();
            StyleText();
            StyleButtons();
            LayoutOverview();
        }

        private void StyleShell()
        {
            var backdrop = FindRect("Backdrop");
            SetImage(backdrop, new Color(0.002f, 0.005f, 0.008f, 0.22f));

            var stripe = FindRect("TopHazardStripe");
            if (stripe != null)
            {
                Place(stripe, 0.020f, 0.986f, 0.165f, 0.991f);
                SetImage(stripe, Amber);
            }

            var header = FindRect("Header");
            if (header != null)
            {
                Place(header, 0.020f, 0.890f, 0.980f, 0.976f);
                SetImage(header, new Color(0.004f, 0.009f, 0.014f, 0.88f));
                EnsureRail(header, "P13C_HeaderTop", Cyan, 0f, 0.985f, 0.23f, 1f, 0.70f);
                EnsureRail(header, "P13C_HeaderBottom", new Color(Cyan.r, Cyan.g, Cyan.b, 1f), 0f, 0f, 1f, 0.008f, 0.22f);
            }

            var nav = FindRect("Navigation");
            if (nav != null)
            {
                Place(nav, 0.020f, 0.808f, 0.980f, 0.875f);
                SetImage(nav, new Color(0.004f, 0.009f, 0.013f, 0.86f));
                SetNamedActive("NavCallsign", false);
                SetNamedActive("BunkerStatus", false);
            }

            var content = FindRect("ContentFrame");
            if (content != null)
            {
                Place(content, 0.020f, 0.105f, 0.980f, 0.795f);
                SetImage(content, new Color(0.002f, 0.005f, 0.008f, 0.14f));
                EnsureRail(content, "P13C_ContentLeft", CyanMuted, 0f, 0f, 0.0025f, 1f, 0.36f);
                EnsureRail(content, "P13C_ContentRight", CyanMuted, 0.9975f, 0f, 1f, 1f, 0.26f);
            }

            var contentViewport = FindRect("ContentViewport");
            if (contentViewport != null)
                Place(contentViewport, 0.010f, 0.018f, 0.990f, 0.982f);

            SetNamedActive("ContentAccent", false);

            var deploy = FindRect("DeployBar");
            if (deploy != null)
            {
                Place(deploy, 0.020f, 0.020f, 0.980f, 0.085f);
                SetImage(deploy, new Color(0.004f, 0.012f, 0.015f, 0.92f));
                _deployPulse = EnsureRail(deploy, "P13C_DeployPulse", Amber, 0f, 0f, 0.24f, 0.018f, 0.48f);
                EnsureRail(deploy, "P13C_DeployCold", Cyan, 0.24f, 0f, 1f, 0.008f, 0.22f);
            }

            var deployAccent = FindRect("DeployAccent");
            if (deployAccent != null)
            {
                Place(deployAccent, 0f, 0f, 0.004f, 1f);
                SetImage(deployAccent, Green);
            }
        }

        private void StyleNavigation()
        {
            var navNames = new[] { "Nav_OVERVIEW", "Nav_ARSENAL", "Nav_OPERATORS", "Nav_CAMPAIGN", "Nav_WORKSHOP", "Nav_STORE" };
            var count = 0;
            foreach (var name in navNames)
                if (FindRect(name) != null) count++;

            if (count == 0)
                return;

            const float margin = 0.006f;
            const float gap = 0.006f;
            var width = (1f - margin * 2f - gap * (count - 1)) / count;
            var slot = 0;

            foreach (var name in navNames)
            {
                var rect = FindRect(name);
                if (rect == null)
                    continue;

                var xMin = margin + slot * (width + gap);
                Place(rect, xMin, 0.12f, xMin + width, 0.88f);

                var image = rect.GetComponent<Image>();
                if (image != null)
                {
                    var active = IsNavActive(image.color);
                    image.color = active ? NavActive : NavIdle;
                    EnsureOutline(image, active ? new Color(Cyan.r, Cyan.g, Cyan.b, 0.42f) : new Color(0.20f, 0.29f, 0.32f, 0.20f));
                    EnsureRail(rect, "P13C_NavRail", active ? Cyan : CyanMuted, 0f, 0f, 1f, 0.035f, active ? 0.90f : 0.22f);
                }

                var label = FindDirectText(rect);
                if (label != null)
                {
                    label.fontSize = Mathf.Max(label.fontSize, 10);
                    label.fontStyle = FontStyle.Bold;
                    label.color = White;
                    label.alignment = TextAnchor.MiddleCenter;
                }
                slot++;
            }
        }

        private static bool IsNavActive(Color color)
        {
            var originalActive = color.r > 0.15f && color.g > 0.06f && color.g < 0.18f && color.b < 0.14f;
            var currentActive = Mathf.Abs(color.r - NavActive.r) < 0.025f && Mathf.Abs(color.g - NavActive.g) < 0.04f;
            return originalActive || currentActive;
        }

        private void StylePanels()
        {
            foreach (var image in GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.GetComponent<Button>() != null)
                    continue;

                var name = image.gameObject.name;
                if (name.StartsWith("P13C_", StringComparison.Ordinal))
                    continue;

                if (name == "MissionPanel")
                {
                    image.color = new Color(Glass.r, Glass.g, Glass.b, 0.90f);
                    EnsureOutline(image, new Color(Amber.r, Amber.g, Amber.b, 0.36f));
                    EnsureRail(image.rectTransform, "P13C_MissionAccent", Amber, 0f, 0f, 0.006f, 1f, 0.85f);
                }
                else if (name == "ProgressPanel")
                {
                    image.color = new Color(Glass.r, Glass.g, Glass.b, 0.88f);
                    EnsureOutline(image, new Color(Cyan.r, Cyan.g, Cyan.b, 0.30f));
                    EnsureRail(image.rectTransform, "P13C_ProgressAccent", Cyan, 0.994f, 0f, 1f, 1f, 0.64f);
                }
                else if (name == "IntelPanel")
                {
                    image.color = new Color(GlassDeep.r, GlassDeep.g, GlassDeep.b, 0.88f);
                }
                else if (IsMajorPanel(name))
                {
                    image.color = new Color(GlassSoft.r, GlassSoft.g, GlassSoft.b, 0.91f);
                    EnsureOutline(image, new Color(CyanMuted.r, CyanMuted.g, CyanMuted.b, 0.22f));
                }
                else if (IsCardLike(name) && image.color.a > 0.10f)
                {
                    var warm = image.color.r > image.color.g * 1.35f;
                    var positive = image.color.g > image.color.r * 1.35f;
                    var tint = warm ? Amber : positive ? Green : CyanMuted;
                    image.color = Color.Lerp(new Color(0.010f, 0.018f, 0.023f, 0.94f), tint, 0.08f);
                }
            }
        }

        private static bool IsMajorPanel(string name)
        {
            return name == "ArsenalList" || name == "ArsenalInspector"
                   || name == "OperatorList" || name == "OperatorInspector"
                   || name == "SectorNavigation" || name == "SectorInfo"
                   || name == "StoreIntro" || name == "WorkshopSummary"
                   || name == "BunkerSystems" || name == "WeaponCalibration";
        }

        private static bool IsCardLike(string name)
        {
            return name.StartsWith("Weapon_", StringComparison.Ordinal)
                   || name.StartsWith("Operator_", StringComparison.Ordinal)
                   || name.StartsWith("Product_", StringComparison.Ordinal)
                   || name.StartsWith("Facility_", StringComparison.Ordinal)
                   || name.StartsWith("WorkshopWeapon_", StringComparison.Ordinal)
                   || name.Contains("Card", StringComparison.OrdinalIgnoreCase);
        }

        private void StyleText()
        {
            foreach (var text in GetComponentsInChildren<Text>(true))
            {
                if (text == null || text.gameObject.name.StartsWith("P13C_", StringComparison.Ordinal))
                    continue;

                var name = text.gameObject.name;
                if (name == "Brand")
                {
                    text.fontSize = 34;
                    text.fontStyle = FontStyle.Bold;
                    text.color = White;
                }
                else if (name == "BrandSub")
                {
                    text.fontSize = 10;
                    text.color = Cyan;
                }
                else if (name == "SectionTitle")
                {
                    text.fontSize = 22;
                    text.fontStyle = FontStyle.Bold;
                    text.color = White;
                }
                else if (name == "SectionSubtitle")
                {
                    text.fontSize = 10;
                    text.color = Muted;
                }
                else if (name == "ProfileSummary")
                {
                    text.fontSize = 11;
                    text.color = Green;
                }
                else if (name == "PanelHeader")
                {
                    text.fontSize = 11;
                    text.fontStyle = FontStyle.Bold;
                }
                else if (name == "Level")
                {
                    text.fontSize = Mathf.Min(text.fontSize, 38);
                    text.color = White;
                }

                EnsureShadow(text);
            }
        }

        private void StyleButtons()
        {
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (button == null || button.image == null)
                    continue;

                var name = button.gameObject.name;
                if (name.StartsWith("Nav_", StringComparison.Ordinal))
                    continue;

                Color baseColor;
                Color accent;
                if (name == "DeployButton")
                {
                    baseColor = new Color(0.22f, 0.040f, 0.026f, 0.98f);
                    accent = Amber;
                    EnsureOutline(button.image, new Color(Amber.r, Amber.g, Amber.b, 0.58f));
                    EnsureRail(button.transform as RectTransform, "P13C_DeployButtonRail", Amber, 0f, 0f, 1f, 0.040f, 0.88f);
                    var label = FindDirectText(button.transform);
                    if (label != null)
                    {
                        label.text = "DEPLOY  //  ENTER";
                        label.fontSize = Mathf.Max(label.fontSize, 12);
                        label.color = White;
                    }
                }
                else
                {
                    var old = button.image.color;
                    var warm = old.r > old.g * 1.35f;
                    var positive = old.g > old.r * 1.35f;
                    accent = warm ? Amber : positive ? Green : CyanMuted;
                    baseColor = Color.Lerp(new Color(0.012f, 0.022f, 0.028f, 0.96f), accent, button.interactable ? 0.10f : 0.04f);
                }

                button.image.color = baseColor;
                var colors = button.colors;
                colors.normalColor = baseColor;
                colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.10f);
                colors.selectedColor = colors.highlightedColor;
                colors.pressedColor = Color.Lerp(baseColor, Color.black, 0.25f);
                colors.disabledColor = new Color(baseColor.r * 0.5f, baseColor.g * 0.5f, baseColor.b * 0.5f, 0.62f);
                colors.fadeDuration = 0.08f;
                colors.colorMultiplier = 1f;
                button.colors = colors;
            }
        }

        private void LayoutOverview()
        {
            var mission = FindRect("MissionPanel");
            var progress = FindRect("ProgressPanel");
            var intel = FindRect("IntelPanel");
            var viewport = FindRect("ContentViewport");

            if (mission != null && progress != null)
            {
                Place(mission, 0.025f, 0.315f, 0.385f, 0.965f);
                Place(progress, 0.800f, 0.485f, 0.975f, 0.965f);
                if (intel != null)
                    intel.gameObject.SetActive(false);
                EnsureCenterFeedLabel(viewport, true);
            }
            else
            {
                EnsureCenterFeedLabel(viewport, false);
            }
        }

        private void EnsureCenterFeedLabel(RectTransform viewport, bool visible)
        {
            if (viewport == null)
                return;

            var existing = FindDeepChild(viewport, "P13C_CenterFeed")?.GetComponent<Text>();
            if (existing == null && visible)
            {
                var go = new GameObject("P13C_CenterFeed", typeof(RectTransform), typeof(Text));
                go.transform.SetParent(viewport, false);
                var text = go.GetComponent<Text>();
                text.font = _font;
                text.text = "BUNKER 07 // TACTICAL FEED";
                text.fontSize = 10;
                text.fontStyle = FontStyle.Bold;
                text.color = new Color(Cyan.r, Cyan.g, Cyan.b, 0.72f);
                text.alignment = TextAnchor.LowerCenter;
                text.raycastTarget = false;
                Place(text.rectTransform, 0.405f, 0.03f, 0.785f, 0.10f);
                EnsureShadow(text);
                existing = text;
            }

            if (existing != null)
                existing.gameObject.SetActive(visible);
        }

        private static void SetImage(RectTransform rect, Color color)
        {
            if (rect == null)
                return;
            var image = rect.GetComponent<Image>();
            if (image != null)
                image.color = color;
        }

        private static Image EnsureRail(RectTransform parent, string name, Color color,
            float xMin, float yMin, float xMax, float yMax, float alpha)
        {
            if (parent == null)
                return null;

            var existing = FindDeepChild(parent, name)?.GetComponent<Image>();
            if (existing != null)
            {
                var ec = color;
                ec.a = alpha;
                existing.color = ec;
                return existing;
            }

            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            Place(rect, xMin, yMin, xMax, yMax);
            var image = go.GetComponent<Image>();
            var c = color;
            c.a = alpha;
            image.color = c;
            image.raycastTarget = false;
            return image;
        }

        private static void EnsureOutline(Graphic graphic, Color color)
        {
            if (graphic == null)
                return;
            var outline = graphic.GetComponent<Outline>();
            if (outline == null)
                outline = graphic.gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;
        }

        private static void EnsureShadow(Graphic graphic)
        {
            if (graphic == null)
                return;
            var shadow = graphic.GetComponent<Shadow>();
            if (shadow == null)
                shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.82f);
            shadow.effectDistance = new Vector2(1f, -1f);
            shadow.useGraphicAlpha = true;
        }

        private void SetNamedActive(string objectName, bool active)
        {
            var child = FindDeepChild(transform, objectName);
            if (child != null && child.gameObject.activeSelf != active)
                child.gameObject.SetActive(active);
        }

        private T FindNamed<T>(string objectName) where T : Component
        {
            foreach (var component in GetComponentsInChildren<T>(true))
            {
                if (component != null && component.gameObject.name == objectName)
                    return component;
            }
            return null;
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

        private static Text FindDirectText(Transform parent)
        {
            if (parent == null)
                return null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var text = parent.GetChild(i).GetComponent<Text>();
                if (text != null)
                    return text;
            }
            return null;
        }

        private static void Place(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            if (rect == null)
                return;
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
