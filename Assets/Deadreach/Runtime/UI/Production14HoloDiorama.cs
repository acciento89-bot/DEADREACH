using UnityEngine;
using UnityEngine.Rendering;

namespace Kamilunavo.Deadreach.UI
{
    public static class Production14HoloDiorama
    {
        public static void Build()
        {
            var existing = GameObject.Find("P14_HoloDiorama");
            if (existing != null)
                UnityEngine.Object.Destroy(existing);

            ConfigureCameraAndAmbient();
            HideLegacySightlineProps();

            var root = new GameObject("P14_HoloDiorama");

            var holo = CreateHoloMaterial(new Color(0.11f, 0.83f, 0.95f, 0.78f));
            var holoBright = CreateHoloMaterial(new Color(0.25f, 0.94f, 1f, 0.94f));
            var holoDim = CreateHoloMaterial(new Color(0.04f, 0.29f, 0.36f, 0.55f));
            var amber = CreateHoloMaterial(new Color(1f, 0.31f, 0.055f, 0.94f));

            var metal = CreateLitMaterial(new Color(0.06f, 0.071f, 0.074f), 0.55f, 0.18f);
            var steel = CreateLitMaterial(new Color(0.19f, 0.205f, 0.205f), 0.46f, 0.42f);
            var darkMetal = CreateLitMaterial(new Color(0.033f, 0.041f, 0.044f), 0.36f, 0.50f);
            var screenCyan = CreateEmissiveMaterial(new Color(0.025f, 0.11f, 0.13f), new Color(0.10f, 0.72f, 0.88f) * 3.2f);
            var screenAmber = CreateEmissiveMaterial(new Color(0.12f, 0.055f, 0.018f), new Color(1f, 0.22f, 0.025f) * 2.5f);

            BuildAuthoredArchitecture(root.transform, steel, darkMetal);
            BuildRearCommandWall(root.transform, metal, steel, screenCyan, screenAmber);
            BuildCommandTable(root.transform, metal, steel, darkMetal, holoDim, holoBright);
            BuildProjectedCity(root.transform, holo, holoBright, holoDim, amber);
            BuildProjectorPod(root.transform, metal, steel, holoBright, holoDim);

            root.AddComponent<Production14HoloAnimator>();

            AddSceneLight(root.transform, "Holo_Cyan_Key", new Vector3(0f, 3.35f, 1.5f), new Color(0.18f, 0.72f, 0.86f), 6.8f, 9f);
            AddSceneLight(root.transform, "Holo_Amber_Left", new Vector3(-3.9f, 2.35f, 1.9f), new Color(1f, 0.24f, 0.055f), 3.1f, 6f);
            AddSceneLight(root.transform, "Rear_Cyan", new Vector3(0f, 4.55f, 6.2f), new Color(0.10f, 0.52f, 0.66f), 5.4f, 8.5f);
            AddSceneLight(root.transform, "Front_Fill", new Vector3(0f, 2.0f, -0.3f), new Color(0.30f, 0.36f, 0.38f), 2.2f, 11f);
            AddSceneLight(root.transform, "Right_Projector", new Vector3(3.75f, 1.65f, 1.65f), new Color(0.13f, 0.78f, 0.92f), 3.7f, 4.8f);
        }

