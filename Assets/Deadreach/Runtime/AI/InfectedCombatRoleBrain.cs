using System;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.AI
{
    public enum InfectedCombatRole
    {
        Walker = 0,
        Runner = 1,
        Brute = 2,
        Stalker = 3
    }

    [DisallowMultipleComponent]
    public sealed class InfectedCombatRoleBrain : MonoBehaviour
    {
        public event Action<InfectedCombatRole> SpecialTriggered;

        public InfectedCombatRole Role { get; private set; }
        public string RoleName => Role.ToString().ToUpperInvariant();

        private CharacterController _controller;
        private Damageable _selfHealth;
        private Transform _target;
        private Damageable _targetHealth;
        private Light _telegraph;
        private float _specialDamage;
        private float _nextSpecialTime;
        private float _telegraphUntil;
        private int _stalkerSide = 1;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _selfHealth = GetComponent<Damageable>();
        }

        private void Start()
        {
            ResolveTarget();
        }

        public void Configure(InfectedCombatRole role, float specialDamage)
        {
            Role = role;
            _specialDamage = Mathf.Max(1f, specialDamage);
            _nextSpecialTime = Time.time + InitialDelay(role);
            EnsureTelegraph();
        }

        private void Update()
        {
            if (_telegraph != null)
                _telegraph.intensity = Time.time < _telegraphUntil ? 2.6f : 0f;

            if (_selfHealth == null || _selfHealth.IsDead)
                return;

            if (_target == null || _targetHealth == null || _targetHealth.IsDead)
            {
                ResolveTarget();
                if (_target == null || _targetHealth == null)
                    return;
            }

            if (Time.time < _nextSpecialTime)
                return;

            var delta = _target.position - transform.position;
            delta.y = 0f;
            var distance = delta.magnitude;
            if (distance <= 0.01f)
                return;

            switch (Role)
            {
                case InfectedCombatRole.Runner:
                    TryRunnerBurst(delta / distance, distance);
                    break;
                case InfectedCombatRole.Brute:
                    TryBruteSlam(delta / distance, distance);
                    break;
                case InfectedCombatRole.Stalker:
                    TryStalkerFlank(delta / distance, distance);
                    break;
            }
        }

        private void TryRunnerBurst(Vector3 direction, float distance)
        {
            if (distance < 3.1f || distance > 9.5f)
                return;

            FlashTelegraph();
            if (_controller != null && _controller.enabled)
                _controller.Move(direction * 2.85f);

            var afterDistance = Vector3.Distance(Flat(transform.position), Flat(_target.position));
            if (afterDistance <= 1.75f)
            {
                _targetHealth.TakeDamage(new DamageInfo(
                    _specialDamage * 0.62f,
                    CombatFaction.Infected,
                    _target.position,
                    direction));
            }

            _nextSpecialTime = Time.time + 4.1f;
            SpecialTriggered?.Invoke(Role);
        }

        private void TryBruteSlam(Vector3 direction, float distance)
        {
            if (distance > 4.25f)
                return;

            FlashTelegraph();
            _targetHealth.TakeDamage(new DamageInfo(
                _specialDamage,
                CombatFaction.Infected,
                _target.position,
                direction));

            _nextSpecialTime = Time.time + 5.8f;
            SpecialTriggered?.Invoke(Role);
        }

        private void TryStalkerFlank(Vector3 direction, float distance)
        {
            if (distance < 3.4f || distance > 10.5f)
                return;

            FlashTelegraph();
            var lateral = Vector3.Cross(Vector3.up, direction) * _stalkerSide;
            _stalkerSide *= -1;
            var flankDirection = (lateral * 0.88f + direction * 0.48f).normalized;

            if (_controller != null && _controller.enabled)
                _controller.Move(flankDirection * 2.45f);

            _nextSpecialTime = Time.time + 4.7f;
            SpecialTriggered?.Invoke(Role);
        }

        private void ResolveTarget()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player == null)
                return;

            _target = player.transform;
            _targetHealth = player.GetComponent<Damageable>();
        }

        private void EnsureTelegraph()
        {
            if (Role == InfectedCombatRole.Walker || _telegraph != null)
                return;

            var lightObject = new GameObject($"RolePulse_{Role}");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.localPosition = Vector3.up * 1.25f;
            _telegraph = lightObject.AddComponent<Light>();
            _telegraph.type = LightType.Point;
            _telegraph.range = Role == InfectedCombatRole.Brute ? 4.8f : 3.2f;
            _telegraph.intensity = 0f;
            _telegraph.shadows = LightShadows.None;
            _telegraph.color = Role switch
            {
                InfectedCombatRole.Runner => new Color(0.12f, 0.72f, 1f),
                InfectedCombatRole.Brute => new Color(1f, 0.24f, 0.08f),
                InfectedCombatRole.Stalker => new Color(0.72f, 0.22f, 1f),
                _ => Color.white
            };
        }

        private void FlashTelegraph()
        {
            _telegraphUntil = Time.time + 0.22f;
        }

        private static float InitialDelay(InfectedCombatRole role)
        {
            return role switch
            {
                InfectedCombatRole.Runner => 1.6f,
                InfectedCombatRole.Brute => 2.4f,
                InfectedCombatRole.Stalker => 2f,
                _ => 99f
            };
        }

        private static Vector3 Flat(Vector3 value)
        {
            value.y = 0f;
            return value;
        }
    }
}
