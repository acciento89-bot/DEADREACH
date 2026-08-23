using System.Collections.Generic;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    public sealed class Production06SectorIdentity : MonoBehaviour
    {
        private enum AtmosphereStyle
        {
            Rain,
            Ash,
            Dust,
            Contamination
        }

        private readonly List<Material> _runtimeMaterials = new();

        private void Start()
        {
            var level = Mathf.Clamp(SaveService.Data.selectedLevel, 1, SaveService.MaxCampaignLevel);
            var sector = Mathf.Clamp((level - 1) / 10 + 1, 1, 5);
            var root = new GameObject($"Production06_Sector_{sector:00}_{RunDifficultyDirector.GetZoneName(level).Replace(' ', '_')}");

            switch (sector)
            {
                case 2:
                    BuildFloodedIndustrial(root.transform);
                    break;
                case 3:
                    BuildAshDistrict(root.transform);
                    break;
                case 4:
                    BuildBlackoutSector(root.transform);
                    break;
                case 5:
                    BuildGroundZero(root.transform);
                    break;
                default:
                    BuildDeadCity(root.transform);
                    break;
            }
        }

        private void BuildDeadCity(Transform root)
        {
            AddPointLight(root, "EmergencyBlue", new Vector3(-4.5f, 2.4f, 4.5f), new Color(0.08f, 0.3f, 1f), 2.8f, 8f);
            AddPointLight(root, "EmergencyRed", new Vector3(4.2f, 2.1f, 11f), new Color(1f, 0.08f, 0.04f), 2.2f, 7f);
        }

        private void BuildFloodedIndustrial(Transform root)
        {
            CreateGroundPatch(root, "Flood_A", new Vector3(-1.6f, 0.075f, 0.4f), new Vector3(4.8f, 0.025f, 4.2f), new Color(0.025f, 0.23f, 0.23f));
            CreateGroundPatch(root, "Flood_B", new Vector3(1.8f, 0.075f, 10.2f), new Vector3(5.2f, 0.025f, 4.8f), new Color(0.02f, 0.17f, 0.2f));
            AddPointLight(root, "FloodCyan_A", new Vector3(-3.4f, 1.2f, 3.5f), new Color(0.05f, 0.9f, 0.72f), 3.2f, 8f);
            AddPointLight(root, "FloodCyan_B", new Vector3(3.1f, 1.5f, 13.2f), new Color(0.05f, 0.55f, 0.72f), 2.7f, 7f);
            AddAtmosphereParticles(root, AtmosphereStyle.Rain, new Color(0.48f, 0.76f, 0.78f, 0.42f), 78f, 0.028f);
        }

        private void BuildAshDistrict(Transform root)
        {
            CreateGroundPatch(root, "Scorch_A", new Vector3(-2f, 0.078f, 5.2f), new Vector3(4.1f, 0.028f, 3.2f), new Color(0.22f, 0.075f, 0.018f));
            CreateGroundPatch(root, "Scorch_B", new Vector3(2.4f, 0.078f, 14.5f), new Vector3(3.2f, 0.028f, 3.5f), new Color(0.17f, 0.045f, 0.015f));
            AddPointLight(root, "AshFire_A", new Vector3(-3.6f, 1.2f, 6.5f), new Color(1f, 0.24f, 0.035f), 4.2f, 8f);
            AddPointLight(root, "AshFire_B", new Vector3(3.8f, 1.0f, 15.2f), new Color(1f, 0.48f, 0.06f), 3.3f, 6f);
            AddAtmosphereParticles(root, AtmosphereStyle.Ash, new Color(0.58f, 0.44f, 0.32f, 0.52f), 48f, 0.055f);
        }

        private void BuildBlackoutSector(Transform root)
        {
            foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light == null || light.transform.IsChildOf(root))
                    continue;
                light.intensity *= 0.28f;
            }

            var flickerA = AddPointLight(root, "BlackoutFlicker_A", new Vector3(-3.8f, 2.2f, 2.8f), new Color(0.30f, 0.20f, 0.78f), 3.8f, 7f);
            flickerA.gameObject.AddComponent<Production06LightFlicker>();
            var flickerB = AddPointLight(root, "BlackoutFlicker_B", new Vector3(3.5f, 2f, 13.8f), new Color(0.10f, 0.34f, 0.82f), 3.1f, 7f);
            flickerB.gameObject.AddComponent<Production06LightFlicker>();
            AddAtmosphereParticles(root, AtmosphereStyle.Dust, new Color(0.23f, 0.28f, 0.38f, 0.34f), 22f, 0.032f);
        }

        private void BuildGroundZero(Transform root)
        {
            CreateGroundPatch(root, "MutationPool_A", new Vector3(-1.8f, 0.08f, 3.4f), new Vector3(3.8f, 0.03f, 3.2f), new Color(0.42f, 0.015f, 0.02f));
            CreateGroundPatch(root, "MutationPool_B", new Vector3(2.3f, 0.08f, 12.1f), new Vector3(4.4f, 0.03f, 4.2f), new Color(0.34f, 0.008f, 0.025f));
            AddPointLight(root, "GroundZeroRed_A", new Vector3(-3f, 1.5f, 4.2f), new Color(1f, 0.015f, 0.02f), 5.5f, 9f);
            AddPointLight(root, "GroundZeroRed_B", new Vector3(3.2f, 1.6f, 14.1f), new Color(1f, 0.06f, 0.015f), 5f, 9f);
            AddAtmosphereParticles(root, AtmosphereStyle.Contamination, new Color(0.72f, 0.06f, 0.045f, 0.50f), 42f, 0.045f);
        }

        private void CreateGroundPatch(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var patch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            patch.name = name;
            patch.transform.SetParent(parent, false);
            patch.transform.position = position;
            patch.transform.localScale = scale;
            Destroy(patch.GetComponent<Collider>());
            var material = CreateMaterial(color);
            if (material != null)
                patch.GetComponent<Renderer>().sharedMaterial = material;
        }

        private Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                return null;

            var material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.62f);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", 0.05f);
            _runtimeMaterials.Add(material);
            return material;
        }

        private Material CreateParticleMaterial()
        {
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                         ?? Shader.Find("Particles/Standard Unlit")
                         ?? Shader.Find("Sprites/Default");
            if (shader == null)
                return null;

            var material = new Material(shader)
            {
                name = "Runtime_Production07_SectorParticles"
            };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", Color.white);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", Color.white);
            _runtimeMaterials.Add(material);
            return material;
        }

        private static Light AddPointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            var light = obj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
            return light;
        }

        private void AddAtmosphereParticles(Transform parent, AtmosphereStyle style, Color color, float rate, float size)
        {
            var obj = new GameObject($"SectorAtmosphere_{style}");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(0f, style == AtmosphereStyle.Contamination ? 1.2f : 7f, 7f);

            var particles = obj.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.loop = true;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startSize = size;
            main.startColor = color;
            main.maxParticles = 500;

            switch (style)
            {
                case AtmosphereStyle.Rain:
                    main.startLifetime = 3.2f;
                    main.startSpeed = 0.2f;
                    main.gravityModifier = 2.4f;
                    break;
                case AtmosphereStyle.Ash:
                    main.startLifetime = 8f;
                    main.startSpeed = 0.12f;
                    main.gravityModifier = 0.06f;
                    break;
                case AtmosphereStyle.Dust:
                    main.startLifetime = 10f;
                    main.startSpeed = 0.08f;
                    main.gravityModifier = 0f;
                    break;
                case AtmosphereStyle.Contamination:
                    main.startLifetime = 7f;
                    main.startSpeed = 0.10f;
                    main.gravityModifier = -0.045f;
                    break;
            }

            var emission = particles.emission;
            emission.rateOverTime = rate;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = style == AtmosphereStyle.Contamination
                ? new Vector3(12f, 1.2f, 24f)
                : new Vector3(13f, 0.25f, 25f);

            if (style != AtmosphereStyle.Rain)
            {
                var noise = particles.noise;
                noise.enabled = true;
                noise.strength = style == AtmosphereStyle.Contamination ? 0.34f : 0.22f;
                noise.frequency = 0.18f;
                noise.scrollSpeed = 0.16f;
            }

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            var particleMaterial = CreateParticleMaterial();
            if (particleMaterial != null)
                renderer.sharedMaterial = particleMaterial;

            if (style == AtmosphereStyle.Rain)
            {
                renderer.renderMode = ParticleSystemRenderMode.Stretch;
                renderer.lengthScale = 1.8f;
                renderer.velocityScale = 0.45f;
            }
            else
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.velocityScale = 0f;
            }
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
