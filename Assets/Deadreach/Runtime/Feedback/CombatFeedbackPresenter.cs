using System.Collections;
using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    public sealed class CombatFeedbackPresenter : MonoBehaviour
    {
        private static CombatFeedbackPresenter _instance;
        private Material _tracerMaterial;
        private Material _impactMaterial;
        private ParticleSystem _impactParticles;
        private ParticleSystemRenderer _impactRenderer;

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
            SetupImpactParticles();
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

        private void HandleShot(ShotFeedback feedback)
        {
            StartCoroutine(ShowTracer(feedback));
        }

        private IEnumerator ShowTracer(ShotFeedback feedback)
        {
            var tracerObject = new GameObject("VFX_Tracer");
            tracerObject.transform.SetParent(transform, false);

            var line = tracerObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.SetPosition(0, feedback.Origin);
            line.SetPosition(1, feedback.EndPoint);
            line.startWidth = Mathf.Max(0.01f, feedback.TracerWidth) * (feedback.Critical ? 1.8f : 1f);
            line.endWidth = line.startWidth * 0.55f;
            line.numCapVertices = 2;
            line.material = _tracerMaterial;

            line.startColor = feedback.Critical
                ? new Color(1f, 0.2f, 0.92f, 1f)
                : feedback.HitDamageable
                    ? new Color(1f, 0.76f, 0.28f, 0.95f)
                    : new Color(0.35f, 0.85f, 1f, 0.9f);
            line.endColor = new Color(line.startColor.r, line.startColor.g, line.startColor.b, 0.08f);

            yield return new WaitForSecondsRealtime(Mathf.Max(0.025f, feedback.TracerDuration));
            Destroy(tracerObject);
        }

        private void HandleImpact(ImpactFeedback feedback)
        {
            if (_impactParticles == null)
                return;

            var color = feedback.Critical
                ? new Color(1f, 0.12f, 0.92f, 1f)
                : feedback.HitDamageable
                    ? new Color(1f, 0.22f, 0.08f, 1f)
                    : new Color(1f, 0.72f, 0.28f, 1f);

            var emit = new ParticleSystem.EmitParams
            {
                position = feedback.Point + feedback.Normal * 0.025f,
                velocity = feedback.Normal * (feedback.Critical ? 2.8f : feedback.HitDamageable ? 1.2f : 2.1f),
                startLifetime = feedback.Critical ? 0.24f : feedback.HitDamageable ? 0.16f : 0.12f,
                startSize = feedback.Critical ? 0.29f : feedback.HitDamageable ? 0.19f : 0.12f,
                startColor = color
            };

            _impactParticles.Emit(emit, feedback.Critical ? 6 : feedback.HitDamageable ? 3 : 2);
        }

        private void SetupMaterials()
        {
            var tracerShader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (tracerShader != null)
            {
                _tracerMaterial = new Material(tracerShader)
                {
                    name = "Runtime_TracerMaterial"
                };
            }

            var particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Sprites/Default");
            if (particleShader != null)
            {
                _impactMaterial = new Material(particleShader)
                {
                    name = "Runtime_ImpactMaterial"
                };
            }
        }

        private void SetupImpactParticles()
        {
            _impactParticles = gameObject.AddComponent<ParticleSystem>();
            _impactParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = _impactParticles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 96;
            main.startSpeed = 0f;
            main.startLifetime = 0.15f;
            main.startSize = 0.15f;

            var emission = _impactParticles.emission;
            emission.enabled = false;

            var shape = _impactParticles.shape;
            shape.enabled = false;

            _impactRenderer = _impactParticles.GetComponent<ParticleSystemRenderer>();
            if (_impactMaterial != null)
                _impactRenderer.sharedMaterial = _impactMaterial;
        }

        private void OnDestroy()
        {
            if (_tracerMaterial != null)
                Destroy(_tracerMaterial);
            if (_impactMaterial != null)
                Destroy(_impactMaterial);

            if (_instance == this)
                _instance = null;
        }
    }
}
