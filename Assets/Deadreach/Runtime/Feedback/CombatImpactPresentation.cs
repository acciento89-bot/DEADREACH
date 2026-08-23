using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    public sealed class CombatImpactPresentation : MonoBehaviour
    {
        private static CombatImpactPresentation _instance;

        private Material _lineMaterial;
        private Material _particleMaterial;
        private ParticleSystem _particles;
        private Camera _camera;
        private float _baseFieldOfView;
        private float _baseOrthoSize;
        private float _cameraKick;
        private Vector3 _hitMarkerPoint;
        private float _hitMarkerUntil;
        private bool _hitMarkerCritical;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_CombatImpact_010");
            _instance = root.AddComponent<CombatImpactPresentation>();
            DontDestroyOnLoad(root);
        }

        private void Awake()
        {
            SetupMaterials();
            SetupParticles();
        }

        private void OnEnable()
        {
            CombatFeedback.AbilityActivated += HandleAbility;
            CombatFeedback.EnemySpecialActivated += HandleEnemySpecial;
            CombatFeedback.Impacted += HandleImpact;
            CombatFeedback.PlayerDamaged += HandlePlayerDamaged;
            CombatFeedback.PlayerDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            CombatFeedback.AbilityActivated -= HandleAbility;
            CombatFeedback.EnemySpecialActivated -= HandleEnemySpecial;
            CombatFeedback.Impacted -= HandleImpact;
            CombatFeedback.PlayerDamaged -= HandlePlayerDamaged;
            CombatFeedback.PlayerDied -= HandlePlayerDied;
            RestoreCameraLens();
        }

        private void Update()
        {
            ResolveCamera();
            _cameraKick = Mathf.MoveTowards(_cameraKick, 0f, Time.unscaledDeltaTime * 4.8f);
            ApplyCameraLensKick();
        }

        private void OnGUI()
        {
            if (Time.unscaledTime >= _hitMarkerUntil)
                return;

            ResolveCamera();
            if (_camera == null)
                return;

            var screen = _camera.WorldToScreenPoint(_hitMarkerPoint);
            if (screen.z <= 0f)
                return;

            var guiPoint = new Vector2(screen.x, Screen.height - screen.y);
            var size = Mathf.Clamp(Screen.safeArea.height * 0.022f, 12f, 25f);
            var thickness = Mathf.Max(2f, size * 0.14f);
            var gap = size * 0.32f;
            var length = size * 0.58f;
            var fade = Mathf.Clamp01((_hitMarkerUntil - Time.unscaledTime) / 0.18f);
            var tint = _hitMarkerCritical
                ? new Color(1f, 0.18f, 0.88f, fade)
                : new Color(1f, 0.72f, 0.24f, fade * 0.92f);

            var old = GUI.color;
            GUI.color = tint;
            GUI.DrawTexture(new Rect(guiPoint.x - gap - length, guiPoint.y - thickness * 0.5f, length, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(guiPoint.x + gap, guiPoint.y - thickness * 0.5f, length, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(guiPoint.x - thickness * 0.5f, guiPoint.y - gap - length, thickness, length), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(guiPoint.x - thickness * 0.5f, guiPoint.y + gap, thickness, length), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void HandleAbility(AbilityImpactFeedback feedback)
        {
            switch (feedback.Kind)
            {
                case CombatAbilityKind.FieldPatch:
                {
                    var green = new Color(0.2f, 1f, 0.58f, 0.95f);
                    var cyan = new Color(0.28f, 0.92f, 1f, 0.82f);
                    SpawnRing(feedback.Origin, 0.22f, feedback.Radius, 0.48f, green, 0.13f);
                    SpawnRing(feedback.Origin + Vector3.up * 0.035f, 0.12f, feedback.Radius * 0.72f, 0.31f, cyan, 0.065f);
                    EmitHeal(feedback.Origin, green, 22, feedback.Radius);
                    KickCamera(0.42f);
                    break;
                }
                case CombatAbilityKind.VectorDash:
                {
                    var blue = new Color(0.14f, 0.76f, 1f, 0.95f);
                    var white = new Color(0.78f, 0.96f, 1f, 0.82f);
                    SpawnTrail(feedback.Origin, feedback.EndPoint, 0.25f, blue, 0.18f);
                    SpawnTrail(feedback.Origin + Vector3.up * 0.08f, feedback.EndPoint + Vector3.up * 0.08f, 0.17f, white, 0.055f);
                    SpawnRing(feedback.EndPoint - Vector3.up * 0.46f, 0.12f, feedback.Radius, 0.26f, blue, 0.075f);
                    EmitTrail(feedback.Origin, feedback.EndPoint, blue, 16);
                    KickCamera(0.34f);
                    break;
                }
                case CombatAbilityKind.Shockwave:
                {
                    var orange = new Color(1f, 0.35f, 0.06f, 0.98f);
                    var hot = new Color(1f, 0.82f, 0.28f, 0.92f);
                    SpawnRing(feedback.Origin, 0.28f, feedback.Radius, 0.52f, orange, 0.2f, 64);
                    SpawnRing(feedback.Origin + Vector3.up * 0.045f, 0.18f, feedback.Radius * 0.78f, 0.33f, hot, 0.085f, 56);
                    EmitRadial(feedback.Origin, orange, 34, 5.2f, 0.105f);
                    KickCamera(1.35f);
                    break;
                }
            }
        }

        private void HandleEnemySpecial(EnemySpecialImpactFeedback feedback)
        {
            switch (feedback.Kind)
            {
                case EnemySpecialKind.RunnerBurst:
                {
                    var cyan = new Color(0.08f, 0.76f, 1f, 0.82f);
                    SpawnTrail(feedback.Origin, feedback.EndPoint, 0.18f, cyan, 0.095f);
                    SpawnRing(feedback.EndPoint - Vector3.up * 0.38f, 0.08f, feedback.Radius, 0.2f, cyan, 0.05f);
                    EmitTrail(feedback.Origin, feedback.EndPoint, cyan, 8);
                    KickCamera(0.1f);
                    break;
                }
                case EnemySpecialKind.BruteSlam:
                {
                    var red = new Color(1f, 0.16f, 0.035f, 0.92f);
                    var ember = new Color(1f, 0.48f, 0.08f, 0.82f);
                    SpawnRing(feedback.Origin, 0.3f, feedback.Radius, 0.4f, red, 0.135f, 56);
                    SpawnRing(feedback.Origin + Vector3.up * 0.035f, 0.2f, feedback.Radius * 0.68f, 0.27f, ember, 0.055f, 48);
                    EmitRadial(feedback.Origin, ember, 18, 3.8f, 0.075f);
                    KickCamera(0.72f);
                    break;
                }
                case EnemySpecialKind.StalkerFlank:
                {
                    var violet = new Color(0.74f, 0.22f, 1f, 0.86f);
                    SpawnTrail(feedback.Origin, feedback.EndPoint, 0.3f, violet, 0.14f);
                    SpawnRing(feedback.Origin - Vector3.up * 0.38f, 0.08f, feedback.Radius * 0.75f, 0.23f, violet, 0.045f);
                    SpawnRing(feedback.EndPoint - Vector3.up * 0.38f, 0.08f, feedback.Radius, 0.25f, violet, 0.06f);
                    EmitTrail(feedback.Origin, feedback.EndPoint, violet, 10);
                    KickCamera(0.12f);
                    break;
                }
            }
        }

        private void HandleImpact(ImpactFeedback feedback)
        {
            if (!feedback.HitDamageable)
                return;

            _hitMarkerPoint = feedback.Point;
            _hitMarkerCritical = feedback.Critical;
            _hitMarkerUntil = Time.unscaledTime + (feedback.Critical ? 0.2f : 0.14f);

            if (!feedback.Critical)
                return;

            var critical = new Color(1f, 0.18f, 0.88f, 0.9f);
            SpawnRing(feedback.Point + feedback.Normal * 0.02f, 0.025f, 0.42f, 0.17f, critical, 0.045f, 28);
            KickCamera(0.2f);
        }

        private void HandlePlayerDamaged(float normalizedDamage)
        {
            KickCamera(Mathf.Lerp(0.22f, 0.95f, Mathf.Clamp01(normalizedDamage * 3.2f)));
        }

        private void HandlePlayerDied()
        {
            KickCamera(1.7f);
        }

        private void SpawnRing(Vector3 origin, float startRadius, float endRadius, float duration, Color color, float width, int segments = 48)
        {
            var root = new GameObject("VFX_ImpactRing");
            var ring = root.AddComponent<RuntimeImpactRing>();
            ring.Initialize(_lineMaterial, origin, startRadius, endRadius, duration, color, width, segments);
        }

        private void SpawnTrail(Vector3 start, Vector3 end, float duration, Color color, float width)
        {
            if ((end - start).sqrMagnitude < 0.01f)
                return;

            var root = new GameObject("VFX_ImpactTrail");
            var line = root.AddComponent<RuntimeImpactLine>();
            line.Initialize(_lineMaterial, start, end, duration, color, width);
        }

        private void EmitHeal(Vector3 origin, Color color, int count, float radius)
        {
            if (_particles == null)
                return;

            for (var i = 0; i < count; i++)
            {
                var angle = Random.value * Mathf.PI * 2f;
                var radial = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                var distance = Random.Range(0.15f, radius * 0.72f);
                var emit = new ParticleSystem.EmitParams
                {
                    position = origin + radial * distance,
                    velocity = radial * Random.Range(0.1f, 0.65f) + Vector3.up * Random.Range(0.75f, 2.1f),
                    startLifetime = Random.Range(0.28f, 0.62f),
                    startSize = Random.Range(0.035f, 0.11f),
                    startColor = Color.Lerp(color, Color.white, Random.Range(0f, 0.45f))
                };
                _particles.Emit(emit, 1);
            }
        }

        private void EmitRadial(Vector3 origin, Color color, int count, float speed, float size)
        {
            if (_particles == null)
                return;

            for (var i = 0; i < count; i++)
            {
                var angle = i / (float)Mathf.Max(1, count) * Mathf.PI * 2f + Random.Range(-0.09f, 0.09f);
                var radial = new Vector3(Mathf.Cos(angle), Random.Range(0.08f, 0.32f), Mathf.Sin(angle)).normalized;
                var emit = new ParticleSystem.EmitParams
                {
                    position = origin + Vector3.up * 0.06f,
                    velocity = radial * Random.Range(speed * 0.55f, speed),
                    startLifetime = Random.Range(0.16f, 0.38f),
                    startSize = Random.Range(size * 0.45f, size),
                    startColor = Color.Lerp(color, Color.white, Random.Range(0f, 0.22f))
                };
                _particles.Emit(emit, 1);
            }
        }

        private void EmitTrail(Vector3 start, Vector3 end, Color color, int count)
        {
            if (_particles == null)
                return;

            var direction = end - start;
            for (var i = 0; i < count; i++)
            {
                var t = count <= 1 ? 0.5f : i / (float)(count - 1);
                var emit = new ParticleSystem.EmitParams
                {
                    position = Vector3.Lerp(start, end, t) + Random.insideUnitSphere * 0.08f,
                    velocity = -direction.normalized * Random.Range(0.15f, 0.75f) + Vector3.up * Random.Range(0.05f, 0.4f),
                    startLifetime = Random.Range(0.12f, 0.3f),
                    startSize = Random.Range(0.025f, 0.085f),
                    startColor = Color.Lerp(color, Color.white, Random.Range(0f, 0.3f))
                };
                _particles.Emit(emit, 1);
            }
        }

        private void SetupMaterials()
        {
            var lineShader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit");
            if (lineShader != null)
                _lineMaterial = new Material(lineShader) { name = "Runtime_010_ImpactLine" };

            var particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Sprites/Default");
            if (particleShader != null)
            {
                _particleMaterial = new Material(particleShader) { name = "Runtime_010_ImpactParticles" };
                if (_particleMaterial.HasProperty("_BaseColor")) _particleMaterial.SetColor("_BaseColor", Color.white);
                if (_particleMaterial.HasProperty("_Color")) _particleMaterial.SetColor("_Color", Color.white);
            }
        }

        private void SetupParticles()
        {
            var root = new GameObject("VFX_010_ImpactParticles");
            root.transform.SetParent(transform, false);
            _particles = root.AddComponent<ParticleSystem>();
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = _particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 420;
            main.startSpeed = 0f;
            main.startLifetime = 0.3f;
            main.startSize = 0.06f;
            main.gravityModifier = 0.12f;

            var emission = _particles.emission;
            emission.enabled = false;
            var shape = _particles.shape;
            shape.enabled = false;

            var renderer = _particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            if (_particleMaterial != null)
                renderer.sharedMaterial = _particleMaterial;
        }

        private void KickCamera(float strength)
        {
            _cameraKick = Mathf.Max(_cameraKick, Mathf.Max(0f, strength));
        }

        private void ResolveCamera()
        {
            var current = Camera.main;
            if (current == _camera)
                return;

            RestoreCameraLens();
            _camera = current;
            if (_camera == null)
                return;

            _baseFieldOfView = _camera.fieldOfView;
            _baseOrthoSize = _camera.orthographicSize;
        }

        private void ApplyCameraLensKick()
        {
            if (_camera == null)
                return;

            var pulse = Mathf.Clamp(_cameraKick, 0f, 2f);
            if (_camera.orthographic)
                _camera.orthographicSize = Mathf.Max(0.1f, _baseOrthoSize * (1f - pulse * 0.012f));
            else
                _camera.fieldOfView = Mathf.Clamp(_baseFieldOfView - pulse * 1.15f, 25f, 90f);
        }

        private void RestoreCameraLens()
        {
            if (_camera == null)
                return;

            if (_camera.orthographic)
                _camera.orthographicSize = _baseOrthoSize;
            else
                _camera.fieldOfView = _baseFieldOfView;
        }

        private void OnDestroy()
        {
            RestoreCameraLens();
            if (_lineMaterial != null) Destroy(_lineMaterial);
            if (_particleMaterial != null) Destroy(_particleMaterial);

            if (_instance == this)
                _instance = null;
        }
    }
}
