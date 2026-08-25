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

            var camera = Camera.main;
            if (camera != null)
            {
                camera.transform.position = new Vector3(0f, 4.35f, -8.9f);
                camera.transform.LookAt(new Vector3(0f, 1.55f, 2.85f));
                camera.fieldOfView = 50f;
            }

            HideLegacySightlineProps();

            var root = new GameObject("P14_HoloDiorama");
            var holo = CreateHoloMaterial(new Color(0.15f, 0.85f, 0.95f, 0.78f));
            var holoDim = CreateHoloMaterial(new Color(0.08f, 0.37f, 0.43f, 0.62f));
            var amber = CreateHoloMaterial(new Color(1f, 0.35f, 0.07f, 0.92f));
            var metal = CreateLitMaterial(new Color(0.075f, 0.085f, 0.086f), 0.62f, 0.08f);
            var steel = CreateLitMaterial(new Color(0.17f, 0.19f, 0.19f), 0.48f, 0.34f);
            var darkMetal = CreateLitMaterial(new Color(0.045f, 0.052f, 0.054f), 0.38f, 0.46f);

            BuildAuthoredArchitecture(root.transform, steel, darkMetal);

            CreateBlock(root.transform, "ConsoleBase", new Vector3(0f, 0.85f, 2.65f), new Vector3(4.8f, 0.75f, 2.8f), metal);
            CreateBlock(root.transform, "ConsoleDeck", new Vector3(0f, 1.30f, 2.65f), new Vector3(5.2f, 0.14f, 3.15f), metal);
            CreateBlock(root.transform, "HoloBed", new Vector3(0f, 1.43f, 2.65f), new Vector3(4.45f, 0.05f, 2.45f), holoDim);

            var city = new GameObject("ProjectedCity");
            city.transform.SetParent(root.transform, false);
            city.transform.position = new Vector3(0f, 1.56f, 2.65f);

            var blocks = new[]
            {
                new Vector4(-1.45f, -0.60f, 0.55f, 0.55f),
                new Vector4(-0.78f, 0.35f, 0.72f, 1.15f),
                new Vector4(-0.18f, -0.30f, 0.80f, 0.80f),
                new Vector4(0.52f, 0.20f, 0.62f, 1.55f),
                new Vector4(1.28f, -0.45f, 0.50f, 0.92f),
                new Vector4(1.05f, 0.68f, 0.46f, 0.68f),
                new Vector4(-1.22f, 0.72f, 0.45f, 0.88f),
                new Vector4(0.12f, 0.82f, 0.55f, 0.58f)
            };

            for (var i = 0; i < blocks.Length; i++)
            {
                var b = blocks[i];
                CreateBlock(city.transform, $"HoloBlock_{i:00}",
                    new Vector3(b.x, b.w * 0.18f, b.y),
                    new Vector3(b.z, Mathf.Max(0.12f, b.w * 0.36f), b.z * 0.78f),
                    i == 3 ? amber : holo);
            }

            CreateMarker(city.transform, new Vector3(-0.78f, 0.75f, 0.35f), amber, 0);
            CreateMarker(city.transform, new Vector3(0.52f, 1.05f, 0.20f), holo, 1);
            CreateMarker(city.transform, new Vector3(1.05f, 0.62f, 0.68f), amber, 2);

            CreateHoloRing(root.transform, new Vector3(0f, 1.60f, 2.65f), 2.35f, holoDim);
            CreateHoloRing(root.transform, new Vector3(0f, 1.61f, 2.65f), 1.55f, holoDim);

            root.AddComponent<Production14HoloAnimator>();

            AddSceneLight(root.transform, "Holo_Cyan", new Vector3(0f, 2.9f, 1.7f), new Color(0.18f, 0.72f, 0.86f), 5.5f, 8f);
            AddSceneLight(root.transform, "Holo_Amber", new Vector3(-3.3f, 2.1f, 2.2f), new Color(1f, 0.28f, 0.07f), 2.4f, 5f);
            AddSceneLight(root.transform, "Rear_Cyan", new Vector3(0f, 4.4f, 6.6f), new Color(0.12f, 0.56f, 0.68f), 3.6f, 7f);
        }

        private static void BuildAuthoredArchitecture(Transform parent, Material steel, Material darkMetal)
        {
            var frameAsset = Resources.Load<GameObject>("Production14/Quaternius/Door_Frame_A");
            var doorAsset = Resources.Load<GameObject>("Production14/Quaternius/Door_DarkMetal");

            if (frameAsset != null)
            {
                var rearFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                rearFrame.name = "P14_Quaternius_RearBulkhead";
                rearFrame.transform.localPosition = new Vector3(0f, 0.05f, 7.72f);
                rearFrame.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                rearFrame.transform.localScale = Vector3.one * 1.16f;
                ApplyMaterial(rearFrame, steel);
                RemoveAllColliders(rearFrame);

                var leftFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                leftFrame.name = "P14_Quaternius_LeftBulkhead";
                leftFrame.transform.localPosition = new Vector3(-6.15f, 0.05f, 4.55f);
                leftFrame.transform.localRotation = Quaternion.Euler(0f, 76f, 0f);
                leftFrame.transform.localScale = Vector3.one * 0.82f;
                ApplyMaterial(leftFrame, steel);
                RemoveAllColliders(leftFrame);

                var rightFrame = UnityEngine.Object.Instantiate(frameAsset, parent);
                rightFrame.name = "P14_Quaternius_RightBulkhead";
                rightFrame.transform.localPosition = new Vector3(6.15f, 0.05f, 4.55f);
                rightFrame.transform.localRotation = Quaternion.Euler(0f, -76f, 0f);
                rightFrame.transform.localScale = Vector3.one * 0.82f;
                ApplyMaterial(rightFrame, steel);
                RemoveAllColliders(rightFrame);
            }

            if (doorAsset != null)
            {
                var door = UnityEngine.Object.Instantiate(doorAsset, parent);
                door.name = "P14_Quaternius_RearDoor";
                door.transform.localPosition = new Vector3(0f, 0.07f, 7.66f);
                door.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                door.transform.localScale = Vector3.one * 1.16f;
                ApplyMaterial(door, darkMetal);
                RemoveAllColliders(door);
            }
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

        private static void CreateMarker(Transform parent, Vector3 localPosition, Material material, int index)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = $"ObjectiveMarker_{index:00}";
            marker.transform.SetParent(parent, false);
            marker.transform.localPosition = localPosition;
            marker.transform.localScale = new Vector3(0.11f, 0.06f, 0.11f);
            marker.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(marker);

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = $"ObjectiveBeam_{index:00}";
            beam.transform.SetParent(parent, false);
            beam.transform.localPosition = localPosition + new Vector3(0f, 0.45f, 0f);
            beam.transform.localScale = new Vector3(0.025f, 0.45f, 0.025f);
            beam.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(beam);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = $"ObjectiveHead_{index:00}";
            head.transform.SetParent(parent, false);
            head.transform.localPosition = localPosition + new Vector3(0f, 0.92f, 0f);
            head.transform.localScale = Vector3.one * 0.15f;
            head.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(head);
        }

        private static void CreateHoloRing(Transform parent, Vector3 position, float radius, Material material)
        {
            var ring = new GameObject($"HoloRing_{radius:0.00}");
            ring.transform.SetParent(parent, false);
            ring.transform.position = position;

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

        private static void AddSceneLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
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
    }

    public sealed class Production14HoloAnimator : MonoBehaviour
    {
        private Transform _city;
        private Vector3 _basePosition;

        private void Start()
        {
            _city = transform.Find("ProjectedCity");
            _basePosition = transform.position;
        }

        private void Update()
        {
            if (_city != null)
                _city.Rotate(0f, 5.5f * Time.unscaledDeltaTime, 0f, Space.Self);

            var bob = Mathf.Sin(Time.unscaledTime * 1.6f) * 0.018f;
            transform.position = _basePosition + Vector3.up * bob;
        }
    }
}
