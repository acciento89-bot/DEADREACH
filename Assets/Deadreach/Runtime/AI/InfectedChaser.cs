using System;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Loot;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.AI
{
    [RequireComponent(typeof(CharacterController), typeof(Damageable))]
    public sealed class InfectedChaser : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 2.8f;
        [SerializeField, Min(0.1f)] private float aggroRange = 18f;
        [SerializeField, Min(0.1f)] private float attackRange = 1.5f;
        [SerializeField, Min(0.1f)] private float attacksPerSecond = 0.85f;
        [SerializeField, Min(0f)] private float attackDamage = 12f;
        [SerializeField, Min(0)] private int scrapDrop = 4;

        public event Action Attacked;

        private CharacterController _controller;
        private Damageable _damageable;
        private Transform _target;
        private Damageable _targetHealth;
        private float _nextAttackTime;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _damageable = GetComponent<Damageable>();
            _damageable.Died += HandleDeath;
        }

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player != null)
            {
                _target = player.transform;
                _targetHealth = player.GetComponent<Damageable>();
            }
        }

        private void OnDestroy()
        {
            if (_damageable != null)
                _damageable.Died -= HandleDeath;
        }

        private void Update()
        {
            if (_damageable.IsDead || _target == null || (_targetHealth != null && _targetHealth.IsDead))
                return;

            var toTarget = _target.position - transform.position;
            toTarget.y = 0f;
            var distance = toTarget.magnitude;
            if (distance > aggroRange)
                return;

            if (distance > attackRange)
            {
                var direction = distance > 0.01f ? toTarget / distance : Vector3.zero;
                var look = Quaternion.LookRotation(direction == Vector3.zero ? transform.forward : direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 12f * Time.deltaTime);

                if (_controller.isGrounded && _verticalVelocity < 0f)
                    _verticalVelocity = -2f;
                else
                    _verticalVelocity -= 22f * Time.deltaTime;

                _controller.Move((direction * moveSpeed + Vector3.up * _verticalVelocity) * Time.deltaTime);
            }
            else if (Time.time >= _nextAttackTime && _targetHealth != null)
            {
                _nextAttackTime = Time.time + 1f / attacksPerSecond;
                Attacked?.Invoke();
                _targetHealth.TakeDamage(new DamageInfo(attackDamage, CombatFaction.Infected, _target.position, toTarget.normalized));
            }
        }

        public void Configure(float speed, float health, float damage, int droppedScrap)
        {
            moveSpeed = Mathf.Max(0.1f, speed);
            attackDamage = Mathf.Max(0f, damage);
            scrapDrop = Mathf.Max(0, droppedScrap);
            GetComponent<Damageable>().Configure(CombatFaction.Infected, health, false);
        }

        private void HandleDeath()
        {
            if (_controller != null)
                _controller.enabled = false;

            if (scrapDrop > 0)
                LootPickup.SpawnScrap(transform.position + Vector3.up * 0.45f, scrapDrop);

            Destroy(gameObject, 0.12f);
        }
    }
}
