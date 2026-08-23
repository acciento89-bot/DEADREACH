using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    public sealed class CombatFeedbackPresenter : MonoBehaviour
    {
        private static CombatFeedbackPresenter _instance;

        [SerializeField, Min(8)] private int tracerPoolSize = 28;

        private Material _tracerCoreMaterial;
        private Material _tracerGlowMaterial;
        private Material _sparkMaterial;
        private Material _goreMaterial;
        private LineRenderer[] _tracerCore;
        private LineRenderer[] _tracerGlow;
        private float[] _tracerHideAt;
        private int _nextTracerIndex;
        private ParticleSystem _sparkParticles;
        private ParticleSystem _goreParticles;
        private ParticleSystem _muzzleParticles;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_CombatFeedback");
            _instance = root.AddComponent<CombatFeedbackPresenter>();
            DontDestroyOnLoad(root);
        }

        private void Awake()
        {
            SetupMaterials();
            SetupTracerPool();
            SetupParticleSystems();
        }

        private void OnEnable()
        {
            CombatFeedback.ShotFired += HandleShot;
            CombatFeedback.Impacted += HandleImpact;
        }

        private void OnDisable()
        {
            CombatFeedback.ShotFired -= HandleShot;
            CombatFeedback.Impacted -= HandleImpact;
        }

        private void Update()
        {
            if (_tracerCore == null)
                return;

            var now = Time.unscaledTime;
            for (var i = 0; i < _tracerCore.Length; i++)
            {
                if (now < _tracerHideAt[i])
                    continue;

                if (_tracerCore[i] != null) _tracerCore[i].enabled = false;
                if (_tracerGlow[i] != null) _tracerGlow[i].enabled = false;
            }
        }

        private void HandleShot(ShotFeedback feedback)
        {
            if (_tracerCore == null || _tracerCore.Length == 0)
                return;

            var index = _nextTracerIndex;
            _nextTracerIndex = (_nextTracerIndex + 1) % _tracerCore.Length;

            var core = _tracerCore[index];
            var glow = _tracerGlow[index];
            var direction = feedback.EndPoint - feedback.Origin;
            var normalizedDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;

            var shotColor = feedback.Critical
                ? new Color(1f, 0.24f, 0.86f, 1f)
                : feedback.HitDamageable
                    ? new Color(1f, 0.64f, 0.16f, 1f)
                    : new Color(0.28f, 0.78f, 1f, 1f);

            ConfigureLine(core, feedback.Origin, feedback.EndPoint,
                Mathf.Max(0.009f, feedback.TracerWidth * 0.52f),
                shotColor, new Color(shotColor.r, shotColor.g, shotColor.b, 0.05f));

            ConfigureLine(glow, feedback.Origin, feedback.EndPoint,
                Mathf.Max(0.035f, feedback.TracerWidth * 2.35f),
                new Color(shotColor.r, shotColor.g, shotColor.b, 0.34f),
                new Color(shotColor.r, shotColor.g, shotColor.b, 0f));

            _tracerHideAt[index] = Time.unscaledTime + Mathf.Max(0.04f, feedback.TracerDuration * 1.35f);
            EmitMuzzleFlash(feedback.Origin, normalizedDirection, shotColor, feedback.Critical);
        }

        private static void ConfigureLine(LineRenderer line, Vector3 origin, Vector3 end, float width, Color start, Color finish)
        {
            if (line == null)
                return;

            line.SetPosition(0, origin);
            line.SetPosition(1, end);
            line.startWidth = width;
            line.endWidth = width * 0.22f;
            line.startColor = start;
            line.endColor = finish;
            line.enabled = true;
        }

        private void HandleImpact(ImpactFeedback feedback)
        {
            if (feedback.HitDamageable)
            {
                EmitGore(feedback);
                EmitSparks(feedback, feedback.Critical ? 7 : 3, feedback.Critical ? 3.2f : 1.6f);
            }
            else
            {
                EmitSparks(feedback, feedback.Critical ? 12 : 8, feedback.Critical ? 4f : 2.8f);
            }
        }

        private void EmitMuzzleFlash(Vector3 origin, Vector3 direction, Color color, bool critical)
        {
            if (_muzzleParticles == null)
                return;

            var count = critical ? 7 : 5;
            for (var i = 0; i < count; i++)
            {
                var jitter = Random.insideUnitSphere * 0.35f;
                var emit = new ParticleSystem.EmitParams
                {
                    position = origin + direction * 0.025f,
                    velocity = direction * Random.Range(1.2f, 3.1f) + jitter,
                    startLifetime = Random.Range(0.035f, 0.075f),
                    startSize = Random.Range(0.035f, critical ? 0.12f : 0.085f),
                    startColor = i == 0 ? Color.white : color
                };
                _muzzleParticles.Emit(emit, 1);
            }
        }

        private void EmitSparks(ImpactFeedback feedback, int count, float speed)
        {
            if (_sparkParticles == null)
                return;

            for (var i = 0; i < count; i++)
            {
                var tangent = Vector3.Cross(feedback.Normal, Random.onUnitSphere);
                var velocity = (feedback.Normal * Random.Range(0.35f, 1f) + tangent * Random.Range(-0.8f, 0.8f)).normalized * Random.Range(speed * 0.55f, speed);
                var color = feedback.Critical
                    ? new Color(1f, 0.2f, 0.9f, 1f)
                    : new Color(1f, Random.Range(0.42f, 0.78f), 0.08f, 1f);

                var emit = new ParticleSystem.EmitParams
                {
                    position = feedback.Point + feedback.Normal * 0.02f,
                    velocity = velocity,
                    startLifetime = Random.Range(0.08f, 0.22f),
                    startSize = Random.Range(0.012f, 0.045f),
                    startColor = color
                };
                _sparkParticles.Emit(emit, 1);
            }
        }

        private void EmitGore(ImpactFeedback feedback)
        {
            if (_goreParticles == null)
                return;

            var count = feedback.Critical ? 14 : 8;
            for (var i = 0; i < count; i++)
            {
                var velocity = feedback.Normal * Random.Range(0.3f, 1.5f) + Random.insideUnitSphere * Random.Range(0.45f, 1.8f);
                velocity.y = Mathf.Abs(velocity.y) + Random.Range(0.05f, 0.55f);
                var emit = new ParticleSystem.EmitParams
                {
                    position = feedback.Point + feedback.Normal * 0.018f,
                    velocity = velocity,
                    startLifetime = Random.Range(0.12f, 0.32f),
                    startSize = Random.Range(0.018f, feedback.Critical ? 0.075f : 0.055f),
                    startColor = feedback.Critical
                        ? new Color(0.95f, 0.04f, 0.32f, 1f)
                        : new Color(Random.Range(0.42f, 0.72f), 0.018f, 0.012f, 1f)
                };
                _goreParticles.Emit(emit, 1);
            }
        }

        private void SetupMaterials()
        {
            var lineShader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit");
            if (lineShader != null)
            {
                _tracerCoreMaterial = new Material(lineShader) { name = "Runtime_TracerCore" };
                _tracerGlowMaterial = new Material(lineShader) { name = "Runtime_TracerGlow" };
            }

            var particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Sprites/Default");
            if (particleShader != null)
            {
                _sparkMaterial = new Material(particleShader) { name = "Runtime_ImpactSparks" };
                _goreMaterial = new Material(particleShader) { name = "Runtime_ImpactGore" };
                PrepareParticleMaterial(_sparkMaterial, Color.white);
                PrepareParticleMaterial(_goreMaterial, Color.white);
            }
        }

        private static void PrepareParticleMaterial(Material material, Color color)
        {
            if (material == null)
                return;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        }

        private void SetupTracerPool()
        {
            var count = Mathf.Max(8, tracerPoolSize);
            _tracerCore = new LineRenderer[count];
            _tracerGlow = new LineRenderer[count];
            _tracerHideAt = new float[count];

            for (var i = 0; i < count; i++)
            {
                var tracerObject = new GameObject($"VFX_Tracer_{i:00}");
                tracerObject.transform.SetParent(transform, false);

                var glow = CreateLine(tracerObject, "Glow", _tracerGlowMaterial, 2);
                var core = CreateLine(tracerObject, "Core", _tracerCoreMaterial, 4);
                _tracerGlow[i] = glow;
                _tracerCore[i] = core;
            }
        }

        private static LineRenderer CreateLine(GameObject root, string name, Material material, int capVertices)
        {
            var child = new GameObject(name);
            child.transform.SetParent(root.transform, false);
            var line = child.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.numCapVertices = capVertices;
            line.numCornerVertices = 2;
            line.textureMode = LineTextureMode.Stretch;
            line.sharedMaterial = material;
            line.enabled = false;
            line.alignment = LineAlignment.View;
            return line;
        }

        private void SetupParticleSystems()
        {
            _muzzleParticles = CreateParticleSystem("VFX_MuzzleFlash", _sparkMaterial, ParticleSystemRenderMode.Stretch, 220, 2.4f);
            _sparkParticles = CreateParticleSystem("VFX_ImpactSparks", _sparkMaterial, ParticleSystemRenderMode.Stretch, 380, 3.2f);
            _goreParticles = CreateParticleSystem("VFX_ImpactGore", _goreMaterial, ParticleSystemRenderMode.Stretch, 320, 0.65f);
        }

        private ParticleSystem CreateParticleSystem(string name, Material material, ParticleSystemRenderMode renderMode, int maxParticles, float lengthScale)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(transform, false);
            var particles = gameObject.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = maxParticles;
            main.startSpeed = 0f;
            main.startLifetime = 0.15f;
            main.startSize = 0.04f;
            main.gravityModifier = name.Contains("Gore") ? 0.8f : 0.18f;

            var emission = particles.emission;
            emission.enabled = false;
            var shape = particles.shape;
            shape.enabled = false;

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = renderMode;
            renderer.lengthScale = lengthScale;
            renderer.velocityScale = 0.55f;
            renderer.cameraVelocityScale = 0f;
            if (material != null)
                renderer.sharedMaterial = material;
            return particles;
        }

        private void OnDestroy()
        {
            if (_tracerCoreMaterial != null) Destroy(_tracerCoreMaterial);
            if (_tracerGlowMaterial != null) Destroy(_tracerGlowMaterial);
            if (_sparkMaterial != null) Destroy(_sparkMaterial);
            if (_goreMaterial != null) Destroy(_goreMaterial);

            if (_instance == this)
                _instance = null;
        }
    }
}