        private static void ConfigureCameraAndAmbient()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                camera.transform.position = new Vector3(0f, 4.75f, -10.15f);
                camera.transform.LookAt(new Vector3(0f, 1.72f, 2.95f));
                camera.fieldOfView = 46f;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.004f, 0.008f, 0.010f, 1f);
            }

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.105f, 0.12f, 0.125f, 1f);
        }

        private static void BuildAuthoredArchitecture(Transform parent, Material steel, Material darkMetal)
        {
            var frameAsset = Resources.Load<GameObject>("Production14/Quaternius/Door_Frame_A");
            var doorAsset = Resources.Load<GameObject>("Production14/Quaternius/Door_DarkMetal");

            if (frameAsset != null)
            {
                var rearFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                rearFrame.name = "P14_Quaternius_RearBulkhead";
                rearFrame.transform.localPosition = new Vector3(0f, 0.03f, 7.78f);
                rearFrame.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                rearFrame.transform.localScale = Vector3.one * 1.28f;
                ApplyMaterial(rearFrame, steel);
                RemoveAllColliders(rearFrame);

                var leftFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                leftFrame.name = "P14_Quaternius_LeftBulkhead";
                leftFrame.transform.localPosition = new Vector3(-6.0f, 0.03f, 4.75f);
                leftFrame.transform.localRotation = Quaternion.Euler(0f, 73f, 0f);
                leftFrame.transform.localScale = Vector3.one * 0.90f;
                ApplyMaterial(leftFrame, steel);
                RemoveAllColliders(leftFrame);

                var rightFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                rightFrame.name = "P14_Quaternius_RightBulkhead";
                rightFrame.transform.localPosition = new Vector3(6.0f, 0.03f, 4.75f);
                rightFrame.transform.localRotation = Quaternion.Euler(0f, -73f, 0f);
                rightFrame.transform.localScale = Vector3.one * 0.90f;
                ApplyMaterial(rightFrame, steel);
                RemoveAllColliders(rightFrame);
            }

            if (doorAsset != null)
            {
                var door = UnityEngine.Object.Instantiate(doorAsset, parent);
                door.name = "P14_Quaternius_RearDoor";
                door.transform.localPosition = new Vector3(0f, 0.05f, 7.70f);
                door.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                door.transform.localScale = Vector3.one * 1.28f;
                ApplyMaterial(door, darkMetal);
                RemoveAllColliders(door);
            }
        }

        private static void BuildRearCommandWall(Transform parent, Material metal, Material steel, Material cyanScreen, Material amberScreen)
        {
            CreateBlock(parent, "RearConsole_Left", new Vector3(-2.85f, 1.75f, 6.65f), new Vector3(1.65f, 2.55f, 0.72f), metal);
            CreateBlock(parent, "RearConsole_Right", new Vector3(2.85f, 1.75f, 6.65f), new Vector3(1.65f, 2.55f, 0.72f), metal);
            CreateBlock(parent, "RearConsole_CenterLow", new Vector3(0f, 0.72f, 6.92f), new Vector3(2.9f, 0.82f, 0.62f), steel);

            CreateBlock(parent, "RearScreen_Left", new Vector3(-2.85f, 2.05f, 6.25f), new Vector3(1.15f, 0.68f, 0.035f), cyanScreen);
            CreateBlock(parent, "RearScreen_Right", new Vector3(2.85f, 2.05f, 6.25f), new Vector3(1.15f, 0.68f, 0.035f), cyanScreen);
            CreateBlock(parent, "RearScreen_Center", new Vector3(0f, 3.15f, 6.55f), new Vector3(1.25f, 0.72f, 0.045f), cyanScreen);
            CreateBlock(parent, "RearAlert_Left", new Vector3(-4.55f, 2.75f, 5.95f), new Vector3(0.34f, 1.0f, 0.04f), amberScreen);
            CreateBlock(parent, "RearAlert_Right", new Vector3(4.55f, 2.75f, 5.95f), new Vector3(0.34f, 1.0f, 0.04f), amberScreen);

            CreateBlock(parent, "RearLightRail", new Vector3(0f, 4.25f, 6.42f), new Vector3(5.4f, 0.07f, 0.08f), cyanScreen);
            CreateBlock(parent, "RearLightRail_Left", new Vector3(-4.55f, 1.2f, 5.95f), new Vector3(0.07f, 2.8f, 0.08f), cyanScreen);
            CreateBlock(parent, "RearLightRail_Right", new Vector3(4.55f, 1.2f, 5.95f), new Vector3(0.07f, 2.8f, 0.08f), cyanScreen);
        }

        private static void BuildCommandTable(Transform parent, Material metal, Material steel, Material darkMetal, Material holoDim, Material holoBright)
        {
            CreateBlock(parent, "ConsoleBase", new Vector3(0f, 0.70f, 2.85f), new Vector3(5.55f, 0.92f, 3.45f), darkMetal);
            CreateBlock(parent, "ConsoleMid", new Vector3(0f, 1.08f, 2.85f), new Vector3(5.90f, 0.18f, 3.70f), steel);
            CreateBlock(parent, "ConsoleDeck", new Vector3(0f, 1.24f, 2.85f), new Vector3(5.45f, 0.16f, 3.25f), metal);
            CreateBlock(parent, "HoloBed", new Vector3(0f, 1.36f, 2.85f), new Vector3(4.85f, 0.055f, 2.52f), holoDim);

            CreateBlock(parent, "FrontRail", new Vector3(0f, 1.00f, 1.02f), new Vector3(5.75f, 0.18f, 0.16f), steel);
            CreateBlock(parent, "FrontRailGlow", new Vector3(0f, 1.10f, 0.91f), new Vector3(4.75f, 0.035f, 0.035f), holoBright);

            var leftWing = CreateBlock(parent, "ConsoleWing_Left", new Vector3(-3.28f, 0.88f, 2.45f), new Vector3(1.55f, 0.72f, 2.15f), metal);
            leftWing.transform.localRotation = Quaternion.Euler(0f, -8f, 0f);
            var rightWing = CreateBlock(parent, "ConsoleWing_Right", new Vector3(3.28f, 0.88f, 2.45f), new Vector3(1.55f, 0.72f, 2.15f), metal);
            rightWing.transform.localRotation = Quaternion.Euler(0f, 8f, 0f);

            var leftScreen = CreateBlock(parent, "ConsoleWingScreen_Left", new Vector3(-3.30f, 1.31f, 2.18f), new Vector3(1.10f, 0.045f, 1.25f), holoDim);
            leftScreen.transform.localRotation = Quaternion.Euler(0f, -8f, 0f);
            var rightScreen = CreateBlock(parent, "ConsoleWingScreen_Right", new Vector3(3.30f, 1.31f, 2.18f), new Vector3(1.10f, 0.045f, 1.25f), holoDim);
            rightScreen.transform.localRotation = Quaternion.Euler(0f, 8f, 0f);

            CreateBlock(parent, "TableEdge_Left", new Vector3(-2.60f, 1.37f, 2.85f), new Vector3(0.045f, 0.05f, 2.55f), holoBright);
            CreateBlock(parent, "TableEdge_Right", new Vector3(2.60f, 1.37f, 2.85f), new Vector3(0.045f, 0.05f, 2.55f), holoBright);
        }

        private static void BuildProjectedCity(Transform parent, Material holo, Material holoBright, Material holoDim, Material amber)
        {
            var city = new GameObject("ProjectedCity");
            city.transform.SetParent(parent, false);
            city.transform.localPosition = new Vector3(0f, 1.47f, 2.85f);

            CreateDistrictPlate(city.transform, "District_A", new Vector3(-1.35f, 0.02f, -0.55f), new Vector3(1.55f, 0.025f, 1.10f), holoDim);
            CreateDistrictPlate(city.transform, "District_B", new Vector3(0.10f, 0.025f, -0.58f), new Vector3(1.20f, 0.025f, 1.15f), holoDim);
            CreateDistrictPlate(city.transform, "District_C", new Vector3(1.30f, 0.03f, -0.05f), new Vector3(1.20f, 0.025f, 1.45f), holoDim);
            CreateDistrictPlate(city.transform, "District_D", new Vector3(-0.78f, 0.035f, 0.78f), new Vector3(1.85f, 0.025f, 0.95f), holoDim);
            CreateDistrictPlate(city.transform, "District_E", new Vector3(0.88f, 0.04f, 0.88f), new Vector3(1.42f, 0.025f, 0.92f), holoDim);

            var buildings = new[]
            {
                new HoloBuildingSpec(-1.72f, -0.72f, 0.42f, 0.40f, 0.55f, false),
                new HoloBuildingSpec(-1.20f, -0.58f, 0.58f, 0.48f, 0.92f, false),
                new HoloBuildingSpec(-0.65f, -0.74f, 0.35f, 0.40f, 0.46f, false),
                new HoloBuildingSpec(-0.25f, -0.52f, 0.52f, 0.52f, 0.76f, false),
                new HoloBuildingSpec(0.35f, -0.62f, 0.42f, 0.42f, 0.64f, false),
                new HoloBuildingSpec(0.86f, -0.52f, 0.48f, 0.50f, 1.04f, false),
                new HoloBuildingSpec(1.45f, -0.52f, 0.38f, 0.42f, 0.72f, false),
                new HoloBuildingSpec(1.64f, 0.12f, 0.42f, 0.44f, 1.62f, true),
                new HoloBuildingSpec(1.08f, 0.22f, 0.48f, 0.50f, 0.82f, false),
                new HoloBuildingSpec(0.48f, 0.18f, 0.58f, 0.55f, 1.16f, false),
                new HoloBuildingSpec(-0.12f, 0.16f, 0.46f, 0.44f, 0.58f, false),
                new HoloBuildingSpec(-0.70f, 0.20f, 0.54f, 0.50f, 1.34f, true),
                new HoloBuildingSpec(-1.40f, 0.15f, 0.45f, 0.45f, 0.72f, false),
                new HoloBuildingSpec(-1.35f, 0.78f, 0.50f, 0.48f, 0.88f, false),
                new HoloBuildingSpec(-0.70f, 0.83f, 0.42f, 0.42f, 0.62f, false),
                new HoloBuildingSpec(-0.10f, 0.78f, 0.62f, 0.48f, 0.98f, false),
                new HoloBuildingSpec(0.58f, 0.78f, 0.40f, 0.42f, 0.70f, false),
                new HoloBuildingSpec(1.12f, 0.84f, 0.52f, 0.48f, 1.18f, false)
            };

            for (var i = 0; i < buildings.Length; i++)
            {
                var b = buildings[i];
                var material = b.Amber ? amber : holo;
                CreateBlock(city.transform, $"HoloBuilding_{i:00}",
                    new Vector3(b.X, b.Height * 0.50f, b.Z),
                    new Vector3(b.Width, b.Height, b.Depth), material);

                if (b.Height >= 1.0f)
                {
                    CreateBlock(city.transform, $"HoloRoof_{i:00}",
                        new Vector3(b.X, b.Height + 0.025f, b.Z),
                        new Vector3(b.Width * 0.72f, 0.035f, b.Depth * 0.72f),
                        b.Amber ? amber : holoBright);
                }
            }

            CreateRoad(city.transform, "Road_NS_Left", holoBright,
                new Vector3(-0.96f, 0.055f, -1.18f), new Vector3(-0.96f, 0.055f, 1.20f));
            CreateRoad(city.transform, "Road_NS_Right", holoBright,
                new Vector3(0.78f, 0.056f, -1.18f), new Vector3(0.78f, 0.056f, 1.20f));
            CreateRoad(city.transform, "Road_EW_Lower", holoBright,
                new Vector3(-2.05f, 0.057f, -0.28f), new Vector3(2.05f, 0.057f, -0.28f));
            CreateRoad(city.transform, "Road_EW_Upper", holoBright,
                new Vector3(-2.05f, 0.058f, 0.58f), new Vector3(2.05f, 0.058f, 0.58f));

            CreateBoundary(city.transform, holoDim, 2.25f, 1.35f);

            CreateMarker(city.transform, new Vector3(-0.70f, 1.34f, 0.20f), amber, 0);
            CreateMarker(city.transform, new Vector3(0.48f, 1.16f, 0.18f), holoBright, 1);
            CreateMarker(city.transform, new Vector3(1.64f, 1.62f, 0.12f), amber, 2);
            CreateMarker(city.transform, new Vector3(-1.20f, 0.92f, -0.58f), holoBright, 3);

            CreateHoloRing(parent, new Vector3(0f, 1.52f, 2.85f), 2.48f, holoDim);
            CreateHoloRing(parent, new Vector3(0f, 1.525f, 2.85f), 1.75f, holoDim);
            CreateHoloRing(parent, new Vector3(0f, 1.53f, 2.85f), 0.95f, holoDim);
        }

        private static void BuildProjectorPod(Transform parent, Material metal, Material steel, Material holoBright, Material holoDim)
        {
            CreateCylinder(parent, "ProjectorBase", new Vector3(3.72f, 0.48f, 1.28f), new Vector3(0.76f, 0.18f, 0.76f), metal);
            CreateCylinder(parent, "ProjectorRing", new Vector3(3.72f, 0.72f, 1.28f), new Vector3(0.63f, 0.08f, 0.63f), steel);
            CreateCylinder(parent, "ProjectorCore", new Vector3(3.72f, 0.82f, 1.28f), new Vector3(0.45f, 0.055f, 0.45f), holoBright);
            CreateHoloRing(parent, new Vector3(3.72f, 0.90f, 1.28f), 0.52f, holoDim);
            CreateHoloRing(parent, new Vector3(3.72f, 1.05f, 1.28f), 0.34f, holoBright);
        }

        private static void HideLegacySightlineProps()
        {
            var names = new[]
            {
                "CommandTable",
                "CommandTableTop",
                "Workshop_Left",
                "Storage_Right",
                "Generator",
                "BlastDoor_Frame_Left",
                "BlastDoor_Frame_Right",
                "BlastDoor_Frame_Top",
                "BlastDoor",
                "BlastDoor_Hazard"
            };

            foreach (var name in names)
            {
                var go = GameObject.Find(name);
                if (go != null)
                    go.SetActive(false);
            }
        }

        private static void CreateDistrictPlate(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            CreateBlock(parent, name, position, scale, material);
        }

        private static void CreateMarker(Transform parent, Vector3 localPosition, Material material, int index)
        {
            var marker = CreateCylinder(parent, $"ObjectiveMarker_{index:00}", localPosition, new Vector3(0.11f, 0.045f, 0.11f), material);
            marker.transform.localRotation = Quaternion.identity;

            var beam = CreateCylinder(parent, $"ObjectiveBeam_{index:00}", localPosition + new Vector3(0f, 0.48f, 0f), new Vector3(0.025f, 0.48f, 0.025f), material);
            beam.transform.localRotation = Quaternion.identity;

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = $"ObjectiveHead_{index:00}";
            head.transform.SetParent(parent, false);
            head.transform.localPosition = localPosition + new Vector3(0f, 0.98f, 0f);
            head.transform.localScale = Vector3.one * 0.15f;
            head.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(head);
        }

        private static void CreateRoad(Transform parent, string name, Material material, Vector3 start, Vector3 end)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startWidth = 0.026f;
            line.endWidth = 0.026f;
            line.sharedMaterial = material;
        }

        private static void CreateBoundary(Transform parent, Material material, float halfWidth, float halfDepth)
        {
            var go = new GameObject("DistrictBoundary");
            go.transform.SetParent(parent, false);
            var line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 4;
            line.startWidth = 0.018f;
            line.endWidth = 0.018f;
            line.sharedMaterial = material;
            line.SetPosition(0, new Vector3(-halfWidth, 0.052f, -halfDepth));
            line.SetPosition(1, new Vector3(halfWidth, 0.052f, -halfDepth));
            line.SetPosition(2, new Vector3(halfWidth, 0.052f, halfDepth));
            line.SetPosition(3, new Vector3(-halfWidth, 0.052f, halfDepth));
        }

        private static void CreateHoloRing(Transform parent, Vector3 position, float radius, Material material)
        {
            var ring = new GameObject($"HoloRing_{position.x:0.00}_{radius:0.00}");
            ring.transform.SetParent(parent, false);
            ring.transform.localPosition = position;

            var line = ring.AddComponent<LineRenderer>();
            line.loop = true;
            line.useWorldSpace = false;
            line.positionCount = 64;
            line.startWidth = 0.018f;
            line.endWidth = 0.018f;
            line.sharedMaterial = material;

            for (var i = 0; i < line.positionCount; i++)
            {
                var angle = i / (float)line.positionCount * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius * 0.58f));
            }
        }

        private static GameObject CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(go);
            return go;
        }

        private static GameObject CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(go);
            return go;
        }

        private static void AddSceneLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static Material CreateLitMaterial(Color color, float smoothness, float metallic)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = "P14_Metal" };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            return material;
        }

        private static Material CreateEmissiveMaterial(Color baseColor, Color emission)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = "P14_Emissive" };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", baseColor);
            if (material.HasProperty("_Color")) material.SetColor("_Color", baseColor);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", emission);
                material.EnableKeyword("_EMISSION");
            }
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.35f);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0.12f);
            return material;
        }

        private static Material CreateHoloMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = "P14_Holo" };

            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_Blend")) material.SetFloat("_Blend", 0f);
            if (material.HasProperty("_SrcBlend")) material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            if (material.HasProperty("_DstBlend")) material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);

            material.renderQueue = 3000;
            return material;
        }

        private static void ApplyMaterial(GameObject root, Material material)
        {
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterial = material;
        }

        private static void RemoveAllColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.Destroy(collider);
        }

        private static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
                UnityEngine.Object.Destroy(collider);
        }

        private readonly struct HoloBuildingSpec
        {
            public HoloBuildingSpec(float x, float z, float width, float depth, float height, bool amber)
            {
                X = x;
                Z = z;
                Width = width;
                Depth = depth;
                Height = height;
                Amber = amber;
            }

            public float X { get; }
            public float Z { get; }
            public float Width { get; }
            public float Depth { get; }
            public float Height { get; }
            public bool Amber { get; }
        }
    }

    public sealed class Production14HoloAnimator : MonoBehaviour
    {
        private Transform _city;
        private Transform _projectorCore;
        private Vector3 _basePosition;
        private Vector3 _projectorScale;

        private void Start()
        {
            _city = transform.Find("ProjectedCity");
            _projectorCore = transform.Find("ProjectorCore");
            _basePosition = transform.position;
            if (_projectorCore != null)
                _projectorScale = _projectorCore.localScale;
        }

        private void Update()
        {
            if (_city != null)
                _city.Rotate(0f, 1.25f * Time.unscaledDeltaTime, 0f, Space.Self);

            var bob = Mathf.Sin(Time.unscaledTime * 1.5f) * 0.008f;
            transform.position = _basePosition + Vector3.up * bob;

            if (_projectorCore != null)
            {
                var pulse = 1f + Mathf.Sin(Time.unscaledTime * 3.2f) * 0.06f;
                _projectorCore.localScale = _projectorScale * pulse;
            }
        }
    }
}
