using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Final non-layout polish for Production 0.16 Meshy Dead City.
    /// Keeps accepted placement/gameplay intact and only hardens static rendering, fallback material
    /// readability and low-cost scene fill lighting. Also reports mesh triangle budgets for mobile QA.
    /// </summary>
    public static class Production16MeshyDeadCityFinalPolish
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity";
        private const string CityRootName = "P16_Meshy_DeadCity";
        private const string PolishRootName = "P16_Meshy_DeadCity_FinalPolish";

        [MenuItem("DEADREACH/Production 0.16/Apply Meshy Dead City Final Polish", priority = 14)]
        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.16 final polish could not open DeadCity_VerticalSlice.");
                return;
            }

            var cityRoot = FindSceneObject(scene, CityRootName);
            if (cityRoot == null)
            {
                Debug.LogError("DEADREACH 0.16 final polish requires P16_Meshy_DeadCity. Run Integrate Meshy Dead City first.");
                return;
            }

            var previous = FindSceneObject(scene, PolishRootName);
            if (previous != null)
                UnityEngine.Object.DestroyImmediate(previous);

            var polishRoot = new GameObject(PolishRootName);
            HardenRenderers(cityRoot);
            BuildReadabilityLights(polishRoot.transform);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = cityRoot;

            ReportTriangleBudget(cityRoot);
            Debug.Log("DEADREACH 0.16 Meshy Dead City final polish PASS: accepted layout unchanged, static renderer hardening + restrained city fill lighting applied.");
        }

        [MenuItem("DEADREACH/Production 0.16/Report Meshy Dead City Triangle Budget", priority = 15)]
        public static void ReportBudgetFromMenu()
        {
            var scene = SceneManager.GetActiveScene();
            var cityRoot = scene.IsValid() ? FindSceneObject(scene, CityRootName) : null;
            if (cityRoot == null)
            {
                Debug.LogWarning("DEADREACH 0.16 triangle budget report requires the built Meshy Dead City scene to be open.");
                return;
            }

            ReportTriangleBudget(cityRoot);
        }

        private static void HardenRenderers(GameObject cityRoot)
        {
            foreach (var renderer in cityRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                    continue;

                renderer.receiveShadows = true;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                renderer.allowOcclusionWhenDynamic = true;

                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                        continue;

                    material.enableInstancing = true;

                    // Only lift texture-less fallback materials. Never wash out accepted Meshy textures.
                    if (!material.HasProperty("_BaseColor") || !material.HasProperty("_BaseMap"))
                        continue;
                    if (material.GetTexture("_BaseMap") != null)
                        continue;

                    var color = material.GetColor("_BaseColor");
                    var luminance = color.r * 0.2126f + color.g * 0.7152f + color.b * 0.0722f;
                    if (luminance < 0.15f)
                    {
                        var lifted = Color.Lerp(color, new Color(0.24f, 0.23f, 0.22f, color.a), 0.45f);
                        material.SetColor("_BaseColor", lifted);
                        EditorUtility.SetDirty(material);
                    }
                }

                EditorUtility.SetDirty(renderer);
            }
        }

        private static void BuildReadabilityLights(Transform root)
        {
            var fillObject = new GameObject("P16_City_FinalFill");
            fillObject.transform.SetParent(root, false);
            fillObject.transform.rotation = Quaternion.Euler(54f, -32f, 0f);
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.46f, 0.54f, 0.66f);
            fill.intensity = 0.16f;
            fill.shadows = LightShadows.None;

            AddPointLight(root, "P16_City_IntersectionFill", new Vector3(0f, 4.5f, 2.8f), new Color(0.46f, 0.54f, 0.64f), 1.15f, 9.5f);
            AddPointLight(root, "P16_City_NorthFill", new Vector3(0f, 4.2f, 13.0f), new Color(0.34f, 0.50f, 0.54f), 0.85f, 8.0f);
        }

        private static void AddPointLight(Transform root, string name, Vector3 position, Color color, float intensity, float range)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(root, false);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void ReportTriangleBudget(GameObject cityRoot)
        {
            var perInstance = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            long totalTriangles = 0;

            foreach (var instance in DirectChildren(cityRoot.transform))
            {
                long triangles = 0;
                foreach (var filter in instance.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (filter == null || filter.sharedMesh == null)
                        continue;
                    triangles += CountTriangles(filter.sharedMesh);
                }

                foreach (var skinned in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (skinned == null || skinned.sharedMesh == null)
                        continue;
                    triangles += CountTriangles(skinned.sharedMesh);
                }

                perInstance[instance.name] = triangles;
                totalTriangles += triangles;
            }

            var lines = new List<string>();
            foreach (var pair in perInstance)
            {
                var marker = pair.Value > 350000 ? " !!" : pair.Value > 180000 ? " !" : string.Empty;
                lines.Add($"{pair.Key}={pair.Value:N0}{marker}");
            }

            var totalMarker = totalTriangles > 1600000 ? " // HIGH FOR MOBILE" : totalTriangles > 900000 ? " // WATCH MOBILE" : " // OK BASELINE";
            Debug.Log($"DEADREACH 0.16 MESHY CITY TRIANGLES: {totalTriangles:N0}{totalMarker}\n" + string.Join("\n", lines));
        }

        private static long CountTriangles(Mesh mesh)
        {
            long triangles = 0;
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var descriptor = mesh.GetSubMesh(i);
                if (descriptor.topology == MeshTopology.Triangles)
                    triangles += descriptor.indexCount / 3;
            }
            return triangles;
        }

        private static IEnumerable<GameObject> DirectChildren(Transform root)
        {
            for (var i = 0; i < root.childCount; i++)
                yield return root.GetChild(i).gameObject;
        }

        private static GameObject FindSceneObject(Scene scene, string name)
        {
            foreach (var sceneRoot in scene.GetRootGameObjects())
            {
                foreach (var transform in sceneRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (transform != null && string.Equals(transform.name, name, StringComparison.Ordinal))
                        return transform.gameObject;
                }
            }
            return null;
        }
    }
}
