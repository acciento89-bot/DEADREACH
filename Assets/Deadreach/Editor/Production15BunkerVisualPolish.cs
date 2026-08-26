using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Final Production 0.15 presentation pass for the Meshy bunker.
    /// Keeps UI/camera/layout untouched and only improves 3D readability in the real Game view.
    /// </summary>
    public static class Production15BunkerVisualPolish
    {
        private const string ScenePath = "Assets/Deadreach/Scenes/Bunker_Hub.unity";
        private const string MeshyRootName = "P15_Meshy_Bunker";
        private const string RigName = "P15_VisualPolishRig";

        [MenuItem("DEADREACH/Production 0.15/Apply Bunker Visual Polish", priority = 4)]
        public static void Apply()
        {
            if (!File.Exists(ScenePath))
            {
                Debug.LogError($"DEADREACH 0.15 visual polish could not find {ScenePath}.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("DEADREACH 0.15 visual polish could not open the Bunker scene.");
                return;
            }

            var meshyRoot = FindSceneObject(scene, MeshyRootName);
            if (meshyRoot == null)
            {
                Debug.LogError("DEADREACH 0.15 visual polish requires the Meshy bunker layout first.");
                return;
            }

            var previous = FindSceneObject(scene, RigName);
            if (previous != null)
                Object.DestroyImmediate(previous);

            ConfigureAtmosphere();
            BoostFixtureLights(meshyRoot.transform);

            var rig = new GameObject(RigName);

            // Neutral/cool command-center readability without flattening the bunker mood.
            CreatePointLight(rig.transform, "Command_Key", new Vector3(0f, 4.15f, 2.1f),
                new Color(0.64f, 0.84f, 0.9f), 6.8f, 11.2f, LightShadows.Soft);

            // Lower central fill lifts the floor and table base without washing the back wall.
            CreatePointLight(rig.transform, "Command_Floor_Fill", new Vector3(0f, 1.65f, 1.4f),
                new Color(0.42f, 0.60f, 0.64f), 2.6f, 6.8f, LightShadows.None);

            // Separate the rear console / blast-door zone from the dark back wall.
            CreatePointLight(rig.transform, "Rear_Fill", new Vector3(0f, 3.05f, 6.05f),
                new Color(0.50f, 0.70f, 0.76f), 3.55f, 8.0f, LightShadows.None);

            // Side-station accents: warm workshop, cool operator/supply side.
            CreatePointLight(rig.transform, "Left_Station_Fill", new Vector3(-4.2f, 2.45f, 3.45f),
                new Color(1f, 0.50f, 0.26f), 3.15f, 6.8f, LightShadows.None);
            CreatePointLight(rig.transform, "Right_Station_Fill", new Vector3(4.2f, 2.45f, 3.45f),
                new Color(0.36f, 0.80f, 0.88f), 3.3f, 6.8f, LightShadows.None);

            // Gentle front lift so foreground props are visible behind the Overview UI panel.
            CreatePointLight(rig.transform, "Front_Lift", new Vector3(0f, 2.25f, -1.0f),
                new Color(0.54f, 0.70f, 0.72f), 2.75f, 8.0f, LightShadows.None);

            Physics.SyncTransforms();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = rig;

            Debug.Log("DEADREACH Production 0.15 Bunker visual polish PASS: Game-view fog reduced, ambient readability raised, fixture lights boosted and command/station fill rig applied. UI/camera/layout unchanged.");
        }

        private static void ConfigureAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.009f;
            RenderSettings.fogColor = new Color(0.020f, 0.029f, 0.030f);

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.10f, 0.122f, 0.125f);
            RenderSettings.ambientEquatorColor = new Color(0.052f, 0.062f, 0.062f);
            RenderSettings.ambientGroundColor = new Color(0.022f, 0.027f, 0.027f);
        }

        private static void BoostFixtureLights(Transform meshyRoot)
        {
            var lights = meshyRoot.GetComponentsInChildren<Light>(true);
            foreach (var light in lights)
            {
                if (light == null || !light.name.StartsWith("Light_"))
                    continue;

                light.intensity = Mathf.Max(light.intensity, 4.1f);
                light.range = Mathf.Max(light.range, 7.6f);
                if (light.color.maxColorComponent < 0.75f)
                    light.color = new Color(0.74f, 0.92f, 0.96f);
                EditorUtility.SetDirty(light);
            }
        }

        private static void CreatePointLight(
            Transform parent,
            string name,
            Vector3 position,
            Color color,
            float intensity,
            float range,
            LightShadows shadows)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.position = position;

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = shadows;
        }

        private static GameObject FindSceneObject(Scene scene, string objectName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var candidate in transforms)
                {
                    if (candidate != null && candidate.name == objectName)
                        return candidate.gameObject;
                }
            }

            return null;
        }
    }
}
